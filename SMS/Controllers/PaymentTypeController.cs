using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Extensions;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing payment type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentTypeController : ControllerBase
    {
        private readonly IPaymentTypeService _service;

        public PaymentTypeController(IPaymentTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedList<PaymentTypeDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<PaymentTypeDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(PaymentTypeDto.Name);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.IsDefault == true,
                x => x.IsDefault != true);
            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<PaymentTypeDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<PaymentTypeDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(PaymentTypeDto.Name);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.IsDefault == isActive.Value : null);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PaymentTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PaymentTypeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"PaymentType with ID {id} not found.");
            }
            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PaymentTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PaymentTypeDto>> UpsertAsync([FromBody] PaymentTypeDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PaymentTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PaymentTypeDto>> UpdateAsync(Guid id, [FromBody] PaymentTypeDto dto)
        {
            if (id != dto.PaymentTypeID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"PaymentType with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"PaymentType with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
