using FluentValidation;

namespace FairWorkly.Application.Payroll.Features.ValidatePayroll;

/// <summary>
/// Validator for ValidatePayrollCommand
/// Note: File size validation is done in the controller before creating the command
/// </summary>
public class ValidatePayrollValidator : AbstractValidator<ValidatePayrollCommand>
{
    private static readonly string[] ValidAwardTypes = { "Retail", "Hospitality", "Clerks" };
    private static readonly string[] ValidPayPeriods = { "Weekly", "Fortnightly", "Monthly" };
    private static readonly string[] ValidStates = { "VIC", "NSW", "QLD", "SA", "WA", "TAS", "NT", "ACT" };

    public ValidatePayrollValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("CSV file is required");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required")
            .Must(name => name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only CSV files are supported");

        RuleFor(x => x.AwardType)
            .NotEmpty()
            .WithMessage("Award type is required")
            .Must(type => ValidAwardTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Award type must be one of: {string.Join(", ", ValidAwardTypes)}");

        RuleFor(x => x.PayPeriod)
            .NotEmpty()
            .WithMessage("Pay period is required")
            .Must(period => ValidPayPeriods.Contains(period, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Pay period must be one of: {string.Join(", ", ValidPayPeriods)}");

        RuleFor(x => x.WeekStarting)
            .NotEmpty()
            .WithMessage("Week starting date is required");

        RuleFor(x => x.WeekEnding)
            .NotEmpty()
            .WithMessage("Week ending date is required")
            .GreaterThanOrEqualTo(x => x.WeekStarting)
            .WithMessage("Week ending date must be on or after week starting date");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required")
            .Must(state => ValidStates.Contains(state, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"State must be one of: {string.Join(", ", ValidStates)}");
    }
}
