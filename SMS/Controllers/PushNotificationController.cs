using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for sending Firebase push notifications to mobile devices.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PushNotificationController : ControllerBase
    {
        private readonly IFirebasePushNotificationService _pushService;
        private readonly ILogger<PushNotificationController> _logger;

        public PushNotificationController(
            IFirebasePushNotificationService pushService,
            ILogger<PushNotificationController> logger)
        {
            _pushService = pushService;
            _logger = logger;
        }

        /// <summary>
        /// Sends a notification to multiple devices by their FCM tokens.
        /// </summary>
        /// <param name="request">Device tokens, title, and body.</param>
        /// <returns>Success and failure counts; optionally use failed tokens to remove from DB.</returns>
        [HttpPost("send-multiple")]
        [ProducesResponseType(typeof(SendNotificationResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendToMultipleDevices([FromBody] NotificationRequestDto request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (request.DeviceTokens == null || request.DeviceTokens.Count == 0)
                return BadRequest("At least one device token is required.");

            try
            {
                var (successCount, failureCount, failedTokens) = await _pushService.SendToMultipleDevicesAsync(request);

                return Ok(new SendNotificationResultDto(successCount, failureCount, failedTokens));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send push notification to {TokenCount} device token(s)", request.DeviceTokens?.Count ?? 0);
                return BadRequest(ex.Message);
            }
        }
    }
}
