"""Parsing utility helpers."""

from __future__ import annotations

from typing import Any, Optional
from datetime import date, time, datetime
import re


_EMPLOYMENT_TYPE_MAP = {
    "fulltime": "full-time",
    "full-time": "full-time",
    "full_time": "full-time",
    "ft": "full-time",
    "parttime": "part-time",
    "part-time": "part-time",
    "part_time": "part-time",
    "pt": "part-time",
    "casual": "casual",
    "cas": "casual",
}


def get_string(value: Any) -> Optional[str]:
    """Convert value to string, return None if empty."""
    if value is None:
        return None
    result = str(value).strip()
    return result if result else None


def normalize_employment_type(value: Optional[str]) -> Optional[str]:
    """Normalize employment type values to canonical forms."""
    if not value:
        return None
    key = value.strip().lower().replace(" ", "").replace("-", "_")
    key = key.replace("_", "")
    return _EMPLOYMENT_TYPE_MAP.get(key)


def parse_date(value: Any) -> Optional[date]:
    """
    Parse various date formats into a date object.
    Supports Excel serial date numbers (e.g., 44927.0 = 2023-01-01).
    """
    if value is None:
        return None

    if isinstance(value, datetime):
        return value.date()

    if isinstance(value, date):
        return value

    # Excel serial date number (numeric)
    if isinstance(value, (int, float)):
        try:
            from datetime import timedelta
            # Excel epoch is 1899-12-30 (for Windows Excel)
            # Note: Excel has a bug where it treats 1900 as a leap year
            excel_epoch = datetime(1899, 12, 30)
            days = int(value)
            return (excel_epoch + timedelta(days=days)).date()
        except (ValueError, OverflowError) as exc:
            raise ValueError(f"Invalid Excel date number: {value}") from exc

    if isinstance(value, str):
        value = value.strip()
        if not value:
            return None

        formats = [
            "%Y-%m-%d",
            "%d/%m/%Y",
            "%m/%d/%Y",
            "%d-%m-%Y",
            "%Y/%m/%d",
            "%d.%m.%Y",
        ]

        for fmt in formats:
            try:
                return datetime.strptime(value, fmt).date()
            except ValueError:
                continue

        raise ValueError(f"Unable to parse date: {value}. Expected formats: YYYY-MM-DD, DD/MM/YYYY, MM/DD/YYYY, etc.")

    raise ValueError(f"Invalid date type: {type(value).__name__}")


def parse_time(value: Any) -> Optional[time]:
    """
    Parse various time formats into a time object.
    """
    if value is None:
        return None

    if isinstance(value, time):
        return value

    if isinstance(value, datetime):
        return value.time()

    if isinstance(value, (int, float)):
        total_seconds = int(round(float(value) * 24 * 60 * 60))
        total_seconds = total_seconds % (24 * 60 * 60)
        hours = (total_seconds // 3600) % 24
        minutes = (total_seconds % 3600) // 60
        seconds = total_seconds % 60
        return time(hours, minutes, seconds)

    if isinstance(value, str):
        value_stripped = value.strip()
        if not value_stripped:
            return None

        # Detect time ranges like "9:00-17:00", "9-5", "9:00 to 17:00"
        # Updated pattern to catch ranges without spaces
        time_range_pattern = r"\d{1,2}(?::\d{2})?\s*(?:[-~～–—]|\bto\b)\s*\d{1,2}(?::\d{2})?"
        if re.search(time_range_pattern, value_stripped, re.IGNORECASE):
            raise ValueError(
                f"Time range detected: '{value_stripped}'. "
                "Please use separate 'Start Time' and 'End Time' columns."
            )

        value = value_stripped.upper()

        is_pm = "PM" in value
        is_am = "AM" in value
        value = value.replace("AM", "").replace("PM", "").strip()

        if value.isdigit():
            hour = int(value)
            if is_pm and hour < 12:
                hour += 12
            elif is_am and hour == 12:
                hour = 0
            if 0 <= hour <= 23:
                return time(hour, 0, 0)

        formats = [
            "%H:%M:%S",
            "%H:%M",
            "%I:%M:%S",
            "%I:%M",
        ]

        for fmt in formats:
            try:
                parsed = datetime.strptime(value, fmt).time()
                if is_pm and parsed.hour < 12:
                    parsed = time(parsed.hour + 12, parsed.minute, parsed.second)
                elif is_am and parsed.hour == 12:
                    parsed = time(0, parsed.minute, parsed.second)
                return parsed
            except ValueError:
                continue

        raise ValueError(f"Unable to parse time: {value}")

    raise ValueError(f"Invalid time type: {type(value)}")


def parse_boolean(value: Any) -> bool:
    """
    Parse various boolean representations.
    Handles strings, numbers, and boolean values consistently.
    """
    if value is None:
        return False

    if isinstance(value, bool):
        return value

    if isinstance(value, (int, float)):
        return bool(value)

    if isinstance(value, str):
        value = value.strip().lower()
        # Explicit true values
        if value in ("true", "yes", "y", "1", "on"):
            return True
        # Explicit false values
        if value in ("false", "no", "n", "0", "off", ""):
            return False
        # Try to parse as number for consistency with numeric handling
        try:
            return bool(float(value))
        except ValueError:
            # If not a number and not in explicit lists, default to False
            return False

    return False


def parse_int(value: Any, allow_fractional: bool = False) -> tuple[Optional[int], Optional[str]]:
    """
    Parse value as integer, return (result, warning_message).

    Args:
        value: Value to parse
        allow_fractional: If False, warn when fractional values are rounded

    Returns:
        Tuple of (parsed_int, warning_message). warning_message is None if no issues.
    """
    if value is None:
        return None, None

    if isinstance(value, int):
        return value, None

    if isinstance(value, float):
        # Check if value has fractional part
        if not allow_fractional and value != int(value):
            rounded = int(round(value))
            warning = f"Value {value} has decimal part, rounded to {rounded}"
            return rounded, warning
        return int(round(value)), None

    if isinstance(value, str):
        value = value.strip()
        if not value:
            return None, None
        try:
            float_val = float(value)
            # Check if string represents a fractional number
            if not allow_fractional and float_val != int(float_val):
                rounded = int(round(float_val))
                warning = f"Value '{value}' has decimal part, rounded to {rounded}"
                return rounded, warning
            return int(round(float_val)), None
        except ValueError:
            return None, None

    return None, None


def build_raw_row(normalized: dict[str, Any], row_num: int, extra_key: str) -> dict[str, Any]:
    """Build a display-friendly raw row for UI rendering."""
    raw: dict[str, Any] = {"excel_row": row_num}
    for key, value in normalized.items():
        if key == "__row__":
            continue
        if key == extra_key and isinstance(value, dict):
            for extra_key_name, extra_value in value.items():
                key_to_use = extra_key_name
                if key_to_use in raw:
                    key_to_use = f"{extra_key_name} (extra)"
                raw[key_to_use] = extra_value
            continue
        if isinstance(value, (datetime, date, time)):
            raw[key] = value.isoformat()
        else:
            raw[key] = value
    return raw
