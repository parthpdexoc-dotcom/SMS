using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for uploading file attachments.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UploadFileController : ControllerBase
    {
        private readonly IUploadFileService _uploadService;

        public UploadFileController(IUploadFileService uploadService)
        {
            _uploadService = uploadService;
        }

        /// <summary>
        /// Upload one or more request files (form field "files"). For department logo use POST department/{departmentId}/logo.
        /// </summary>
        /// <param name="requestID">The request ID to attach the files to.</param>
        /// <returns>JSON array of saved file metadata.</returns>
        [HttpPost("upload/{requesterId:guid}/{requestId:guid}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [ProducesResponseType(typeof(UploadFileDto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        public async Task<ActionResult<IReadOnlyList<UploadFileDto>>> Upload(Guid requestId, Guid requesterId, CancellationToken cancellationToken = default)
        {
            var formFiles = Request.Form.Files
                .Where(f => string.Equals(f.Name, UploadFileService.FormFieldName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (formFiles.Count == 0)
                return BadRequest(new { message = "No files provided. Use form field 'files' for one or more files." });

            try
            {
                var result = await _uploadService.UploadAsync(requestId, requesterId, formFiles, cancellationToken).ConfigureAwait(false);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("exceeds the maximum"))
                    return StatusCode(StatusCodes.Status413PayloadTooLarge, new { message = ex.Message });
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get files for a request: omit fileId for list of file metadata; pass fileId for single file stream.
        /// Returns empty list when no result (no error). For department logo use GET department/{departmentId}/logo.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="fileId">Optional. When null, returns list of file metadata; when set, returns that file stream.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of files (JSON), single file stream, or empty list when nothing found.</returns>
        [HttpGet("Get/{requesterId:guid}/{requestId:guid}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [ProducesResponseType(typeof(UploadFileDto[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilesOrFile(Guid requestId,Guid requesterId, [FromQuery] Guid? fileId, CancellationToken cancellationToken = default)
        {
            var result = await _uploadService.GetFilesOrFileAsync(requestId, requesterId, fileId, cancellationToken).ConfigureAwait(false);

            if (result?.List != null)
                return Ok(result.List);
            if (result?.File != null)
                return File(result.File.Stream, result.File.ContentType, result.File.FileName);

            return Ok(Array.Empty<UploadFileDto>());
        }

        /// <summary>
        /// Upload or replace department logo (single image). Pass departmentId in route. Form field name: file. Allowed: jpg, jpeg, png, gif, webp. Replaces existing logo.
        /// </summary>
        /// <param name="departmentId">The department ID (from route).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Saved file name (e.g. logo.png) or error.</returns>
        [HttpPost("departmentLogo/{departmentId:guid}/logo")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        public async Task<ActionResult<object>> UploadDepartmentLogo(Guid departmentId, CancellationToken cancellationToken = default)
        {
            var file = Request.Form.Files
                .FirstOrDefault(f => string.Equals(f.Name, UploadFileService.LogoFormFieldName, StringComparison.OrdinalIgnoreCase));

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided. Use form field 'file' for the logo image." });

            try
            {
                var fileName = await _uploadService.UploadDepartmentLogoAsync(departmentId, file, cancellationToken).ConfigureAwait(false);
                return Ok(new { fileName });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("exceeds the maximum"))
                    return StatusCode(StatusCodes.Status413PayloadTooLarge, new { message = ex.Message });
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get department logo image (departmentId in route). Returns 404 if no logo set.
        /// </summary>
        /// <param name="departmentId">The department ID (from route).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Image stream or 404.</returns>
        [HttpGet("departmentLogoGet/{departmentId:guid}/logo")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDepartmentLogo(Guid departmentId, CancellationToken cancellationToken = default)
        {
            var result = await _uploadService.GetDepartmentLogoAsync(departmentId, cancellationToken).ConfigureAwait(false);
            if (result == null)
                return NotFound(new { message = "Department logo not found." });

            return File(result.Value.Stream, result.Value.ContentType, result.Value.FileName);
        }

        /// <summary>
        /// Upload employee profile photo. POST UploadFile/employeePhoto/{employeeId}/photo. Form field "file" (single file). Returns { fileName }.
        /// </summary>
        /// <param name="employeeId">The employee ID (from route).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Saved file name (e.g. {employeeId}_photo.png).</returns>
        [HttpPost("employeePhoto/{employeeId:guid}/photo")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        public async Task<ActionResult<object>> UploadEmployeePhoto(Guid employeeId, CancellationToken cancellationToken = default)
        {
            var file = Request.Form.Files
                .FirstOrDefault(f => string.Equals(f.Name, UploadFileService.LogoFormFieldName, StringComparison.OrdinalIgnoreCase));

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided. Use form field 'file' for the profile photo." });

            try
            {
                var fileName = await _uploadService.UploadEmployeePhotoAsync(employeeId, file, cancellationToken).ConfigureAwait(false);
                return Ok(new { fileName });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("exceeds the maximum"))
                    return StatusCode(StatusCodes.Status413PayloadTooLarge, new { message = ex.Message });
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get employee profile photo as image stream. GET UploadFile/employeePhotoGet/{employeeId}/photo. Returns 404 when no photo.
        /// </summary>
        /// <param name="employeeId">The employee ID (from route).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Image stream or 404.</returns>
        [HttpGet("employeePhotoGet/{employeeId:guid}/photo")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeePhoto(Guid employeeId, CancellationToken cancellationToken = default)
        {
            var result = await _uploadService.GetEmployeePhotoAsync(employeeId, cancellationToken).ConfigureAwait(false);
            if (result == null)
                return NotFound(new { message = "Employee profile photo not found." });

            return File(result.Value.Stream, result.Value.ContentType, result.Value.FileName);
        }

        /// <summary>
        /// Deletes a file for a request by request ID and file ID.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="fileId">The uploaded file ID (FileUploadedId from list).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>204 No Content on success; 404 if request or file not found.</returns>
        [HttpDelete("{requesterId:guid}/{requestId:guid}/{fileId:guid}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFile(Guid requestId,Guid requesterId, Guid fileId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _uploadService.DeleteFileAsync(requestId, requesterId, fileId, cancellationToken).ConfigureAwait(false);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
