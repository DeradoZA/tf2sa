"""Minimal asyncio Source RCON client (hand-rolled: the protocol is ~100 lines,
avoids an external dependency of uncertain asyncio quality).

Protocol: little-endian framed packets over TCP.
  int32 size | int32 id | int32 type | body bytes | 0x00 0x00
Types: 3 = SERVERDATA_AUTH, 2 = SERVERDATA_EXECCOMMAND / AUTH_RESPONSE,
       0 = SERVERDATA_RESPONSE_VALUE. Auth failure => response id == -1.
"""

from __future__ import annotations

import asyncio
import logging
import struct

log = logging.getLogger(__name__)

_AUTH = 3
_AUTH_RESPONSE = 2
_EXEC = 2
_RESPONSE = 0


class RconError(Exception):
    pass


class RconClient:
    def __init__(self, host: str, port: int, password: str) -> None:
        self._host = host
        self._port = port
        self._password = password
        self._reader: asyncio.StreamReader | None = None
        self._writer: asyncio.StreamWriter | None = None
        self._lock = asyncio.Lock()
        self._next_id = 1

    # ---- framing ---------------------------------------------------------

    def _encode(self, req_id: int, ptype: int, body: str) -> bytes:
        payload = struct.pack("<ii", req_id, ptype) + body.encode("utf-8") + b"\x00\x00"
        return struct.pack("<i", len(payload)) + payload

    async def _read_packet(self) -> tuple[int, int, str]:
        assert self._reader is not None
        size_raw = await self._reader.readexactly(4)
        (size,) = struct.unpack("<i", size_raw)
        payload = await self._reader.readexactly(size)
        req_id, ptype = struct.unpack("<ii", payload[:8])
        body = payload[8:-2].decode("utf-8", errors="replace")
        return req_id, ptype, body

    # ---- lifecycle -------------------------------------------------------

    async def connect(self) -> None:
        self._reader, self._writer = await asyncio.open_connection(self._host, self._port)
        auth_id = self._next_id
        self._next_id += 1
        self._writer.write(self._encode(auth_id, _AUTH, self._password))
        await self._writer.drain()
        # Server may send an empty RESPONSE_VALUE before the AUTH_RESPONSE.
        for _ in range(3):
            req_id, ptype, _body = await self._read_packet()
            if ptype == _AUTH_RESPONSE:
                if req_id == -1:
                    raise RconError("RCON authentication failed (bad password)")
                log.info("RCON authenticated to %s:%d", self._host, self._port)
                return
        raise RconError("no AUTH_RESPONSE received")

    async def close(self) -> None:
        if self._writer is not None:
            self._writer.close()
            try:
                await self._writer.wait_closed()
            except Exception:
                pass
        self._reader = self._writer = None

    # ---- commands --------------------------------------------------------

    async def execute(self, command: str) -> str:
        """Run a command, reconnecting once on failure."""
        async with self._lock:
            for attempt in (1, 2):
                try:
                    if self._writer is None:
                        await self.connect()
                    return await self._execute_once(command)
                except (RconError, OSError, asyncio.IncompleteReadError) as exc:
                    log.warning("RCON error (%s), attempt %d: %s", command.split()[0], attempt, exc)
                    await self.close()
                    if attempt == 2:
                        raise
                    await asyncio.sleep(0.5)
            raise RconError("unreachable")

    async def _execute_once(self, command: str) -> str:
        """Execute and drain a possibly multi-packet response.

        Source splits long responses (cvarlist, status on full servers) across
        packets with no length header. Standard technique: send an empty
        RESPONSE_VALUE marker right after the command — the server echoes it
        after all response fragments, so reading until the marker id collects
        the full body.
        """
        assert self._writer is not None
        req_id = self._next_id
        marker_id = self._next_id + 1
        self._next_id += 2
        self._writer.write(self._encode(req_id, _EXEC, command))
        self._writer.write(self._encode(marker_id, _RESPONSE, ""))
        await self._writer.drain()

        parts: list[str] = []
        while True:
            resp_id, _ptype, body = await asyncio.wait_for(self._read_packet(), timeout=5.0)
            if resp_id == marker_id:
                # Server follows the echoed marker with a junk 0x01 packet;
                # swallow it so it doesn't pollute the next command's read.
                try:
                    await asyncio.wait_for(self._read_packet(), timeout=0.5)
                except (asyncio.TimeoutError, asyncio.IncompleteReadError):
                    pass
                break
            if resp_id == req_id:
                parts.append(body)
            else:
                log.debug("RCON unexpected response id %d (expected %d)", resp_id, req_id)
        return "".join(parts)
