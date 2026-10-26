using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Repositories.Interfaces;

/// <summary>
/// Interface for payroll repository
/// </summary>
public interface IPayrollRepository
{
    /// <param name="departmentId">When set and not empty, only rows for employees in this department.</param>
    Task<List<PayrollDataRecordDto>> GetPayrollDataAsync(DateTime startDate, DateTime endDate, Guid? departmentId = null);
}
