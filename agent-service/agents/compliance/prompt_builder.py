from llm import load_prompt
from pathlib import Path

COMPLIANCE_PROMPT = load_prompt(
    Path(__file__).resolve().parents[1] / "system_prompt.txt",
    "You are FairWorkly's compliance agent. Follow Australian HR rules.",
)