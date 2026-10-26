using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Api;
using SMS.Generic.Common;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for notification operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Gets Send Notification modal data. Pass exactly one of postPositionId, postId, or requestId in the JSON body.
        /// Optional postIds (request level), startDate, and endDate filter assigned employees by post/date.
        /// Optional searchTerm filters assigned employees only (not To/Cc/Bcc suggested recipients).
        /// Optional pagination (page/pageSize or offset/limit) applies to assignedEmployees only.
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPost("notification-data")]
        [ProducesResponseType(typeof(NotificationDataDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationDataDto>> GetNotificationDataAsync(
            [FromBody] NotificationDataRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var result = await _notificationService.GetNotificationDataAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Sends a notification email. Supports JSON (no attachments) or multipart/form-data (with attachments).
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpPost("send")]
        [Consumes("application/json", "multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        public async Task<IActionResult> SendNotificationAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _notificationService.SendNotificationAsync(Request, cancellationToken);
                return NoContent();
            }
            catch (WebApiException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { message = ex.ErrorMessage });
            }
        }
    }
}
