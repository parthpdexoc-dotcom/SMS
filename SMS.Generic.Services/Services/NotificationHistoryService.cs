using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using SMS.Generic.Common;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Notification history using <see cref="GenericService{TDto, TEntity}"/>.
    /// </summary>
    public class NotificationHistoryService
        : GenericService<NotificationHistoryListItemDto, NotificationHistory>,
        INotificationHistoryService
    {
        private static readonly Expression<Func<NotificationHistory, object>>[] HistoryListIncludes =
        {
            x => x.Recipients!
        };

        private static readonly Expression<Func<NotificationHistory, object>>[] HistoryDetailIncludes =
        {
            x => x.Recipients!,
            x => x.Attachments!
        };

        private readonly INotificationHistoryRepository _historyRepository;
        private readonly IEmployeeService _employeeService;
        private readonly ITokenParser _tokenParser;

        public NotificationHistoryService(
            INotificationHistoryRepository historyRepository,
            IEmployeeService employeeService,
            IMapper mapper,
            ITokenParser tokenParser)
            : base(historyRepository, mapper, nameof(NotificationHistory.NotificationHistoryID), tokenParser)
        {
            _historyRepository = historyRepository ?? throw new ArgumentNullException(nameof(historyRepository));
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _tokenParser = tokenParser ?? throw new ArgumentNullException(nameof(tokenParser));
        }

        /// <inheritdoc />
        public override async Task<PagedList<NotificationHistoryListItemDto>> GetAllPagedAsync(
            Expression<Func<NotificationHistory, bool>>? filter = null,
            PageParam pageParam = null,
            Expression<Func<NotificationHistory, object>>[] includes = null)
        {
            includes ??= HistoryListIncludes;
            return await base.GetAllPagedAsync(
                filter,
                pageParam ?? new PageParam(),
                includes).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<PagedList<NotificationHistoryListItemDto>> GetHistoryAsync(
            Guid? postPositionId = null,
            Guid? postId = null,
            Guid? requestId = null,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            EnsureSingleNotificationContextId(postPositionId, postId, requestId);

            var effectivePageParam = pagination ?? new PageParam();
            if (string.IsNullOrWhiteSpace(effectivePageParam.SortBy))
            {
                effectivePageParam.SortBy = nameof(NotificationHistory.SentAt);
                effectivePageParam.SortDirection = "desc";
            }

            return await base.GetAllPagedAsync(
                BuildNotificationHistoryFilter(postPositionId, postId, requestId, departmentId: null),
                effectivePageParam,
                HistoryListIncludes).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<NotificationHistoryDetailDto> GetDetailAsync(
            Guid notificationHistoryId,
            CancellationToken cancellationToken = default)
        {
            var history = (await _historyRepository.GetAllAsync(
                h => h.NotificationHistoryID == notificationHistoryId && !h.IsDeleted,
                HistoryDetailIncludes,
                null).ConfigureAwait(false)).FirstOrDefault();

            var departmentId = _tokenParser.GetDepartmentID();
            if (history == null
                || (departmentId.HasValue && departmentId.Value != Guid.Empty && history.DepartmentID != departmentId.Value))
                ThrowNotFound(ModelValidationErrorCodes.NOTIFICATION_HISTORY_NOT_FOUND, notificationHistoryId);

            return MapHistoryDetail(history);
        }

        /// <inheritdoc />
        public async Task SaveAsync(
            SendNotificationRequestDto request,
            IReadOnlyList<string> to,
            IReadOnlyList<string> cc,
            IReadOnlyList<string> bcc,
            IReadOnlyList<EmailAttachmentDto> attachments,
            CancellationToken cancellationToken = default)
        {
            var departmentId = _tokenParser.GetDepartmentID();
            if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
                return;

            var allEmails = to.Concat(cc).Concat(bcc).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var employeesByEmail = await LookupEmployeesByEmailAsync(allEmails, departmentId.Value).ConfigureAwait(false);
            var sentAt = DateTime.UtcNow;
            var historyId = Guid.NewGuid();

            var history = new NotificationHistory
            {
                NotificationHistoryID = historyId,
                DepartmentID = departmentId.Value,
                RequestID = request.RequestId,
                RequesterID = request.RequesterId,
                PostID = request.PostId,
                PostPositionID = request.PostPositionId,
                ContextType = ResolveContextType(request),
                Subject = request.Subject.Trim(),
                Body = HtmlTextHelper.ConvertHtmlToPlainText(request.Body),
                Status = NotificationStatus.Sent,
                CreatedDate = sentAt,
                CreatedUserID = _tokenParser.GetEmployeeID(),
                CreatedBy = GetCurrentUserDisplayName(),
                Recipients = BuildHistoryRecipients(historyId, to, cc, bcc, employeesByEmail),
                Attachments = BuildHistoryAttachments(historyId, attachments)
            };

            await _historyRepository.UpsertAsync(history, nameof(NotificationHistory.NotificationHistoryID)).ConfigureAwait(false);
        }

        private async Task<IReadOnlyDictionary<string, EmployeeDto>> LookupEmployeesByEmailAsync(
            IReadOnlyList<string> emails,
            Guid departmentId)
        {
            if (emails.Count == 0)
                return new Dictionary<string, EmployeeDto>(StringComparer.OrdinalIgnoreCase);

            var emailSet = emails.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var employees = (await _employeeService.GetAllAsync(
                e => e.DepartmentID == departmentId
                    && ((e.PrimaryEmail != null && emailSet.Contains(e.PrimaryEmail))
                        || (e.OffDutyCallBackEmail != null && emailSet.Contains(e.OffDutyCallBackEmail))),
                null,
                null).ConfigureAwait(false)).ToList();

            var map = new Dictionary<string, EmployeeDto>(StringComparer.OrdinalIgnoreCase);
            foreach (var employee in employees)
            {
                if (!string.IsNullOrWhiteSpace(employee.PrimaryEmail))
                    map.TryAdd(employee.PrimaryEmail.Trim(), employee);
                if (!string.IsNullOrWhiteSpace(employee.OffDutyCallBackEmail))
                    map.TryAdd(employee.OffDutyCallBackEmail.Trim(), employee);
            }

            return map;
        }

        private static void EnsureSingleNotificationContextId(Guid? postPositionId, Guid? postId, Guid? requestId)
        {
            var providedCount = new[] { postPositionId, postId, requestId }.Count(id => id.HasValue && id.Value != Guid.Empty);
            if (providedCount != 1)
                ThrowBadRequest(ModelValidationErrorCodes.REQUIRED_NOTIFICATION_SINGLE_CONTEXT_ID);
        }

        private static Expression<Func<NotificationHistory, bool>> BuildNotificationHistoryFilter(
            Guid? postPositionId,
            Guid? postId,
            Guid? requestId,
            Guid? departmentId)
        {
            if (postPositionId.HasValue && postPositionId.Value != Guid.Empty)
            {
                return departmentId.HasValue && departmentId.Value != Guid.Empty
                    ? h => h.PostPositionID == postPositionId.Value
                        && h.ContextType == NotificationContextType.PostPosition
                        && !h.IsDeleted
                        && h.DepartmentID == departmentId.Value
                    : h => h.PostPositionID == postPositionId.Value
                        && h.ContextType == NotificationContextType.PostPosition
                        && !h.IsDeleted;
            }

            if (postId.HasValue && postId.Value != Guid.Empty)
            {
                return departmentId.HasValue && departmentId.Value != Guid.Empty
                    ? h => h.PostID == postId.Value
                        && h.ContextType == NotificationContextType.Post
                        && !h.IsDeleted
                        && h.DepartmentID == departmentId.Value
                    : h => h.PostID == postId.Value
                        && h.ContextType == NotificationContextType.Post
                        && !h.IsDeleted;
            }

            return departmentId.HasValue && departmentId.Value != Guid.Empty
                ? h => h.RequestID == requestId!.Value
                    && h.ContextType == NotificationContextType.Request
                    && !h.IsDeleted
                    && h.DepartmentID == departmentId.Value
                : h => h.RequestID == requestId!.Value
                    && h.ContextType == NotificationContextType.Request
                    && !h.IsDeleted;
        }

        private static List<NotificationHistoryRecipient> BuildHistoryRecipients(
            Guid historyId,
            IReadOnlyList<string> to,
            IReadOnlyList<string> cc,
            IReadOnlyList<string> bcc,
            IReadOnlyDictionary<string, EmployeeDto> employeesByEmail)
        {
            var recipients = new List<NotificationHistoryRecipient>();
            AddHistoryRecipients(recipients, historyId, NotificationRecipientType.To, to, employeesByEmail);
            AddHistoryRecipients(recipients, historyId, NotificationRecipientType.Cc, cc, employeesByEmail);
            AddHistoryRecipients(recipients, historyId, NotificationRecipientType.Bcc, bcc, employeesByEmail);
            return recipients;
        }

        private static void AddHistoryRecipients(
            List<NotificationHistoryRecipient> recipients,
            Guid historyId,
            NotificationRecipientType recipientType,
            IReadOnlyList<string> emails,
            IReadOnlyDictionary<string, EmployeeDto> employeesByEmail)
        {
            foreach (var email in emails)
            {
                employeesByEmail.TryGetValue(email, out var employee);
                recipients.Add(new NotificationHistoryRecipient
                {
                    NotificationHistoryRecipientID = Guid.NewGuid(),
                    NotificationHistoryID = historyId,
                    RecipientType = recipientType,
                    Email = email,
                    EmployeeID = employee?.EmployeeID,
                    EmployeeName = employee == null
                        ? null
                        : BaseDto.FormatDisplayName(employee.LastName, employee.FirstName)
                });
            }
        }

        private static List<NotificationHistoryAttachment> BuildHistoryAttachments(
            Guid historyId,
            IReadOnlyList<EmailAttachmentDto> attachments)
        {
            var rows = new List<NotificationHistoryAttachment>(attachments.Count);

            foreach (var attachment in attachments)
            {
                rows.Add(new NotificationHistoryAttachment
                {
                    NotificationHistoryAttachmentID = Guid.NewGuid(),
                    NotificationHistoryID = historyId,
                    FileUploadedId = attachment.FileUploadedId,
                    FileName = attachment.FileName,
                    FileSizeBytes = attachment.Content.LongLength,
                    ContentType = attachment.ContentType
                });
            }

            return rows;
        }

        private static NotificationContextType ResolveContextType(SendNotificationRequestDto request)
        {
            if (request.ContextType.HasValue)
                return request.ContextType.Value;

            if (request.PostPositionId is Guid postPositionId && postPositionId != Guid.Empty)
                return NotificationContextType.PostPosition;

            if (request.PostId is Guid postId && postId != Guid.Empty)
                return NotificationContextType.Post;

            return NotificationContextType.Request;
        }

        private string? GetCurrentUserDisplayName()
        {
            var fullName = _tokenParser.GetClaimValue(ClaimNames.FullName);
            if (string.IsNullOrWhiteSpace(fullName))
            {
                fullName = PersonNameFormatHelper.FormatLastNameCommaFirstName(
                    _tokenParser.GetClaimValue(ClaimNames.LastName),
                    _tokenParser.GetClaimValue(ClaimNames.FirstName));
            }

            return string.IsNullOrWhiteSpace(fullName) ? _tokenParser.GetUsername() : fullName;
        }

        private static NotificationHistoryListItemDto MapHistoryListItem(NotificationHistory history)
        {
            var recipients = history.Recipients ?? Array.Empty<NotificationHistoryRecipient>();
            var toCount = 0;
            var ccCount = 0;
            var bccCount = 0;

            foreach (var recipient in recipients)
            {
                switch (recipient.RecipientType)
                {
                    case NotificationRecipientType.To:
                        toCount++;
                        break;
                    case NotificationRecipientType.Cc:
                        ccCount++;
                        break;
                    case NotificationRecipientType.Bcc:
                        bccCount++;
                        break;
                }
            }

            return new NotificationHistoryListItemDto
            {
                NotificationHistoryId = history.NotificationHistoryID,
                Subject = history.Subject,
                SentBy = history.CreatedBy,
                SentAt = history.SentAt,
                ToCount = toCount,
                CcCount = ccCount,
                BccCount = bccCount,
                Status = history.Status == NotificationStatus.Sent ? "Sent" : "Failed"
            };
        }

        private static NotificationHistoryDetailDto MapHistoryDetail(NotificationHistory history)
        {
            var listItem = MapHistoryListItem(history);
            return new NotificationHistoryDetailDto
            {
                NotificationHistoryId = listItem.NotificationHistoryId,
                Subject = listItem.Subject,
                SentBy = listItem.SentBy,
                SentAt = listItem.SentAt,
                ToCount = listItem.ToCount,
                CcCount = listItem.CcCount,
                BccCount = listItem.BccCount,
                Status = listItem.Status,
                Body = history.Body,
                To = MapHistoryRecipients(history.Recipients, NotificationRecipientType.To),
                Cc = MapHistoryRecipients(history.Recipients, NotificationRecipientType.Cc),
                Bcc = MapHistoryRecipients(history.Recipients, NotificationRecipientType.Bcc),
                Attachments = history.Attachments.Select(a => new NotificationHistoryAttachmentDto
                {
                    FileUploadedId = a.FileUploadedId,
                    FileName = a.FileName,
                    FileSizeBytes = a.FileSizeBytes
                }).ToList()
            };
        }

        private static List<NotificationHistoryRecipientDto> MapHistoryRecipients(
            IEnumerable<NotificationHistoryRecipient> recipients,
            NotificationRecipientType recipientType) =>
            recipients
                .Where(r => r.RecipientType == recipientType)
                .Select(r => new NotificationHistoryRecipientDto
                {
                    Email = r.Email,
                    EmployeeName = recipientType == NotificationRecipientType.To
                        ? r.EmployeeName
                        : null
                })
                .ToList();

        private static void ThrowBadRequest(string errorCode, params object[] args) =>
            throw new WebApiException(
                HttpStatusCode.BadRequest,
                ((int)HttpStatusCode.BadRequest).ToString(),
                ModelValidationErrorCodes.GetErrorMessage(errorCode, args));

        private static void ThrowNotFound(string errorCode, params object[] args) =>
            throw new WebApiException(
                HttpStatusCode.NotFound,
                ((int)HttpStatusCode.NotFound).ToString(),
                ModelValidationErrorCodes.GetErrorMessage(errorCode, args));
    }
}
