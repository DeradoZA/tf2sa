"""AI Director entrypoint: UDP log stream in -> LLM decisions -> RCON chat out."""

from __future__ import annotations

import asyncio
import logging
import os
import signal

from .config import Settings
from .dispatcher import Dispatcher
from .llm import make_llm
from .log_listener import start_listener
from .log_parser import parse_line
from .personas import PersonaManager
from .rcon_client import RconClient
from .server_setup import configure_logging
from .state import MatchState
from .strategist import Strategist

log = logging.getLogger("ailobby")


async def parse_loop(raw_queue: asyncio.Queue[str], bus: asyncio.Queue) -> None:
    while True:
        line = await raw_queue.get()
        bus.put_nowait(parse_line(line))


async def main() -> None:
    logging.basicConfig(
        level=os.environ.get("LOG_LEVEL", "INFO"),
        format="%(asctime)s %(levelname)s %(name)s: %(message)s",
    )
    settings = Settings.from_env()
    llm = make_llm(settings)
    log.info("AI Director starting (backend=%s)", llm.label)

    raw_queue: asyncio.Queue[str] = asyncio.Queue(maxsize=1000)
    bus: asyncio.Queue = asyncio.Queue(maxsize=1000)

    state = MatchState()
    rcon = RconClient(settings.rcon_host, settings.rcon_port, settings.rcon_password)
    dispatcher = Dispatcher(rcon, state, settings)
    personas = PersonaManager()
    strategist = Strategist(bus, state, personas, llm, dispatcher, settings)

    transport = await start_listener(raw_queue, settings.log_listen_port, settings.log_secret)

    # Point srcds's log stream at us (loopback is unreliable — use the host IP).
    # Runs in the background so we don't block startup while the server boots.
    setup_task = asyncio.create_task(
        configure_logging(rcon, settings.log_listen_port), name="server-setup"
    )

    stop = asyncio.Event()
    loop = asyncio.get_running_loop()
    for sig in (signal.SIGINT, signal.SIGTERM):
        loop.add_signal_handler(sig, stop.set)

    tasks = [
        setup_task,
        asyncio.create_task(parse_loop(raw_queue, bus), name="parse"),
        asyncio.create_task(strategist.run(), name="strategist"),
    ]
    try:
        await stop.wait()
    finally:
        log.info("shutting down (llm calls this session: %d)", llm.total_calls)
        transport.close()
        for t in tasks:
            t.cancel()
        await asyncio.gather(*tasks, return_exceptions=True)
        await rcon.close()


if __name__ == "__main__":
    asyncio.run(main())
