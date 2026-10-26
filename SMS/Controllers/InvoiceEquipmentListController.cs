using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing invoice equipment list operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class InvoiceEquipmentListController : ControllerBase
    {
        private readonly IInvoiceEquipmentListService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceEquipmentListController"/> class.
        /// </summary>
        /// <param name="service">The invoice equipment list service.</param>
        public InvoiceEquipmentListController(IInvoiceEquipmentListService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all invoice equipment lists.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of invoice equipment lists.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvoiceEquipmentListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InvoiceEquipmentListDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an invoice equipment list by ID.
        /// </summary>
        /// <param name="id">The invoice equipment list ID.</param>
        /// <returns>The invoice equipment list if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InvoiceEquipmentListDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceEquipmentListDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"InvoiceEquipmentList with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new invoice equipment list.
        /// </summary>
        /// <param name="dto">The invoice equipment list data to create.</param>
        /// <returns>The created invoice equipment list.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(InvoiceEquipmentListDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InvoiceEquipmentListDto>> UpsertAsync([FromBody] InvoiceEquipmentListDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing invoice equipment list.
        /// </summary>
        /// <param name="id">The invoice equipment list ID.</param>
        /// <param name="dto">The invoice equipment list data to update.</param>
        /// <returns>The updated invoice equipment list.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(InvoiceEquipmentListDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceEquipmentListDto>> UpdateAsync(int id, [FromBody] InvoiceEquipmentListDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.InvoiceEquipmentListID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"InvoiceEquipmentList with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an invoice equipment list by ID.
        /// </summary>
        /// <param name="id">The invoice equipment list ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"InvoiceEquipmentList with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

