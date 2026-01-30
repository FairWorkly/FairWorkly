import pytest
from openpyxl import Workbook

class TestParseEmployeeExcel:
    """Tests for parse_employee_excel() function."""

    def test_parse_valid_employees(self, handler, employee_excel):
        """Test parsing a valid employee file."""
        response = handler.parse_employee_excel(str(employee_excel))

        result, errors = response.result, response.issues
        entries = result.entries

        assert len(entries) == 2
        assert len(errors) == 0

        assert entries[0].name == "John Smith"
        assert entries[0].email == "john@example.com"
        assert entries[0].role == "Manager"
        assert entries[0].department == "Sales"
        assert entries[0].excel_row == 2

