using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for HR division service operations.
    /// </summary>
    public interface IHrDivisionService : IGenericService<HrDivisionDto, HrDivision>
    {
        /// <summary>
        /// Retrieves an HR division by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The HR division DTO if found; otherwise, null.</returns>
        Task<HrDivisionDto?> GetByIdAsync(int id);
    }
}

