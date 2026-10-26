using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Helper;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing admin operations using the generic service pattern.
    /// </summary>
    public class AdminService : GenericService<AdminDto, Admin>, IAdminService
    {
        private readonly IAdminRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminService> _logger;
        private readonly ITokenParser? _tokenParser;
        private readonly IAdminTypeService? _adminTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminService"/> class.
        /// </summary>
        /// <param name="repository">The admin repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="tokenParser">Optional token parser for accessing current user information for audit tracking.</param>
        /// <param name="adminTypeService">The admin type service (used to validate AdminTypeID).</param>
        public AdminService(
            IAdminRepository repository,
            IMapper mapper,
            ILogger<AdminService> logger,
            IConfiguration configuration,
            ITokenParser? tokenParser = null,
            IAdminTypeService? adminTypeService = null) : base(repository, mapper, "AdminID", tokenParser)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenParser = tokenParser;
            _adminTypeService = adminTypeService;
        }

        /// <summary>
        /// Loads an admin, applies JSON Patch (replace-only), normalizes key fields, and returns the DTO.
        /// FluentValidation and persistence are handled by the API layer (validate then <see cref="UpsertAsync"/>).
        /// When the patched DTO is later upserted, <c>/password</c> plaintext is hashed on save.
        /// </summary>
        public async Task<AdminDto?> BuildPatchedAdminAsync(Guid id, JsonPatchDocument<AdminDto> adminOprerations)
        {
            if (adminOprerations == null)
                throw new ArgumentNullException(nameof(adminOprerations));

            var invalidOp = adminOprerations.Operations
                .FirstOrDefault(op => op.OperationType != OperationType.Replace);
            if (invalidOp != null)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    "400",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.ADMIN_PATCH_ONLY_REPLACE_ALLOWED));
            }

            var existing = await GetByIdAsync(id);
            if (existing == null)
                return null;

            var dto = new AdminDto();
            dto.CloneAdmin(existing);

            try
            {
                adminOprerations.ApplyTo(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply JSON patch to admin {AdminId}", id);
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    "400",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.ADMIN_PATCH_INVALID_DOCUMENT));
            }

            dto.AdminID = existing.AdminID;
            if (dto.AdminTypeID == default)
                dto.AdminTypeID = existing.AdminTypeID;
            if (string.IsNullOrWhiteSpace(dto.UserName))
                dto.UserName = existing.UserName;

            return dto;
        }

        /// <summary>
        /// Inserts a new admin or updates an existing one.
        /// For new records: password is hashed when provided.
        /// For updates: omit password or leave it empty to keep the current hash; provide a new plaintext password to replace it (stored hashed).
        /// </summary>
        /// <param name="dto">The admin DTO to insert or update.</param>
        /// <returns>The upserted admin DTO.</returns>
        public override async Task<AdminDto> UpsertAsync(AdminDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate AdminTypeID exists in database
            await ValidateForeignKeyAsync<IAdminTypeService, AdminTypeDto>(_adminTypeService, dto.AdminTypeID, "AdminType");

            try
            {
                bool isUpdate = dto.AdminID != Guid.Empty;
                
                if (isUpdate)
                {
                    AdminDto? existingEntity = await GetByIdAsync(dto.AdminID);
                    if (existingEntity == null)
                        throw new KeyNotFoundException($"Admin with ID {dto.AdminID} not found.");

                    if (string.IsNullOrWhiteSpace(dto.Password))
                        dto.Password = existingEntity.Password;
                    else if (!string.Equals(dto.Password, existingEntity.Password, StringComparison.Ordinal))
                        dto.Password = PasswordHelper.HashPassword(dto.Password);
                }
                else
                {
                    // For new records: Hash password if provided
                    if (!string.IsNullOrWhiteSpace(dto.Password))
                    {
                        dto.Password = PasswordHelper.HashPassword(dto.Password);
                    }
                }

                // Call base.UpsertAsync to handle audit fields and upsert logic
                return await base.UpsertAsync(dto);
            }
            catch (Exception ex)
            {
                // Re-throw the exception - logging is handled by the base class
                throw;
            }
        }

        /// <summary>
        /// Authenticates an admin and generates a JWT token.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <returns>The token DTO if authentication is successful; otherwise, null.</returns>
        public async Task<TokenDto?> LoginAsync(LoginDto loginDto)
        {
            // 1. Validate input
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                _logger.LogWarning("Admin login attempt with empty username or password");
                return null;
            }

            // 2. Get admin with all details (single query)
            var adminInfo = await _repository.GetAdminWithDetailsForLoginAsync(loginDto.Username);
            if (adminInfo == null)
            {
                _logger.LogWarning("Admin login attempt with invalid username: {Username}", loginDto.Username);
                return null;
            }

            // 3. Check admin status
            if (!adminInfo.Status)
            {
                _logger.LogWarning("Admin login attempt for inactive admin: {Username}", loginDto.Username);
                return null;
            }

            // 4. Validate password and hash if needed (for backward compatibility)
            if (string.IsNullOrWhiteSpace(adminInfo.Password))
            {
                _logger.LogWarning("Invalid password for admin username: {Username}", loginDto.Username);
                return null;
            }

            // Use common authentication helper for password validation
            var isPasswordValid = await AuthenticationHelper.ValidateAndHashPasswordIfNeededAsync(
                loginDto.Password,
                adminInfo.Password,
                adminInfo.AdminID,
                async (id, hashedPassword) => await _repository.UpdatePasswordAsync(id, hashedPassword),
                _logger,
                UserType.Admin);

            if (!isPasswordValid)
            {
                _logger.LogWarning("Invalid password for admin username: {Username}", loginDto.Username);
                return null;
            }

            // 5. Generate JWT with comprehensive claims using common token helper
            var token = TokenHelper.GenerateAdminToken(adminInfo, _configuration);
            var expiresAt = DateTime.UtcNow.AddMinutes(TokenHelper.GetTokenExpirationInMinutes(_configuration));

            _logger.LogInformation("Admin {Username} authenticated successfully", loginDto.Username);

            return new TokenDto(token, expiresAt, "Bearer");
        }

        /// <inheritdoc />
        public async Task<TokenDto?> GenerateAccessTokenAsync(Guid adminId)
        {
            var adminInfo = await _repository
                .GetAdminWithDetailsByIdAsync(adminId)
                .ConfigureAwait(false);
            if (adminInfo == null || !adminInfo.Status)
                return null;

            var token = TokenHelper.GenerateAdminToken(adminInfo, _configuration);
            var expiresAt = DateTime.UtcNow.AddMinutes(TokenHelper.GetTokenExpirationInMinutes(_configuration));
            return new TokenDto(token, expiresAt, "Bearer");
        }

        /// <summary>
        /// Initiates password recovery process for an admin.
        /// </summary>
        /// <param name="forgotPasswordDto">The password recovery request containing username and new password.</param>
        /// <returns>The password recovery response with login ID and confirmation message.</returns>
        public async Task<PasswordRecoveryResponseDto?> ForgotPasswordAsync(AdminForgotPasswordDto forgotPasswordDto)
        {
            if (forgotPasswordDto == null || string.IsNullOrWhiteSpace(forgotPasswordDto.Username) || string.IsNullOrWhiteSpace(forgotPasswordDto.NewPassword))
                return null;

            // Find admin by username
            var admin = await _repository.GetByUsernameAsync(forgotPasswordDto.Username);
            if (admin == null || !admin.Status)
                return null;

            // Hash and update password (no validation - just hash and store)
            var hashedPassword = PasswordHelper.HashPassword(forgotPasswordDto.NewPassword);
            var updated = await _repository.UpdatePasswordAsync(admin.AdminID, hashedPassword);
            
            if (!updated)
                return null;

            return new PasswordRecoveryResponseDto
            {
                LoginId = admin.UserName ?? string.Empty,
                Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_PASSWORD_RESET_SUCCESS),
                EmailAddress = string.Empty
            };
        }
    }
}

