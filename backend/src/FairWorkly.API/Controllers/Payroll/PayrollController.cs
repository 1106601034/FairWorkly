using FairWorkly.Application.Payroll.Features.ValidatePayroll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Payroll;

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly IMediator _mediator;
    private const int MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

    public PayrollController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Validates payroll data from a CSV file
    /// </summary>
    /// <param name="file">CSV file containing payroll data</param>
    /// <param name="request">Validation request parameters</param>
    /// <returns>Validation result with issues found</returns>
    [HttpPost("validation")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> Validate(
        IFormFile file,
        [FromForm] ValidatePayrollRequest request)
    {
        // Validate file
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { code = 400, msg = "CSV file is required" });
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return BadRequest(new { code = 400, msg = "File size must not exceed 10MB" });
        }

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { code = 400, msg = "Only CSV files are supported" });
        }

        // Create command from request + file stream
        var command = new ValidatePayrollCommand
        {
            FileStream = file.OpenReadStream(),
            FileName = file.FileName,
            AwardType = request.AwardType,
            PayPeriod = request.PayPeriod,
            WeekStarting = request.WeekStarting,
            WeekEnding = request.WeekEnding,
            State = request.State,
            EnableBaseRateCheck = request.EnableBaseRateCheck,
            EnablePenaltyCheck = request.EnablePenaltyCheck,
            EnableCasualLoadingCheck = request.EnableCasualLoadingCheck,
            EnableSuperCheck = request.EnableSuperCheck
        };

        var result = await _mediator.Send(command);

        return Ok(new
        {
            code = 200,
            msg = "Audit completed successfully",
            data = result
        });
    }
}

/// <summary>
/// Request model for payroll validation (excludes file which is handled separately)
/// </summary>
public class ValidatePayrollRequest
{
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
