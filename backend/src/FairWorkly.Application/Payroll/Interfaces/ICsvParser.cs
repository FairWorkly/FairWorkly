using FairWorkly.Domain.Common;

namespace FairWorkly.Application.Payroll.Interfaces;

public interface ICsvParser
{
    Result<List<string[]>> Parse(Stream stream);
}
