using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Helper;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Implements secure superuser-to-employee impersonation token issuance.
    /// </summary>
    public class ImpersonationAuthService : IImpersonationAuthService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITokenParser _tokenParser;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ImpersonationAuthService> _logger;
        private readonly IDepartmentConfigurationService _departmentConfigurationService;
        private readonly IRequesterManagerService _requesterManagerService;
        private readonly IUploadFileService _uploadFileService;

        public ImpersonationAuthService(
            IEmployeeRepository employeeRepository,
            ITokenParser tokenParser,
            IConfiguration configuration,
            IDepartmentConfigurationService departmentConfigurationService,
            IRequesterManagerService requesterManagerService,
            IUploadFileService uploadFileService,
            ILogger<ImpersonationAuthService> logger)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _tokenParser = tokenParser ?? throw new ArgumentNullException(nameof(tokenParser));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration)); 
            _departmentConfigurationService = departmentConfigurationService ?? throw new ArgumentNullException(nameof(departmentConfigurationService));
            _requesterManagerService = requesterManagerService ?? throw new ArgumentNullException(nameof(requesterManagerService));
            _uploadFileService = uploadFileService ?? throw new ArgumentNullException(nameof(uploadFileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<TokenDto> ImpersonateEmployeeAsync(ImpersonateEmployeeRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.EmployeeId))
                throw new WebApiException(HttpStatusCode.BadRequest, "400", "employeeId is required.");

            if (!_tokenParser.IsInRole(RoleTypeNames.SuperUser()))
                throw new WebApiException(HttpStatusCode.Forbidden, "403", "Only superusers can impersonate employees.");

            if (!Guid.TryParse(request.EmployeeId, out var employeeId))
                throw new WebApiException(HttpStatusCode.BadRequest, "400", "employeeId must be a valid GUID.");

            var superUserId = _tokenParser.GetAdminID();
            if (!superUserId.HasValue || superUserId.Value == Guid.Empty)
                throw new WebApiException(HttpStatusCode.Unauthorized, "401", "Unable to resolve current superuser identity.");

            var employeeInfo = await _employeeRepository.GetEmployeeWithDetailsByIdAsync(employeeId).ConfigureAwait(false);
            if (employeeInfo == null)
                throw new WebApiException(HttpStatusCode.NotFound, "404", $"Employee with ID {employeeId} not found.");

            if (employeeInfo.IsSystemUser)
                throw new WebApiException(HttpStatusCode.BadRequest, "400", "System users cannot be impersonated.");

            if (!employeeInfo.IsApproved)
                throw new WebApiException(HttpStatusCode.BadRequest, "400", "Employee is not approved for login.");

            var tokenDto = await CreateImpersonationTokenAsync(employeeInfo, superUserId.Value).ConfigureAwait(false);

            _logger.LogInformation(
                "Superuser {SuperUserId} impersonated employee {EmployeeId}. Token expires at {ExpiresAtUtc}.",
                superUserId.Value,
                employeeInfo.EmployeeID,
                tokenDto.ExpiresAt);

            return tokenDto;
        }

        /// <inheritdoc />
        public async Task<TokenDto?> GenerateAccessTokenAsync(Guid employeeId, Guid impersonatedBySuperUserId)
        {
            if (employeeId == Guid.Empty || impersonatedBySuperUserId == Guid.Empty)
                return null;

            var employeeInfo = await _employeeRepository.GetEmployeeWithDetailsByIdAsync(employeeId).ConfigureAwait(false);

            if (employeeInfo == null || employeeInfo.IsSystemUser || !employeeInfo.IsApproved)
                return null;

            return await CreateImpersonationTokenAsync(employeeInfo, impersonatedBySuperUserId).ConfigureAwait(false);
        }

        private async Task<TokenDto> CreateImpersonationTokenAsync(
            EmployeeLoginInfoDto employeeInfo,
            Guid impersonatedBySuperUserId)
        {
            var enableOpenAgencyWaitLists = false;
            var allowEmployeeCancellation = false;
            var enableEmployeeSplittingPosts = false;
            var enablePolicyManagement = true;
            var useAgencyDefinedFields = false;
            var enableEmployeeLocation = false;
            try
            {
                var deptConfig = await _departmentConfigurationService.GetByDepartmentIdAsync(employeeInfo.DepartmentID).ConfigureAwait(false);
                if (deptConfig != null)
                {
                    enableOpenAgencyWaitLists = deptConfig.EnableOpenAgencyWaitLists;
                    allowEmployeeCancellation = deptConfig.AllowEmployeeCancellation;
                    enableEmployeeSplittingPosts = deptConfig.EnableEmployeeSplittingPosts;
                    enablePolicyManagement = deptConfig.EnablePolicyManagement;
                    useAgencyDefinedFields = deptConfig.UseAgencyDefinedFields;
                    enableEmployeeLocation = deptConfig.EnableEmployeeLocation;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_JWT_DEPT_CONFIG_LOAD_FAILED));
            }

            var releaseAsTabTier = AvailablePostReleaseTabTier.RegularEmployee;
            try
            {
                releaseAsTabTier = await _requesterManagerService
                    .GetAvailablePostReleaseTabTierForLoginAsync(employeeInfo)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_JWT_RELEASE_TAB_TIER_DEFAULTED));
            }

            var isLogo = false;
            try
            {
                isLogo = await _uploadFileService.HasEmployeePhotoAsync(employeeInfo.EmployeeID, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_JWT_IS_LOGO_CHECK_FAILED, employeeInfo.EmployeeID));
            }

            var tokenExpirationInMinutes = TokenHelper.GetTokenExpirationInMinutes(_configuration);

            var token = TokenHelper.GenerateEmployeeToken(
                employeeInfo,
                _configuration,
                enableOpenAgencyWaitLists,
                allowEmployeeCancellation,
                enableEmployeeSplittingPosts,
                enablePolicyManagement,
                useAgencyDefinedFields,
                enableEmployeeLocation,
                releaseAsTabTier,
                isLogo,
                isImpersonation: true,
                impersonatedBy: impersonatedBySuperUserId,
                expirationInMinutesOverride: tokenExpirationInMinutes,
                includeAdminTypeRoles: true);

            var expiresAt = DateTime.UtcNow.AddMinutes(tokenExpirationInMinutes);
            return new TokenDto(token, expiresAt, "Bearer");
        }
    }
}
