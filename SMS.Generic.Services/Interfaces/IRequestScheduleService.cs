using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for RequestSchedule service operations.
    /// </summary>
    public interface IRequestScheduleService : IGenericService<RequestScheduleDto, RequestSchedule>
    {
        Task<RequestScheduleBulkUpdateResult> UpdateReleasedOnToReleasedAsync(DateTime asOfUtc);

        Task<RequestScheduleBulkUpdateResult> UpdateReleasedToCompleteAsync(DateTime asOfUtc);
        Task UpsertRequestProcessingResultAsync(Guid requestId,RequestScheduleProcessingResultDto result,CancellationToken ct,ReleaseAs? releaseAsOnComplete = null);

        /// <summary>
        /// Finds the request schedule covering <paramref name="dateOfWork"/> and returns
        /// <c>true</c> when that schedule's <see cref="ReleaseAs"/> is <see cref="ReleaseAs.TenureBased"/>
        /// (for <c>WaitingPostPosition.HasAppliedInTenure</c>).
        /// </summary>
        Task<bool> GetHasAppliedInTenureByDateAsync(Guid requestId, DateOnly dateOfWork);

        /// <summary>
        /// Schedules for a request. Optional dates keep windows that overlap the range.
        /// Optional <paramref name="releaseAs"/> keeps the selected release types.
        /// Optional <paramref name="requestStatus"/> keeps the selected release statuses.
        /// </summary>
        Task<PagedList<RequestScheduleDto>> GetByRequestIdAsync(
            Guid requestId,
            DateOnly? startDate,
            DateOnly? endDate,
            IReadOnlyCollection<ReleaseAs>? releaseAs,
            IReadOnlyCollection<RequestStatus>? requestStatus,
            PageParam? pageParam);

        /// <summary>
        /// Creates schedules from the request type (weekly/monthly/etc.).
        /// Schedule date is one week before each weekly occurrence, or one month before each monthly occurrence.
        /// </summary>
        Task<RequestScheduleDto> GenerateAsync(GenerateRequestScheduleDto dto, bool isSuperUser = false);
    }
}

