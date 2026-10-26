using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    /// Handles send-notification emails and notification modal data using <see cref="GenericService{TDto, TEntity}"/> for post position queries.
    /// </summary>
    public class NotificationService : GenericService<PostPositionDto, PostPosition>, INotificationService
    {
        public const long MaxAttachmentSizeBytes = 15 * 1024 * 1024;

        private static readonly HashSet<string> AttachmentFormFieldNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "attachments",
            "attachment",
            "files"
        };

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, DateTime> LastJobsPostedNotificationByDept = new();

        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmployeeService _employeeService;
        private readonly IPostPositionRepository _postPositionRepository;
        private readonly IPostService _postService;
        private readonly IRequestService _requestService;
        private readonly IRequesterService _requesterService;
        private readonly IUploadFileService _uploadFileService;
        private readonly INotificationHistoryService _notificationHistoryService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;
        private readonly ITokenParser _tokenParser;

        public NotificationService(
            IEmailSender emailSender,
            ISmsSender smsSender,
            IEmailTemplateService emailTemplateService,
            IEmployeeService employeeService,
            IPostPositionRepository postPositionRepository,
            IPostService postService,
            IRequestService requestService,
            IMapper mapper,
            IRequesterService requesterService,
            IUploadFileService uploadFileService,
            INotificationHistoryService notificationHistoryService,
            ITokenParser tokenParser,
            IConfiguration configuration,
            ILogger<NotificationService> logger)
            : base(postPositionRepository, mapper, nameof(PostPosition.PostPositionID), tokenParser)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _postPositionRepository = postPositionRepository ?? throw new ArgumentNullException(nameof(postPositionRepository));
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _requesterService = requesterService ?? throw new ArgumentNullException(nameof(requesterService));
            _uploadFileService = uploadFileService ?? throw new ArgumentNullException(nameof(uploadFileService));
            _notificationHistoryService = notificationHistoryService ?? throw new ArgumentNullException(nameof(notificationHistoryService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenParser = tokenParser ?? throw new ArgumentNullException(nameof(tokenParser));
        }

        /// <inheritdoc />
        public async Task<NotificationDataDto> GetNotificationDataAsync(
            NotificationDataRequestDto request,
            CancellationToken cancellationToken = default)
        {
            request ??= new NotificationDataRequestDto();

            EnsureSingleNotificationContextId(request.PostPositionId, request.PostId, request.RequestId);

            if (request.PostPositionId.HasValue && request.PostPositionId.Value != Guid.Empty)
                return await PopulatePostPositionNotificationDataAsync(request.PostPositionId.Value, cancellationToken).ConfigureAwait(false);

            return await PopulateAvailableJobNotificationDataAsync(
                request.PostId,
                request.RequestId,
                request.PostIds,
                request.StartDate,
                request.EndDate,
                request.SearchTerm,
                request.PageParam,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SendNotificationAsync(HttpRequest httpRequest, CancellationToken cancellationToken = default)
        {
            var (request, multipartAttachments) = await ParseSendNotificationRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            ValidateSendNotificationRequest(request);

            var validTo = NormalizeEmails(request.To);
            var validCc = NormalizeEmails(request.Cc);
            var validBcc = NormalizeEmails(request.Bcc);

            if (validTo.Count == 0)
                ThrowBadRequest(ModelValidationErrorCodes.INVALID_NOTIFICATION_TO_RECIPIENTS);

            multipartAttachments = await EnsureAttachmentFileIdsAsync(
                httpRequest, request, multipartAttachments, cancellationToken).ConfigureAwait(false);

            if (request.AttachmentFileIds is not { Count: > 0 }
                && multipartAttachments.Count == 0
                && httpRequest.HasFormContentType)
            {
                multipartAttachments = await ReadAttachmentsAsync(httpRequest.Form.Files, cancellationToken).ConfigureAwait(false);
            }

            var attachments = await ResolveAllAttachmentsAsync(request, multipartAttachments, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation(
                "Sending notification email to {ToCount} To, {CcCount} Cc, {BccCount} Bcc recipient(s) with {AttachmentCount} attachment(s). Subject: {Subject}",
                validTo.Count,
                validCc.Count,
                validBcc.Count,
                attachments.Count,
                request.Subject);

            await _emailSender.SendAsync(
                validTo,
                request.Subject,
                request.Body,
                isBodyHtml: true,
                ccAddresses: validCc.Count > 0 ? validCc : null,
                bccAddresses: validBcc.Count > 0 ? validBcc : null,
                attachments: attachments.Count > 0 ? attachments : null,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await _notificationHistoryService.SaveAsync(
                request,
                validTo,
                validCc,
                validBcc,
                attachments,
                cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Notification email send completed for subject: {Subject}", request.Subject);
        }

        /// <inheritdoc />
        public async Task SendEmployeePreferenceNotificationsAsync(
            bool notifyWhenJobsPosted,
            Guid postId,
            EmailType emailType = EmailType.JobsPostedEmail,
            CancellationToken cancellationToken = default)
        {
            if (!notifyWhenJobsPosted || postId == Guid.Empty)
                return;

            var post = await _postService.GetByIdAsync(postId).ConfigureAwait(false);
            if (post == null)
                return;

            RequestDto? requestDto = null;
            Guid? departmentId = null;

            if (post.RequestID.HasValue && post.RequestID.Value != Guid.Empty)
            {
                requestDto = await _requestService.GetByIdAsync(post.RequestID.Value).ConfigureAwait(false);
                if (requestDto != null && requestDto.DepartmentID != Guid.Empty)
                    departmentId = requestDto.DepartmentID;
            }

            departmentId ??= _tokenParser.GetDepartmentID();

            if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
                return;

            // When multiple jobs or requests are released at the same time in the same department,
            // send only ONE email to eligible employees (deduplicated within 30 seconds).
            if (emailType == EmailType.JobsPostedEmail)
            {
                var nowUtc = DateTime.UtcNow;
                if (LastJobsPostedNotificationByDept.TryGetValue(departmentId.Value, out var lastSent)
                    && (nowUtc - lastSent).TotalSeconds < 30)
                {
                    _logger.LogInformation(
                        "Skipping duplicate JobsPostedEmail for department {DepartmentId} (already sent at {LastSent:O}).",
                        departmentId.Value,
                        lastSent);
                    return;
                }

                LastJobsPostedNotificationByDept[departmentId.Value] = nowUtc;
            }

            List<string> jobTitles = new();
            if (emailType == EmailType.RequestReleasedEmail && post.RequestID.HasValue && post.RequestID.Value != Guid.Empty)
            {
                var posts = await _postService.GetAllAsync(p => p.RequestID == post.RequestID.Value, pageParam: null).ConfigureAwait(false);
                jobTitles = posts
                    .Select(p => p.PostTitle?.Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t!)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }

            if (jobTitles.Count == 0 && !string.IsNullOrWhiteSpace(post.PostTitle))
            {
                jobTitles.Add(post.PostTitle.Trim());
            }

            var requestName = !string.IsNullOrWhiteSpace(requestDto?.Title)
                ? requestDto.Title.Trim()
                : (PersonNameFormatHelper.FormatLastNameCommaFirstName(requestDto?.LastName, requestDto?.FirstName)
                   ?? requestDto?.RequestCode
                   ?? string.Empty);

            var jobNamesFormatted = jobTitles.Count > 0 ? string.Join(", ", jobTitles) : string.Empty;

            var employees = (await _employeeService.GetAllAsync(
                e => e.DepartmentID == departmentId.Value && e.IsApproved && e.NotifyWhenJobsPosted,
                pageParam: null).ConfigureAwait(false)).ToList();

            if (employees.Count == 0)
            {
                _logger.LogInformation("No approved employees with NotifyWhenJobsPosted enabled found for department {DepartmentId}.", departmentId.Value);
                return;
            }

            var smsMessage = await GetTemplateSmsMessageAsync(emailType).ConfigureAwait(false);

            foreach (var employee in employees)
            {
                try
                {
                    var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        [EnumHelper.ToStringValue(EmailKeyword.RequestName)] = requestName,
                        ["RequestName"] = requestName,
                        ["Request"] = requestName,
                        [EnumHelper.ToStringValue(EmailKeyword.Post)] = jobNamesFormatted,
                        ["JobNames"] = jobNamesFormatted,
                        ["JobName"] = jobNamesFormatted,
                        ["JobsList"] = jobNamesFormatted,
                        ["Jobs"] = jobNamesFormatted,
                        [EnumHelper.ToStringValue(EmailKeyword.EmployeeName)] = employee.FirstName?.Trim() ?? string.Empty,
                        [EnumHelper.ToStringValue(EmailKeyword.FullName)] = PersonNameFormatHelper.FormatLastNameCommaFirstName(employee.LastName, employee.FirstName) ?? employee.FirstName?.Trim() ?? string.Empty,
                        ["Employee"] = PersonNameFormatHelper.FormatLastNameCommaFirstName(employee.LastName, employee.FirstName) ?? employee.FirstName?.Trim() ?? string.Empty,
                        [EnumHelper.ToStringValue(EmailKeyword.DepartmentName)] = string.Empty
                    };

                    await SendEmployeeTemplateEmailAsync(
                        employee,
                        emailType,
                        templateParameters,
                        cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send {EmailType} to employee {EmployeeId} ({Email})", emailType, employee.EmployeeID, employee.PrimaryEmail);
                }

                if (!string.IsNullOrWhiteSpace(smsMessage))
                {
                    await TrySendScheduleSmsAsync(
                        employee,
                        smsMessage,
                        cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <inheritdoc />
        public async Task SendJobStartingReminderEmailAsync(
            EmployeeDto employee,
            PostPositionDto postPosition,
            EmailType emailType,
            CancellationToken cancellationToken = default)
        {
            var postTitle = postPosition.Post?.PostTitle?.Trim() ?? string.Empty;
            var postDate = DateHelper.ToDisplayDate(postPosition.DateOfWork);
            var postStartTime = postPosition.PostStartTime ?? postPosition.Post?.PostStartTime;
            var startTime = postStartTime?.ToString("h:mm tt", CultureInfo.InvariantCulture) ?? string.Empty;

            var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = postDate,
                [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = startTime,
                [EnumHelper.ToStringValue(EmailKeyword.EmployeeName)] = employee.FirstName?.Trim() ?? string.Empty
            };

            try
            {
                await SendEmployeeTemplateEmailAsync(employee, emailType, templateParameters, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Failed to send {EmailType} to employee {EmployeeId}.", emailType, employee.EmployeeID);
            }

            var smsMessage = await GetTemplateSmsMessageAsync(emailType, templateParameters).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(smsMessage))
            {
                await TrySendScheduleSmsAsync(
                    employee,
                    smsMessage,
                    cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task TrySendScheduleSmsAsync(EmployeeDto employee, string body, CancellationToken cancellationToken)
        {
            if (!employee.HasSmsConsent || string.IsNullOrWhiteSpace(employee.SmsTelephoneNumber))
                return;

            try
            {
                await _smsSender.SendAsync(employee.SmsTelephoneNumber, body, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Failed to send schedule SMS to employee {EmployeeId}.", employee.EmployeeID);
            }
        }


        private async Task<string?> GetTemplateSmsMessageAsync(
            EmailType emailType,
            IReadOnlyDictionary<string, string>? templateParameters = null)
        {
            var emailTemplate = (await _emailTemplateService.GetAllAsync(
                t => t.EmailType == emailType,
                pageParam: null).ConfigureAwait(false)).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(emailTemplate?.SmsContent))
                return null;

            return FormatSmsMessage(emailTemplate.SmsContent, templateParameters);
        }

        private static string FormatSmsMessage(string smsContent, IReadOnlyDictionary<string, string>? templateParameters = null)
        {
            var smsBody = smsContent;
            if (templateParameters?.Count > 0)
            {
                smsBody = templateParameters.Aggregate(smsBody, (text, p) =>
                    !string.IsNullOrEmpty(p.Key)
                        ? text.Replace($"[{p.Key}]", p.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                        : text);
            }

            return WithSmsFooter(smsBody.StartsWith("CheckTheSite.com", StringComparison.OrdinalIgnoreCase)
                ? smsBody
                : $"CheckTheSite.com: {smsBody}");
        }

        private static string WithSmsFooter(string message)
        {
            const string footer = " Reply STOP to cancel. Reply HELP for help.";
            const int maxLength = 320;
            if (message.Length + footer.Length <= maxLength)
                return message + footer;

            var room = maxLength - footer.Length - 3;
            return message[..Math.Max(room, 0)].TrimEnd() + "..." + footer;
        }

        private async Task SendEmployeeTemplateEmailAsync(
            EmployeeDto employee,
            EmailType emailType,
            Dictionary<string, string>? templateParameters,
            CancellationToken cancellationToken)
        {
            var resolvedEmail = employee.OffDutyCallBackEmail?.Trim() ?? employee.PrimaryEmail?.Trim();
            if (string.IsNullOrWhiteSpace(resolvedEmail))
            {
                await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(employee, emailType, cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            await _emailTemplateService.SendMailAsync(
                new SendMailRequestDto(emailType, resolvedEmail, fromEmail: null, templateParameters),
                cancellationToken).ConfigureAwait(false);
        }

        private async Task<(SendNotificationRequestDto Request, IReadOnlyList<EmailAttachmentDto> Attachments)> ParseSendNotificationRequestAsync(
            HttpRequest httpRequest,
            CancellationToken cancellationToken)
        {
            if (httpRequest.HasFormContentType)
            {
                var form = httpRequest.Form;
                var request = ParseSendNotificationRequestFromForm(form);
                if (CanDeferMultipartAttachmentRead(request, form.Files))
                    return (request, Array.Empty<EmailAttachmentDto>());

                var attachments = await ReadAttachmentsAsync(form.Files, cancellationToken).ConfigureAwait(false);
                return (request, attachments);
            }

            var jsonRequest = await httpRequest.ReadFromJsonAsync<SendNotificationRequestDto>(cancellationToken).ConfigureAwait(false);
            if (jsonRequest == null)
                ThrowBadRequest(ModelValidationErrorCodes.REQUIRED_NOTIFICATION_REQUEST_BODY);

            return (jsonRequest, Array.Empty<EmailAttachmentDto>());
        }

        private static void ValidateSendNotificationRequest(SendNotificationRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Subject))
                ThrowBadRequest(ModelValidationErrorCodes.REQUIRED_NOTIFICATION_SUBJECT);

            if (string.IsNullOrWhiteSpace(request.Body))
                ThrowBadRequest(ModelValidationErrorCodes.REQUIRED_NOTIFICATION_BODY);

            if (request.To == null || request.To.Count == 0)
                ThrowBadRequest(ModelValidationErrorCodes.REQUIRED_NOTIFICATION_TO_RECIPIENTS);
        }

        private async Task<NotificationDataDto> PopulatePostPositionNotificationDataAsync(
            Guid postPositionId,
            CancellationToken cancellationToken)
        {
            var postPosition = await GetByIdAsync(
                postPositionId,
                new Expression<Func<PostPosition, object>>[]
                {
                    x => x.Employee!,
                    x => x.Placeholder!,
                    x => x.Post!,
                    x => x.Post!.Request!
                }).ConfigureAwait(false);

            if (postPosition == null)
                ThrowNotFound(ModelValidationErrorCodes.NOTIFICATION_POST_POSITION_NOT_FOUND, postPositionId);

            if (!postPosition.IsPlaceholder && postPosition.EmployeeID == Guid.Empty)
                ThrowBadRequest(ModelValidationErrorCodes.NOTIFICATION_POST_POSITION_NO_EMPLOYEE);

            var post = postPosition.Post
                ?? throw CreateNotFoundException(ModelValidationErrorCodes.NOTIFICATION_POST_NOT_FOUND, postPosition.PostID);

            var requester = await GetRequesterAsync(post.Request?.RequesterID).ConfigureAwait(false);

            return new NotificationDataDto
            {
                RequestId = post.RequestID,
                PostId = post.PostID,
                EmployeeId = postPosition.EmployeeID,
                RequesterName = requester?.Name,
                SuggestedSubject = NotificationDataDto.BuildPostSuggestedSubject(post.PostTitle, post.PostCode),
                PostTitle = post.PostTitle,
                PostLocation = post.PostLocation,
                PostCode = post.PostCode,
                Employee = MapNotificationEmployee(postPosition),
                SuggestedRecipients = BuildSuggestedRecipients(requester, post.Request, new[] { post }),
            };
        }

        private async Task<NotificationDataDto> PopulateAvailableJobNotificationDataAsync(
            Guid? postId,
            Guid? requestId,
            List<Guid>? postIds,
            DateOnly? startDate,
            DateOnly? endDate,
            string? searchTerm,
            PageParam? pagination,
            CancellationToken cancellationToken)
        {
            if (postId.HasValue && postId.Value != Guid.Empty)
                return await PopulatePostNotificationDataAsync(postId.Value, startDate, endDate, searchTerm, pagination, cancellationToken).ConfigureAwait(false);

            return await PopulateRequestNotificationDataAsync(requestId!.Value, postIds, startDate, endDate, searchTerm, pagination, cancellationToken).ConfigureAwait(false);
        }

        private async Task<NotificationDataDto> PopulatePostNotificationDataAsync(
            Guid postId,
            DateOnly? startDate,
            DateOnly? endDate,
            string? searchTerm,
            PageParam? pagination,
            CancellationToken cancellationToken)
        {
            var post = await _postService.GetPostWithRequestAsync(postId).ConfigureAwait(false);
            if (post == null)
                ThrowNotFound(ModelValidationErrorCodes.NOTIFICATION_POST_NOT_FOUND, postId);

            var assignedEmployees = await GetAssignedEmployeesPagedAsync(
                new[] { postId }, startDate, endDate, searchTerm, pagination).ConfigureAwait(false);
            var requester = await GetRequesterAsync(post.Request?.RequesterID).ConfigureAwait(false);

            return new NotificationDataDto
            {
                PostId = postId,
                RequesterName = requester?.Name,
                SuggestedSubject = NotificationDataDto.BuildPostSuggestedSubject(post.PostTitle, post.PostCode),
                PostTitle = post.PostTitle,
                PostLocation = post.PostLocation,
                PostCode = post.PostCode,
                AssignedEmployees = assignedEmployees,
                SuggestedRecipients = BuildSuggestedRecipients(requester, post.Request, new[] { post }),
            };
        }

        private async Task<NotificationDataDto> PopulateRequestNotificationDataAsync(
            Guid requestId,
            List<Guid>? postIds,
            DateOnly? startDate,
            DateOnly? endDate,
            string? searchTerm,
            PageParam? pagination,
            CancellationToken cancellationToken)
        {
            var request = await GetRequestAsync(requestId).ConfigureAwait(false);
            if (request == null)
                ThrowNotFound(ModelValidationErrorCodes.NOTIFICATION_REQUEST_NOT_FOUND, requestId);

            var postsList = await GetRequestPostsForNotificationAsync(requestId, postIds).ConfigureAwait(false);
            var postIdsToQuery = postsList.Select(p => p.PostID!.Value).ToList();
            var requester = await GetRequesterAsync(request.RequesterID).ConfigureAwait(false);

            return new NotificationDataDto
            {
                RequestId = requestId,
                RequesterName = requester?.Name,
                SuggestedSubject = NotificationDataDto.BuildRequestSuggestedSubject(request.Title, request.RequestCode),
                Posts = MapNotificationPosts(postsList),
                AssignedEmployees = await GetAssignedEmployeesPagedAsync(
                    postIdsToQuery, startDate, endDate, searchTerm, pagination).ConfigureAwait(false),
                SuggestedRecipients = BuildSuggestedRecipients(requester, request, postsList),
            };
        }

        private static void EnsureSingleNotificationContextId(Guid? postPositionId, Guid? postId, Guid? requestId)
        {
            var providedCount = new[] { postPositionId, postId, requestId }.Count(id => id.HasValue && id.Value != Guid.Empty);
            if (providedCount != 1)
                ThrowBadRequest(ModelValidationErrorCodes.REQUIRED_NOTIFICATION_SINGLE_CONTEXT_ID);
        }

        private async Task<List<PostDto>> GetRequestPostsForNotificationAsync(Guid requestId, List<Guid>? postIds)
        {
            var postsList = (await _postService.GetPostByRequestIdAsync(requestId, null).ConfigureAwait(false))?.Data?
                .Where(p => p.PostID.HasValue && p.PostID.Value != Guid.Empty)
                .ToList() ?? new List<PostDto>();

            if (postIds is not { Count: > 0 })
                return postsList;

            var postIdFilter = postIds.ToHashSet();
            return postsList.Where(p => postIdFilter.Contains(p.PostID!.Value)).ToList();
        }

        private static List<NotificationPostDto> MapNotificationPosts(IReadOnlyList<PostDto> posts) =>
            posts.Select(post => new NotificationPostDto
            {
                PostId = post.PostID!.Value,
                PostTitle = post.PostTitle,
                PostLocation = post.PostLocation,
            }).ToList();

        private async Task<PagedList<NotificationEmployeeDto>> GetAssignedEmployeesPagedAsync(
            IReadOnlyCollection<Guid> postIds,
            DateOnly? startDate,
            DateOnly? endDate,
            string? searchTerm,
            PageParam? pagination)
        {
            var effectivePageParam = pagination ?? new PageParam();

            if (postIds.Count == 0)
                return new PagedList<NotificationEmployeeDto>(effectivePageParam);

            var postIdList = postIds as ICollection<Guid> ?? postIds.ToList();
            var filter = BuildAssignedPostPositionFilter(postIdList, startDate, endDate, searchTerm);

            // Count + paging run in the database (same approach as skill listing / GetAllPagedAsync),
            // with dedupe to one row per recipient email handled in the same query.
            var (rows, total) = await _postPositionRepository
                .GetAssignedRecipientsPagedAsync(filter, effectivePageParam)
                .ConfigureAwait(false);

            return new PagedList<NotificationEmployeeDto>(
                rows.Select(MapNotificationEmployee).ToList(),
                effectivePageParam,
                total);
        }

        private static Expression<Func<PostPosition, bool>> BuildAssignedPostPositionFilter(
            ICollection<Guid> postIds,
            DateOnly? startDate,
            DateOnly? endDate,
            string? searchTerm)
        {
            var normalizedSearch = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();

            return pp =>
                postIds.Contains(pp.PostID)
                && (pp.PostPositionStatus == PostPositionStatus.Assigned
         
                    || pp.PostPositionStatus == PostPositionStatus.CancellationPending
                 )
                && (pp.IsPlaceholder
                    || (pp.EmployeeID != null && pp.EmployeeID != Guid.Empty))
                && (!startDate.HasValue || pp.DateOfWork >= startDate.Value)
                && (!endDate.HasValue || pp.DateOfWork <= endDate.Value)
                && (normalizedSearch == null
                    || (!pp.IsPlaceholder && pp.Employee != null && (
                        (pp.Employee.FirstName != null && pp.Employee.FirstName.Contains(normalizedSearch))
                        || (pp.Employee.LastName != null && pp.Employee.LastName.Contains(normalizedSearch))
                        || (pp.Employee.EmployeeNumber != null && pp.Employee.EmployeeNumber.Contains(normalizedSearch))
                        || (pp.Employee.PrimaryEmail != null && pp.Employee.PrimaryEmail.Contains(normalizedSearch))
                        || (pp.Employee.OffDutyCallBackEmail != null && pp.Employee.OffDutyCallBackEmail.Contains(normalizedSearch))))
                    || (pp.IsPlaceholder && pp.Placeholder != null && (
                        (pp.Placeholder.FirstName != null && pp.Placeholder.FirstName.Contains(normalizedSearch))
                        || (pp.Placeholder.LastName != null && pp.Placeholder.LastName.Contains(normalizedSearch))
                        || (pp.Placeholder.EmployeeNumber != null && pp.Placeholder.EmployeeNumber.Contains(normalizedSearch)))));
        }

        private NotificationEmployeeDto MapNotificationEmployee(PostPosition postPosition)
        {
            if (postPosition.IsPlaceholder)
            {
                var placeholder = postPosition.Placeholder;
                var supportEmail = _configuration["PayrollSettings:Notifications:SupportEmail"];
                return new NotificationEmployeeDto
                {
                    EmployeeId = Guid.Empty,
                    PostPositionId = postPosition.PostPositionID,
                    DateOfWork = postPosition.DateOfWork,
                    EmployeeName = placeholder == null ? null : BaseDto.FormatDisplayName(placeholder.LastName, placeholder.FirstName),
                    EmployeeNumber = placeholder?.EmployeeNumber,
                    PrimaryEmail = supportEmail,
                    OffDutyCallBackEmail = supportEmail,
                    IsPlaceholder = true,
                };
            }

            var employee = postPosition.Employee;
            return new NotificationEmployeeDto
            {
                EmployeeId = postPosition.EmployeeID ?? Guid.Empty,
                PostPositionId = postPosition.PostPositionID,
                DateOfWork = postPosition.DateOfWork,
                EmployeeName = employee == null ? null : BaseDto.FormatDisplayName(employee.LastName, employee.FirstName),
                EmployeeNumber = employee?.EmployeeNumber,
                PrimaryEmail = employee?.PrimaryEmail,
                OffDutyCallBackEmail = employee?.OffDutyCallBackEmail,
                IsPlaceholder = false,
            };
        }

        private NotificationEmployeeDto MapNotificationEmployee(PostPositionDto postPosition)
        {
            if (postPosition.IsPlaceholder)
            {
                var placeholder = postPosition.PlaceholderDetail;
                var supportEmail = _configuration["PayrollSettings:Notifications:SupportEmail"];
                return new NotificationEmployeeDto
                {
                    EmployeeId = Guid.Empty,
                    PostPositionId = postPosition.PostPositionID ?? Guid.Empty,
                    DateOfWork = postPosition.DateOfWork,
                    EmployeeName = placeholder == null ? null : BaseDto.FormatDisplayName(placeholder.LastName, placeholder.FirstName),
                    EmployeeNumber = placeholder?.EmployeeNumber,
                    PrimaryEmail = supportEmail,
                    OffDutyCallBackEmail = supportEmail,
                    IsPlaceholder = true,
                };
            }

            var employee = postPosition.Employee;
            return new NotificationEmployeeDto
            {
                EmployeeId = postPosition.EmployeeID,
                PostPositionId = postPosition.PostPositionID ?? Guid.Empty,
                DateOfWork = postPosition.DateOfWork,
                EmployeeName = employee == null ? null : BaseDto.FormatDisplayName(employee.LastName, employee.FirstName),
                EmployeeNumber = employee?.EmployeeNumber,
                PrimaryEmail = employee?.PrimaryEmail,
                OffDutyCallBackEmail = employee?.OffDutyCallBackEmail,
                IsPlaceholder = false,
            };
        }

        private async Task<RequesterDto?> GetRequesterAsync(Guid? requesterId)
        {
            if (!requesterId.HasValue || requesterId.Value == Guid.Empty)
                return null;

            return await _requesterService.GetByIdAsync(requesterId.Value).ConfigureAwait(false);
        }

        private async Task<RequestDto?> GetRequestAsync(Guid requestId) =>
            await _requestService.GetByIdAsync(requestId).ConfigureAwait(false);

        private static List<NotificationSuggestedRecipientDto> BuildSuggestedRecipients(
            RequesterDto? requester,
            RequestDto? request,
            IEnumerable<PostDto> posts)
        {
            var groupedByEmail = new Dictionary<string, (List<string> Sources, string? FirstName, string? LastName)>(StringComparer.OrdinalIgnoreCase);

            AddSourceToGroup(groupedByEmail, "Requester", requester?.Email, requester?.PrimaryContact, null);
            AddSourceToGroup(
                groupedByEmail,
                "Request",
                request?.PrimaryEmail ?? request?.SecondaryEmail,
                request?.FirstName,
                request?.LastName);

            foreach (var post in posts)
                AddSourceToGroup(groupedByEmail, "Post", post.PrimaryEmail, post.FirstName, post.LastName);

            return groupedByEmail.Select(kvp => new NotificationSuggestedRecipientDto
            {
                PrimaryEmail = kvp.Key,
                Sources = kvp.Value.Sources,
                DisplayName = CombineDisplayName(kvp.Value.FirstName, kvp.Value.LastName)
            }).ToList();
        }

        private static void AddSourceToGroup(
            Dictionary<string, (List<string> Sources, string? FirstName, string? LastName)> groupedByEmail,
            string source,
            string? email,
            string? firstName,
            string? lastName)
        {
            var normalizedEmail = NormalizeEmail(email);
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                return;

            if (!groupedByEmail.TryGetValue(normalizedEmail, out var existing))
            {
                groupedByEmail[normalizedEmail] = (new List<string> { source }, firstName, lastName);
                return;
            }

            if (!string.IsNullOrWhiteSpace(firstName))
                existing.FirstName = firstName;
            if (!string.IsNullOrWhiteSpace(lastName))
                existing.LastName = lastName;
            if (!existing.Sources.Contains(source, StringComparer.OrdinalIgnoreCase))
                existing.Sources.Add(source);

            groupedByEmail[normalizedEmail] = existing;
        }

        private static string? CombineDisplayName(string? firstName, string? lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
                return null;
            if (string.IsNullOrWhiteSpace(firstName))
                return lastName?.Trim();
            if (string.IsNullOrWhiteSpace(lastName))
                return firstName?.Trim();
            return $"{firstName.Trim()} {lastName.Trim()}";
        }

        private static string? NormalizeEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var trimmed = email.Trim();
            return EmailAddressHelper.IsValidEmail(trimmed) ? trimmed : null;
        }

        private static List<string> ParseEmailList(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return new List<string>();

            raw = raw.Trim();
            if (raw.StartsWith('['))
            {
                try
                {
                    return JsonSerializer.Deserialize<List<string>>(raw)?
                        .Where(e => !string.IsNullOrWhiteSpace(e))
                        .Select(e => e.Trim())
                        .ToList() ?? new List<string>();
                }
                catch (JsonException)
                {
                    // Fall back to comma-separated parsing.
                }
            }

            return raw
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .ToList();
        }

        private static List<string> NormalizeEmails(IEnumerable<string>? emails) =>
            emails?
                .Where(e => !string.IsNullOrWhiteSpace(e) && EmailAddressHelper.IsValidEmail(e.Trim()))
                .Select(e => e.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

        private async Task<IReadOnlyList<EmailAttachmentDto>> ReadAttachmentsAsync(
            IFormFileCollection files,
            CancellationToken cancellationToken)
        {
            var attachmentFiles = files
                .Where(f => AttachmentFormFieldNames.Contains(f.Name) && f.Length > 0)
                .ToList();

            if (attachmentFiles.Count == 0)
                return Array.Empty<EmailAttachmentDto>();

            var attachments = new List<EmailAttachmentDto>(attachmentFiles.Count);
            foreach (var file in attachmentFiles)
            {
                EnsureAttachmentSize(file.FileName, file.Length);
                await using var stream = file.OpenReadStream();
                attachments.Add(await CreateEmailAttachmentFromStreamAsync(
                    stream,
                    file.FileName,
                    file.ContentType,
                    fileUploadedId: null,
                    cancellationToken).ConfigureAwait(false));
            }

            return attachments;
        }

        private async Task<IReadOnlyList<EmailAttachmentDto>> ResolveAllAttachmentsAsync(
            SendNotificationRequestDto request,
            IReadOnlyList<EmailAttachmentDto> multipartAttachments,
            CancellationToken cancellationToken)
        {
            if (request.AttachmentFileIds is not { Count: > 0 })
                return multipartAttachments;

            if (!request.RequestId.HasValue || request.RequestId == Guid.Empty
                || !request.RequesterId.HasValue || request.RequesterId == Guid.Empty)
            {
                ThrowBadRequest(ModelValidationErrorCodes.REQUIRED_NOTIFICATION_ATTACHMENT_CONTEXT);
            }

            var attachments = multipartAttachments.ToList();

            foreach (var fileIdRaw in request.AttachmentFileIds)
            {
                if (string.IsNullOrWhiteSpace(fileIdRaw)
                    || !Guid.TryParse(fileIdRaw.Trim(), out var fileId)
                    || fileId == Guid.Empty)
                {
                    throw CreateNotFoundException(ModelValidationErrorCodes.NOTIFICATION_ATTACHMENT_NOT_FOUND, fileIdRaw);
                }

                var result = await _uploadFileService.GetNotificationFileAsync(
                    request.RequestId.Value,
                    request.RequesterId.Value,
                    fileId,
                    cancellationToken).ConfigureAwait(false)
                    ?? await _uploadFileService.GetFilesOrFileAsync(
                        request.RequestId.Value,
                        request.RequesterId.Value,
                        fileId,
                        cancellationToken).ConfigureAwait(false);

                if (result?.File == null)
                    ThrowNotFound(ModelValidationErrorCodes.NOTIFICATION_ATTACHMENT_NOT_FOUND, fileId);

                var file = result.File;
                using (file.Stream)
                {
                    attachments.Add(await CreateEmailAttachmentFromStreamAsync(
                        file.Stream,
                        file.FileName,
                        file.ContentType,
                        fileId,
                        cancellationToken).ConfigureAwait(false));
                }
            }

            return attachments;
        }

        private async Task<IReadOnlyList<EmailAttachmentDto>> EnsureAttachmentFileIdsAsync(
            HttpRequest httpRequest,
            SendNotificationRequestDto request,
            IReadOnlyList<EmailAttachmentDto> multipartAttachments,
            CancellationToken cancellationToken)
        {
            if (request.AttachmentFileIds is { Count: > 0 })
                return multipartAttachments;

            if (!request.RequestId.HasValue || request.RequestId == Guid.Empty
                || !request.RequesterId.HasValue || request.RequesterId == Guid.Empty
                || !httpRequest.HasFormContentType)
            {
                return multipartAttachments;
            }

            var formFiles = httpRequest.Form.Files
                .Where(f => AttachmentFormFieldNames.Contains(f.Name) && f.Length > 0)
                .ToList();
            if (formFiles.Count == 0)
                return multipartAttachments;

            var uploaded = await _uploadFileService.UploadNotificationAsync(
                request.RequestId.Value,
                request.RequesterId.Value,
                formFiles,
                cancellationToken).ConfigureAwait(false);

            request.AttachmentFileIds = uploaded.Select(u => u.FileUploadedId).ToList();
            return Array.Empty<EmailAttachmentDto>();
        }

        private static SendNotificationRequestDto ParseSendNotificationRequestFromForm(IFormCollection form) =>
            new()
            {
                Subject = form["subject"].ToString(),
                Body = form["body"].ToString(),
                To = ParseEmailList(form["to"].ToString()),
                Cc = ParseEmailList(form["cc"].ToString()),
                Bcc = ParseEmailList(form["bcc"].ToString()),
                RequestId = TryParseGuid(form["requestId"].ToString()),
                RequesterId = TryParseGuid(form["requesterId"].ToString()),
                PostId = TryParseGuid(form["postId"].ToString()),
                PostPositionId = TryParseGuid(form["postPositionId"].ToString()),
                ContextType = TryParseNotificationContextType(form["contextType"].ToString()),
                AttachmentFileIds = ParseEmailList(form["attachmentFileIds"].ToString())
            };

        private static bool CanDeferMultipartAttachmentRead(SendNotificationRequestDto request, IFormFileCollection files) =>
            request.AttachmentFileIds is not { Count: > 0 }
            && request.RequestId.HasValue && request.RequestId != Guid.Empty
            && request.RequesterId.HasValue && request.RequesterId != Guid.Empty
            && files.Any(f => AttachmentFormFieldNames.Contains(f.Name) && f.Length > 0);

        private static void EnsureAttachmentSize(string? fileName, long size)
        {
            if (size <= MaxAttachmentSizeBytes)
                return;

            throw new WebApiException(
                HttpStatusCode.RequestEntityTooLarge,
                ((int)HttpStatusCode.RequestEntityTooLarge).ToString(),
                ModelValidationErrorCodes.GetErrorMessage(
                    ModelValidationErrorCodes.UPLOAD_FILE_SIZE_EXCEEDED,
                    fileName,
                    MaxAttachmentSizeBytes / (1024 * 1024)));
        }

        private static async Task<EmailAttachmentDto> CreateEmailAttachmentFromStreamAsync(
            Stream stream,
            string? fileName,
            string? contentType,
            Guid? fileUploadedId,
            CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
            var content = memoryStream.ToArray();
            EnsureAttachmentSize(fileName, content.Length);

            return new EmailAttachmentDto
            {
                FileName = string.IsNullOrWhiteSpace(fileName) ? "attachment" : Path.GetFileName(fileName),
                ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
                Content = content,
                FileUploadedId = fileUploadedId
            };
        }

        private static Guid? TryParseGuid(string? value) =>
            Guid.TryParse(value, out var id) && id != Guid.Empty ? id : null;

        private static NotificationContextType? TryParseNotificationContextType(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return Enum.TryParse<NotificationContextType>(value.Trim(), true, out var parsed)
                ? parsed
                : null;
        }

        private static void ThrowBadRequest(string errorCode, params object[] args) =>
            throw new WebApiException(
                HttpStatusCode.BadRequest,
                ((int)HttpStatusCode.BadRequest).ToString(),
                ModelValidationErrorCodes.GetErrorMessage(errorCode, args));

        private static void ThrowNotFound(string errorCode, params object[] args) =>
            throw CreateNotFoundException(errorCode, args);

        private static WebApiException CreateNotFoundException(string errorCode, params object[] args) =>
            new(
                HttpStatusCode.NotFound,
                ((int)HttpStatusCode.NotFound).ToString(),
                ModelValidationErrorCodes.GetErrorMessage(errorCode, args));
    }
}
