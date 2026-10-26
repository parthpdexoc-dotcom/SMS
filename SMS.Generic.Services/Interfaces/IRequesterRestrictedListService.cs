using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for requester restricted list service operations.
    /// </summary>
    public interface IRequesterRestrictedListService : IGenericService<RequesterRestrictedListDto, RequesterRestrictedList>
    {
        /// <summary>
        /// Gets all requester restricted list records with navigation properties included.
        /// </summary>
        /// <param name="filter">Optional filter expression.</param>
        /// <param name="includes">Optional navigation properties to include.</param>
        /// <param name="pageParam">Optional pagination parameters.</param>
        /// <param name="requesterId">When set, only rows for this requester are returned.</param>
        /// <returns>Paged requester restricted list response DTOs with names.</returns>
        Task<PagedList<RequesterRestrictedListResponseDto>> GetAllWithNamesPagedAsync(
            Expression<Func<RequesterRestrictedList, bool>>? filter = null,
            Expression<Func<RequesterRestrictedList, object>>[]? includes = null,
            PageParam? pageParam = null,
            Guid? requesterId = null);

        /// <summary>
        /// Gets a requester restricted list record by ID with navigation properties included.
        /// </summary>
        /// <param name="id">The requester restricted list ID.</param>
        /// <returns>The requester restricted list response DTO with names, or null if not found.</returns>
        Task<RequesterRestrictedListResponseDto?> GetByIdWithNamesAsync(Guid id);
        Task<List<RequesterRestrictedListDto>> GetRequesterIdsByEmployeeId(Guid employeeId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Gets employee dropdown options excluding employees already restricted for the requester.
        /// </summary>
        Task<IEnumerable<SelectOptionDto>> GetAvailableEmployeeOptionsAsync(
            string? searchTerm = null,
            Guid? requesterId = null);
    }
}

