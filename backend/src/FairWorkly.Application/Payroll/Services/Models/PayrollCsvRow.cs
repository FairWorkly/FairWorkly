namespace FairWorkly.Application.Payroll.Services.Models;

/// <summary>
/// Represents a single row from the payroll CSV file
/// Used for parsing and validation before creating Payslip entities
/// </summary>
public class PayrollCsvRow
{
    /// <summary>
    /// Employee ID from CSV (maps to EmployeeNumber in Employee entity)
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>
    /// Employee full name from CSV
    /// Will be split into FirstName and LastName for Employee entity
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// Pay period start date (YYYY-MM-DD format in CSV)
    /// </summary>
    public DateOnly PayPeriodStart { get; set; }

    /// <summary>
    /// Pay period end date (YYYY-MM-DD format in CSV)
    /// </summary>
    public DateOnly PayPeriodEnd { get; set; }

    /// <summary>
    /// Award type from CSV (e.g., "Retail", "MA000004")
    /// Needs parsing to AwardType enum
    /// </summary>
    public string AwardType { get; set; } = string.Empty;

    /// <summary>
    /// Classification level from CSV (e.g., "Level 1", "Level 2")
    /// Needs parsing to extract level number
    /// </summary>
    public string Classification { get; set; } = string.Empty;

    /// <summary>
    /// Employment type from CSV (e.g., "FullTime", "PartTime", "Casual", "FixedTerm")
    /// Maps to EmploymentType enum
    /// </summary>
    public string EmploymentType { get; set; } = string.Empty;

    /// <summary>
    /// Hourly rate from CSV (employee's configured rate)
    /// </summary>
    public decimal HourlyRate { get; set; }

    /// <summary>
    /// Ordinary hours worked (base hours, Monday-Friday)
    /// </summary>
    public decimal OrdinaryHours { get; set; }

    /// <summary>
    /// Ordinary pay amount (base pay)
    /// </summary>
    public decimal OrdinaryPay { get; set; }

    /// <summary>
    /// Saturday hours worked (optional, defaults to 0)
    /// </summary>
    public decimal SaturdayHours { get; set; }

    /// <summary>
    /// Saturday pay amount (optional, defaults to 0)
    /// </summary>
    public decimal SaturdayPay { get; set; }

    /// <summary>
    /// Sunday hours worked (optional, defaults to 0)
    /// </summary>
    public decimal SundayHours { get; set; }

    /// <summary>
    /// Sunday pay amount (optional, defaults to 0)
    /// </summary>
    public decimal SundayPay { get; set; }

    /// <summary>
    /// Public holiday hours worked (optional, defaults to 0)
    /// </summary>
    public decimal PublicHolidayHours { get; set; }

    /// <summary>
    /// Public holiday pay amount (optional, defaults to 0)
    /// </summary>
    public decimal PublicHolidayPay { get; set; }

    /// <summary>
    /// Gross pay (total before tax)
    /// </summary>
    public decimal GrossPay { get; set; }

    /// <summary>
    /// Superannuation paid (employer contribution)
    /// </summary>
    public decimal SuperannuationPaid { get; set; }
}
