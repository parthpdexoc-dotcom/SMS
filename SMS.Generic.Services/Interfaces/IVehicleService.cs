using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for vehicle service operations.
    /// </summary>
    public interface IVehicleService : IGenericService<VehicleDto, Vehicle>
    {
        /// <summary>
        /// Retrieves a vehicle by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The vehicle DTO if found; otherwise, null.</returns>
        Task<VehicleDto?> GetByIdAsync(int id);
    }
}

