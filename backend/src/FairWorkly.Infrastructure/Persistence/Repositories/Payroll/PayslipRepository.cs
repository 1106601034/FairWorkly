using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Payroll;

/// <summary>
/// Repository implementation for Payslip entity
/// </summary>
public class PayslipRepository : IPayslipRepository
{
    private readonly FairWorklyDbContext _context;

    public PayslipRepository(FairWorklyDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(
        IEnumerable<Payslip> payslips,
        CancellationToken cancellationToken = default)
    {
        await _context.Payslips.AddRangeAsync(payslips, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
