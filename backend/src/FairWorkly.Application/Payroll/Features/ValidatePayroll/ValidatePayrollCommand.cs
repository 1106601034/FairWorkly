using MediatR;

namespace FairWorkly.Application.Payroll.Features.ValidatePayroll;

/// <summary>
/// Command to validate payroll data from a CSV file
/// Note: The controller handles IFormFile and extracts FileStream + FileName
/// </summary>
public class ValidatePayrollCommand : IRequest<ValidationResultDto>
{
    /// <summary>
    /// The CSV file stream
    /// </summary>
    public Stream FileStream { get; set; } = null!;

    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Award type (Retail, Hospitality, Clerks)
    /// </summary>
    public string AwardType { get; set; } = string.Empty;

    /// <summary>
    /// Pay period type (Weekly, Fortnightly, Monthly)
    /// </summary>
    public string PayPeriod { get; set; } = string.Empty;

    /// <summary>
    /// Pay period start date
    /// </summary>
    public DateOnly WeekStarting { get; set; }

    /// <summary>
    /// Pay period end date
    /// </summary>
    public DateOnly WeekEnding { get; set; }

    /// <summary>
    /// Australian state (VIC, NSW, QLD, etc.)
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Enable base rate compliance check
    /// </summary>
    public bool EnableBaseRateCheck { get; set; } = true;

    /// <summary>
    /// Enable penalty rate compliance check
    /// </summary>
    public bool EnablePenaltyCheck { get; set; } = true;

    /// <summary>
    /// Enable casual loading compliance check
    /// </summary>
    public bool EnableCasualLoadingCheck { get; set; } = true;

    /// <summary>
    /// Enable superannuation compliance check
    /// </summary>
    public bool EnableSuperCheck { get; set; } = true;
}
