# Roster Import Module

A modular Python package for parsing roster/employee Excel files (.xlsx) with comprehensive validation and error handling.

## Module Structure

```text
roster_import/
├── __init__.py       # Public exports
├── parser.py         # RosterExcelParser orchestrator
├── models.py         # Pydantic models & enums
├── excel_reader.py   # Low-level Excel reading
├── header_map.py     # Header alias mapping
├── row_parsers.py    # Row-level parsing logic
├── issues.py         # Issue aggregation & response building
└── utils.py          # Parsing utilities (date, time, int, etc.)
```

## Quick Start

```python
from agents.roster.services.roster_import import RosterExcelParser, ParseMode

# Create parser
parser = RosterExcelParser()

# Parse roster Excel file
response = parser.parse_roster_excel("roster.xlsx", mode=ParseMode.LENIENT)
result, issues = response.result, response.issues

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

| Column         | Example            | Description         |
| -------------- | ------------------ | ------------------- |
| Employee Email | `john@example.com` | Valid email address |
| Date           | `2024-01-15`       | Shift date          |
| Start Time     | `09:00`            | When the shift starts |
| End Time       | `17:00`            | When the shift ends |
### Optional Columns

| Column | Example | Description |
|--------|---------|-------------|
| Employee Number | `EMP001` | Employee ID for secondary matching |
| Employee Name | `John Smith` | Display name (not used for matching) |
| Employment Type | `full-time` | full-time / part-time / casual |
| Is Overnight | `No` | Explicitly mark overnight shifts |
| Has Meal Break | `Yes` | Whether shift has a meal break |
| Meal Break Duration | `30` | Meal break length in minutes |
| Has Rest Breaks | `Yes` | Whether shift has rest breaks |
| Rest Breaks Duration | `20` | Rest breaks length in minutes |
| Is Public Holiday | `No` | Whether it's a public holiday |
| Public Holiday Name | `Christmas` | Name of the holiday |
| Is On Call | `No` | Whether it's an on-call shift |
| Location | `Head Office` | Work location |
| Notes | `Training day` | Any additional notes |

The provided template includes `Employment Type` and `Is Overnight` columns (appended at the end).

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

The parser normalizes:
- Extra spaces
- UPPERCASE/lowercase
Special characters and full‑width variants are preserved, so include explicit aliases if needed.

## Overnight Shifts

If `End Time` is earlier than `Start Time`, the parser detects it as an overnight shift.
If the `Is Overnight` column is present, its value is respected and suppresses the warning.

**Example:**
- Start Time: `22:00`
- End Time: `06:00`
- Result: `is_overnight = True`

## Common Mistakes to Avoid

### 1. Time Ranges in One Cell

**Wrong:**
```text
| Start Time    |
| 9:00-17:00    |   <-- This will fail!
```

**Correct:**
```text
| Start Time | End Time |
| 9:00       | 17:00    |
```

### 2. Using .xls Format

Only `.xlsx` files are supported. Save your file as "Excel Workbook (.xlsx)".

### 3. Invalid Email Format

```text
| Employee Email    |
| john              |   <-- This will fail!
| john@example.com  |   <-- This works!
```

## Error Handling

The parser returns a `ParseResponse`:
1. `result` - Parsed roster entries + summary stats
2. `issues` - List of errors and warnings
3. `summary` - Overall status counts
4. `issue_summary` - Aggregated issue groups

```python
response = parser.parse_roster_excel("roster.xlsx")
result, issues = response.result, response.issues

# Separate errors from warnings
errors = [i for i in issues if i.severity == "error"]
warnings = [i for i in issues if i.severity == "warning"]

print(f"Parsed {len(result.entries)} entries")
print(f"Found {len(errors)} errors and {len(warnings)} warnings")
print(f"Overall status: {response.summary.status}")

# Show each error with its Excel row number
for error in errors:
    print(f"Row {error.row}: [{error.code}] {error.message}")
```

### Blocking Error Codes (row=0)

| Code | Meaning |
|------|---------|
| `FILE_NOT_FOUND` | File does not exist |
| `FILE_READ_ERROR` | Cannot read/open file |
| `INVALID_HEADER_ROW` | Specified header row not found |
| `EMPTY_FILE` | Excel file has no data |
| `MISSING_REQUIRED_COLUMNS` | Required columns not found |

### Row-Level Error Codes

| Code | Meaning |
|------|---------|
| `MISSING_REQUIRED_FIELD` | Cell is empty but required |
| `INVALID_EMAIL` | Email format is wrong |
| `INVALID_DATE` | Cannot parse date |
| `INVALID_TIME` | Cannot parse time |
| `TIME_RANGE_DETECTED` | Time range in single cell (e.g., "9:00-17:00") |
| `ROW_PARSE_ERROR` | General parsing error |

### Warning Codes

| Code | Meaning |
|------|---------|
| `MEAL_BREAK_DURATION_MISSING` | has_meal_break=true but duration is empty |
| `REST_BREAKS_DURATION_MISSING` | has_rest_breaks=true but duration is empty |
| `EMPLOYMENT_TYPE_MISSING` | Employment type not specified |
| `EMPLOYMENT_TYPE_INVALID` | Unrecognized employment type |
| `OVERNIGHT_ASSUMED` | End time < start time, overnight inferred |
| `DUPLICATE_CANONICAL_COLUMN_IN_ROW` | Multiple headers map to same field |
| `FRACTIONAL_VALUE_ROUNDED` | Decimal value rounded to integer |

## header_row and raw_rows (UI Preview)

If your spreadsheet has a title row or blank rows above the header, pass `header_row` (1-based).

```python
response = parser.parse_roster_excel("roster.xlsx", header_row=2)
```

For UI preview, use `response.result.raw_rows`. It preserves original columns (including unknown headers)
and includes `excel_row` for mapping issues to cells.

```python
for row in response.result.raw_rows:
    print(row["excel_row"], row)
```

## Parsing Modes

Use `ParseMode.STRICT` to turn select warnings into errors (e.g., inferred overnight, missing/unknown employment type).
Default is `ParseMode.LENIENT`.

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

## Computed Fields

### RosterEntry

```python
entry.duration_hours  # Gross shift hours (handles overnight)
entry.net_hours       # Duration minus breaks
```

### RosterParseResult

```python
result.week_start_date   # Earliest date
result.week_end_date     # Latest date
result.total_shifts      # Count of entries
result.total_hours       # Sum of duration_hours
result.unique_employees  # Distinct email count
```

## Template

A ready-to-use Excel template is available at:

```
agent-service/shared/assets/templates/roster_template.xlsx
```

This template includes:
- All columns with proper headers
- Sample data rows
- Instructions sheet
- Color-coded required vs optional columns

## Full Example

```python
from agents.roster.services.roster_import import RosterExcelParser, ParseMode

def import_roster(file_path: str):
    parser = RosterExcelParser()
    response = parser.parse_roster_excel(file_path, mode=ParseMode.LENIENT)

    # Check for blocking errors (file not found, missing columns, etc.)
    if response.summary.status == "blocking":
        print(f"Cannot proceed: {response.issues[0].message}")
        return None

    result = response.result

    # Report issues
    print(f"Status: {response.summary.status}")
    print(f"Errors: {response.summary.error_count}, Warnings: {response.summary.warning_count}")

    for issue in response.issues:
        prefix = "ERROR" if issue.severity == "error" else "WARN"
        print(f"  [{prefix}] Row {issue.row}: {issue.message}")

    # Process valid entries
    print(f"\nSuccessfully parsed {result.total_shifts} shifts from {result.unique_employees} employees")
    print(f"Date range: {result.week_start_date} to {result.week_end_date}")
    print(f"Total hours: {result.total_hours}")

    for entry in result.entries:
        overnight = " (overnight)" if entry.is_overnight else ""
        print(f"  {entry.employee_email}: {entry.date} {entry.start_time}-{entry.end_time}{overnight}")
        print(f"    Duration: {entry.duration_hours}h, Net: {entry.net_hours}h")

    return result

# Usage
result = import_roster("my_roster.xlsx")
```

## FastAPI Integration

```python
from fastapi import APIRouter, UploadFile
from agents.roster.services.roster_import import RosterExcelParser, ParseMode

router = APIRouter()
parser = RosterExcelParser()

@router.post("/roster/upload")
async def upload_roster(file: UploadFile):
    import tempfile
    import os

    # Save to temp file
    with tempfile.NamedTemporaryFile(delete=False, suffix=".xlsx") as tmp:
        content = await file.read()
        tmp.write(content)
        tmp_path = tmp.name

    try:
        response = parser.parse_roster_excel(tmp_path, mode=ParseMode.LENIENT)
        return response.model_dump(by_alias=True)  # camelCase JSON
    finally:
        os.unlink(tmp_path)
```

## Dependencies

- `openpyxl` - Excel file reading
- `pydantic` - Data validation and serialization
