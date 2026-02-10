using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Employees.Interfaces;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Common;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Employees.Entities;
using FairWorkly.Domain.Payroll;
using FairWorkly.Domain.Payroll.ComplianceEngine;
using FairWorkly.Domain.Payroll.Entities;
using FairWorkly.Domain.Payroll.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FairWorkly.Application.Payroll.Features.ValidatePayroll;

public class ValidatePayrollHandler(
    ICsvParser csvParser,
    ICsvValidator csvValidator,
    IEmployeeRepository employeeRepository,
    IPayrollValidationRepository validationRepository,
    IPayslipRepository payslipRepository,
    IPayrollIssueRepository issueRepository,
    IEnumerable<IComplianceRule> complianceRules,
    ICurrentUserService currentUserService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork,
    ILogger<ValidatePayrollHandler> logger
) : IRequestHandler<ValidatePayrollCommand, Result<ValidatePayrollDto>>
{
    public async Task<Result<ValidatePayrollDto>> Handle(
        ValidatePayrollCommand command,
        CancellationToken cancellationToken)
    {
        // Guard: must belong to an organization
        var organizationId = currentUserService.OrganizationId;
        if (organizationId == null)
            return Result<ValidatePayrollDto>.Forbidden("User does not belong to an organization");

        var orgId = organizationId.Value;

        logger.LogInformation("Starting payroll validation: file={FileName}, size={FileSize}",
            command.FileName, command.FileSize);

        // Parse AwardType enum from command string
        var awardType = Enum.Parse<AwardType>(command.AwardType);

        // Layer 2: CsvParser
        var parseResult = csvParser.Parse(command.FileStream);
        if (parseResult.IsFailure)
        {
            logger.LogWarning("CSV parsing failed: {Error}", parseResult.ErrorMessage);
            var errors = new List<ValidationError>
            {
                new CsvValidationError { RowNumber = 0, Field = "File", Message = parseResult.ErrorMessage! }
            };
            return Result<ValidatePayrollDto>.ProcessingFailure("CSV file parsing failed", errors);
        }

        logger.LogInformation("CSV parsed successfully: {RowCount} rows", parseResult.Value!.Count);

        // Layer 3: CsvValidator
        var validateResult = csvValidator.Validate(parseResult.Value!, awardType);
        if (validateResult.IsFailure)
        {
            logger.LogWarning("CSV validation failed: {ErrorCount} errors",
                validateResult.ValidationErrors?.Count ?? 0);
            return Result<ValidatePayrollDto>.ProcessingFailure(
                validateResult.ErrorMessage!, validateResult.ValidationErrors!);
        }

        var validatedRows = validateResult.Value!;
        logger.LogInformation("CSV validation passed: {RowCount} rows", validatedRows.Count);

        // ═══ Stage A: Employee Upsert ═══
        var employeeNumbers = validatedRows.Select(r => r.EmployeeId).Distinct().ToList();
        var existingEmployees = await employeeRepository.GetByEmployeeNumbersAsync(
            orgId, employeeNumbers, cancellationToken);
        var employeeMap = existingEmployees.ToDictionary(e => e.EmployeeNumber!, e => e);

        foreach (var row in validatedRows)
        {
            if (employeeMap.TryGetValue(row.EmployeeId, out var existing))
            {
                // Update existing employee fields from CSV
                existing.FirstName = row.FirstName;
                existing.LastName = row.LastName;
                existing.EmploymentType = row.EmploymentType;
                existing.AwardType = awardType;
                existing.AwardLevelNumber = ParseLevelNumber(row.Classification);
                existing.EmployeeNumber = row.EmployeeId;
            }
            else
            {
                // Create new employee
                var newEmployee = new Employee
                {
                    OrganizationId = orgId,
                    FirstName = row.FirstName,
                    LastName = row.LastName,
                    JobTitle = "Unknown",
                    EmploymentType = row.EmploymentType,
                    StartDate = DateTime.MinValue,
                    AwardType = awardType,
                    AwardLevelNumber = ParseLevelNumber(row.Classification),
                    EmployeeNumber = row.EmployeeId,
                };
                employeeRepository.Add(newEmployee);
                employeeMap[row.EmployeeId] = newEmployee;
            }
        }

        // ═══ Stage B: Create PayrollValidation ═══
        var now = dateTimeProvider.UtcNow;
        var validation = new PayrollValidation
        {
            OrganizationId = orgId,
            PayPeriodStart = validatedRows[0].PayPeriodStart,
            PayPeriodEnd = validatedRows[0].PayPeriodEnd,
            Status = ValidationStatus.InProgress,
            StartedAt = now,
            FileName = command.FileName,
            BaseRateCheckPerformed = command.EnableBaseRateCheck,
            PenaltyRateCheckPerformed = command.EnablePenaltyCheck,
            CasualLoadingCheckPerformed = command.EnableCasualLoadingCheck,
            SuperannuationCheckPerformed = command.EnableSuperCheck,
            STPCheckPerformed = false,
        };
        validationRepository.Add(validation);

        // ═══ Stage C: Create Payslips ═══
        var payslips = new List<Payslip>();
        foreach (var row in validatedRows)
        {
            var employee = employeeMap[row.EmployeeId];
            var payslip = new Payslip
            {
                OrganizationId = orgId,
                EmployeeId = employee.Id,
                PayrollValidationId = validation.Id,
                PayPeriodStart = row.PayPeriodStart,
                PayPeriodEnd = row.PayPeriodEnd,
                PayDate = row.PayDate,
                EmployeeName = $"{row.FirstName} {row.LastName}",
                EmployeeNumber = row.EmployeeId,
                EmploymentType = row.EmploymentType,
                AwardType = awardType,
                Classification = row.Classification,
                HourlyRate = row.HourlyRate,
                OrdinaryHours = row.OrdinaryHours,
                OrdinaryPay = row.OrdinaryPay,
                SaturdayHours = row.SaturdayHours,
                SaturdayPay = row.SaturdayPay,
                SundayHours = row.SundayHours,
                SundayPay = row.SundayPay,
                PublicHolidayHours = row.PublicHolidayHours,
                PublicHolidayPay = row.PublicHolidayPay,
                GrossPay = row.GrossPay,
                Superannuation = row.Superannuation,
            };
            payslips.Add(payslip);
        }
        payslipRepository.AddRange(payslips);

        // ═══ Stage D: Run ComplianceRules ═══
        var enabledCategories = new HashSet<IssueCategory>();
        if (command.EnableBaseRateCheck) enabledCategories.Add(IssueCategory.BaseRate);
        if (command.EnablePenaltyCheck) enabledCategories.Add(IssueCategory.PenaltyRate);
        if (command.EnableCasualLoadingCheck) enabledCategories.Add(IssueCategory.CasualLoading);
        if (command.EnableSuperCheck) enabledCategories.Add(IssueCategory.Superannuation);

        var filteredRules = complianceRules.Where(r => enabledCategories.Contains(r.Category)).ToList();
        var allIssues = new List<PayrollIssue>();

        foreach (var payslip in payslips)
        {
            foreach (var rule in filteredRules)
            {
                var issues = rule.Evaluate(payslip, validation.Id);
                allIssues.AddRange(issues);
            }
        }

        if (allIssues.Count > 0)
            issueRepository.AddRange(allIssues);

        // ═══ Stage E: Update PayrollValidation Stats ═══
        var payslipIdsWithIssues = allIssues.Select(i => i.PayslipId).Distinct().ToHashSet();
        validation.TotalPayslips = payslips.Count;
        validation.PassedCount = payslips.Count(p => !payslipIdsWithIssues.Contains(p.Id));
        validation.FailedCount = payslips.Count(p => payslipIdsWithIssues.Contains(p.Id));
        validation.TotalIssuesCount = allIssues.Count;
        validation.CriticalIssuesCount = allIssues.Count(i => i.Severity == IssueSeverity.Critical);
        validation.Status = allIssues.Count > 0 ? ValidationStatus.Failed : ValidationStatus.Passed;
        validation.CompletedAt = dateTimeProvider.UtcNow;

        // ═══ SaveChanges ═══
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Payroll validation saved: validationId={ValidationId}, issues={IssueCount}",
            validation.Id, allIssues.Count);

        // ═══ Build DTO ═══
        var dto = BuildDto(validation, payslips, allIssues);
        return Result<ValidatePayrollDto>.Success(dto);
    }

    private static ValidatePayrollDto BuildDto(
        PayrollValidation validation,
        List<Payslip> payslips,
        List<PayrollIssue> allIssues)
    {
        // Build issue DTOs
        var issueDtos = allIssues.Select(issue =>
        {
            var payslip = payslips.First(p => p.Id == issue.PayslipId);
            var impactAmount = CalculateImpactAmount(issue);

            return new IssueDto
            {
                IssueId = issue.Id,
                CategoryType = issue.CategoryType.ToString(),
                EmployeeName = payslip.EmployeeName,
                EmployeeId = payslip.EmployeeNumber,
                Severity = (int)issue.Severity,
                ImpactAmount = impactAmount,
                Description = issue.WarningMessage == null
                    ? new IssueDescriptionDto
                    {
                        ActualValue = issue.ActualValue ?? 0,
                        ExpectedValue = issue.ExpectedValue ?? 0,
                        AffectedUnits = issue.AffectedUnits ?? 0,
                        UnitType = issue.UnitType ?? "",
                        ContextLabel = issue.ContextLabel ?? "",
                    }
                    : null,
                Warning = issue.WarningMessage,
            };
        }).ToList();

        // Build category DTOs
        var categories = allIssues
            .GroupBy(i => i.CategoryType)
            .Select(g =>
            {
                var categoryIssues = g.ToList();
                return new CategoryDto
                {
                    Key = g.Key.ToString(),
                    AffectedEmployeeCount = categoryIssues.Select(i => i.EmployeeId).Distinct().Count(),
                    TotalUnderpayment = categoryIssues.Sum(CalculateImpactAmount),
                };
            })
            .ToList();

        // Build summary
        var affectedEmployeeIds = allIssues.Select(i => i.EmployeeId).Distinct().Count();
        var totalUnderpayment = allIssues.Sum(CalculateImpactAmount);

        return new ValidatePayrollDto
        {
            ValidationId = validation.Id,
            Status = validation.Status.ToString(),
            Timestamp = validation.CompletedAt ?? validation.StartedAt ?? DateTimeOffset.UtcNow,
            PayPeriodStart = validation.PayPeriodStart.ToString("yyyy-MM-dd"),
            PayPeriodEnd = validation.PayPeriodEnd.ToString("yyyy-MM-dd"),
            Summary = new SummaryDto
            {
                PassedCount = validation.PassedCount,
                TotalIssues = validation.TotalIssuesCount,
                TotalUnderpayment = totalUnderpayment,
                AffectedEmployees = affectedEmployeeIds,
            },
            Categories = categories,
            Issues = issueDtos,
        };
    }

    private static decimal CalculateImpactAmount(PayrollIssue issue)
    {
        if (issue.WarningMessage != null)
            return 0m;

        return issue.UnitType switch
        {
            "Hour" => ((issue.ExpectedValue ?? 0) - (issue.ActualValue ?? 0)) * (issue.AffectedUnits ?? 0),
            "Currency" => (issue.ExpectedValue ?? 0) - (issue.ActualValue ?? 0),
            _ => 0m,
        };
    }

    private static int ParseLevelNumber(string classification)
    {
        // Extract number from "Level 2" → 2
        var parts = classification.Replace("Level ", "");
        return int.TryParse(parts, out var level) ? level : 1;
    }
}
