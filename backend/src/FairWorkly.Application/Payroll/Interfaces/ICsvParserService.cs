using FairWorkly.Application.Payroll.Services.Models;

namespace FairWorkly.Application.Payroll.Interfaces;

/// <summary>
/// Service for parsing payroll CSV files
/// </summary>
public interface ICsvParserService
{
    /// <summary>
    /// Parses a CSV stream into structured payroll data
    /// </summary>
    /// <param name="csvStream">CSV file stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Tuple containing:
    /// - List of successfully parsed rows
    /// - List of error messages for rows that failed to parse
    /// </returns>
    /// <remarks>
    /// - Required fields: EmployeeId, EmployeeName, PayPeriodStart, PayPeriodEnd,
    ///   AwardType, Classification, EmploymentType, HourlyRate, OrdinaryHours,
    ///   OrdinaryPay, GrossPay, SuperannuationPaid
    /// - Optional fields (default to 0): SaturdayHours, SaturdayPay, SundayHours,
    ///   SundayPay, PublicHolidayHours, PublicHolidayPay
    /// - Missing required fields: Record error and continue processing other rows
    /// - Missing optional fields: Use default value 0
    /// </remarks>
    Task<(List<PayrollCsvRow> Rows, List<string> Errors)> ParseAsync(
        Stream csvStream,
        CancellationToken cancellationToken = default);
}
