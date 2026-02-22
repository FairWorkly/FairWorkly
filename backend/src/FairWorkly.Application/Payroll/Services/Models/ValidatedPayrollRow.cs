using FairWorkly.Domain.Common.Enums;

namespace FairWorkly.Application.Payroll.Services.Models;

public class ValidatedPayrollRow
{
    public string EmployeeId { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTimeOffset PayPeriodStart { get; set; }
    public DateTimeOffset PayPeriodEnd { get; set; }
    public DateTimeOffset PayDate { get; set; }
    public string AwardType { get; set; } = "";
    public string Classification { get; set; } = "";
    public EmploymentType EmploymentType { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal OrdinaryHours { get; set; }
    public decimal OrdinaryPay { get; set; }
    public decimal SaturdayHours { get; set; }
    public decimal SaturdayPay { get; set; }
    public decimal SundayHours { get; set; }
    public decimal SundayPay { get; set; }
    public decimal PublicHolidayHours { get; set; }
    public decimal PublicHolidayPay { get; set; }
    public decimal GrossPay { get; set; }
    public decimal Superannuation { get; set; }
}
