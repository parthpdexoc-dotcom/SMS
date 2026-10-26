using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for response area service operations.
    /// </summary>
    public interface IDepartmentResponseAreaService : IGenericService<DepartmentResponseAreaDto, DepartmentResponseArea>
    {
        /// <summary>
        /// Gets response areas by department ID from the token.
        /// </summary>
        /// <returns>A collection of response areas for the department from the token.</returns>
        Task<IEnumerable<DepartmentResponseAreaDto>> GetAllActiveDepartmentAsync(Guid? selectedId = null);
    }
}
