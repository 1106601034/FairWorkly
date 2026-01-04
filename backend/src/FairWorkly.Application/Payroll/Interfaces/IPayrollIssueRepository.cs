using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Application.Payroll.Interfaces;

/// <summary>
/// Repository for PayrollIssue entity operations
/// </summary>
public interface IPayrollIssueRepository
{
    /// <summary>
    /// Adds multiple PayrollIssues to the database
    /// </summary>
    /// <param name="issues">The issues to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddRangeAsync(IEnumerable<PayrollIssue> issues, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
