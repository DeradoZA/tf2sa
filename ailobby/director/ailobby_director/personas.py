"""Persona definitions and stable assignment of personas to in-game bots."""

from __future__ import annotations

from dataclasses import dataclass
from importlib import resources

import yaml


@dataclass(frozen=True)
class Persona:
    label: str
    personality: str
    style: str
    chattiness: str
    examples: tuple[str, ...]


def load_personas() -> list[Persona]:
    text = (resources.files("ailobby_director") / "personas.yaml").read_text()
    raw = yaml.safe_load(text)
    return [
        Persona(
            label=p["label"],
            personality=p["personality"].strip(),
            style=p["style"],
            chattiness=p["chattiness"],
            examples=tuple(p.get("examples", [])),
        )
        for p in raw["personas"]
    ]


class PersonaManager:
    """Assigns personas to bot names in first-seen order (stable per session).

    Bot names come from the TFBot quota (random TF2 names); the persona supplies
    the personality/voice. If a bot leaves and a new one joins, the freed
    persona is reused.
    """

    def __init__(self, personas: list[Persona] | None = None) -> None:
        self._personas = personas or load_personas()
        self._assigned: dict[str, Persona] = {}  # bot name -> persona

    def sync(self, bot_names: list[str]) -> None:
        current = set(bot_names)
        for name in list(self._assigned):
            if name not in current:
                del self._assigned[name]
        in_use = {p.label for p in self._assigned.values()}
        free = [p for p in self._personas if p.label not in in_use]
        for name in bot_names:
            if name not in self._assigned:
                if free:
                    self._assigned[name] = free.pop(0)
                else:  # more bots than personas: cycle
                    self._assigned[name] = self._personas[len(self._assigned) % len(self._personas)]

    def assignments(self) -> dict[str, Persona]:
        return dict(self._assigned)

    def render_for_prompt(self) -> str:
        lines = []
        for name, p in self._assigned.items():
            examples = " | ".join(p.examples[:2])
            lines.append(
                f'- "{name}" — {p.personality} Style: {p.style}. '
                f"Chattiness: {p.chattiness}. Example lines: {examples}"
            )
        return "\n".join(lines)
