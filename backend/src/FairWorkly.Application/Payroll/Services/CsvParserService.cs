using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Payroll.Services.Models;

namespace FairWorkly.Application.Payroll.Services;

/// <summary>
/// Implementation of CSV parser service using CsvHelper library
/// </summary>
public class CsvParserService : ICsvParserService
{
    public async Task<(List<PayrollCsvRow> Rows, List<string> Errors)> ParseAsync(
        Stream csvStream,
        CancellationToken cancellationToken = default)
    {
        var rows = new List<PayrollCsvRow>();
        var errors = new List<string>();

        try
        {
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Allow missing fields for optional columns
                MissingFieldFound = null,
                // Trim whitespace from fields
                TrimOptions = TrimOptions.Trim,
                // Skip empty records
                ShouldSkipRecord = args => args.Row.Parser.Record?.All(string.IsNullOrWhiteSpace) ?? true
            });

            csv.Context.RegisterClassMap<PayrollCsvRowMap>();

            await csv.ReadAsync();
            csv.ReadHeader();

            var rowNumber = 1; // Header is row 0, data starts at row 1

            while (await csv.ReadAsync())
            {
                rowNumber++;
                try
                {
                    var row = csv.GetRecord<PayrollCsvRow>();
                    if (row != null)
                    {
                        // Validate required fields
                        var validationErrors = ValidateRow(row, rowNumber);
                        if (validationErrors.Any())
                        {
                            errors.AddRange(validationErrors);
                            continue; // Skip this row but continue processing
                        }

                        rows.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {rowNumber}: Failed to parse - {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add($"CSV file parsing failed: {ex.Message}");
        }

        return (rows, errors);
    }

    /// <summary>
    /// Validates required fields in a CSV row
    /// </summary>
    private List<string> ValidateRow(PayrollCsvRow row, int rowNumber)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(row.EmployeeId))
            errors.Add($"Row {rowNumber}: Employee ID is required");

        if (string.IsNullOrWhiteSpace(row.EmployeeName))
            errors.Add($"Row {rowNumber}: Employee Name is required");

        if (row.PayPeriodStart == default)
            errors.Add($"Row {rowNumber}: Pay Period Start is required");

        if (row.PayPeriodEnd == default)
            errors.Add($"Row {rowNumber}: Pay Period End is required");

        if (string.IsNullOrWhiteSpace(row.AwardType))
            errors.Add($"Row {rowNumber}: Award Type is required");

        if (string.IsNullOrWhiteSpace(row.Classification))
            errors.Add($"Row {rowNumber}: Classification is required");

        if (string.IsNullOrWhiteSpace(row.EmploymentType))
            errors.Add($"Row {rowNumber}: Employment Type is required");

        if (row.HourlyRate <= 0)
            errors.Add($"Row {rowNumber}: Hourly Rate must be greater than 0");

        if (row.OrdinaryHours < 0)
            errors.Add($"Row {rowNumber}: Ordinary Hours cannot be negative");

        if (row.OrdinaryPay < 0)
            errors.Add($"Row {rowNumber}: Ordinary Pay cannot be negative");

        if (row.GrossPay < 0)
            errors.Add($"Row {rowNumber}: Gross Pay cannot be negative");

        if (row.SuperannuationPaid < 0)
            errors.Add($"Row {rowNumber}: Superannuation Paid cannot be negative");

        return errors;
    }
}

/// <summary>
/// CsvHelper class map for PayrollCsvRow
/// Maps CSV column headers to PayrollCsvRow properties
/// </summary>
public class PayrollCsvRowMap : ClassMap<PayrollCsvRow>
{
    public PayrollCsvRowMap()
    {
        // Required fields
        Map(m => m.EmployeeId).Name("Employee ID");
        Map(m => m.EmployeeName).Name("Employee Name");
        Map(m => m.PayPeriodStart).Name("Pay Period Start").TypeConverterOption.Format("yyyy-MM-dd");
        Map(m => m.PayPeriodEnd).Name("Pay Period End").TypeConverterOption.Format("yyyy-MM-dd");
        Map(m => m.AwardType).Name("Award Type");
        Map(m => m.Classification).Name("Classification");
        Map(m => m.EmploymentType).Name("Employment Type");
        Map(m => m.HourlyRate).Name("Hourly Rate");
        Map(m => m.OrdinaryHours).Name("Ordinary Hours");
        Map(m => m.OrdinaryPay).Name("Ordinary Pay");
        Map(m => m.GrossPay).Name("Gross Pay");
        Map(m => m.SuperannuationPaid).Name("Superannuation Paid");

        // Optional fields (default to 0 if missing)
        Map(m => m.SaturdayHours).Name("Saturday Hours").Default(0m).Optional();
        Map(m => m.SaturdayPay).Name("Saturday Pay").Default(0m).Optional();
        Map(m => m.SundayHours).Name("Sunday Hours").Default(0m).Optional();
        Map(m => m.SundayPay).Name("Sunday Pay").Default(0m).Optional();
        Map(m => m.PublicHolidayHours).Name("Public Holiday Hours").Default(0m).Optional();
        Map(m => m.PublicHolidayPay).Name("Public Holiday Pay").Default(0m).Optional();
    }
}
