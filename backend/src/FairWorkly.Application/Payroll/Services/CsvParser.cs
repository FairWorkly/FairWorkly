using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Result;
using FairWorkly.Domain.Payroll;
using FairWorkly.Domain.Payroll.Errors;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.Payroll.Services;

public class CsvParser(ILogger<CsvParser> logger) : ICsvParser
{
    public Result<List<string[]>> Parse(Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using var csv = new CsvReader(reader, config);

            var rows = new List<string[]>();
            while (csv.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var record = new string[csv.Parser.Count];
                for (var i = 0; i < csv.Parser.Count; i++)
                {
                    record[i] = csv.GetField(i) ?? "";
                }
                rows.Add(record);
            }

            if (rows.Count == 0)
            {
                var errors = new List<Csv422Error>
                {
                    new() { RowNumber = 0, Field = "File", Message = "CSV file is corrupted or cannot be parsed" }
                };
                return Result<List<string[]>>.Of422("CSV file is corrupted or cannot be parsed", errors);
            }

            return Result<List<string[]>>.Of200("CSV parsed successfully", rows);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "CSV parsing failed with exception");
            var errors = new List<Csv422Error>
            {
                new() { RowNumber = 0, Field = "File", Message = "CSV file is corrupted or cannot be parsed" }
            };
            return Result<List<string[]>>.Of422("CSV file is corrupted or cannot be parsed", errors);
        }
    }
}
