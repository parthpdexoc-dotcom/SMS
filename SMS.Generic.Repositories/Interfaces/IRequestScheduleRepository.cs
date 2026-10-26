using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for RequestSchedule-specific repository operations.
    /// </summary>
    public interface IRequestScheduleRepository : IGenericRepository<RequestSchedule>
    {
        Task<RequestScheduleBulkUpdateResult> UpdateReleasedOnToReleasedAsync(DateTime asOfUtc);

        Task<RequestScheduleBulkUpdateResult> UpdateReleasedToCompleteAsync(DateTime asOfUtc);
    }
}

