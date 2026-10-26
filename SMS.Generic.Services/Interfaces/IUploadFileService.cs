using Microsoft.AspNetCore.Http;
using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Service for upload file operations (files stored on disk only; no DB).
    /// </summary>
    public interface IUploadFileService
    {
        /// <summary>
        /// Uploads one or more files for a request (form field "files"). For department logo use UploadDepartmentLogoAsync via the department/logo endpoint.
        /// </summary>
        /// <param name="requestId">The request ID to attach the files to.</param>
        /// <param name="files">The uploaded files (form field "files").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of saved file metadata (DTOs) for the response.</returns>
        Task<IReadOnlyList<UploadFileDto>> UploadAsync(
            Guid requestId,
            Guid requesterId,
            IEnumerable<IFormFile> files,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Uploads notification email attachments under the request's Email Attachments subfolder.
        /// Does not enforce the request-level one-file-per-type rule.
        /// </summary>
        Task<IReadOnlyList<UploadFileDto>> UploadNotificationAsync(
            Guid requestId,
            Guid requesterId,
            IEnumerable<IFormFile> files,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a notification attachment stream by file ID from the Email Attachments subfolder.
        /// </summary>
        Task<RequestFilesOrFileResultDto?> GetNotificationFileAsync(
            Guid requestId,
            Guid requesterId,
            Guid fileId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets files for a request: when fileId is null returns list of file metadata; when fileId has value returns that single file stream.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="fileId">Optional. When null, returns list of all files; when set, returns that file stream.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result with List or File set; null if request/folder not found or (when fileId set) file not found.</returns>
        Task<RequestFilesOrFileResultDto?> GetFilesOrFileAsync(Guid requestId, Guid requesterId, Guid? fileId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uploads or replaces the department logo (single image). Stored under BasePath/Files/Logo/departmentLogo/{departmentId}/. Replaces any existing logo.
        /// </summary>
        /// <param name="departmentId">The department ID.</param>
        /// <param name="file">Single image file (jpg, jpeg, png, gif, webp).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The saved file name (e.g. logo.png), or null on failure.</returns>
        Task<string?> UploadDepartmentLogoAsync(Guid departmentId, IFormFile file, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the department logo image stream if one exists.
        /// </summary>
        /// <param name="departmentId">The department ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Stream, content type, and file name if found; null if no logo.</returns>
        Task<(Stream Stream, string ContentType, string FileName)?> GetDepartmentLogoAsync(Guid departmentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uploads or replaces employee profile photo. Stored under BasePath/Files/Logo/EmployeeProfileImage/{employeeId}/. File name: {employeeId}_{fileName}. Replaces any existing photo.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="file">Single image file (jpg, jpeg, png, gif, webp). Form field "file".</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The saved file name (e.g. {employeeId}_photo.png).</returns>
        Task<string?> UploadEmployeePhotoAsync(Guid employeeId, IFormFile file, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the employee profile photo stream if one exists.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Stream, content type, and file name if found; null if no photo.</returns>
        Task<(Stream Stream, string ContentType, string FileName)?> GetEmployeePhotoAsync(Guid employeeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// True when the employee profile photo folder contains at least one file (same rule as <see cref="GetEmployeePhotoAsync"/> without opening a stream).
        /// </summary>
        Task<bool> HasEmployeePhotoAsync(Guid employeeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a file for a request by request ID and file ID.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="fileId">The uploaded file ID (FileUploadedId).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="KeyNotFoundException">When the request or file is not found.</exception>
        Task DeleteFileAsync(Guid requestId, Guid requesterId, Guid fileId, CancellationToken cancellationToken = default);
    }
}
