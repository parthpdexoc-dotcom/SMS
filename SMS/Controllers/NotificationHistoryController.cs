using System.Net;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Api;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for notification send history.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationHistoryController : BaseController
    {
        private readonly INotificationHistoryService _notificationHistoryService;

        public NotificationHistoryController(INotificationHistoryService notificationHistoryService)
        {
            _notificationHistoryService = notificationHistoryService;
        }

        /// <summary>
        /// Gets notification history for a request, post, or post position (pass exactly one of postPositionId, postId, or requestId).
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<NotificationHistoryListItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<NotificationHistoryListItemDto>>> GetHistoryAsync(
            [FromQuery] Guid? postPositionId = null,
            [FromQuery] Guid? postId = null,
            [FromQuery] Guid? requestId = null,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                pagination ??= new PageParam();
                return Ok(await _notificationHistoryService.GetHistoryAsync(
                    postPositionId, postId, requestId, pagination, cancellationToken));
            }
            catch (WebApiException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { message = ex.ErrorMessage });
            }
        }

        /// <summary>
        /// Gets notification history detail including recipients and attachments.
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("{notificationHistoryId:guid}")]
        [ProducesResponseType(typeof(NotificationHistoryDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationHistoryDetailDto>> GetDetailAsync(
            Guid notificationHistoryId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _notificationHistoryService.GetDetailAsync(notificationHistoryId, cancellationToken));
            }
            catch (WebApiException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { message = ex.ErrorMessage });
            }
        }
    }
}
