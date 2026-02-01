import pytest
from openpyxl import Workbook

from agents.roster.services.roster_import import RosterExcelParser

@pytest.fixture
def handler():
    """Create a RosterExcelParser instance."""
    return RosterExcelParser()

@pytest.fixture
def temp_excel_path(tmp_path):
    """Helper to create temporary Excel file path."""
    return tmp_path / "test.xlsx"

@pytest.fixture
def roster_excel(temp_excel_path):
    """Create a valid roster Excel file."""
    wb = Workbook()
    ws = wb.active
    ws.title = "Roster"

    headers = [
        "Employee Email", "Employee Number", "Employee Name", "Employment Type", "Date", "Start Time", "End Time",
        "Has Meal Break", "Meal Break Duration", "Location", "Notes"
    ]
    ws.append(headers)

    ws.append(["john@example.com", "EMP001", "John Smith", "full-time", "2024-01-15", "09:00", "17:00", "Yes", 30, "Office", "Regular shift"])
    ws.append(["jane@example.com", "EMP002", "Jane Doe", "part-time", "2024-01-15", "14:00", "22:00", "Yes", 30, "Store", None])
    ws.append(["bob@example.com", "EMP003", "Bob Wilson", "casual", "2024-01-16", "06:00", "14:00", "No", None, None, "Early shift"])

    wb.save(temp_excel_path)
    return temp_excel_path

@pytest.fixture
def employee_excel(temp_excel_path):
    """Create a valid employee Excel file."""
    wb = Workbook()
    ws = wb.active

    headers = ["Name", "Email", "Role", "Department", "Start Date"]
    ws.append(headers)
    ws.append(["John Smith", "john@example.com", "Manager", "Sales", "2023-01-01"])
    ws.append(["Jane Doe", "jane@example.com", "Developer", "Engineering", "2023-06-15"])

    wb.save(temp_excel_path)
    return temp_excel_path
