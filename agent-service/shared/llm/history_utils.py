"""Utilities for sanitizing chat history passed from the frontend."""

from __future__ import annotations

from typing import Any, Dict, List

ALLOWED_ROLES = {"user", "assistant"}


def normalize_chat_history(
    raw_history: Any,
    *,
    max_messages: int = 12,
    max_content_chars: int = 1200,
) -> List[Dict[str, str]]:
    """Return normalized history in OpenAI/Anthropic-compatible message format."""
    if not isinstance(raw_history, list):
        return []

    normalized: List[Dict[str, str]] = []
    for item in raw_history:
        if not isinstance(item, dict):
            continue

        role_value = item.get("role", item.get("Role", ""))
        if not isinstance(role_value, str):
            continue
        role = role_value.strip().lower()
        if role not in ALLOWED_ROLES:
            continue

        content_value = item.get("content", item.get("Content", ""))
        if not isinstance(content_value, str):
            continue
        content = content_value.strip()
        if not content:
            continue

        normalized.append(
            {
                "role": role,
                "content": content[:max_content_chars],
            }
        )

    if max_messages <= 0:
        return []

    return normalized[-max_messages:]
