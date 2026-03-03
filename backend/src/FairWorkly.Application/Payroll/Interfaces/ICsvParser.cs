using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;

namespace FairWorkly.Application.Payroll.Interfaces;

public interface ICsvParser
{
    Result<List<string[]>> Parse(Stream stream, CancellationToken cancellationToken);
}
