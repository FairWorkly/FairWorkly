from pathlib import Path

import yaml
PROMPT_PATH = Path(__file__).resolve().parent / "prompt.yaml"
FALLBACK_PROMPT = "You are FairWorkly's compliance agent. Follow Australian HR rules."


def _load_prompt_template() -> str:
    try:
        data = yaml.safe_load(PROMPT_PATH.read_text(encoding="utf-8")) or {}
        return data.get("prompt_template", FALLBACK_PROMPT).strip()
    except FileNotFoundError:
        return FALLBACK_PROMPT


COMPLIANCE_PROMPT = _load_prompt_template()
