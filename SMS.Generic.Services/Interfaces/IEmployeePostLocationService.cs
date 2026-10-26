using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    public interface IEmployeePostLocationService : IGenericService<EmployeePostLocationDto, EmployeePostLocation>
    {
        Task<EmployeePostLocationDto> SavePostLocationAsync(EmployeePostLocationDto request);

        /// <summary>
        /// Gets GPS points for an employee on a job slot, oldest first.
        /// </summary>
        Task<List<EmployeePostLocationDto>> GetCoordinatesByEmployeeIdAsync(
            Guid employeeId,
            Guid postPositionId,
            CancellationToken cancellationToken = default);
    }
}
