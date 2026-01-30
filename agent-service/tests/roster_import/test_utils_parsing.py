import pytest
from datetime import date, time

from agents.roster.services.roster_import.utils import (
    parse_date,
    parse_time,
    parse_boolean,
    parse_int,
    normalize_employment_type,
)

class TestParseDateHelper:
    """Tests for _parse_date() helper."""

    def test_parse_iso_format(self, handler):
        """Test parsing YYYY-MM-DD format."""
        result = parse_date("2024-01-15")
        assert result == date(2024, 1, 15)

    def test_parse_slash_format_dmy(self, handler):
        """Test parsing DD/MM/YYYY format."""
        result = parse_date("15/01/2024")
        assert result == date(2024, 1, 15)

    def test_parse_dash_format_dmy(self, handler):
        """Test parsing DD-MM-YYYY format."""
        result = parse_date("15-01-2024")
        assert result == date(2024, 1, 15)

    def test_parse_datetime_object(self, handler):
        """Test parsing datetime object."""
        from datetime import datetime
        dt = datetime(2024, 1, 15, 10, 30)
        result = parse_date(dt)
        assert result == date(2024, 1, 15)

    def test_parse_date_object(self, handler):
        """Test parsing date object."""
        d = date(2024, 1, 15)
        result = parse_date(d)
        assert result == date(2024, 1, 15)

    def test_parse_none(self, handler):
        """Test parsing None returns None."""
        result = parse_date(None)
        assert result is None

    def test_parse_invalid_date(self, handler):
        """Test parsing invalid date raises error."""
        with pytest.raises(ValueError, match="Unable to parse date"):
            parse_date("not-a-date")

class TestParseTimeHelper:
    """Tests for _parse_time() helper."""

    def test_parse_24hour_format(self, handler):
        """Test parsing HH:MM format."""
        result = parse_time("14:30")
        assert result == time(14, 30)

    def test_parse_24hour_with_seconds(self, handler):
        """Test parsing HH:MM:SS format."""
        result = parse_time("14:30:45")
        assert result == time(14, 30, 45)

    def test_parse_12hour_pm(self, handler):
        """Test parsing 12-hour PM format."""
        result = parse_time("2:30 PM")
        assert result == time(14, 30)

    def test_parse_12hour_am(self, handler):
        """Test parsing 12-hour AM format."""
        result = parse_time("9:30 AM")
        assert result == time(9, 30)

    def test_parse_12_pm(self, handler):
        """Test parsing 12:00 PM (noon)."""
        result = parse_time("12:00 PM")
        assert result == time(12, 0)

    def test_parse_12_am(self, handler):
        """Test parsing 12:00 AM (midnight)."""
        result = parse_time("12:00 AM")
        assert result == time(0, 0)

    def test_parse_hour_only(self, handler):
        """Test parsing hour-only formats like 9, 9AM, 9 PM."""
        result = parse_time("9")
        assert result == time(9, 0)

        result = parse_time("9AM")
        assert result == time(9, 0)

        result = parse_time("9 PM")
        assert result == time(21, 0)

    def test_parse_excel_time_float(self, handler):
        """Test parsing Excel time as float (fraction of day)."""
        # 0.5 = 12:00 (noon)
        result = parse_time(0.5)
        assert result == time(12, 0, 0)

        # 0.25 = 06:00
        result = parse_time(0.25)
        assert result == time(6, 0, 0)

    def test_parse_time_object(self, handler):
        """Test parsing time object."""
        t = time(14, 30)
        result = parse_time(t)
        assert result == time(14, 30)

    def test_parse_none(self, handler):
        """Test parsing None returns None."""
        result = parse_time(None)
        assert result is None

class TestParseBooleanHelper:
    """Tests for _parse_boolean() helper."""

    def test_parse_true_values(self, handler):
        """Test various true representations."""
        assert parse_boolean(True) is True
        assert parse_boolean("true") is True
        assert parse_boolean("True") is True
        assert parse_boolean("yes") is True
        assert parse_boolean("Yes") is True
        assert parse_boolean("Y") is True
        assert parse_boolean("1") is True
        assert parse_boolean(1) is True

    def test_parse_false_values(self, handler):
        """Test various false representations."""
        assert parse_boolean(False) is False
        assert parse_boolean("false") is False
        assert parse_boolean("no") is False
        assert parse_boolean("N") is False
        assert parse_boolean("0") is False
        assert parse_boolean(0) is False
        assert parse_boolean(None) is False
        assert parse_boolean("") is False

class TestParseIntHelper:
    """Tests for _parse_int() helper."""

    def test_parse_int(self, handler):
        """Test parsing integer."""
        value, warning = parse_int(30)
        assert value == 30
        assert warning is None

    def test_parse_float(self, handler):
        """Test parsing float rounds to int with warning."""
        value, warning = parse_int(30.5)
        assert value == 30
        assert warning is not None
        assert "decimal part" in warning

    def test_parse_string(self, handler):
        """Test parsing numeric string."""
        value, warning = parse_int("30")
        assert value == 30
        assert warning is None

        value, warning = parse_int("30.5")
        assert value == 30
        assert warning is not None
        assert "decimal part" in warning

    def test_parse_none(self, handler):
        """Test parsing None returns None."""
        value, warning = parse_int(None)
        assert value is None
        assert warning is None

    def test_parse_invalid(self, handler):
        """Test parsing invalid string returns None."""
        value, warning = parse_int("not-a-number")
        assert value is None
        assert warning is None

        value, warning = parse_int("")
        assert value is None
        assert warning is None

class TestEmploymentTypeNormalization:
    def test_normalize_employment_type(self):
        assert normalize_employment_type("FT") == "full-time"
        assert normalize_employment_type("full time") == "full-time"
        assert normalize_employment_type("Part-Time") == "part-time"
        assert normalize_employment_type("casual") == "casual"
        assert normalize_employment_type("unknown") is None
