using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for tenure waiting position repository operations, extending IGenericRepository.
    /// </summary>
    public interface IWaitingPostPositionRepository : IGenericRepository<WaitingPostPosition>
    {
        // Add tenure waiting position-specific repository methods here if needed
        // Otherwise, leave empty - generic methods are inherited
        Task<List<WaitingPostPosition>> GetWaitingPostPositionsByEmployeeAndDateRangeAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, Guid? departmentId = null);

        /// <summary>
        /// Waiting and special-event rows for an employee with Post.Request and WaitingPostPosition.Request eager-loaded for mapping.
        /// </summary>
        Task<List<WaitingPostPosition>> GetForLoggedInEmployeeListingAsync(Guid employeeId);
    }
}

