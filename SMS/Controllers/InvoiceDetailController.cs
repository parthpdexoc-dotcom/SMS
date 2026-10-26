using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using System.Net;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing invoice detail operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class InvoiceDetailController : ControllerBase
    {
        private readonly IInvoiceDetailService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceDetailController"/> class.
        /// </summary>
        /// <param name="service">The invoice detail service.</param>
        public InvoiceDetailController(IInvoiceDetailService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all invoice details (no invoice filter).
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of all invoice details.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDetailDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InvoiceDetailDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets original billing lines for an invoice (financial impact None) with closed post work date/time; summary totals match the stored invoice header.
        /// </summary>
        /// <param name="invoicesId">Invoice ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Line items and summary totals; 404 when invoice is missing.</returns>
        [HttpGet("invoice/{invoicesId:guid}/view-original")]
        [ProducesResponseType(typeof(ViewOriginalInvoiceDetailsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ViewOriginalInvoiceDetailsResponseDto>> ViewOriginalDetailsByInvoiceIdAsync(
            [FromRoute] Guid invoicesId,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.ViewOriginalDetailsByInvoiceIdAsync(invoicesId, cancellationToken).ConfigureAwait(false);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Gets all invoice details for a specific invoice ID. Includes department logo from the logged-in user's token (department ID).
        /// </summary>
        /// <param name="invoicesId">The invoice ID to filter by.</param>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Response with Items (invoice details) and optional Logo (contentType + base64) when token has department ID.</returns>
        [HttpGet("invoice/{invoicesId:guid}")]
        [ProducesResponseType(typeof(InvoiceDetailsWithLogoResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<InvoiceDetailsWithLogoResponseDto>> GetByInvoiceIdAsync(
            [FromRoute] Guid invoicesId,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetByInvoiceIdWithLogoAsync(invoicesId, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Gets an invoice detail by ID.
        /// </summary>
        /// <param name="id">The invoice detail ID.</param>
        /// <returns>The invoice detail if found; otherwise, NotFound.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(InvoiceDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceDetailDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"InvoiceDetail with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Returns the prior billing version for each current line on the invoice (read-only; no database changes).
        /// </summary>
        /// <param name="invoicesId">Invoice ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Previous version per line that has one (match on <c>currentInvoiceDetailId</c> on each item).</returns>
        [HttpGet("invoice/{invoicesId:guid}/view-previous")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<InvoiceDetailDto>>> ViewPreviousAsync(
            [FromRoute] Guid invoicesId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var results = await _service.ViewPreviousInvoiceVersionsAsync(invoicesId, cancellationToken).ConfigureAwait(false);
                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Generates invoices from request data by project numbers and date range. Persists results into the Invoice and InvoiceDetails tables.
        /// </summary>
        /// <param name="projectNumber">Combined project numbers (e.g. 202600001).</param>
        /// <param name="startDate">Start date filter.</param>
        /// <param name="endDate">End date filter.</param>
        /// <returns>Created invoices, or NotFound/BadRequest with SuccessResult when no request/data or validation errors.</returns>
        [HttpPost("generate-invoice")]
        [ProducesResponseType(typeof(SuccessResult<List<InvoiceDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SuccessResult<List<InvoiceDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResult<List<InvoiceDto>>>> GenerateInvoice(
            [FromQuery] List<string> projectNumber,
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate)
        {
            if (projectNumber == null || projectNumber.Count == 0)
                return BadRequest("At least one project number is required.");

            var result = await _service.GenerateInvoicesAsync(projectNumber, startDate, endDate).ConfigureAwait(false);
            if (result.Status == (int)HttpStatusCode.NotFound)
                return NotFound(result);

            if (result.Status == (int)HttpStatusCode.BadRequest)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new invoice detail.
        /// </summary>
        /// <param name="dto">The invoice detail data to create.</param>
        /// <returns>The created invoice detail.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(InvoiceDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InvoiceDetailDto>> UpsertAsync([FromBody] InvoiceDetailDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates one or more invoice details (creates new Credit versions) and returns all latest lines for the invoice.
        /// </summary>
        /// <param name="dtos">Invoice detail line items to update.</param>
        /// <returns>Latest invoice detail lines after updates.</returns>
        [HttpPut]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IEnumerable<InvoiceDetailDto>>> UpdateAsync([FromBody] List<InvoiceDetailDto> dtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dtos == null || dtos.Count == 0)
            {
                return BadRequest("At least one invoice detail is required.");
            }

            try
            {
                var updatedItems = await _service.UpdateInvoiceDetailsAsync(dtos).ConfigureAwait(false);
                return Ok(updatedItems);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(new
                {
                    error = "Failed to update invoice details",
                    message = ex.ErrorMessage,
                    refundRequired = ex.RefundRequired,
                    invoiceId = ex.InvoiceId
                });
            }
            catch (WebApiException ex)
            {
                return BadRequest(new
                {
                    error = "Failed to update invoice details",
                    message = ex.ErrorMessage
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Undoes all manual modifications for an invoice (soft-deletes Modified rows and reverts to original lines).
        /// </summary>
        /// <param name="invoicesId">Invoice primary key.</param>
        /// <returns>Latest invoice detail lines after undo.</returns>
        [HttpDelete]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IEnumerable<InvoiceDetailDto>>> DeleteAsync([FromBody] Guid invoicesId)
        {
            if (invoicesId == Guid.Empty)
            {
                return BadRequest("Invoice ID is required.");
            }

            try
            {
                var result = await _service.UndoInvoiceModificationsAsync(invoicesId).ConfigureAwait(false);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(new
                {
                    error = "Failed to undo invoice modifications",
                    message = ex.ErrorMessage,
                    refundRequired = ex.RefundRequired,
                    invoiceId = ex.InvoiceId ?? invoicesId
                });
            }
            catch (WebApiException ex)
            {
                return BadRequest(new
                {
                    error = "Failed to undo invoice modifications",
                    message = ex.ErrorMessage
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
