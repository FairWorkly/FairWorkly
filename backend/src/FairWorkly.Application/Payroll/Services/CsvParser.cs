using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.Payroll.Services;

public class CsvParser(ILogger<CsvParser> logger) : ICsvParser
{
    public Result<List<string[]>> Parse(Stream stream)
    {
        try
        {
            using var reader = new StreamReader(stream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using var csv = new CsvReader(reader, config);

            var rows = new List<string[]>();
            while (csv.Read())
            {
                var record = new string[csv.Parser.Count];
                for (var i = 0; i < csv.Parser.Count; i++)
                {
                    record[i] = csv.GetField(i) ?? "";
                }
                rows.Add(record);
            }

            if (rows.Count == 0)
                return Result<List<string[]>>.Failure("CSV file is corrupted or cannot be parsed");

            return Result<List<string[]>>.Success(rows);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "CSV parsing failed with exception");
            return Result<List<string[]>>.Failure("CSV file is corrupted or cannot be parsed");
        }
    }
}
