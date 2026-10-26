using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service implementation for EmployeeDeviceToken operations.
    /// </summary>
    public class EmployeeDeviceTokenService : GenericService<EmployeeDeviceTokenDto, EmployeeDeviceToken>, IEmployeeDeviceTokenService
    {
        private readonly IGenericRepository<EmployeeDeviceToken> _repository;
        private readonly ILogger<EmployeeDeviceTokenService> _logger;

        public EmployeeDeviceTokenService(
            IGenericRepository<EmployeeDeviceToken> repository,
            IMapper mapper,
            ILogger<EmployeeDeviceTokenService> logger,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "EmployeeDeviceTokenID", tokenParser)
        {
            _repository = repository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task UpsertDeviceTokenAsync(Guid employeeId, string deviceToken)
        {
            if (string.IsNullOrWhiteSpace(deviceToken))
            {
                _logger.LogWarning("Skipping device-token upsert because token is empty. EmployeeId: {EmployeeId}", employeeId);
                return;
            }

            if (employeeId == Guid.Empty)
            {
                _logger.LogWarning("Skipping device-token upsert because employee id is empty. TokenSuffix: {TokenSuffix}", MaskTokenForLog(deviceToken));
                return;
            }

            // Load all existing tokens for this employee
            var existingTokens = await _repository.GetAllAsync(
                e => e.EmployeeID == employeeId,
                null,
                null).ConfigureAwait(false);

            // If this exact token is already stored, do nothing
            if (existingTokens.Any(t => t.DeviceToken == deviceToken))
            {
                _logger.LogInformation(
                    "Device token already exists. EmployeeId: {EmployeeId}, TokenSuffix: {TokenSuffix}, ExistingTokenCount: {ExistingTokenCount}",
                    employeeId,
                    MaskTokenForLog(deviceToken),
                    existingTokens.Count());
                return;
            }

            // Otherwise insert a new record (even if other tokens already exist)
            await _repository.AddRangeAsync(new[] { new EmployeeDeviceToken(employeeId, deviceToken) }).ConfigureAwait(false);
            _logger.LogInformation(
                "Device token upserted successfully. EmployeeId: {EmployeeId}, TokenSuffix: {TokenSuffix}, ExistingTokenCountBeforeInsert: {ExistingTokenCount}",
                employeeId,
                MaskTokenForLog(deviceToken),
                existingTokens.Count());
        }

        private static string MaskTokenForLog(string deviceToken)
        {
            if (string.IsNullOrWhiteSpace(deviceToken))
                return "<empty>";

            var token = deviceToken.Trim();
            return token.Length <= 8
                ? token
                : token[^8..];
        }
    }
}
