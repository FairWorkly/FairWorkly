import os
from functools import lru_cache
from pathlib import Path
from typing import Any, Mapping

import yaml

CONFIG_PATH = Path(os.getenv("CONFIG_PATH", "config.yaml"))


@lru_cache(maxsize=1)
def load_config() -> Mapping[str, Any]:
    with CONFIG_PATH.open(encoding="utf-8") as fh:
        return yaml.safe_load(fh) or {}


CONFIG = load_config()
