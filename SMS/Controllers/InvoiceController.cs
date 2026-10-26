using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing invoice operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceController"/> class.
        /// </summary>
        /// <param name="service">The invoice service.</param>
        public InvoiceController(IInvoiceService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets invoices filtered by requester (and child requesters), paged.
        /// </summary>
        /// <param name="requesterId">Filter to get invoices for a specific requester.</param>
        /// <param name="isPaid">When true (e.g. 1), only paid invoices; when false (e.g. 0), only unpaid; when omitted, all.</param>
        /// <param name="searchTerm">Optional filter on invoice number or requester name (partial match).</param>
        /// <param name="dueDateRangeDays">Due-date lookback: 30, 60, or 90 days from today (inclusive).</param>
        /// <param name="pagination">Paging/sort (e.g. page, pageSize).</param>
        /// <returns>Paged list of invoices for the given requester filter.</returns>
        [HttpGet("by-requester")]
        [ProducesResponseType(typeof(PagedList<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<InvoiceDto>>> GetAllByRequesterAsync(
            [FromQuery] Guid? requesterId,
            [FromQuery] bool? isPaid = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? dueDateRangeDays = null,
            [FromQuery] PageParam? pagination = null)
        {
            var result = await _service.GetAllByRequesterIncludingChildrenAsync(requesterId, pagination, isPaid, searchTerm, dueDateRangeDays);
            return Ok(result);
        }

        /// <summary>
        /// Gets paged requesters with invoice counts only (header rows for the grouped invoice list).
        /// </summary>
        [HttpPost("requester-invoice-counts")]
        [ProducesResponseType(typeof(PagedList<RequesterInvoiceCountDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<RequesterInvoiceCountDto>>> GetRequesterInvoiceCountsByBodyAsync(
            [FromBody] RequesterInvoiceCountFilterDto? filter,
            CancellationToken cancellationToken = default)
        {
            var pageParam = filter?.PageParam ?? new PageParam();
            var result = await _service.GetRequesterInvoiceCountsAsync(
                pageParam,
                filter?.RequesterId,
                filter?.IsPaid,
                filter?.SearchTerm,
                filter?.DueDateRangeDays,
                cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Returns false when the requester has any unpaid invoice past due; otherwise true.
        /// </summary>
        [HttpGet("requester/{requesterId:guid}/clear-of-overdue-unpaid")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> IsRequesterClearOfOverdueUnpaidAsync(
            Guid requesterId,
            CancellationToken cancellationToken = default)
        {
            if (requesterId == Guid.Empty)
                return BadRequest("Requester ID is required.");

            var result = await _service.IsRequesterClearOfOverdueUnpaidInvoicesAsync(requesterId, cancellationToken)
                .ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Gets requester invoice summary (all invoices) and invoices filtered by paid status.
        /// </summary>
        [HttpGet("by-requester/details")]
        [ProducesResponseType(typeof(RequesterInvoiceDetailsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterInvoiceDetailsResponseDto>> GetRequesterInvoiceDetailsAsync([FromQuery] Guid requesterId,[FromQuery] bool isPaid,[FromQuery] PageParam? pagination = null,CancellationToken cancellationToken = default)
        {
            if (requesterId == Guid.Empty)
                return BadRequest("Requester ID is required.");

            var result = await _service.GetRequesterInvoiceDetailsAsync(requesterId, isPaid, pagination, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Gets all invoices.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of invoices.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<InvoiceDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isPaid = null,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
                pageParam.SortBy = nameof(InvoiceDto.DateCreated);

            var result = await _service.SearchAsync(searchTerm, pageParam, isPaid.HasValue ? x => x.IsPaid == isPaid.Value : null);
            return Ok(result);
        }

        /// <summary>
        /// Gets an invoice by ID.
        /// </summary>
        /// <param name="id">The invoice ID.</param>
        /// <returns>The invoice if found; otherwise, NotFound.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Invoice with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Gets generated invoice(s) for a request by project number from the Invoices table.
        /// </summary>
        /// <param name="projectNumber">Request project number (e.g. 00001), not fiscal year + project.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Invoice header(s) with line items; NotFound when none exist for the project.</returns>
        [HttpGet("invoice-by-projectNumber")]
        [ProducesResponseType(typeof(IReadOnlyList<InvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<InvoiceDto>>> GetByProjectNumberAsync(
            [FromQuery] string projectNumber,
            CancellationToken cancellationToken = default)
        {
            var items = await _service.GetInvoiceByProjectNumberAsync(projectNumber, cancellationToken).ConfigureAwait(false);
            if (items.Count == 0)
                return NotFound($"No invoice found for project number {projectNumber.Trim()}.");

            return Ok(items);
        }

        /// <summary>
        /// Gets invoice payment information for the pay flow: invoice summary, payment history, and requester available balance.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Invoice payment information when found; otherwise, NotFound.</returns>
        [HttpGet("{invoiceId:guid}/payment-information")]
        [ProducesResponseType(typeof(InvoicePaymentInformationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoicePaymentInformationResponseDto>> GetPaymentInformationAsync(
            [FromRoute] Guid invoiceId,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetInvoicePaymentInformationAsync(invoiceId, cancellationToken).ConfigureAwait(false);
            if (result == null)
                return NotFound($"Invoice with ID {invoiceId} not found.");

            return Ok(result);
        }

        /// <summary>
        /// Gets invoice payment summary: total, paid, balance due, and net amount per payment.
        /// </summary>
        [HttpGet("{invoiceId:guid}/payment-summary")]
        [ProducesResponseType(typeof(InvoicePaymentSummaryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoicePaymentSummaryResponseDto>> GetPaymentSummaryAsync(
            [FromRoute] Guid invoiceId,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetInvoicePaymentSummaryAsync(invoiceId, cancellationToken).ConfigureAwait(false);
            if (result == null)
                return NotFound($"Invoice with ID {invoiceId} not found.");

            return Ok(result);
        }

        /// <summary>
        /// Voids an active payment allocation for an invoice (by payment id) and recalculates invoice balances.
        /// </summary>
        [HttpPost("{invoiceId:guid}/payment/{paymentId:guid}/void")]
        [ProducesResponseType(typeof(InvoicePaymentInformationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        public async Task<ActionResult<InvoicePaymentInformationResponseDto>> VoidPaymentAsync(
            [FromRoute] Guid invoiceId,
            [FromRoute] Guid paymentId,
            [FromBody] PaymentAllocationVoidRequestDto? request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _service.VoidInvoicePaymentAsync(invoiceId, paymentId, request?.Reason ?? string.Empty, cancellationToken).ConfigureAwait(false));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new invoice.
        /// </summary>
        /// <param name="dto">The invoice data to create.</param>
        /// <returns>The created invoice.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        public async Task<ActionResult<InvoiceDto>> UpsertAsync([FromBody] InvoiceDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing invoice.
        /// </summary>
        /// <param name="id">The invoice ID.</param>
        /// <param name="dto">The invoice data to update.</param>
        /// <returns>The updated invoice.</returns>
        [HttpPut("{id:Guid}")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        public async Task<ActionResult<InvoiceDto>> UpdateAsync(Guid id, [FromBody] InvoiceDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.InvoicesID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Invoice with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an invoice by ID.
        /// </summary>
        /// <param name="id">The invoice ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Invoice with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

