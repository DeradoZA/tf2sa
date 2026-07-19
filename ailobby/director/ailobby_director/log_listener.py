"""Asyncio UDP listener for the srcds remote log stream (logaddress_add).

Packet format: b"\\xff\\xff\\xff\\xff" + indicator + log line + b"\\n\\x00"
  indicator "R"          — standard packet (no sv_logsecret)
  indicator "S<secret>L" — sv_logsecret packet; secret precedes the line
The log line itself starts "L mm/dd/yyyy - HH:MM:SS: <message>".
"""

from __future__ import annotations

import asyncio
import logging

log = logging.getLogger(__name__)

_MAGIC = b"\xff\xff\xff\xff"


def extract_line(data: bytes, secret: str) -> str | None:
    """Return the raw log line from a UDP payload, or None if invalid."""
    if not data.startswith(_MAGIC):
        return None
    body = data[len(_MAGIC):].rstrip(b"\x00\n")
    try:
        text = body.decode("utf-8", errors="replace")
    except Exception:  # pragma: no cover - decode with replace shouldn't raise
        return None

    if text.startswith("R"):
        if secret:
            log.warning("dropping unsecured log packet (LOG_SECRET is set)")
            return None
        return text[1:].lstrip()
    if text.startswith("S"):
        # "S<secret>L <date> - <time>: ..."
        idx = text.find("L ")
        if idx == -1:
            return None
        got = text[1:idx]
        if secret and got != secret:
            log.warning("dropping log packet with wrong sv_logsecret")
            return None
        return text[idx:]
    return None


class LogListenerProtocol(asyncio.DatagramProtocol):
    """Pushes raw log lines onto a queue for the parser."""

    def __init__(self, queue: asyncio.Queue[str], secret: str) -> None:
        self.queue = queue
        self.secret = secret

    def datagram_received(self, data: bytes, addr) -> None:  # noqa: ANN001
        line = extract_line(data, self.secret)
        if line is not None:
            try:
                self.queue.put_nowait(line)
            except asyncio.QueueFull:
                log.warning("log queue full; dropping line")


async def start_listener(
    queue: asyncio.Queue[str], port: int, secret: str
) -> asyncio.DatagramTransport:
    loop = asyncio.get_running_loop()
    transport, _ = await loop.create_datagram_endpoint(
        lambda: LogListenerProtocol(queue, secret),
        local_addr=("0.0.0.0", port),
    )
    log.info("listening for srcds log packets on udp/%d", port)
    return transport
