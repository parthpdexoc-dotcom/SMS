using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using System.Net;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing post operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostController"/> class.
        /// </summary>
        /// <param name="service">The post service.</param>
        /// <param name="postPositionService">The post position service.</param>
        public PostController(IPostService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all posts.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of posts.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(
            nameof(AuthorizationPolicyType.RequesterManagerOnly),
            nameof(AuthorizationPolicyType.EmployeesViewManage),
            nameof(AuthorizationPolicyType.CommCenterOrDepartmentAdmin),
            nameof(AuthorizationPolicyType.IAAuditViewOnly))]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets all posts where Is Supervisor Post is true (for Parent Post dropdown).
        /// Optional requestId filters to supervisor posts for that request only.
        /// </summary>
        /// <param name="requestId">Optional request ID to filter posts by request.</param>
        /// <returns>A collection of supervisor posts.</returns>
        [HttpGet("supervisor-posts")]
        [ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<IEnumerable<PostDto>>> GetSupervisorPostsAsync([FromQuery] Guid? requestId = null)
        {
            var items = await _service.GetSupervisorPostsAsync(requestId);
            return Ok(items);
        }

        /// <summary>
        /// Gets a post by ID.
        /// </summary>
        /// <param name="id">The post ID.</param>
        /// <returns>The post if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly),nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<PostDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Post with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Gets all posts for a request with hierarchy levels (posts only; no position enrichment).
        /// </summary>
        [HttpGet("all-by-request/{requestId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPostsByRequestIdAsync(Guid requestId)
        {
            var items = await _service.GetAllPostsByRequestIdAsync(requestId);
            return Ok(items);
        }

        /// <summary>Gets the by request identifier asynchronous.</summary>
        /// <param name="requestId">The post identifier.</param>
        /// <param name="pagination">The pagination.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet("by-request/{requestId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<IEnumerable<PostDto>>> GetPostByRequestIdAsync(Guid requestId, [FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(PostDto.PostTitle);
            }

            var items = await _service.GetPostByRequestIdAsync(requestId, pagination);
            return Ok(items);
        }

        /// <summary>Gets posts for a request with optional search filter (Daily Workspace).</summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPost("request-posts")]
        [ProducesResponseType(typeof(PagedList<PostInRequestListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedList<PostInRequestListDto>>> GetRequestPostsAsync(
            [FromBody] RequestPostsFilterDto filter,
            CancellationToken cancellationToken)
        {
            if (filter.RequestId == Guid.Empty)
                return BadRequest("RequestId is required.");

            var result = await _service.GetPostsByRequestFilterAsync(filter, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets assigned employees with position details and latest GPS coordinates for a post,
        /// resolving current department date and shifts starting within 30 minutes before or after current department time.
        /// Only PostId is required from the caller.
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("post-employees-with-locations")]
        [ProducesResponseType(typeof(PagedList<PostPositionEmployeeLocationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedList<PostPositionEmployeeLocationDto>>> GetPostEmployeesWithLocationsAsync(
            [FromQuery] Guid postId,
            CancellationToken cancellationToken)
        {
            if (postId == Guid.Empty)
                return BadRequest("PostId is required.");

            var result = await _service.GetEmployeesWithLocationsByPostIdAsync(postId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new post.
        /// </summary>
        /// <param name="dto">The post data to create.</param>
        /// <returns>The created post.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PostDecreaseTotalPositionsConflictDto), StatusCodes.Status409Conflict)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<PostDto>> UpsertAsync([FromBody] PostDto dto)
        {
            try
            {
                var createdItem = await _service.UpsertAsync(dto);
                return createdItem;
            }
            catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict && ex.ResponseData != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.ResponseData);
            }
        }

        /// <summary>
        /// Clones an existing post by post ID.
        /// </summary>
        [HttpPost("clone-post")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        public async Task<ActionResult<PostDto>> ClonePostAsync([FromQuery] Guid postId)
        {
            var cloned = await _service.ClonePostAsync(postId);
            return Ok(cloned);
        }

        /// <summary>
        /// Gets eligible employees for a post by post ID.
        /// Filters employees based on rank, available time, skills, and not already assigned.
        /// </summary>
        /// <param name="postId">The post ID.</param>
        /// <returns>A collection of eligible employees.</returns>
        [HttpGet("{postId:guid}/eligible-employees")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEligibleEmployeesByPostIdAsync(Guid postId)
        {
            var employees = await _service.GetEligibleEmployeesByPostIdAsync(postId);
            return Ok(employees);
        }

        /// <summary>
        /// Deletes a post by ID.
        /// </summary>
        /// <param name="id">The post ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Post with ID {id} not found.");
            }
            return Ok(new { message = "Record deleted successfully." });
        }
    }
}

