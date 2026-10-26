using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for employee device token service operations.
    /// </summary>
    public interface IEmployeeDeviceTokenService : IGenericService<EmployeeDeviceTokenDto, EmployeeDeviceToken>
    {
        /// <summary>
        /// Upserts the device token for an employee: inserts when no record exists, otherwise updates.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="deviceToken">The device token (e.g. FCM/APNs).</param>
        Task UpsertDeviceTokenAsync(Guid employeeId, string deviceToken);
    }
}
