using System.Text.Json.Serialization;

namespace FairWorkly.Application.Payroll.Features.ValidatePayroll;

/// <summary>
/// Validation result response conforming to API Contract v1.3
/// </summary>
public class ValidationResultDto
{
    /// <summary>
    /// Unique validation ID (format: VAL-{first 8 chars of GUID})
    /// </summary>
    public string ValidationId { get; set; } = string.Empty;

    /// <summary>
    /// Validation status: Passed, Failed, InProgress, Pending
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of validation completion (ISO 8601)
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Summary statistics
    /// </summary>
    public SummaryDto Summary { get; set; } = new();

    /// <summary>
    /// Issue categories with aggregated statistics
    /// </summary>
    public List<CategoryDto> Categories { get; set; } = new();

    /// <summary>
    /// Detailed list of all issues found
    /// </summary>
    public List<IssueDto> Issues { get; set; } = new();
}

/// <summary>
/// Summary statistics for the validation
/// </summary>
public class SummaryDto
{
    /// <summary>
    /// Number of employees that passed all checks
    /// </summary>
    public int PassedCount { get; set; }

    /// <summary>
    /// Total number of issues found
    /// </summary>
    public int TotalIssues { get; set; }

    /// <summary>
    /// Total underpayment amount across all issues
    /// </summary>
    public decimal TotalUnderpayment { get; set; }

    /// <summary>
    /// Number of employees affected by at least one issue
    /// </summary>
    public int AffectedEmployees { get; set; }
}

/// <summary>
/// Category statistics for grouping issues
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// Category key: PreValidation, BaseRate, PenaltyRate, CasualLoading, Superannuation
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Number of employees affected in this category
    /// </summary>
    public int AffectedEmployeeCount { get; set; }

    /// <summary>
    /// Total underpayment in this category (0 for PreValidation and warnings)
    /// </summary>
    public decimal TotalUnderpayment { get; set; }
}

/// <summary>
/// Individual issue details
/// </summary>
public class IssueDto
{
    /// <summary>
    /// Unique issue ID (GUID)
    /// </summary>
    public Guid IssueId { get; set; }

    /// <summary>
    /// Category: PreValidation, BaseRate, PenaltyRate, CasualLoading, Superannuation
    /// </summary>
    public string CategoryType { get; set; } = string.Empty;

    /// <summary>
    /// Employee full name
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// Employee number/ID from CSV
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>
    /// Issue status: OPEN or RESOLVED
    /// </summary>
    public string IssueStatus { get; set; } = "OPEN";

    /// <summary>
    /// Severity level: 1=Info, 2=Warning, 3=Error, 4=Critical
    /// </summary>
    public int Severity { get; set; }

    /// <summary>
    /// Underpayment amount (0 for warnings)
    /// </summary>
    public decimal ImpactAmount { get; set; }

    /// <summary>
    /// Underpayment evidence (null for warnings)
    /// Used for Error/Critical severity issues
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DescriptionDto? Description { get; set; }

    /// <summary>
    /// Warning message (null for underpayment issues)
    /// Used for Warning severity issues
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Warning { get; set; }
}

/// <summary>
/// Evidence/description for underpayment issues (API v1.3)
/// </summary>
public class DescriptionDto
{
    /// <summary>
    /// Actual value paid
    /// </summary>
    public decimal ActualValue { get; set; }

    /// <summary>
    /// Expected value per legal requirement
    /// </summary>
    public decimal ExpectedValue { get; set; }

    /// <summary>
    /// Number of units affected (hours or currency amount)
    /// </summary>
    public decimal AffectedUnits { get; set; }

    /// <summary>
    /// Unit type: Hour or Currency
    /// </summary>
    public string UnitType { get; set; } = string.Empty;

    /// <summary>
    /// Context label for display (e.g., "Retail Award Level 2", "Saturday shift", "12%")
    /// </summary>
    public string ContextLabel { get; set; } = string.Empty;
}
