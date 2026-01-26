# Roster Excel Parser

A Python utility for parsing roster/shift data from Excel files (.xlsx).

## Quick Start

```python
from agents.shared.roster_excel_parser import RosterExcelParser

# Create parser
parser = RosterExcelParser()

# Parse roster Excel file
result, issues = parser.parse_roster_excel("roster.xlsx")

# Check for errors
for issue in issues:
    if issue.severity == "error":
        print(f"Row {issue.row}: {issue.message}")

# Use the parsed entries
for entry in result.entries:
    print(f"{entry.employee_email}: {entry.date} {entry.start_time}-{entry.end_time}")
```

## Excel File Format

### Required Columns

Your Excel file **must** have these 4 columns:

| Column | Example | Description |
|--------|---------|-------------|
| Employee Email | `john@example.com` | Valid email address |
| Date | `2024-01-15` | Shift date |
| Start Time | `09:00` | When the shift starts |
| End Time | `17:00` | When the shift ends |

### Optional Columns

| Column | Example | Description |
|--------|---------|-------------|
| Employee Number | `EMP001` | Employee ID for secondary matching |
| Employee Name | `John Smith` | Display name (not used for matching) |
| Has Meal Break | `Yes` | Whether shift has a meal break |
| Meal Break Duration | `30` | Meal break length in minutes |
| Has Rest Breaks | `Yes` | Whether shift has rest breaks |
| Rest Breaks Duration | `20` | Rest breaks length in minutes |
| Is Public Holiday | `No` | Whether it's a public holiday |
| Public Holiday Name | `Christmas` | Name of the holiday |
| Is On Call | `No` | Whether it's an on-call shift |
| Location | `Head Office` | Work location |
| Notes | `Training day` | Any additional notes |

## Supported Formats

### Date Formats

All of these work:

- `2024-01-15` (recommended)
- `15/01/2024`
- `01/15/2024`
- `15-01-2024`
- `15.01.2024`

### Time Formats

All of these work:

- `09:00` or `9:00` (recommended)
- `09:00:00`
- `9:00 AM` or `9AM`
- `5:00 PM` or `5PM`
- `17:00`

### Boolean Values (Yes/No fields)

Any of these mean "Yes":
- `Yes`, `yes`, `YES`
- `True`, `true`
- `Y`, `y`
- `1`

Anything else (including empty) means "No".

## Column Header Flexibility

The parser is flexible with column names. These all work:

| What you write | What parser understands |
|----------------|------------------------|
| `Employee Email` | employee_email |
| `employee_email` | employee_email |
| `Staff Email` | employee_email |
| `员工邮箱` | employee_email |
| `Start Time` | start_time |
| `start` | start_time |
| `开始时间` | start_time |

The parser ignores:
- Extra spaces
- UPPERCASE/lowercase
- Special characters like `*`, `#`, `-`, `_`

## Overnight Shifts

If `End Time` is earlier than `Start Time`, the parser automatically detects it as an overnight shift.

**Example:**
- Start Time: `22:00`
- End Time: `06:00`
- Result: `is_overnight = True`

## Common Mistakes to Avoid

### 1. Time Ranges in One Cell

**Wrong:**
```
| Start Time    |
| 9:00-17:00    |   <-- This will fail!
```

**Correct:**
```
| Start Time | End Time |
| 9:00       | 17:00    |
```

### 2. Using .xls Format

Only `.xlsx` files are supported. Save your file as "Excel Workbook (.xlsx)".

### 3. Invalid Email Format

```
| Employee Email    |
| john              |   <-- This will fail!
| john@example.com  |   <-- This works!
```

## Error Handling

The parser returns two things:
1. `result` - Parsed roster entries + summary stats
2. `issues` - List of errors and warnings

```python
result, issues = parser.parse_roster_excel("roster.xlsx")

# Separate errors from warnings
errors = [i for i in issues if i.severity == "error"]
warnings = [i for i in issues if i.severity == "warning"]

print(f"Parsed {len(result.entries)} entries")
print(f"Found {len(errors)} errors and {len(warnings)} warnings")

# Show each error with its Excel row number
for error in errors:
    print(f"Row {error.row}: [{error.code}] {error.message}")
```

### Error Codes

| Code | Meaning |
|------|---------|
| `EMPTY_FILE` | Excel file has no data |
| `MISSING_REQUIRED_COLUMNS` | Required columns not found |
| `MISSING_REQUIRED_FIELD` | Cell is empty but required |
| `INVALID_EMAIL` | Email format is wrong |
| `INVALID_DATE` | Cannot parse date |
| `INVALID_TIME` | Cannot parse time |
| `ROW_PARSE_ERROR` | General parsing error |

### Warning Codes

| Code | Meaning |
|------|---------|
| `MEAL_BREAK_DURATION_MISSING` | Has meal break = Yes, but duration is empty |
| `REST_BREAKS_DURATION_MISSING` | Has rest breaks = Yes, but duration is empty |

## JSON Output

When converting to JSON (for API responses), the parser uses **camelCase**:

```python
entry = result.entries[0]
print(entry.model_dump(by_alias=True))
```

Output:
```json
{
  "excelRow": 2,
  "employeeEmail": "john@example.com",
  "employeeNumber": "EMP001",
  "employeeName": "John Smith",
  "date": "2024-01-15",
  "startTime": "09:00:00",
  "endTime": "17:00:00",
  "isOvernight": false,
  "hasMealBreak": true,
  "mealBreakDuration": 30,
  "hasRestBreaks": false,
  "restBreaksDuration": null,
  "isPublicHoliday": false,
  "publicHolidayName": null,
  "isOnCall": false,
  "location": "Head Office",
  "notes": "Regular shift"
}
```

## Template

A ready-to-use Excel template is available at:

```
agent-service/templates/roster_template.xlsx
```

This template includes:
- All columns with proper headers
- Sample data rows
- Instructions sheet
- Color-coded required vs optional columns

## Full Example

```python
from agents.shared.roster_excel_parser import RosterExcelParser

def import_roster(file_path: str):
    parser = RosterExcelParser()

    try:
        result, issues = parser.parse_roster_excel(file_path)
    except FileNotFoundError:
        print(f"File not found: {file_path}")
        return
    except ValueError as e:
        print(f"Invalid file: {e}")
        return

    # Report issues
    errors = [i for i in issues if i.severity == "error"]
    warnings = [i for i in issues if i.severity == "warning"]

    if errors:
        print(f"Found {len(errors)} errors:")
        for err in errors:
            print(f"  Row {err.row}: {err.message}")

    if warnings:
        print(f"Found {len(warnings)} warnings:")
        for warn in warnings:
            print(f"  Row {warn.row}: {warn.message}")

    # Process valid entries
    print(f"\nSuccessfully parsed {len(result.entries)} shifts:")
    for entry in result.entries:
        overtime = " (overnight)" if entry.is_overnight else ""
        print(f"  {entry.employee_email}: {entry.date} {entry.start_time}-{entry.end_time}{overtime}")

    return result

# Usage
result = import_roster("my_roster.xlsx")
```

## Dependencies

- `openpyxl` - Excel file reading
- `pydantic` - Data validation and serialization
