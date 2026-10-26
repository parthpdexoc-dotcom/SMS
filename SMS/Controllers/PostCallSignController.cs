using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostCallSignController : ControllerBase
    {
        private readonly IPostCallSignService _service;

        public PostCallSignController(IPostCallSignService service)
        {
            _service = service;
        }

        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<PostCallSignDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        public async Task<ActionResult<IEnumerable<PostCallSignDto>>> GetAvailableForDropdownAsync(
            [FromQuery] Guid postId,
            [FromQuery] DateOnly dateOfWork)
        {
            var result = await _service.GetAvailableForDropdownAsync(postId, dateOfWork);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedList<PostCallSignDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<PostCallSignDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] Guid? postId = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(PostCallSignDto.CreatedDate);
                pagination.Limit = 5000;
            }

            var result = await _service.GetAllPagedAsync(
                postId.HasValue ? x => x.PostID == postId.Value : null,
                pagination);
            return Ok(result);
        }

        [HttpPost("bulk")]
        [ProducesResponseType(typeof(IEnumerable<PostCallSignDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<IEnumerable<PostCallSignDto>>> BulkUpsertAsync(
            [FromBody] IEnumerable<PostCallSignDto> dtos)
        {
            var results = await _service.BulkUpsertAsync(dtos);
            return Ok(results);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
