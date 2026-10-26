using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces
{
    public interface IPlaceholderService : IGenericService<PlaceholderDto, Placeholder>
    {
        /// <summary>
        /// Gets placeholders ordered by name. When <paramref name="status"/> is set, only that status;
        /// when omitted, Visible and Hidden only.
        /// </summary>
        Task<PagedList<PlaceholderDto>> GetAllAsync(PlaceholderStatus? status = null, PageParam? pageParam = null);

        /// <summary>
        /// Searches placeholders. When <paramref name="status"/> is set, only that status;
        /// when omitted, Visible and Hidden only.
        /// </summary>
        Task<IReadOnlyList<PlaceholderDto>> SearchAsync(string? searchTerm, PlaceholderStatus? status = null);

        /// <summary>
        /// Validates that new placeholder employee numbers are unique in the payload and department.
        /// </summary>
        Task ValidateBulkEmployeeNumbersAsync(IEnumerable<BulkAssignPlaceholderItemDto>? placeholders);

        /// <summary>
        /// Creates placeholder rows for items without <c>PlaceholderID</c> and writes the new IDs back.
        /// </summary>
        Task CreateMissingAndSetIdsAsync(IEnumerable<BulkAssignPlaceholderItemDto>? placeholders);

        /// <summary>
        /// Sets each placeholder's <c>PlaceholderStatus</c> from the id → status map (caller's department only).
        /// </summary>
        Task<SuccessResult<List<BulkPlaceholderStatusItemDto>>> BulkUpdateStatusAsync(IReadOnlyDictionary<Guid, PlaceholderStatus> placeholders);
    }
}
