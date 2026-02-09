using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Common;

namespace FairWorkly.Application.Payroll.Services;

public class CsvParser : ICsvParser
{
    public Result<List<string[]>> Parse(Stream stream)
    {
        throw new NotImplementedException();
    }
}
