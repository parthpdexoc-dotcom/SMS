using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Interfaces;

public interface IPayrollDataService
{
    /// <summary>
    /// Gets payroll data from start date through today
    /// </summary>
    /// <param name="startDate">Start date for payroll data</param>
    /// <param name="departmentId">When set and not empty, restricts to employees in this department (passed to repository SQL filter).</param>
    /// <returns>List of payroll records</returns>
    Task<List<PayrollDataRecordDto>> GetPayrollDataAsync(DateOnly startDate, DateOnly endDate, Guid? departmentId = null);
}