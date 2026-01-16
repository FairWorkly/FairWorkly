from __future__ import annotations

import functools
from pathlib import Path
from typing import Any, Dict

import yaml
from dotenv import load_dotenv

CONFIG_PATH = Path(__file__).resolve().parent.parent / "config.yaml"

# Load .env once when the module is imported so os.getenv works everywhere.
load_dotenv(dotenv_path=CONFIG_PATH.parent / ".env", override=False)


@functools.lru_cache(maxsize=1)
def load_config() -> Dict[str, Any]:
    with CONFIG_PATH.open("r", encoding="utf-8") as fp:
        return yaml.safe_load(fp)


def resolve_document_faiss_path(
    config: Dict[str, Any],
    embedding_mode: str | None = None,
) -> str:
    """Return the document FAISS path for the requested embedding mode."""
    paths = config.get("paths") or {}
    configured = paths.get("document_faiss_path")
    if configured is None:
        raise ValueError("paths.document_faiss_path is not configured")

    mode = (embedding_mode or config.get("model_params", {}).get("deployment_mode_embedding", "local")).lower()

    if isinstance(configured, str):
        return configured

    if isinstance(configured, dict):
        if mode in configured:
            return configured[mode]
        if "default" in configured:
            return configured["default"]
        raise ValueError(f"paths.document_faiss_path missing entry for mode '{mode}'")

    raise TypeError("paths.document_faiss_path must be a string or mapping")
