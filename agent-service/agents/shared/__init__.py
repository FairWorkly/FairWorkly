"""DEPRECATED: Shim package to re-export shared as agents.shared.

Keep for 1–2 iterations to support external scripts/notebooks/older branches.
Remove once all consumers migrate to `shared.*` imports.
"""

import importlib
import sys

_shared = importlib.import_module("shared")
sys.modules[__name__] = _shared
