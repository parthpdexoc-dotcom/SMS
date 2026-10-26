using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for requester authorized service operations.
    /// </summary>
    public interface IRequesterAuthorizedService : IGenericService<RequesterAuthorizedDto, RequesterAuthorized>
    {
        /// <summary>
        /// Gets paged requester authorized records with optional filtering by status and requester ID.
        /// </summary>
        /// <param name="isActive">Filter by status (true = active, false = inactive, null = no filter)</param>
        /// <param name="requesterId">Filter by requester ID (null = no filter)</param>
        /// <param name="pagination">Pagination parameters</param>
        /// <returns>Paged list of requester authorized records</returns>
        Task<PagedList<RequesterAuthorizedDto>> GetAllPagedAsyncWithFilters(
            bool? isActive = null,
            Guid? requesterId = null,
            PageParam? pagination = null);

        /// <summary>
        /// Gets lightweight requester authorized options for dropdowns (name label, searchable).
        /// </summary>
        Task<IEnumerable<SelectOptionDto>> GetOptionsAsync(
            string? searchTerm = null,
            Guid? requesterId = null,
            Guid? selectedId = null);
    }
}


