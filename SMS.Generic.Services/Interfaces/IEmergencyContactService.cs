using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for emergency contact service operations.
    /// </summary>
    public interface IEmergencyContactService : IGenericService<EmergencyContactDto, EmergencyContact>
    {
        /// <summary>
        /// Retrieves an emergency contact by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The emergency contact DTO if found; otherwise, null.</returns>
        Task<EmergencyContactDto?> GetByIdAsync(int id);
    }
}

