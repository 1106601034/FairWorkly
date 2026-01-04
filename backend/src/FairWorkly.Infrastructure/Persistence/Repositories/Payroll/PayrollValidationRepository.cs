using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Payroll;

/// <summary>
/// Repository implementation for PayrollValidation entity
/// </summary>
public class PayrollValidationRepository : IPayrollValidationRepository
{
    private readonly FairWorklyDbContext _context;

    public PayrollValidationRepository(FairWorklyDbContext context)
    {
        _context = context;
    }

    public async Task<PayrollValidation> AddAsync(
        PayrollValidation validation,
        CancellationToken cancellationToken = default)
    {
        _context.PayrollValidations.Add(validation);
        return await Task.FromResult(validation);
    }

    public Task UpdateAsync(
        PayrollValidation validation,
        CancellationToken cancellationToken = default)
    {
        _context.PayrollValidations.Update(validation);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
