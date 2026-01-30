"""Header alias mapping and normalization helpers."""

from __future__ import annotations

import re
from typing import Any, Optional


EXTRA_KEY = "__extra__"

# Header aliases: canonical_key -> list of accepted variations
# Note: Roster uses "employee_email" prefix, Employee uses "email"
# This avoids alias conflicts between the two file types
HEADER_ALIASES: dict[str, list[str]] = {
    # Roster-specific fields - simplified for template-based imports
    # Only supports template column names with case-insensitive + whitespace tolerance
    "employee_email": ["employee email", "employee_email"],
    "employee_number": ["employee number", "employee_number"],
    "employee_name": ["employee name", "employee_name"],
    "employment_type": ["employment type", "employment_type"],
    "date": ["date"],
    "start_time": ["start time", "start_time"],
    "end_time": ["end time", "end_time"],
    "is_overnight": ["is overnight", "is_overnight"],
    "has_meal_break": ["has meal break", "has_meal_break"],
    "meal_break_duration": ["meal break duration", "meal_break_duration"],
    "has_rest_breaks": ["has rest breaks", "has_rest_breaks"],
    "rest_breaks_duration": ["rest breaks duration", "rest_breaks_duration"],
    "is_public_holiday": ["is public holiday", "is_public_holiday"],
    "public_holiday_name": ["public holiday name", "public_holiday_name"],
    "is_on_call": ["is on call", "is_on_call"],
    "location": ["location"],
    "notes": ["notes"],
    # Employee-specific fields
    "name": ["name"],
    "email": ["email"],
    "role": ["role"],
    "department": ["department"],
    "start_date": ["start date", "start_date"],
}


def normalize_header(header: str) -> str:
    """
    Normalize header string for robust matching.

    Simplified for template-based imports:
    - Case-insensitive: "Employee Email" -> "employee email"
    - Multiple spaces: "Employee  Email" -> "employee email"
    - Leading/trailing whitespace trimmed
    """
    if not header:
        return ""
    header = header.lower()
    header = re.sub(r"\s+", " ", header)
    return header.strip()


class HeaderMap:
    """Maps raw header strings to canonical keys."""

    def __init__(self, aliases: Optional[dict[str, list[str]]] = None) -> None:
        self._aliases = aliases or HEADER_ALIASES
        self._alias_to_canonical: dict[str, str] = {}
        for canonical, alias_list in self._aliases.items():
            for alias in alias_list:
                self._alias_to_canonical[normalize_header(alias)] = canonical

    def normalize_headers(self, row: dict[str, Any]) -> dict[str, Any]:
        """
        Normalize header names to canonical keys using alias mapping.

        Returns a new dict with canonical keys.
        """
        normalized: dict[str, Any] = {}
        extras: dict[str, Any] = {}
        for key, value in row.items():
            if key == "__row__":
                normalized[key] = value
                continue
            key_normalized = normalize_header(str(key))
            canonical = self._alias_to_canonical.get(key_normalized)
            if canonical:
                normalized[canonical] = value
            else:
                extras[str(key)] = value
        if extras:
            normalized[EXTRA_KEY] = extras
        return normalized

    def detect_duplicate_canonical_headers(self, row: dict[str, Any]) -> dict[str, list[str]]:
        """Return canonical keys that appear more than once in the row."""
        seen: dict[str, str] = {}
        duplicates: dict[str, list[str]] = {}
        for key in row.keys():
            if key == "__row__":
                continue
            key_normalized = normalize_header(str(key))
            canonical = self._alias_to_canonical.get(key_normalized)
            if not canonical:
                continue
            if canonical in seen:
                if canonical not in duplicates:
                    duplicates[canonical] = [seen[canonical]]
                duplicates[canonical].append(str(key))
            else:
                seen[canonical] = str(key)
        return duplicates

    def normalize_and_detect_duplicates(
        self, row: dict[str, Any]
    ) -> tuple[dict[str, Any], dict[str, list[str]]]:
        """
        Normalize headers and detect duplicates in a single pass.

        Returns (normalized_dict, duplicates_dict) where:
        - normalized_dict: Row with canonical keys
        - duplicates_dict: Canonical keys that appear multiple times
        """
        normalized: dict[str, Any] = {}
        extras: dict[str, Any] = {}
        seen: dict[str, str] = {}
        duplicates: dict[str, list[str]] = {}

        for key, value in row.items():
            if key == "__row__":
                normalized[key] = value
                continue

            key_normalized = normalize_header(str(key))
            canonical = self._alias_to_canonical.get(key_normalized)

            if canonical:
                # Check for duplicates
                if canonical in seen:
                    if canonical not in duplicates:
                        duplicates[canonical] = [seen[canonical]]
                    duplicates[canonical].append(str(key))
                else:
                    seen[canonical] = str(key)

                # Normalize
                normalized[canonical] = value
            else:
                # Extra column
                extras[str(key)] = value

        if extras:
            normalized[EXTRA_KEY] = extras

        return normalized, duplicates
