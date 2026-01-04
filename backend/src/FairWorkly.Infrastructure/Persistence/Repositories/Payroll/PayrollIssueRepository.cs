using FairWorkly.Application.Payroll.Interfaces;
using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Infrastructure.Persistence.Repositories.Payroll;

/// <summary>
/// Repository implementation for PayrollIssue entity
/// </summary>
public class PayrollIssueRepository : IPayrollIssueRepository
{
    private readonly FairWorklyDbContext _context;

    public PayrollIssueRepository(FairWorklyDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(
        IEnumerable<PayrollIssue> issues,
        CancellationToken cancellationToken = default)
    {
        await _context.PayrollIssues.AddRangeAsync(issues, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
