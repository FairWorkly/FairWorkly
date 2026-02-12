using System.Globalization;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Payroll.Services.Models;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Common.Parsing;
using FairWorkly.Domain.Payroll;

namespace FairWorkly.Application.Payroll.Services;

public class CsvValidator : ICsvValidator
{
    private static readonly string[] ExpectedHeaders =
    {
        "Employee ID", "First Name", "Last Name", "Pay Period Start", "Pay Period End", "Pay Date",
        "Award Type", "Classification", "Employment Type", "Hourly Rate",
        "Ordinary Hours", "Ordinary Pay", "Saturday Hours", "Saturday Pay",
        "Sunday Hours", "Sunday Pay", "Public Holiday Hours", "Public Holiday Pay",
        "Gross Pay", "Superannuation Paid",
    };

    public Result<List<ValidatedPayrollRow>> Validate(List<string[]> rows, AwardType awardType, CancellationToken cancellationToken)
    {
        if (rows.Count == 0)
        {
            var emptyErrors = new List<Csv422Error>
            {
                new Csv422Error { RowNumber = 0, Field = "File", Message = "CSV file contains no data" }
            };
            return Result<List<ValidatedPayrollRow>>.Of422("CSV format validation failed", emptyErrors);
        }

        // Stage 1: Header validation
        var headerErrors = ValidateHeader(rows[0]);
        if (headerErrors != null)
            return Result<List<ValidatedPayrollRow>>.Of422("CSV format validation failed", headerErrors);

        // Stage 2: Global validation
        var dataRows = rows.Skip(1).ToList();
        var globalErrors = ValidateGlobal(dataRows);
        if (globalErrors != null)
            return Result<List<ValidatedPayrollRow>>.Of422("CSV format validation failed", globalErrors);

        // Stage 3: Field-level validation
        var (validatedRows, fieldErrors) = ValidateFields(dataRows, awardType, cancellationToken);
        if (fieldErrors != null)
            return Result<List<ValidatedPayrollRow>>.Of422("CSV format validation failed", fieldErrors);

        return Result<List<ValidatedPayrollRow>>.Of200("CSV validation passed", validatedRows!);
    }

    private static List<Csv422Error>? ValidateHeader(string[] headerRow)
    {
        var errors = new List<Csv422Error>();

        if (headerRow.Length != 20)
        {
            errors.Add(new Csv422Error
            {
                RowNumber = 1,
                Field = "Header",
                Message = $"Expected 20 columns, found {headerRow.Length}",
            });
            return errors;
        }

        for (var i = 0; i < ExpectedHeaders.Length; i++)
        {
            if (!string.Equals(headerRow[i].Trim(), ExpectedHeaders[i], StringComparison.Ordinal))
            {
                errors.Add(new Csv422Error
                {
                    RowNumber = 1,
                    Field = "Header",
                    Message = $"Column {i + 1}: expected \"{ExpectedHeaders[i]}\", found \"{headerRow[i].Trim()}\"",
                });
            }
        }

        return errors.Count > 0 ? errors : null;
    }

    private static List<Csv422Error>? ValidateGlobal(List<string[]> dataRows)
    {
        var errors = new List<Csv422Error>();

        // No data rows
        if (dataRows.Count == 0)
        {
            errors.Add(new Csv422Error
            {
                RowNumber = 0,
                Field = "File",
                Message = "CSV file has no data rows",
            });
            return errors;
        }

        // Pay Period consistency (string comparison)
        var firstStart = dataRows[0][3].Trim();
        var firstEnd = dataRows[0][4].Trim();
        var payPeriodInconsistent = dataRows.Any(r => r[3].Trim() != firstStart || r[4].Trim() != firstEnd);
        if (payPeriodInconsistent)
        {
            errors.Add(new Csv422Error
            {
                RowNumber = 0,
                Field = "Pay Period",
                Message = "Pay Period must be the same for all rows",
            });
        }

        // Duplicate Employee ID
        var idGroups = dataRows
            .Select((r, i) => new { Id = r[0].Trim(), Row = i + 2 })
            .Where(x => !string.IsNullOrEmpty(x.Id))
            .GroupBy(x => x.Id)
            .Where(g => g.Count() > 1);

        foreach (var group in idGroups)
        {
            var rowNumbers = string.Join(", ", group.Select(x => x.Row));
            errors.Add(new Csv422Error
            {
                RowNumber = 0,
                Field = "Employee ID",
                Message = $"Duplicate Employee ID '{group.Key}' found in rows: {rowNumbers}",
            });
        }

        if (errors.Count > 0)
            return errors;

        // Pay Period format validation
        if (!DateOnly.TryParseExact(firstStart, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate)
            || !DateOnly.TryParseExact(firstEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
        {
            errors.Add(new Csv422Error
            {
                RowNumber = 0,
                Field = "Pay Period",
                Message = "Invalid Pay Period format. Expected YYYY-MM-DD (e.g., 2026-01-30)",
            });
            return errors;
        }

        // Pay Period Start ≤ End
        if (startDate > endDate)
        {
            errors.Add(new Csv422Error
            {
                RowNumber = 0,
                Field = "Pay Period",
                Message = "Invalid Pay Period. Start date must be on or before end date",
            });
            return errors;
        }

        return null;
    }

    private static (List<ValidatedPayrollRow>?, List<Csv422Error>?) ValidateFields(
        List<string[]> dataRows, AwardType awardType, CancellationToken cancellationToken)
    {
        var errors = new List<Csv422Error>();
        var validatedRows = new List<ValidatedPayrollRow>();

        for (var i = 0; i < dataRows.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var row = dataRows[i];
            var rowNumber = i + 2; // Row 1 is header, data starts from row 2

            // Guard: each data row must have exactly 20 columns
            if (row.Length != 20)
            {
                errors.Add(CreateError(rowNumber, "Row", $"Expected 20 columns, found {row.Length}"));
                continue;
            }

            var validatedRow = new ValidatedPayrollRow();
            var rowHasError = false;

            // Employee ID (col 0)
            var employeeId = row[0].Trim();
            if (string.IsNullOrEmpty(employeeId))
            {
                errors.Add(CreateError(rowNumber, "Employee ID", "Employee ID is required"));
                rowHasError = true;
            }
            else
            {
                validatedRow.EmployeeId = employeeId;
            }

            // First Name (col 1)
            var firstName = row[1].Trim();
            if (string.IsNullOrEmpty(firstName))
            {
                errors.Add(CreateError(rowNumber, "First Name", "First Name is required"));
                rowHasError = true;
            }
            else
            {
                validatedRow.FirstName = firstName;
            }

            // Last Name (col 2)
            var lastName = row[2].Trim();
            if (string.IsNullOrEmpty(lastName))
            {
                errors.Add(CreateError(rowNumber, "Last Name", "Last Name is required"));
                rowHasError = true;
            }
            else
            {
                validatedRow.LastName = lastName;
            }

            // Pay Period Start (col 3) — format and consistency already verified in global validation
            validatedRow.PayPeriodStart = ParseDateToUtc(row[3].Trim());

            // Pay Period End (col 4)
            validatedRow.PayPeriodEnd = ParseDateToUtc(row[4].Trim());

            // Pay Date (col 5)
            var payDateStr = row[5].Trim();
            if (!DateOnly.TryParseExact(payDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                errors.Add(CreateError(rowNumber, "Pay Date",
                    "Invalid Pay Date format. Expected YYYY-MM-DD (e.g., 2026-01-30)"));
                rowHasError = true;
            }
            else
            {
                validatedRow.PayDate = ParseDateToUtc(payDateStr);
            }

            // Award Type (col 6)
            var awardTypeValue = row[6].Trim();
            var expectedShortName = AwardTypeParser.ToShortName(awardType);
            if (!string.Equals(awardTypeValue, expectedShortName, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(CreateError(rowNumber, "Award Type",
                    $"Award Type is required and must be \"{expectedShortName}\" to match your selected award"));
                rowHasError = true;
            }
            else
            {
                validatedRow.AwardType = awardTypeValue;
            }

            // Classification (col 7)
            var classification = row[7].Trim();
            if (string.IsNullOrEmpty(classification) || !IsValidClassification(classification))
            {
                errors.Add(CreateError(rowNumber, "Classification",
                    "Classification is required. Must be Level 1-8"));
                rowHasError = true;
            }
            else
            {
                validatedRow.Classification = classification;
            }

            // Employment Type (col 8)
            var empTypeStr = row[8].Trim();
            if (!EmploymentTypeParser.TryParse(empTypeStr, out var empType))
            {
                errors.Add(CreateError(rowNumber, "Employment Type",
                    "Employment Type is required. Must be one of: full-time, part-time, casual, fixed-term"));
                rowHasError = true;
            }
            else
            {
                validatedRow.EmploymentType = empType;
            }

            // Hourly Rate (col 9) — must be > 0
            if (!TryParsePositiveDecimal(row[9].Trim(), out var hourlyRate))
            {
                errors.Add(CreateError(rowNumber, "Hourly Rate", "Hourly Rate must be a positive number"));
                rowHasError = true;
            }
            else
            {
                validatedRow.HourlyRate = hourlyRate;
            }

            // Ordinary Hours (col 10) — must be >= 0
            if (!TryParseNonNegativeDecimal(row[10].Trim(), out var ordinaryHours))
            {
                errors.Add(CreateError(rowNumber, "Ordinary Hours", "Ordinary Hours must be a number >= 0"));
                rowHasError = true;
            }
            else
            {
                validatedRow.OrdinaryHours = ordinaryHours;
            }

            // Ordinary Pay (col 11) — must be a number
            if (!decimal.TryParse(row[11].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var ordinaryPay))
            {
                errors.Add(CreateError(rowNumber, "Ordinary Pay", "Ordinary Pay must be a number"));
                rowHasError = true;
            }
            else
            {
                validatedRow.OrdinaryPay = ordinaryPay;
            }

            // Saturday Hours/Pay (cols 12-13)
            ValidateOptionalHoursPay(row, 12, 13, "Saturday", rowNumber, validatedRow, errors, ref rowHasError);

            // Sunday Hours/Pay (cols 14-15)
            ValidateOptionalHoursPay(row, 14, 15, "Sunday", rowNumber, validatedRow, errors, ref rowHasError);

            // Public Holiday Hours/Pay (cols 16-17)
            ValidateOptionalHoursPay(row, 16, 17, "Public Holiday", rowNumber, validatedRow, errors, ref rowHasError);

            // Gross Pay (col 18) — must be a number
            if (!decimal.TryParse(row[18].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var grossPay))
            {
                errors.Add(CreateError(rowNumber, "Gross Pay", "Gross Pay must be a number"));
                rowHasError = true;
            }
            else
            {
                validatedRow.GrossPay = grossPay;
            }

            // Superannuation Paid (col 19) — must be a number
            if (!decimal.TryParse(row[19].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var super))
            {
                errors.Add(CreateError(rowNumber, "Superannuation Paid", "Superannuation Paid must be a number"));
                rowHasError = true;
            }
            else
            {
                validatedRow.Superannuation = super;
            }

            if (!rowHasError)
                validatedRows.Add(validatedRow);
        }

        if (errors.Count > 0)
            return (null, errors);

        return (validatedRows, null);
    }

    private static void ValidateOptionalHoursPay(
        string[] row, int hoursCol, int payCol, string label,
        int rowNumber, ValidatedPayrollRow validatedRow, List<Csv422Error> errors, ref bool rowHasError)
    {
        var hoursStr = row[hoursCol].Trim();
        var payStr = row[payCol].Trim();
        decimal hours = 0;
        decimal pay = 0;

        // Hours: optional, defaults to 0; if provided, must be numeric and >= 0
        if (!string.IsNullOrEmpty(hoursStr))
        {
            if (!TryParseNonNegativeDecimal(hoursStr, out hours))
            {
                errors.Add(CreateError(rowNumber, $"{label} Hours", $"{label} Hours must be a number >= 0"));
                rowHasError = true;
                return; // Hours parsing failed, skip Pay check
            }
        }

        // Pay: required when Hours > 0
        if (hours > 0)
        {
            if (string.IsNullOrEmpty(payStr) ||
                !decimal.TryParse(payStr, NumberStyles.Any, CultureInfo.InvariantCulture, out pay))
            {
                errors.Add(CreateError(rowNumber, $"{label} Pay",
                    $"{label} Pay is required when {label} Hours > 0"));
                rowHasError = true;
                return;
            }
        }
        else if (!string.IsNullOrEmpty(payStr))
        {
            if (!decimal.TryParse(payStr, NumberStyles.Any, CultureInfo.InvariantCulture, out pay))
            {
                errors.Add(CreateError(rowNumber, $"{label} Pay", $"{label} Pay must be a number"));
                rowHasError = true;
                return;
            }
        }

        // Assign values
        switch (label)
        {
            case "Saturday":
                validatedRow.SaturdayHours = hours;
                validatedRow.SaturdayPay = pay;
                break;
            case "Sunday":
                validatedRow.SundayHours = hours;
                validatedRow.SundayPay = pay;
                break;
            case "Public Holiday":
                validatedRow.PublicHolidayHours = hours;
                validatedRow.PublicHolidayPay = pay;
                break;
        }
    }

    private static Csv422Error CreateError(int rowNumber, string field, string message) =>
        new() { RowNumber = rowNumber, Field = field, Message = message };

    private static bool TryParsePositiveDecimal(string value, out decimal result)
    {
        result = 0;
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) && result > 0;
    }

    private static bool TryParseNonNegativeDecimal(string value, out decimal result)
    {
        result = 0;
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) && result >= 0;
    }

    private static bool IsValidClassification(string value) =>
        value is "Level 1" or "Level 2" or "Level 3" or "Level 4"
            or "Level 5" or "Level 6" or "Level 7" or "Level 8";

    private static DateTimeOffset ParseDateToUtc(string dateStr) =>
        new(DateOnly.ParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToDateTime(TimeOnly.MinValue),
            TimeSpan.Zero);
}
