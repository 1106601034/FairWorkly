using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Application.Payroll.Services.ComplianceEngine;
using FairWorkly.Application.Payroll.Services.Models;
using FairWorkly.Domain.Common.Enums;
using FairWorkly.Domain.Payroll.Entities;
using MediatR;

namespace FairWorkly.Application.Payroll.Features.ValidatePayroll;

/// <summary>
/// Handler for validating payroll data from CSV
/// Implements the 11-step validation flow as per ISSUE_03
/// </summary>
public class ValidatePayrollHandler : IRequestHandler<ValidatePayrollCommand, ValidationResultDto>
{
    // MVP Organization ID (hardcoded)
    private static readonly Guid MvpOrganizationId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly ICsvParserService _csvParserService;
    private readonly IEmployeeSyncService _employeeSyncService;
    private readonly IPayrollValidationRepository _validationRepository;
    private readonly IPayslipRepository _payslipRepository;
    private readonly IPayrollIssueRepository _issueRepository;
    private readonly IEnumerable<IComplianceRule> _complianceRules;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IFileStorageService _fileStorageService;

    public ValidatePayrollHandler(
        ICsvParserService csvParserService,
        IEmployeeSyncService employeeSyncService,
        IPayrollValidationRepository validationRepository,
        IPayslipRepository payslipRepository,
        IPayrollIssueRepository issueRepository,
        IEnumerable<IComplianceRule> complianceRules,
        IDateTimeProvider dateTimeProvider,
        IFileStorageService fileStorageService)
    {
        _csvParserService = csvParserService;
        _employeeSyncService = employeeSyncService;
        _validationRepository = validationRepository;
        _payslipRepository = payslipRepository;
        _issueRepository = issueRepository;
        _complianceRules = complianceRules;
        _dateTimeProvider = dateTimeProvider;
        _fileStorageService = fileStorageService;
    }

    public async Task<ValidationResultDto> Handle(
        ValidatePayrollCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Save CSV to storage
        var filePath = await _fileStorageService.UploadAsync(
            request.FileStream,
            request.FileName,
            cancellationToken);

        // Reset stream position for parsing
        if (request.FileStream.CanSeek)
        {
            request.FileStream.Position = 0;
        }

        // Convert dates
        var payPeriodStart = ToDateTimeOffset(request.WeekStarting);
        var payPeriodEnd = ToDateTimeOffset(request.WeekEnding, endOfDay: true);

        // Step 2: Create PayrollValidation record (Status: InProgress)
        var validation = new PayrollValidation
        {
            OrganizationId = MvpOrganizationId,
            Status = ValidationStatus.InProgress,
            FilePath = filePath,
            FileName = request.FileName,
            PayPeriodStart = payPeriodStart,
            PayPeriodEnd = payPeriodEnd,
            StartedAt = _dateTimeProvider.Now,
            BaseRateCheckPerformed = request.EnableBaseRateCheck,
            PenaltyRateCheckPerformed = request.EnablePenaltyCheck,
            CasualLoadingCheckPerformed = request.EnableCasualLoadingCheck,
            SuperannuationCheckPerformed = request.EnableSuperCheck,
            STPCheckPerformed = false // Future feature
        };

        await _validationRepository.AddAsync(validation, cancellationToken);
        await _validationRepository.SaveChangesAsync(cancellationToken);

        // Step 3: Parse CSV
        var (rows, parseErrors) = await _csvParserService.ParseAsync(request.FileStream, cancellationToken);

        // Step 4: If parse fails completely, return failed result
        if (rows.Count == 0 && parseErrors.Count > 0)
        {
            validation.Status = ValidationStatus.Failed;
            validation.CompletedAt = _dateTimeProvider.Now;
            validation.Notes = string.Join("; ", parseErrors);
            await _validationRepository.UpdateAsync(validation, cancellationToken);
            await _validationRepository.SaveChangesAsync(cancellationToken);

            return BuildFailedResult(validation, parseErrors);
        }

        // Step 5: Sync employees
        var employeeMapping = await _employeeSyncService.SyncEmployeesAsync(
            rows,
            MvpOrganizationId,
            cancellationToken);

        // Step 6: Create Payslips
        var payslips = CreatePayslips(rows, validation.Id, employeeMapping, payPeriodStart, payPeriodEnd);
        await _payslipRepository.AddRangeAsync(payslips, cancellationToken);
        await _payslipRepository.SaveChangesAsync(cancellationToken);

        // Step 7-9: Pre-Validation and Rule Evaluation
        var allIssues = new List<PayrollIssue>();
        var employeesWithIssues = new HashSet<Guid>();

        foreach (var payslip in payslips)
        {
            // Step 7: Pre-Validation
            var preValidationIssues = ValidatePayslipData(payslip, validation.Id);
            if (preValidationIssues.Any())
            {
                allIssues.AddRange(preValidationIssues);
                employeesWithIssues.Add(payslip.EmployeeId);
                continue; // Skip rule checks for this payslip
            }

            // Step 8: Rule evaluation
            foreach (var rule in _complianceRules)
            {
                if (!ShouldRunRule(rule, request))
                    continue;

                var ruleIssues = rule.Evaluate(payslip, validation.Id);
                if (ruleIssues.Any())
                {
                    allIssues.AddRange(ruleIssues);
                    employeesWithIssues.Add(payslip.EmployeeId);
                }
            }
        }

        // Step 9: Save all issues
        if (allIssues.Any())
        {
            await _issueRepository.AddRangeAsync(allIssues, cancellationToken);
            await _issueRepository.SaveChangesAsync(cancellationToken);
        }

        // Step 10: Update statistics
        validation.TotalPayslips = payslips.Count;
        validation.PassedCount = payslips.Count - employeesWithIssues.Count;
        validation.FailedCount = employeesWithIssues.Count;
        validation.TotalIssuesCount = allIssues.Count;
        validation.CriticalIssuesCount = allIssues.Count(i => i.Severity == IssueSeverity.Critical);
        validation.Status = allIssues.Any() ? ValidationStatus.Failed : ValidationStatus.Passed;
        validation.CompletedAt = _dateTimeProvider.Now;

        await _validationRepository.UpdateAsync(validation, cancellationToken);
        await _validationRepository.SaveChangesAsync(cancellationToken);

        // Step 11: Build and return ValidationResultDto
        return BuildResult(validation, payslips, allIssues);
    }

    /// <summary>
    /// Converts DateOnly to DateTimeOffset using UTC midnight
    /// </summary>
    private static DateTimeOffset ToDateTimeOffset(DateOnly date, bool endOfDay = false)
    {
        var time = endOfDay ? new TimeOnly(23, 59, 59) : TimeOnly.MinValue;
        var dateTime = date.ToDateTime(time);
        return new DateTimeOffset(dateTime, TimeSpan.Zero);
    }

    /// <summary>
    /// Creates Payslip entities from CSV rows
    /// </summary>
    private List<Payslip> CreatePayslips(
        List<PayrollCsvRow> rows,
        Guid validationId,
        Dictionary<string, Guid> employeeMapping,
        DateTimeOffset payPeriodStart,
        DateTimeOffset payPeriodEnd)
    {
        return rows.Select(row => new Payslip
        {
            OrganizationId = MvpOrganizationId,
            PayrollValidationId = validationId,
            EmployeeId = employeeMapping.GetValueOrDefault(row.EmployeeId, Guid.Empty),
            EmployeeName = row.EmployeeName,
            EmployeeNumber = row.EmployeeId,
            EmploymentType = ParseEmploymentType(row.EmploymentType),
            AwardType = ParseAwardType(row.AwardType),
            Classification = row.Classification,
            HourlyRate = row.HourlyRate,
            PayPeriodStart = payPeriodStart,
            PayPeriodEnd = payPeriodEnd,
            PayDate = payPeriodEnd, // User decision: PayDate = PayPeriodEnd
            OrdinaryHours = row.OrdinaryHours,
            OrdinaryPay = row.OrdinaryPay,
            SaturdayHours = row.SaturdayHours,
            SaturdayPay = row.SaturdayPay,
            SundayHours = row.SundayHours,
            SundayPay = row.SundayPay,
            PublicHolidayHours = row.PublicHolidayHours,
            PublicHolidayPay = row.PublicHolidayPay,
            GrossPay = row.GrossPay,
            Superannuation = row.SuperannuationPaid,
            Tax = 0, // Not in CSV
            NetPay = row.GrossPay - row.SuperannuationPaid, // User decision
            SourceData = SerializeRowToJson(row)
        }).ToList();
    }

    /// <summary>
    /// Pre-validation: Checks mandatory fields for a payslip
    /// </summary>
    private List<PayrollIssue> ValidatePayslipData(Payslip payslip, Guid validationId)
    {
        var issues = new List<PayrollIssue>();
        var missingFields = new List<string>();

        if (string.IsNullOrEmpty(payslip.Classification))
            missingFields.Add("Classification");

        if (payslip.HourlyRate <= 0)
            missingFields.Add("Hourly Rate");

        if (payslip.OrdinaryHours < 0)
            missingFields.Add("Ordinary Hours");

        if (payslip.OrdinaryPay < 0)
            missingFields.Add("Ordinary Pay");

        if (payslip.GrossPay < 0)
            missingFields.Add("Gross Pay");

        if (missingFields.Any())
        {
            issues.Add(new PayrollIssue
            {
                OrganizationId = payslip.OrganizationId,
                PayrollValidationId = validationId,
                PayslipId = payslip.Id,
                EmployeeId = payslip.EmployeeId,
                CategoryType = IssueCategory.PreValidation,
                Severity = IssueSeverity.Warning,
                WarningMessage = $"Unable to verify: {string.Join(", ", missingFields)} is missing or invalid",
                ImpactAmount = 0
            });
        }

        return issues;
    }

    /// <summary>
    /// Determines if a rule should run based on command switches
    /// </summary>
    private static bool ShouldRunRule(IComplianceRule rule, ValidatePayrollCommand command)
    {
        return rule.RuleName switch
        {
            "Base Rate Check" => command.EnableBaseRateCheck,
            "Penalty Rate Check" => command.EnablePenaltyCheck,
            "Casual Loading Check" => command.EnableCasualLoadingCheck,
            "Superannuation Check" => command.EnableSuperCheck,
            _ => true
        };
    }

    /// <summary>
    /// Builds the validation result DTO
    /// </summary>
    private ValidationResultDto BuildResult(
        PayrollValidation validation,
        List<Payslip> payslips,
        List<PayrollIssue> issues)
    {
        var payslipLookup = payslips.ToDictionary(p => p.Id);

        // Group issues by category
        var categoryStats = issues
            .GroupBy(i => i.CategoryType)
            .Select(g => new CategoryDto
            {
                Key = g.Key.ToString(),
                AffectedEmployeeCount = g.Select(i => i.EmployeeId).Distinct().Count(),
                TotalUnderpayment = g.Sum(i => i.ImpactAmount ?? 0)
            })
            .OrderBy(c => c.Key)
            .ToList();

        // Map issues to DTOs
        var issueDtos = issues.Select(issue =>
        {
            var payslip = payslipLookup.GetValueOrDefault(issue.PayslipId);
            var isWarning = issue.Severity == IssueSeverity.Warning;

            return new IssueDto
            {
                IssueId = issue.Id,
                CategoryType = issue.CategoryType.ToString(),
                EmployeeName = payslip?.EmployeeName ?? "Unknown",
                EmployeeId = payslip?.EmployeeNumber ?? "Unknown",
                IssueStatus = issue.IsResolved ? "RESOLVED" : "OPEN",
                Severity = (int)issue.Severity,
                ImpactAmount = issue.ImpactAmount ?? 0,
                Description = isWarning ? null : new DescriptionDto
                {
                    ActualValue = issue.ActualValue ?? 0,
                    ExpectedValue = issue.ExpectedValue ?? 0,
                    AffectedUnits = issue.AffectedUnits ?? 0,
                    UnitType = issue.UnitType ?? "Hour",
                    ContextLabel = issue.ContextLabel ?? ""
                },
                Warning = isWarning ? issue.WarningMessage : null
            };
        }).ToList();

        return new ValidationResultDto
        {
            ValidationId = $"VAL-{validation.Id.ToString("N")[..8]}",
            Status = validation.Status.ToString(),
            Timestamp = validation.CompletedAt ?? _dateTimeProvider.Now,
            Summary = new SummaryDto
            {
                PassedCount = validation.PassedCount,
                TotalIssues = validation.TotalIssuesCount,
                TotalUnderpayment = issues.Sum(i => i.ImpactAmount ?? 0),
                AffectedEmployees = validation.FailedCount
            },
            Categories = categoryStats,
            Issues = issueDtos
        };
    }

    /// <summary>
    /// Builds a failed result when CSV parsing fails
    /// </summary>
    private ValidationResultDto BuildFailedResult(PayrollValidation validation, List<string> errors)
    {
        return new ValidationResultDto
        {
            ValidationId = $"VAL-{validation.Id.ToString("N")[..8]}",
            Status = ValidationStatus.Failed.ToString(),
            Timestamp = validation.CompletedAt ?? _dateTimeProvider.Now,
            Summary = new SummaryDto
            {
                PassedCount = 0,
                TotalIssues = errors.Count,
                TotalUnderpayment = 0,
                AffectedEmployees = 0
            },
            Categories = new List<CategoryDto>
            {
                new CategoryDto
                {
                    Key = "PreValidation",
                    AffectedEmployeeCount = 0,
                    TotalUnderpayment = 0
                }
            },
            Issues = errors.Select((error, index) => new IssueDto
            {
                IssueId = Guid.NewGuid(),
                CategoryType = "PreValidation",
                EmployeeName = "N/A",
                EmployeeId = "N/A",
                IssueStatus = "OPEN",
                Severity = (int)IssueSeverity.Warning,
                ImpactAmount = 0,
                Description = null,
                Warning = error
            }).ToList()
        };
    }

    /// <summary>
    /// Parses employment type from string
    /// </summary>
    private static EmploymentType ParseEmploymentType(string employmentTypeString)
    {
        var normalized = employmentTypeString.Trim().ToLowerInvariant();
        return normalized switch
        {
            "fulltime" or "full-time" or "full time" => EmploymentType.FullTime,
            "parttime" or "part-time" or "part time" => EmploymentType.PartTime,
            "casual" => EmploymentType.Casual,
            "fixedterm" or "fixed-term" or "fixed term" => EmploymentType.FixedTerm,
            _ => EmploymentType.FullTime
        };
    }

    /// <summary>
    /// Parses award type from string
    /// </summary>
    private static AwardType ParseAwardType(string awardTypeString)
    {
        var normalized = awardTypeString.ToLowerInvariant().Trim();

        if (normalized.Contains("retail") || normalized.Contains("ma000004"))
            return AwardType.GeneralRetailIndustryAward2020;

        if (normalized.Contains("hospitality") || normalized.Contains("ma000009"))
            return AwardType.HospitalityIndustryAward2020;

        if (normalized.Contains("clerk") || normalized.Contains("ma000002"))
            return AwardType.ClerksPrivateSectorAward2020;

        return AwardType.GeneralRetailIndustryAward2020;
    }

    /// <summary>
    /// Serializes a CSV row to JSON for audit trail
    /// </summary>
    private static string SerializeRowToJson(PayrollCsvRow row)
    {
        return System.Text.Json.JsonSerializer.Serialize(row);
    }
}
