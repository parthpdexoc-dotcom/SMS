namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Validation rules specific to employee lifecycle operations.
    /// </summary>
    public interface IEmployeeValidationService
    {
        /// <summary>
        /// Ensures an employee can be marked inactive.
        /// Throws when the employee is used in ongoing post positions.
        /// </summary>
        Task ValidateCanDeactivateEmployeeAsync(Guid employeeId);
    }
}
