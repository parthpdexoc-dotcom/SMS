using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for requester restricted list repository operations, extending IGenericRepository.
    /// </summary>
    public interface IRequesterRestrictedListRepository : IGenericRepository<RequesterRestrictedList>
    {
        Task<List<RequesterRestrictedList>> GetRequesterIdsByEmployeeId(Guid employeeId, DateOnly startDate, DateOnly endDate);
    }
}

