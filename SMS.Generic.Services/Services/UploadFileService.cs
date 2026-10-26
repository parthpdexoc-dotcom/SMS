using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for uploading and storing file attachments on disk only (no DB).
    /// Folder structure: Uploaded Files -> Department -> {departmentId} -> files -> RequesterId -> RequestId -> FileUniqueId_FileName.ext
    ///                   Uploaded Files -> Department -> {departmentId} -> logo -> (single department logo, replaced on each upload)
    ///                   Uploaded Files -> Employee -> ProfilePhoto -> {employeeId} -> EmployeeId_FileName.ext
    /// </summary>
    public class UploadFileService : IUploadFileService
    {
        /// <summary>
        /// Maximum allowed file size in bytes (15 MB).
        /// </summary>
        public const long MaxFileSizeBytes = 15 * 1024 * 1024;

        /// <summary>
        /// Maximum allowed department logo size in bytes (2 MB).
        /// </summary>
        public const long MaxDepartmentLogoSizeBytes = 2 * 1024 * 1024;

        /// <summary>
        /// Form field name for multiple files.
        /// </summary>
        public const string FormFieldName = "files";

        /// <summary>
        /// Form field name for department logo (single file).
        /// </summary>
        public const string LogoFormFieldName = "file";

        private const string DepartmentFolder = "Department";
        private const string EmployeeFolder = "Employee";
        private const string FilesSubfolder = "files";
        private const string EmailAttachmentsSubfolder = "Email Attachments";
        private const string LogoSubfolder = "logo";
        private const string ProfilePhotoSubfolder = "ProfilePhoto";
        private const string NoDepartmentFolder = "_NoDepartment_";
        private const string NoRequesterFolder = "_NoRequester_";

        private static readonly HashSet<string> AllowedLogoExtensions = new(StringComparer.OrdinalIgnoreCase) { "jpg", "jpeg", "png", "gif", "webp" };
        private readonly IUploadFileStoragePathProvider _pathProvider;
        private readonly ITokenParser _tokenParser;
        private readonly ILogger<UploadFileService> _logger;

        public UploadFileService(
            IUploadFileStoragePathProvider pathProvider,
            ITokenParser tokenParser,
            ILogger<UploadFileService> logger)
        {
            _pathProvider = pathProvider;
            _tokenParser = tokenParser;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<UploadFileDto>> UploadAsync(
            Guid requestId,
            Guid requesterId,
            IEnumerable<IFormFile> files,
            CancellationToken cancellationToken = default)
        {
            var filesList = files?.ToList() ?? new List<IFormFile>();
            if (filesList.Count == 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_AT_LEAST_ONE_FILE_REQUIRED), nameof(files));

            var departmentId = _tokenParser.GetDepartmentID() ?? Guid.Empty;

            var directoryPath = GetPathUnderDepartment(departmentId, FilesSubfolder, GetRequesterSegment(requesterId), requestId.ToString());
            Directory.CreateDirectory(directoryPath);

            // Build set of existing file types for this request (only one file per type allowed)
            var existingTypes = await GetExistingFileTypesForRequestAsync(requestId, requesterId).ConfigureAwait(false);
            var result = new List<UploadFileDto>();

            foreach (IFormFile file in filesList)
            {
                if (file.Length == 0)
                    throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_FILES_EMPTY));

                if (file.Length > MaxFileSizeBytes)
                    throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_FILE_SIZE_EXCEEDED, file.FileName, MaxFileSizeBytes / (1024 * 1024)));

                var typeOfFile = GetTypeOfFile(file.FileName, file.ContentType);
                if (existingTypes.Contains(typeOfFile))
                    throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_DUPLICATE_FILE_TYPE, typeOfFile));

                existingTypes.Add(typeOfFile);

                var fileUploadedId = Guid.NewGuid();
                var fileNamePrefix = Path.GetFileNameWithoutExtension(file.FileName);
                if (string.IsNullOrWhiteSpace(fileNamePrefix))
                    fileNamePrefix = file.FileName ?? "file";

                var safeFileName = SanitizeFileName(fileNamePrefix);
                var physicalFileName = $"{fileUploadedId}_{safeFileName}.{typeOfFile}";
                var fullPath = Path.Combine(directoryPath, physicalFileName);

                await using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await file.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
                }

                result.Add(CreateUploadFileDto(fileUploadedId.ToString(), fileNamePrefix, typeOfFile, requestId.ToString(), file.Length, DateTime.UtcNow));
            }

            _logger.LogInformation("Uploaded {Count} file(s) for request {RequestId} to {Path}.", result.Count, requestId, directoryPath);

            return result;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<UploadFileDto>> UploadNotificationAsync(
            Guid requestId,
            Guid requesterId,
            IEnumerable<IFormFile> files,
            CancellationToken cancellationToken = default)
        {
            var filesList = files?.ToList() ?? new List<IFormFile>();
            if (filesList.Count == 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_AT_LEAST_ONE_FILE_REQUIRED), nameof(files));

            var departmentId = _tokenParser.GetDepartmentID() ?? Guid.Empty;
            var directoryPath = GetNotificationDirectoryPath(departmentId, requesterId, requestId);
            Directory.CreateDirectory(directoryPath);

            var result = new List<UploadFileDto>();

            foreach (IFormFile file in filesList)
            {
                if (file.Length == 0)
                    throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_FILES_EMPTY));

                if (file.Length > MaxFileSizeBytes)
                    throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_FILE_SIZE_EXCEEDED, file.FileName, MaxFileSizeBytes / (1024 * 1024)));

                var typeOfFile = GetTypeOfFile(file.FileName, file.ContentType);
                var fileUploadedId = Guid.NewGuid();
                var fileNamePrefix = Path.GetFileNameWithoutExtension(file.FileName);
                if (string.IsNullOrWhiteSpace(fileNamePrefix))
                    fileNamePrefix = file.FileName ?? "file";

                var safeFileName = SanitizeFileName(fileNamePrefix);
                var physicalFileName = $"{fileUploadedId}_{safeFileName}.{typeOfFile}";
                var fullPath = Path.Combine(directoryPath, physicalFileName);

                await using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await file.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
                }

                result.Add(CreateUploadFileDto(fileUploadedId.ToString(), fileNamePrefix, typeOfFile, requestId.ToString(), file.Length, DateTime.UtcNow));
            }

            _logger.LogInformation("Uploaded {Count} notification file(s) for request {RequestId} to {Path}.", result.Count, requestId, directoryPath);

            return result;
        }

        /// <inheritdoc />
        public Task<RequestFilesOrFileResultDto?> GetNotificationFileAsync(
            Guid requestId,
            Guid requesterId,
            Guid fileId,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            var departmentId = _tokenParser.GetDepartmentID();
            if (departmentId == null)
                return Task.FromResult<RequestFilesOrFileResultDto?>(null);

            var directoryPath = GetNotificationDirectoryPath(departmentId.Value, requesterId, requestId);
            if (!Directory.Exists(directoryPath))
                return Task.FromResult<RequestFilesOrFileResultDto?>(null);

            var prefix = $"{fileId}_";
            var filePath = Directory.GetFiles(directoryPath)
                .FirstOrDefault(f => Path.GetFileName(f).StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

            if (filePath == null)
                return Task.FromResult<RequestFilesOrFileResultDto?>(null);

            var fileName = Path.GetFileName(filePath);
            var fileParsed = TryParsePhysicalFileName(fileName, fileId);
            if (fileParsed == null)
                return Task.FromResult<RequestFilesOrFileResultDto?>(null);

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var contentType = GetContentType(fileParsed.Value.TypeOfFile);
            var downloadFileName = string.IsNullOrEmpty(fileParsed.Value.TypeOfFile) || fileParsed.Value.TypeOfFile == "bin"
                ? fileParsed.Value.BaseName
                : $"{fileParsed.Value.BaseName}.{fileParsed.Value.TypeOfFile}";

            return Task.FromResult<RequestFilesOrFileResultDto?>(new RequestFilesOrFileResultDto
            {
                File = CreateRequestFileStreamDto(stream, contentType, downloadFileName)
            });
        }

        /// <inheritdoc />
        public async Task<RequestFilesOrFileResultDto?> GetFilesOrFileAsync(Guid requestId, Guid requesterId, Guid? fileId, CancellationToken cancellationToken = default)
        {
            Guid? departmentId = _tokenParser.GetDepartmentID();
            if (departmentId != null)
            {
                var directoryPath = await GetRequestDirectoryPathAsync(requestId, requesterId, departmentId.Value).ConfigureAwait(false);
                if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
                    return null;

                if (!fileId.HasValue)
                {
                    // Return list of all files
                    var list = new List<UploadFileDto>();
                    foreach (var fullPath in Directory.GetFiles(directoryPath))
                    {
                        var physicalFileName = Path.GetFileName(fullPath);
                        var parsed = TryParsePhysicalFileName(physicalFileName);
                        if (parsed == null)
                            continue;

                        long? fileSize = null;
                        DateTime? createdAt = null;
                        try
                        {
                            var fileInfo = new FileInfo(fullPath);
                            fileSize = fileInfo.Length;
                            createdAt = fileInfo.LastWriteTimeUtc;
                        }
                        catch { /* ignore */ }

                        list.Add(CreateUploadFileDto(parsed.Value.FileId.ToString(), parsed.Value.BaseName, parsed.Value.TypeOfFile, requestId.ToString(), fileSize, createdAt));
                    }
                    return new RequestFilesOrFileResultDto { List = list };
                }

                // Return single file stream
                var prefix = $"{fileId.Value}_";
                var filePath = Directory.GetFiles(directoryPath)
                    .FirstOrDefault(f => Path.GetFileName(f).StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

                if (filePath == null)
                    return null;

                var fileName = Path.GetFileName(filePath);
                var fileParsed = TryParsePhysicalFileName(fileName, fileId);
                if (fileParsed == null)
                    return null;

                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var contentType = GetContentType(fileParsed.Value.TypeOfFile);
                var downloadFileName = string.IsNullOrEmpty(fileParsed.Value.TypeOfFile) || fileParsed.Value.TypeOfFile == "bin"
                    ? fileParsed.Value.BaseName
                    : $"{fileParsed.Value.BaseName}.{fileParsed.Value.TypeOfFile}";

                return new RequestFilesOrFileResultDto
                {
                    File = CreateRequestFileStreamDto(stream, contentType, downloadFileName)
                };
            }
            return new RequestFilesOrFileResultDto();
        }

        /// <inheritdoc />
        public async Task<string?> UploadDepartmentLogoAsync(Guid departmentId, IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_LOGO_REQUIRED), nameof(file));

            if (file.Length > MaxDepartmentLogoSizeBytes)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_LOGO_SIZE_EXCEEDED, MaxDepartmentLogoSizeBytes / (1024 * 1024)));

            var typeOfFile = GetTypeOfFile(file.FileName, file.ContentType);
            if (!AllowedLogoExtensions.Contains(typeOfFile))
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_LOGO_INVALID_TYPE, typeOfFile));

            var logoDir = GetDepartmentLogoPath(departmentId);
            Directory.CreateDirectory(logoDir);

            // Replace existing logo: delete any existing file(s) in the logo folder
            foreach (var existing in Directory.GetFiles(logoDir))
            {
                try
                {
                    System.IO.File.Delete(existing);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not delete existing logo in {Path}", logoDir);
                }
            }

            var fileName = $"logo.{typeOfFile}";
            var fullPath = Path.Combine(logoDir, fileName);

            await using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await file.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
            }

            _logger.LogInformation("Uploaded department logo for DepartmentId {DepartmentId} to {Path}.", departmentId, fullPath);
            return fileName;
        }

        /// <inheritdoc />
        public Task<(Stream Stream, string ContentType, string FileName)?> GetDepartmentLogoAsync(Guid departmentId, CancellationToken cancellationToken = default)
        {
            var logoDir = GetDepartmentLogoPath(departmentId);
            if (!Directory.Exists(logoDir))
                return Task.FromResult<(Stream Stream, string ContentType, string FileName)?>(null);

            var files = Directory.GetFiles(logoDir);
            var logoPath = files.FirstOrDefault();
            if (logoPath == null)
                return Task.FromResult<(Stream Stream, string ContentType, string FileName)?>(null);

            var downloadFileName = Path.GetFileName(logoPath);
            var ext = Path.GetExtension(logoPath).TrimStart('.').ToLowerInvariant();
            var contentType = GetContentType(ext);
            var stream = new FileStream(logoPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return Task.FromResult<(Stream, string, string)?>((stream, contentType, downloadFileName));
        }

        /// <inheritdoc />
        public async Task<string?> UploadEmployeePhotoAsync(Guid employeeId, IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_PROFILE_PHOTO_REQUIRED), nameof(file));

            if (file.Length > MaxDepartmentLogoSizeBytes)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_PHOTO_SIZE_EXCEEDED, MaxDepartmentLogoSizeBytes / (1024 * 1024)));

            var typeOfFile = GetTypeOfFile(file.FileName, file.ContentType);
            if (!AllowedLogoExtensions.Contains(typeOfFile))
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_PHOTO_INVALID_TYPE, typeOfFile));

            var employeeDir = GetEmployeeProfileImagePath(employeeId);
            Directory.CreateDirectory(employeeDir);

            foreach (var existing in Directory.GetFiles(employeeDir))
            {
                try
                {
                    System.IO.File.Delete(existing);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not delete existing profile photo in {Path}", employeeDir);
                }
            }

            var originalName = Path.GetFileNameWithoutExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(originalName))
                originalName = "photo";
            var safeName = SanitizeFileName(originalName);
            var fileName = $"{employeeId}_{safeName}.{typeOfFile}";
            var fullPath = Path.Combine(employeeDir, fileName);

            await using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await file.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
            }

            _logger.LogInformation("Uploaded employee profile photo for EmployeeId {EmployeeId} to {Path}.", employeeId, fullPath);
            return fileName;
        }

        /// <inheritdoc />
        public Task<(Stream Stream, string ContentType, string FileName)?> GetEmployeePhotoAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            var employeeDir = GetEmployeeProfileImagePath(employeeId);
            if (!Directory.Exists(employeeDir))
                return Task.FromResult<(Stream Stream, string ContentType, string FileName)?>(null);

            var files = Directory.GetFiles(employeeDir);
            var photoPath = files.FirstOrDefault();
            if (photoPath == null)
                return Task.FromResult<(Stream Stream, string ContentType, string FileName)?>(null);

            var downloadFileName = Path.GetFileName(photoPath);
            var ext = Path.GetExtension(photoPath).TrimStart('.').ToLowerInvariant();
            var contentType = GetContentType(ext);
            var stream = new FileStream(photoPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return Task.FromResult<(Stream, string, string)?>((stream, contentType, downloadFileName));
        }

        /// <inheritdoc />
        public Task<bool> HasEmployeePhotoAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            var employeeDir = GetEmployeeProfileImagePath(employeeId);
            if (!Directory.Exists(employeeDir))
                return Task.FromResult(false);

            var files = Directory.GetFiles(employeeDir);
            return Task.FromResult(files.Length > 0);
        }

        /// <inheritdoc />
        public async Task DeleteFileAsync(Guid requestId, Guid requesterId, Guid fileId, CancellationToken cancellationToken = default)
        {
            Guid? departmentId = _tokenParser.GetDepartmentID();
            if (departmentId != null)
            {
                var directoryPath = await GetRequestDirectoryPathAsync(requestId, requesterId, departmentId.Value).ConfigureAwait(false);
                if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
                    throw new KeyNotFoundException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_REQUEST_NOT_FOUND_OR_NO_FOLDER, requestId));

                var prefix = $"{fileId}_";
                var filePath = Directory.GetFiles(directoryPath)
                    .FirstOrDefault(f => Path.GetFileName(f).StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

                if (filePath == null)
                    throw new KeyNotFoundException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.UPLOAD_FILE_NOT_FOUND_FOR_REQUEST, fileId, requestId));

                try
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation("Deleted file {FileId} for request {RequestId} at {Path}.", fileId, requestId, filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete file {FileId} for request {RequestId}.", fileId, requestId);
                    throw;
                }
            }
        }

        /// <summary>
        /// Returns the set of file types (extensions) already uploaded for the given request. Used to enforce one file per type per request.
        /// </summary>
        private async Task<HashSet<string>> GetExistingFileTypesForRequestAsync(Guid requestId, Guid requesterId)
        {
            var result = await GetFilesOrFileAsync(requestId, requesterId, null, cancellationToken: default).ConfigureAwait(false);
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (result?.List == null)
                return set;
            foreach (var dto in result.List)
            {
                if (!string.IsNullOrEmpty(dto.TypeOfFile))
                    set.Add(dto.TypeOfFile);
            }
            return set;
        }

        /// <summary>
        /// Resolves the directory path for a request (BasePath/DepartmentId/files/RequesterId/RequestId). Returns null if request not found.
        /// </summary>
        private async Task<string?> GetRequestDirectoryPathAsync(Guid requestId, Guid requesterId, Guid departmentId)
        {
            return GetPathUnderDepartment(departmentId, FilesSubfolder, GetRequesterSegment(requesterId), requestId.ToString());
        }

        private string GetNotificationDirectoryPath(Guid departmentId, Guid requesterId, Guid requestId) =>
            GetPathUnderDepartment(
                departmentId,
                FilesSubfolder,
                GetRequesterSegment(requesterId),
                requestId.ToString(),
                EmailAttachmentsSubfolder);

        /// <summary>
        /// Returns the folder name for a department (Guid string or placeholder).
        /// </summary>
        private static string GetDepartmentSegment(Guid departmentId)
        {
            return (departmentId != Guid.Empty && departmentId != default) ? departmentId.ToString() : NoDepartmentFolder;
        }

        /// <summary>
        /// Returns the folder name for a requester (Guid string or placeholder).
        /// </summary>
        private static string GetRequesterSegment(Guid? requesterId)
        {
            return (requesterId.HasValue && requesterId.Value != Guid.Empty) ? requesterId.Value.ToString() : NoRequesterFolder;
        }

        /// <summary>
        /// Builds path: BasePath/Department/{departmentId}/subfolder/[additionalSegments]. Used for request files and department logo.
        /// </summary>
        private string GetPathUnderDepartment(Guid departmentId, string subfolder, params string[] additionalSegments)
        {
            var basePath = _pathProvider.GetBasePath();
            var departmentSegment = GetDepartmentSegment(departmentId);
            if (additionalSegments == null || additionalSegments.Length == 0)
                return Path.Combine(basePath, DepartmentFolder, departmentSegment, subfolder);
            return Path.Combine(basePath, DepartmentFolder, departmentSegment, subfolder, Path.Combine(additionalSegments));
        }

        /// <summary>Path for department logo: BasePath/Department/{departmentId}/logo.</summary>
        private string GetDepartmentLogoPath(Guid departmentId)
        {
            return GetPathUnderDepartment(departmentId, LogoSubfolder);
        }

        /// <summary>Path for employee profile photo: BasePath/Employee/ProfilePhoto/{employeeId}.</summary>
        private string GetEmployeeProfileImagePath(Guid employeeId)
        {
            return Path.Combine(_pathProvider.GetBasePath(), EmployeeFolder, ProfilePhotoSubfolder, employeeId.ToString());
        }

        /// <summary>
        /// Creates a RequestFileStreamDto with the given values (common for stream responses).
        /// </summary>
        private static RequestFileStreamDto CreateRequestFileStreamDto(Stream stream, string contentType, string fileName)
        {
            return new RequestFileStreamDto
            {
                Stream = stream,
                ContentType = contentType,
                FileName = fileName
            };
        }

        /// <summary>
        /// Creates an UploadFileDto with the given values (common constructor for all usages).
        /// </summary>
        private static UploadFileDto CreateUploadFileDto(string fileUploadedId, string fileName, string typeOfFile, string requestId, long? fileSize, DateTime? createdAt)
        {
            return new UploadFileDto
            {
                FileUploadedId = fileUploadedId,
                FileName = fileName,
                TypeOfFile = typeOfFile,
                RequestID = requestId,
                FileSize = fileSize,
                CreatedAt = createdAt
            };
        }

        /// <summary>
        /// Parses physical file name "{fileId}_{baseName}.{typeOfFile}". Returns (FileId, BaseName, TypeOfFile) or null if invalid.
        /// </summary>
        private static (Guid FileId, string BaseName, string TypeOfFile)? TryParsePhysicalFileName(string? physicalFileName, Guid? expectedFileId = null)
        {
            if (string.IsNullOrEmpty(physicalFileName) || !physicalFileName.Contains("_"))
                return null;

            var underscoreIdx = physicalFileName.IndexOf('_');
            var fileIdStr = physicalFileName.Substring(0, underscoreIdx);
            if (!Guid.TryParse(fileIdStr, out var fileUploadedId))
                return null;

            if (expectedFileId.HasValue && fileUploadedId != expectedFileId.Value)
                return null;

            var afterPrefix = physicalFileName.Substring(underscoreIdx + 1);
            var extIdx = afterPrefix.LastIndexOf('.');
            var typeOfFile = extIdx >= 0 ? afterPrefix.Substring(extIdx + 1) : "bin";
            var baseName = extIdx >= 0 ? afterPrefix.Substring(0, extIdx) : afterPrefix;

            return (fileUploadedId, baseName, typeOfFile);
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "file";
            var invalid = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries)).Trim();
        }

        private static string GetContentType(string typeOfFile)
        {
            return typeOfFile?.ToLowerInvariant() switch
            {
                "pdf" => "application/pdf",
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "gif" => "image/gif",
                "webp" => "image/webp",
                "doc" => "application/msword",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "xls" => "application/vnd.ms-excel",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "txt" => "text/plain",
                "csv" => "text/csv",
                _ => MediaTypeNames.Application.Octet
            };
        }

        private static string GetTypeOfFile(string fileName, string? contentType)
        {
            var ext = Path.GetExtension(fileName);
            if (!string.IsNullOrWhiteSpace(ext))
                return ext.TrimStart('.').ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(contentType))
                return "bin";

            var slash = contentType.IndexOf('/');
            if (slash >= 0 && slash < contentType.Length - 1)
            {
                var part = contentType[(slash + 1)..].Trim();
                if (part.Equals("jpeg", StringComparison.OrdinalIgnoreCase))
                    return "jpg";
                return part.ToLowerInvariant();
            }

            return "bin";
        }
    }
}
