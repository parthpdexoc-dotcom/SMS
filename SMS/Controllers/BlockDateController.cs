using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing block date operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BlockDateController : ControllerBase
    {
        private readonly IBlockDateService _blockDateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDateController"/> class.
        /// </summary>
        /// <param name="blockDateService">The block date service.</param>
        public BlockDateController(IBlockDateService blockDateService)
        {
            _blockDateService = blockDateService;
        }

        /// <summary>
        /// Gets all block dates.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="postid">The post ID to filter by (optional).</param>
        /// <returns>A collection of block dates.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<BlockDateDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<PagedList<BlockDateDto>>> GetAllAsync([FromQuery] PageParam? pagination = null, [FromQuery] Guid? postid = null)
        {
            var result = await _blockDateService.GetAllPagedAsync(
            postid is { } id && id != Guid.Empty
            ? x => x.PostID == id
            : null,
            pagination,
            null);

            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<BlockDateDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<BlockDateDto>>> SearchAsync([FromQuery] string? searchTerm, [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
                pageParam.SortBy = nameof(BlockDateDto.StartDateTime);

            var result = await _blockDateService.SearchAsync(searchTerm, pageParam);
            return Ok(result);
        }

        /// <summary>
        /// Gets a block date by ID.
        /// </summary>
        /// <param name="id">The block date ID.</param>
        /// <returns>The block date if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BlockDateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.OperatorOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<BlockDateDto>> GetByIdAsync(Guid id)
        {
            var item = await _blockDateService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Block date with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new block date or updates an existing one.
        /// </summary>
        /// <param name="dto">The block date data to create or update.</param>
        /// <returns>The created or updated block date.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BlockDateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<BlockDateDto>> UpsertAsync([FromBody] BlockDateDto dto)
        {
            var createdItem = await _blockDateService.UpsertAsync(dto);
            return Ok(createdItem);
        }

        /// <summary>
        /// Deletes a block date by ID.
        /// </summary>
        /// <param name="id">The block date ID.</param>
        /// <returns>Ok if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.OperatorOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _blockDateService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Block date with ID {id} not found.");
            }
            return Ok(new { message = "Record deleted successfully." });
        }
    }
}
