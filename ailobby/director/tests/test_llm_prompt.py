"""Backend-agnostic tests: prompt building, JSON parsing, backend selection.

No network / no model required — these exercise the shared llm.py logic and the
factory wiring only.
"""

import pytest

from ailobby_director import llm
from ailobby_director.config import Settings


def _settings(**over) -> Settings:
    base = dict(
        backend="ollama",
        model="claude-haiku-4-5",
        ollama_host="127.0.0.1:11434",
        ollama_model="qwen2.5:3b-instruct",
        rcon_host="127.0.0.1",
        rcon_port=27015,
        rcon_password="pw",
        log_listen_port=27100,
        log_secret="",
    )
    base.update(over)
    return Settings(**base)


def test_build_system_includes_personas_and_rules():
    s = llm.build_system('- "Bot1" — chirpy scout')
    assert "NEVER reveal" in s
    assert '"Bot1"' in s


def test_build_user_has_trigger_and_state():
    u = llm.build_user("Medic died", "Score: Red 1 - Blue 0")
    assert "Medic died" in u and "Red 1" in u


def test_parse_valid_json():
    lines = llm.parse_chat_lines('{"messages":[{"bot":"A","delay_ms":500,"text":"go go"}]}')
    assert len(lines) == 1
    assert lines[0].bot == "A" and lines[0].delay_ms == 500 and lines[0].text == "go go"


def test_parse_empty_and_junk():
    assert llm.parse_chat_lines('{"messages":[]}') == []
    assert llm.parse_chat_lines("not json at all") == []
    assert llm.parse_chat_lines(None) == []


def test_parse_strips_code_fences():
    # small local models sometimes wrap output in ```json fences
    fenced = '```json\n{"messages":[{"bot":"B","delay_ms":0,"text":"hi"}]}\n```'
    lines = llm.parse_chat_lines(fenced)
    assert len(lines) == 1 and lines[0].bot == "B"


def test_parse_skips_malformed_items():
    lines = llm.parse_chat_lines('{"messages":[{"bot":"A"},{"bot":"B","delay_ms":10,"text":"ok"}]}')
    assert [l.bot for l in lines] == ["B"]


def test_factory_builds_ollama_without_network():
    client = llm.make_llm(_settings(backend="ollama"))
    assert client.label == "ollama:qwen2.5:3b-instruct"
    assert client.total_calls == 0


def test_factory_rejects_unknown_backend():
    with pytest.raises(ValueError):
        llm.make_llm(_settings(backend="gpt5"))
