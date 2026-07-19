"""Self-configure srcds logging over RCON on director startup.

srcds silently drops UDP log packets sent to 127.0.0.1 in some builds, but
delivers fine to the host's real IP. Since the director and server share the
host network namespace, we detect the host IP the director listens on and point
srcds's logaddress at it. Done over RCON so it survives IP changes, map changes,
and server restarts — no static IP in server.cfg.
"""

from __future__ import annotations

import asyncio
import logging
import os
import socket

from .rcon_client import RconClient

log = logging.getLogger(__name__)


def detect_host_ip() -> str:
    """Primary outbound IP of this host (works in WSL2 host networking)."""
    override = os.environ.get("LOG_SEND_IP")
    if override:
        return override
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    try:
        # No packet is actually sent; this just selects the primary interface.
        s.connect(("10.254.254.254", 1))
        return s.getsockname()[0]
    except OSError:
        return "127.0.0.1"
    finally:
        s.close()


async def configure_logging(rcon: RconClient, port: int, retries: int = 40) -> None:
    """Point srcds's log stream at the director. Retries until the server is up."""
    ip = detect_host_ip()
    for attempt in range(1, retries + 1):
        try:
            # delall first so repeated director restarts don't accumulate
            # duplicate destinations (which would double every event -> double chat).
            await rcon.execute("logaddress_delall")
            await rcon.execute(f"logaddress_add {ip}:{port}")
            await rcon.execute("log on")
            log.info("srcds logging configured -> %s:%d", ip, port)
            return
        except Exception as exc:  # server not up yet, or RCON not ready
            if attempt == 1 or attempt % 10 == 0:
                log.info("waiting for server RCON to configure logging (%s)", exc)
            await asyncio.sleep(3)
    log.warning("could not configure srcds logging after %d attempts", retries)
