using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for employee job-location readings.
    /// </summary>
    public interface IEmployeePostLocationRepository : IGenericRepository<EmployeePostLocation>
    {
        /// <summary>
        /// Projects GPS points for one employee on a job slot, oldest first.
        /// </summary>
        Task<List<EmployeePostLocationDto>> GetCoordinatesByEmployeeIdAsync(
            Guid employeeId,
            Guid postPositionId,
            CancellationToken cancellationToken = default);
    }
}
