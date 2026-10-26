using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Services.Mapping;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Common.Implementations.Paginations;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing email template operations using the generic service pattern.
    /// </summary>
    public class EmailTemplateService : GenericService<EmailTemplateDto, EmailTemplate>, IEmailTemplateService
    {
        private readonly IEmailTemplateRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EmailTemplateService> _logger;
        private readonly ITokenParser? _tokenParser;
        private readonly IDepartmentAdminRepository _departmentAdminRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateService"/> class.
        /// </summary>
        /// <param name="repository">The email template repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="emailSender">The email sender.</param>
        /// <param name="departmentAdminRepository">The department admin repository.</param>
        /// <param name="tokenParser">Optional token parser for audit tracking.</param>
        public EmailTemplateService(
            IEmailTemplateRepository repository,
            IMapper mapper,
            ILogger<EmailTemplateService> logger,
            IEmailSender emailSender,
            IDepartmentAdminRepository departmentAdminRepository,
            ITokenParser? tokenParser = null) : base(repository, mapper, "EmailTemplateID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _departmentAdminRepository = departmentAdminRepository ?? throw new ArgumentNullException(nameof(departmentAdminRepository));
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts or updates an email template with duplicate title validation.
        /// </summary>
        public override async Task<EmailTemplateDto> UpsertAsync(EmailTemplateDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            if (!string.IsNullOrWhiteSpace(dto.Title))
            {
                await ValidateDuplicateAsync(
                    propertyName: "Title",
                    value: dto.Title,
                    dto: dto,
                    isDepartmentScoped: false,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_EMAIL_TEMPLATE_TITLE);
            }

            return await base.UpsertAsync(dto);
        }

        /// <inheritdoc />
        public async Task SendMailAsync(SendMailRequestDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // If there are no target emails, quietly skip sending instead of failing the whole process.
            if (request.Emails == null || request.Emails.Count == 0)
            {
                _logger.LogInformation("Skipping email send for EmailType {EmailType}: no recipient emails provided.", request.EmailType);
                return;
            }

            var template = await _repository.GetByEmailTypeAsync(request.EmailType);
            if (template == null && request.EmailType == EmailType.RequestReleasedEmail)
            {
                template = await _repository.GetByEmailTypeAsync(EmailType.JobsPostedEmail);
            }
            if (template == null)
            {
                // No template for this email type – log and continue without failing the caller.
                _logger.LogWarning("Skipping email send for EmailType {EmailType}: no email template found.", request.EmailType);
                return;
            }

            string subject = ReplaceTemplateParameters(template.Subject ?? string.Empty, request.TemplateParameters);
            string body = ReplaceTemplateParameters(template.Content ?? string.Empty, request.TemplateParameters);

            var toAddresses = request.Emails
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => e.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (toAddresses.Count == 0)
            {
                _logger.LogInformation("Skipping email send for EmailType {EmailType}: all recipient emails were empty/invalid.", request.EmailType);
                return;
            }

            try
            {
                await _emailSender.SendAsync(
                    toAddresses,
                    subject,
                    body,
                    isBodyHtml: true,
                    calendarInvite: request.CalendarInvite,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Email send failed for EmailType {EmailType} to {RecipientCount} recipient(s). Subject: {Subject}",
                    request.EmailType,
                    toAddresses.Count,
                    subject);
            }
        }

        /// <summary>
        /// Searches email templates by title, subject, or content.
        /// </summary>
        public override Task<PagedList<EmailTemplateDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<EmailTemplate, bool>>? filter = null,
            Expression<Func<EmailTemplate, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            Expression<Func<EmailTemplate, bool>> baseFilter = x => true;
            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(EmailTemplate), "x");
                var combinedBody = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));
                baseFilter = Expression.Lambda<Func<EmailTemplate, bool>>(combinedBody, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties == null || searchableProperties.Length == 0)
                    ? new[] { nameof(EmailTemplate.Title), nameof(EmailTemplate.Subject), nameof(EmailTemplate.Content), nameof(EmailTemplate.SmsContent) }
                    : searchableProperties;

            return base.SearchAsync(
                searchTerm,
                pageParam,
                baseFilter,
                includes,
                effectiveSearchableProperties);
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetKeywordsByEmailType(EmailType emailType)
        {
            return EmailTypeKeywordMapping.GetKeywordsByEmailType(emailType);
        }

        /// <inheritdoc />
        public IReadOnlyList<EmailTypeOptionDto> GetEmailTypes()
        {
            return EmailTypeKeywordMapping.GetEmailTypes();
        }

        /// <inheritdoc />
        public async Task SendEmployeeEmailUnavailableNotificationAsync(
            EmployeeDto employee,
            EmailType failedEmailType,
            CancellationToken cancellationToken = default)
        {
            var departmentAdmins = (await _departmentAdminRepository.GetAllAsync(
                filter: da => da.DepartmentID == employee.DepartmentID,
                includes: new Expression<Func<DepartmentAdmin, object>>[]
                {
                    da => da.Employee,
                    da => da.AdminType
                },
                pageParam: null).ConfigureAwait(false)).ToList();

            var departmentAdminEmails = departmentAdmins
                .Where(da => IsDepartmentAdminAdminType(da.AdminType?.AdminTypeShort) && da.Employee != null)
                .Select(da => da.Employee!.OffDutyCallBackEmail?.Trim() ?? da.Employee!.PrimaryEmail?.Trim())
                .Where(email => !string.IsNullOrWhiteSpace(email))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var notificationContext = EmailTypeKeywordMapping.GetEmailTypeLabel(failedEmailType);
            var templateParameters = BuildEmployeeEmailUnavailableNotificationParameters(employee, notificationContext);

            var sendMailRequest = new SendMailRequestDto(
                EmailType.EmployeeEmailUnavailable,
                departmentAdminEmails,
                fromEmail: null,
                templateParameters);

            await SendMailAsync(sendMailRequest, cancellationToken).ConfigureAwait(false);
        }

        private static bool IsDepartmentAdminAdminType(string? adminTypeShort)
        {
            var normalized = string.Concat(adminTypeShort.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            return string.Equals(normalized, RoleTypeNames.DepartmentAdmin(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Builds template parameters for <see cref="EmailType.EmployeeEmailUnavailable"/>.
        /// </summary>
        public static Dictionary<string, string> BuildEmployeeEmailUnavailableNotificationParameters(
            EmployeeDto employee,
            string notificationContext)
        {
            var fullName = PersonNameFormatHelper.FormatLastNameCommaFirstName(employee.LastName, employee.FirstName) ?? string.Empty;
            var employeeType = employee.EmployeeType?.EmployeeTypeLong?.Trim()
                ?? employee.EmployeeType?.EmployeeTypeShort?.Trim()
                ?? string.Empty;

            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [EnumHelper.ToStringValue(EmailKeyword.FullName)] = fullName,
                [EnumHelper.ToStringValue(EmailKeyword.UserName)] = employee.Username?.Trim() ?? string.Empty,
                [EnumHelper.ToStringValue(EmailKeyword.EmployeeNumber)] = employee.EmployeeNumber?.Trim() ?? string.Empty,
                [EnumHelper.ToStringValue(EmailKeyword.EmployeeType)] = employeeType,
                [EnumHelper.ToStringValue(EmailKeyword.NotificationContext)] = notificationContext?.Trim() ?? string.Empty,
            };
        }

        private static string ReplaceTemplateParameters(string text, IReadOnlyDictionary<string, string>? parameters)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            if (parameters == null || parameters.Count == 0)
                return text;

            foreach (var kvp in parameters)
            {
                if (string.IsNullOrEmpty(kvp.Key))
                    continue;
                text = text.Replace("[" + kvp.Key + "]", kvp.Value ?? string.Empty, StringComparison.Ordinal);
            }
            return text;
        }
    }
}
