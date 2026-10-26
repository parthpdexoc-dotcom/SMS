using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Notification history operations using the generic service pattern.
    /// </summary>
    public interface INotificationHistoryService : IGenericService<NotificationHistoryListItemDto, NotificationHistory>
    {
        Task<PagedList<NotificationHistoryListItemDto>> GetHistoryAsync(
            Guid? postPositionId = null,
            Guid? postId = null,
            Guid? requestId = null,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default);

        Task<NotificationHistoryDetailDto> GetDetailAsync(
            Guid notificationHistoryId,
            CancellationToken cancellationToken = default);

        Task SaveAsync(
            SendNotificationRequestDto request,
            IReadOnlyList<string> to,
            IReadOnlyList<string> cc,
            IReadOnlyList<string> bcc,
            IReadOnlyList<EmailAttachmentDto> attachments,
            CancellationToken cancellationToken = default);
    }
}
