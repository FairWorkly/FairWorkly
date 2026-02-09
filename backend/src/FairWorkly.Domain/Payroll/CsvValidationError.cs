using FairWorkly.Domain.Common;

namespace FairWorkly.Domain.Payroll;

public class CsvValidationError : ValidationError
{
    public int RowNumber { get; init; }
}
