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
