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

    def test_invalid_start_date_reports_issue(self, handler, temp_excel_path):
        """Invalid start_date should return INVALID_DATE issue."""
        wb = Workbook()
        ws = wb.active
        ws.append(["Name", "Email", "Role", "Department", "Start Date"])
        ws.append(["John Smith", "john@example.com", "Manager", "Sales", "2023-13-40"])
        wb.save(temp_excel_path)

        response = handler.parse_employee_excel(str(temp_excel_path))

        assert len(response.result.entries) == 0
        assert len(response.issues) == 1
        issue = response.issues[0]
        assert issue.code == "INVALID_DATE"
        assert issue.column == "start_date"
