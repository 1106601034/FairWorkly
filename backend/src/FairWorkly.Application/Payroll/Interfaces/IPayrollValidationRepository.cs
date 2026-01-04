using FairWorkly.Domain.Payroll.Entities;

namespace FairWorkly.Application.Payroll.Interfaces;

/// <summary>
/// Repository for PayrollValidation entity operations
/// </summary>
public interface IPayrollValidationRepository
{
    /// <summary>
    /// Adds a new PayrollValidation to the database
    /// </summary>
    /// <param name="validation">The validation to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added validation with generated Id</returns>
    Task<PayrollValidation> AddAsync(PayrollValidation validation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing PayrollValidation
    /// </summary>
    /// <param name="validation">The validation to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(PayrollValidation validation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
