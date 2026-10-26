using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
   public class PostDocumentController : ControllerBase
    
    {
       private readonly IPostDocumentService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostDocumentController"/> class.
        /// </summary>
        /// <param name="service">The post equipment service.</param>
        public PostDocumentController(IPostDocumentService service)
            {
                _service = service;
            }

            /// <summary>
            /// Gets all post equipment records.
            /// </summary>
            /// <param name="pagination">The pagination parameters (optional).</param>
            /// <returns>A collection of post equipment records.</returns>
            [HttpGet]
            [ProducesResponseType(typeof(IEnumerable<PostDocumentDto>), StatusCodes.Status200OK)]
            public async Task<ActionResult<IEnumerable<PostDocumentDto>>> GetAllAsync()
            {
                var items = await _service.GetAllAsync();
                return Ok(items);
            }
          

            /// <summary>
            /// Gets a post equipment record by ID.
            /// </summary>
            /// <param name="id">The post equipment ID.</param>
            /// <returns>The post equipment record if found; otherwise, NotFound.</returns>
            [HttpGet("{id}")]
             public async Task<ActionResult<PostDocumentDto>> GetByIdAsync(Guid id)
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound($"PostEquipment with ID {id} not found.");
                }
                return Ok(item);
            }

            /// <summary>
            /// Creates or updates a post equipment record.
            /// </summary>
            /// <param name="dto">The post equipment data to create or update.</param>
            /// <returns>The created or updated post equipment record.</returns>
            [HttpPost]
            [ProducesResponseType(typeof(PostDocumentDto), StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<ActionResult<PostDocumentDto>> UpsertAsync([FromBody] PostDocumentDto dto)
            {
                var createdItem = await _service.UpsertAsync(dto);
                return createdItem;
            }

            /// <summary>
            /// Updates an existing post equipment record.
            /// </summary>
            /// <param name="id">The post equipment ID.</param>
            /// <param name="dto">The post equipment data to update.</param>
            /// <returns>The updated post equipment record.</returns>
            [HttpPut("{id}")]
            [ProducesResponseType(typeof(PostDocumentDto), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]

            public async Task<ActionResult<PostDocumentDto>> UpdateAsync(Guid id, [FromBody] PostDocumentDto dto)
            {

            var existingDocument = await _service.GetByIdAsync(id);

            Guid? postDocumentId = null;
            if (existingDocument != null)
            {
                postDocumentId = existingDocument.PostDocumentId;
            }

            // Optionally, set the ID in the DTO if you want to update it
            dto.PostDocumentId = postDocumentId ?? Guid.Empty; // or leave as null if your DTO allows

            if (dto.PostDocumentId != id)
                {
                    return BadRequest("ID mismatch between route and body.");
                }

                var updatedItem = await _service.UpsertAsync(dto);
                return Ok(updatedItem);
            }

            /// <summary>
            /// Deletes a post equipment record by ID.
            /// </summary>
            /// <param name="id">The post equipment ID.</param>
            /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
            [HttpDelete("{id}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> DeleteAsync(Guid id)
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound($"PostEquipment with ID {id} not found.");
                }
                return Ok(new { message = "Record deleted successfully." });
            }
        }



    }

