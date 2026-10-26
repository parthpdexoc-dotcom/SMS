using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using SMS.Generic.Enums;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing closed post position equipment operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClosedPostPositionEquipmentController : ControllerBase
    {
        private readonly IClosedPostPositionEquipmentService _service;
        private readonly ILogger<ClosedPostPositionEquipmentController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedPostPositionEquipmentController"/> class.
        /// </summary>
        /// <param name="service">The closed post position equipment service.</param>
        /// <param name="logger">The logger instance.</param>
        public ClosedPostPositionEquipmentController(
            IClosedPostPositionEquipmentService service,
            ILogger<ClosedPostPositionEquipmentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Gets all closed post position equipment records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of closed post position equipment records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClosedPostPositionEquipmentDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        public async Task<ActionResult<IEnumerable<ClosedPostPositionEquipmentDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets the closed post position and its related equipment records for a given post position ID.
        /// </summary>
        /// <param name="postPositionId">The post position ID.</param>
        /// <returns>The closed post position with its equipment records.</returns>
        [HttpGet("post-position/{postPositionId}")]
        [ProducesResponseType(typeof(ClosedPostPositionDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<ClosedPostPositionDetailsDto>> GetClosedDetailsByPostPositionIdAsync(Guid postPositionId)
        {
            var response = await _service.GetClosedDetailsByPostPositionIdAsync(postPositionId);
            if (response == null)
                return NotFound($"Closed post position for PostPositionID {postPositionId} not found.");

            return Ok(response);
        }

        /// <summary>
        /// Gets all closed post position detail records for a given post position ID.
        /// </summary>
        /// <param name="postPositionId">The post position ID.</param>
        [HttpGet("post-position/{postPositionId}/list")]
        [ProducesResponseType(typeof(IEnumerable<ClosedPostPositionDetailsDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<IEnumerable<ClosedPostPositionDetailsDto>>> GetClosedDetailsListByPostPositionIdAsync(Guid postPositionId)
        {
            var response = await _service.GetClosedDetailsListByPostPositionIdAsync(postPositionId);
            return Ok(response);
        }

        /// <summary>
        /// Gets a closed post position equipment record by ID.
        /// </summary>
        /// <param name="id">The closed post position equipment ID (GUID).</param>
        /// <returns>The record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClosedPostPositionEquipmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<ClosedPostPositionEquipmentDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Closed job position equipment with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a closed post position equipment record.
        /// </summary>
        /// <param name="dto">The closed post position equipment data to create or update.</param>
        /// <returns>The created or updated record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ClosedPostPositionEquipmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<ClosedPostPositionEquipmentDto>> UpsertAsync([FromBody] ClosedPostPositionEquipmentDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a closed post position equipment record by ID.
        /// </summary>
        /// <param name="id">The closed post position equipment ID (GUID).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Closed job position equipment with ID {id} not found.");
            }
            return NoContent();
        }

        /// <summary>
        /// Splits and closes a post position (creates closed records and updates status).
        /// </summary>
        /// <param name="request">Split/close request payload.</param>
        /// <returns>Closed post position record.</returns>
        [HttpPost("split")]
        [ProducesResponseType(typeof(PostPositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<IActionResult> SplitPostPositionAsync([FromBody] SplitPostPositionRequestDto request)
        {
            try
            {
                var result = await _service.SplitAndCloseAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex is not WebApiException)
                    _logger.LogError(ex, "Failed to split job position");
                var message = ex is WebApiException webEx ? webEx.ErrorMessage : ex.Message;
                return BadRequest(new
                {
                    error = "Failed to split job position",
                    message
                });
            }
        }

        /// <summary>
        /// Closes a post position (creates closed records and updates status).
        /// </summary>
        /// <param name="request">Close request payload.</param>
        /// <returns>Updated post position record.</returns>
        [HttpPost("close")]
        [ProducesResponseType(typeof(PostPositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<IActionResult> ClosePostPositionAsync([FromBody] SplitPostPositionRequestDto request)
        {
            try
            {
                var result = await _service.CloseAsync(request);
                return Ok(result);
            }
            catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(new
                {
                    error = "Failed to close job position",
                    message = ex.ErrorMessage,
                    refundRequired = ex.RefundRequired,
                    invoiceId = ex.InvoiceId,
                    closedPostPositionId = ex.ClosedPostPositionId ?? request.ParentClosedPostPositionID,
                    editPayload = request
                });
            }
            catch (Exception ex)
            {
                if (ex is not WebApiException)
                    _logger.LogError(ex, "Failed to close job position");
                var message = ex is WebApiException webEx ? webEx.ErrorMessage : ex.Message;
                return BadRequest(new
                {
                    error = "Failed to close job position",
                    message
                });
            }
        }

        /// <summary>
        /// Marks a post position as unclosed.
        /// </summary>
        /// <param name="postPositionId">Post position id.</param>
        [HttpPut("{postPositionId:guid}/unclose")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<IActionResult> UnclosePostPositionAsync(Guid postPositionId)
        {
            try
            {
                var updatedCount = await _service.UncloseAsync(postPositionId);
                return Ok(new
                {
                    updated = updatedCount,
                    status = PostPositionStatus.Unclosed.ToString(),
                    postPositionId = postPositionId
                });
            }
            catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(new
                {
                    error = "Failed to unclose job position",
                    message = ex.ErrorMessage,
                    refundRequired = ex.RefundRequired,
                    invoiceId = ex.InvoiceId,
                    postPositionId,
                    closedPostPositionId = ex.ClosedPostPositionId
                });
            }
            catch (Exception ex)
            {
                if (ex is not WebApiException)
                    _logger.LogError(ex, "Failed to unclose job position {PostPositionId}", postPositionId);
                var message = ex is WebApiException webEx ? webEx.ErrorMessage : ex.Message;
                return BadRequest(new
                {
                    error = "Failed to unclose job position",
                    message,
                    postPositionId
                });
            }
        }
    }
}
