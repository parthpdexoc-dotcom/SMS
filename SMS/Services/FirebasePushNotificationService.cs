using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Services
{
    /// <summary>
    /// Sends push notifications to mobile devices via Firebase Cloud Messaging.
    /// </summary>
    public class FirebasePushNotificationService : IFirebasePushNotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FirebasePushNotificationService> _logger;
        private readonly IEmployeeDeviceTokenService _employeeDeviceTokenService;
        private static bool _firebaseInitialized;
        private static readonly object _initLock = new();

        public FirebasePushNotificationService(
            IConfiguration configuration,
            ILogger<FirebasePushNotificationService> logger,
            IEmployeeDeviceTokenService employeeDeviceTokenService)
        {
            _configuration = configuration;
            _logger = logger;
            _employeeDeviceTokenService = employeeDeviceTokenService ?? throw new ArgumentNullException(nameof(employeeDeviceTokenService));
            EnsureFirebaseInitialized();
        }

        private void EnsureFirebaseInitialized()
        {
            if (_firebaseInitialized)
                return;
            lock (_initLock)
            {
                if (_firebaseInitialized)
                    return;
                var credentialPath = _configuration["Firebase:CredentialPath"];
                if (string.IsNullOrWhiteSpace(credentialPath) || !File.Exists(credentialPath))
                {
                    _logger.LogWarning("Firebase credential path not set or file not found. Push notifications will not be sent.");
                    return;
                }
                try
                {
                    if (FirebaseApp.DefaultInstance == null)
                        FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromFile(credentialPath) });
                    _firebaseInitialized = true;
                    _logger.LogInformation("Firebase initialized for push notifications.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize Firebase.");
                }
            }
        }

        /// <inheritdoc />
        public async Task<(int SuccessCount, int FailureCount, IReadOnlyList<string> FailedTokens)> SendToMultipleDevicesAsync(
            NotificationRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request?.DeviceTokens == null || request.DeviceTokens.Count == 0)
            {
                _logger.LogWarning("SendToMultipleDevicesAsync skipped because no device tokens were provided.");
                return (0, 0, Array.Empty<string>());
            }

            if (FirebaseApp.DefaultInstance == null)
            {
                _logger.LogWarning(
                    "Firebase not initialized. Skipping push notification. Title: {Title}, TokenCount: {TokenCount}",
                    request.Title,
                    request.DeviceTokens.Count);
                return (0, request.DeviceTokens.Count, request.DeviceTokens);
            }

            _logger.LogInformation(
                "Sending push notification via FCM. Title: {Title}, TokenCount: {TokenCount}",
                request.Title,
                request.DeviceTokens.Count);

            var message = new MulticastMessage
            {
                Tokens = request.DeviceTokens,
                Notification = new Notification
                {
                    Title = request.Title ?? string.Empty,
                    Body = request.Body ?? string.Empty
                },
                Data = new Dictionary<string, string>
                {
                    { "click_action", "FLUTTER_NOTIFICATION_CLICK" },
                    { "time", DateTime.UtcNow.ToString("O") }
                }
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, cancellationToken).ConfigureAwait(false);

                var failedTokens = new List<string>();
                if (response.FailureCount > 0)
                {
                    for (var i = 0; i < response.Responses.Count; i++)
                    {
                        if (!response.Responses[i].IsSuccess)
                        {
                            failedTokens.Add(request.DeviceTokens[i]);
                            _logger.LogWarning(
                                "FCM token delivery failed. TokenSuffix: {TokenSuffix}, Error: {Error}",
                                MaskTokenForLog(request.DeviceTokens[i]),
                                response.Responses[i].Exception?.Message ?? "Unknown FCM error");
                        }
                    }
                }

                _logger.LogInformation(
                    "FCM send completed. Success: {Success}, Failed: {Failed}, Title: {Title}",
                    response.SuccessCount,
                    response.FailureCount,
                    request.Title);
                return (response.SuccessCount, response.FailureCount, failedTokens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FCM send failed. Title: {Title}, TokenCount: {TokenCount}", request.Title, request.DeviceTokens.Count);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task SendToEmployeesAsync(
            IReadOnlyList<Guid> employeeIds,
            string title,
            string body,
            CancellationToken cancellationToken = default)
        {
            if (employeeIds == null || employeeIds.Count == 0 || string.IsNullOrWhiteSpace(title))
            {
                _logger.LogWarning(
                    "SendToEmployeesAsync skipped because inputs are invalid. EmployeeCount: {EmployeeCount}, HasTitle: {HasTitle}",
                    employeeIds?.Count ?? 0,
                    !string.IsNullOrWhiteSpace(title));
                return;
            }

            try
            {
                _logger.LogInformation(
                    "Resolving device tokens for push notification. EmployeeCount: {EmployeeCount}, Title: {Title}",
                    employeeIds.Count,
                    title);
                var deviceTokenDtos = await _employeeDeviceTokenService.GetAllAsync(
                    e => employeeIds.Contains(e.EmployeeID),
                    null,
                    null).ConfigureAwait(false);

                if (!deviceTokenDtos.Any())
                {
                    _logger.LogWarning(
                        "No device token records found for requested employees. EmployeeCount: {EmployeeCount}, Title: {Title}",
                        employeeIds.Count,
                        title);
                }

                var deviceTokens = deviceTokenDtos
                    .Where(d => !string.IsNullOrWhiteSpace(d.DeviceToken))
                    .Select(d => d.DeviceToken!)
                    .Distinct()
                    .ToList();
                if (deviceTokens.Count == 0)
                {
                    _logger.LogWarning(
                        "Push notification skipped because no valid device tokens are available. EmployeeCount: {EmployeeCount}, Title: {Title}",
                        employeeIds.Count,
                        title);
                    return;
                }

                var (successCount, failureCount, failedTokens) = await SendToMultipleDevicesAsync(new NotificationRequestDto
                {
                    DeviceTokens = deviceTokens,
                    Title = title ?? string.Empty,
                    Body = body ?? string.Empty
                }, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation(
                    "SendToEmployeesAsync completed. EmployeeCount: {EmployeeCount}, DeviceTokenCount: {DeviceTokenCount}, Success: {Success}, Failure: {Failure}, Title: {Title}",
                    employeeIds.Count,
                    deviceTokens.Count,
                    successCount,
                    failureCount,
                    title);

                if (failedTokens.Count > 0)
                {
                    _logger.LogWarning(
                        "Push notification failed for {FailedTokenCount} token(s). FailedTokenSuffixes: {FailedTokenSuffixes}",
                        failedTokens.Count,
                        string.Join(",", failedTokens.Select(MaskTokenForLog)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "SendToEmployeesAsync failed for {Count} employee(s). Title: {Title}. Notification is best-effort.",
                    employeeIds.Count,
                    title);
            }
        }

        private static string MaskTokenForLog(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return "<empty>";

            var trimmed = token.Trim();
            return trimmed.Length <= 8
                ? trimmed
                : trimmed[^8..];
        }
    }
}
