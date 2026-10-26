using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for activity type service operations.
    /// </summary>
    public interface IActivityTypeService : IGenericService<ActivityTypeDto, ActivityType>
    {
        /// <summary>
        /// Retrieves an activity type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The activity type DTO if found; otherwise, null.</returns>
        Task<ActivityTypeDto?> GetByIdAsync(int id);
    }
}

