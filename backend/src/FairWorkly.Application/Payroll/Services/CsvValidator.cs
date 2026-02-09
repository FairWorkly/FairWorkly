using System.Globalization;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Payroll.Services.Models;
using FairWorkly.Domain.Common;
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

    public Result<List<ValidatedPayrollRow>> Validate(List<string[]> rows, string awardType)
    {
        // 阶段 1：表头验证
        var headerErrors = ValidateHeader(rows[0]);
        if (headerErrors != null)
            return Result<List<ValidatedPayrollRow>>.ValidationFailure("CSV format validation failed", headerErrors);

        // 阶段 2：全局验证
        var dataRows = rows.Skip(1).ToList();
        var globalErrors = ValidateGlobal(dataRows);
        if (globalErrors != null)
            return Result<List<ValidatedPayrollRow>>.ValidationFailure("CSV format validation failed", globalErrors);

        // 阶段 3：字段验证
        var (validatedRows, fieldErrors) = ValidateFields(dataRows, awardType);
        if (fieldErrors != null)
            return Result<List<ValidatedPayrollRow>>.ValidationFailure("CSV format validation failed", fieldErrors);

        return Result<List<ValidatedPayrollRow>>.Success(validatedRows!);
    }

    private static List<ValidationError>? ValidateHeader(string[] headerRow)
    {
        var errors = new List<ValidationError>();

        if (headerRow.Length != 20)
        {
            errors.Add(new CsvValidationError
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
                errors.Add(new CsvValidationError
                {
                    RowNumber = 1,
                    Field = "Header",
                    Message = $"Column {i + 1}: expected \"{ExpectedHeaders[i]}\", found \"{headerRow[i].Trim()}\"",
                });
            }
        }

        return errors.Count > 0 ? errors : null;
    }

    private static List<ValidationError>? ValidateGlobal(List<string[]> dataRows)
    {
        var errors = new List<ValidationError>();

        // 无数据行
        if (dataRows.Count == 0)
        {
            errors.Add(new CsvValidationError
            {
                RowNumber = 0,
                Field = "File",
                Message = "CSV file has no data rows",
            });
            return errors;
        }

        // Pay Period 一致性（string 比较）
        var firstStart = dataRows[0][3].Trim();
        var firstEnd = dataRows[0][4].Trim();
        var payPeriodInconsistent = dataRows.Any(r => r[3].Trim() != firstStart || r[4].Trim() != firstEnd);
        if (payPeriodInconsistent)
        {
            errors.Add(new CsvValidationError
            {
                RowNumber = 0,
                Field = "Pay Period",
                Message = "Pay Period must be the same for all rows",
            });
        }

        // Employee ID 重复
        var idGroups = dataRows
            .Select((r, i) => new { Id = r[0].Trim(), Row = i + 2 })
            .Where(x => !string.IsNullOrEmpty(x.Id))
            .GroupBy(x => x.Id)
            .Where(g => g.Count() > 1);

        foreach (var group in idGroups)
        {
            var rowNumbers = string.Join(", ", group.Select(x => x.Row));
            errors.Add(new CsvValidationError
            {
                RowNumber = 0,
                Field = "Employee ID",
                Message = $"Duplicate Employee ID '{group.Key}' found in rows: {rowNumbers}",
            });
        }

        if (errors.Count > 0)
            return errors;

        // Pay Period 格式验证
        if (!DateOnly.TryParseExact(firstStart, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate)
            || !DateOnly.TryParseExact(firstEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
        {
            errors.Add(new CsvValidationError
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
            errors.Add(new CsvValidationError
            {
                RowNumber = 0,
                Field = "Pay Period",
                Message = "Invalid Pay Period. Start date must be on or before end date",
            });
            return errors;
        }

        return null;
    }

    private static (List<ValidatedPayrollRow>?, List<ValidationError>?) ValidateFields(
        List<string[]> dataRows, string awardType)
    {
        var errors = new List<ValidationError>();
        var validatedRows = new List<ValidatedPayrollRow>();

        for (var i = 0; i < dataRows.Count; i++)
        {
            var row = dataRows[i];
            var rowNumber = i + 2; // 行号：1 是表头，2 起是数据
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

            // Pay Period Start (col 3) — 已在全局验证中确认格式和一致性
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
            if (!string.Equals(awardTypeValue, awardType, StringComparison.Ordinal))
            {
                errors.Add(CreateError(rowNumber, "Award Type",
                    $"Award Type is required and must be \"{awardType}\" to match your selected award"));
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
        int rowNumber, ValidatedPayrollRow validatedRow, List<ValidationError> errors, ref bool rowHasError)
    {
        var hoursStr = row[hoursCol].Trim();
        var payStr = row[payCol].Trim();
        decimal hours = 0;
        decimal pay = 0;

        // Hours: 可不填默认 0；填了必须是数值且 >= 0
        if (!string.IsNullOrEmpty(hoursStr))
        {
            if (!TryParseNonNegativeDecimal(hoursStr, out hours))
            {
                errors.Add(CreateError(rowNumber, $"{label} Hours", $"{label} Hours must be a number >= 0"));
                rowHasError = true;
                return; // Hours 解析失败，不再检查 Pay
            }
        }

        // Pay: Hours > 0 时必填
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
            decimal.TryParse(payStr, NumberStyles.Any, CultureInfo.InvariantCulture, out pay);
        }

        // 赋值
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

    private static CsvValidationError CreateError(int rowNumber, string field, string message) =>
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
