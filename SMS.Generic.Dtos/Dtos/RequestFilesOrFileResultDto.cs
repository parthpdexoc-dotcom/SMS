namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Result of GET by requestId: either list of file metadata (when fileId is null) or single file stream (when fileId is set).
    /// Exactly one of List or File should be set.
    /// </summary>
    public class RequestFilesOrFileResultDto
    {
        /// <summary>
        /// Set when fileId was null: list of file metadata for the request.
        /// </summary>
        public IReadOnlyList<UploadFileDto>? List { get; set; }

        /// <summary>
        /// Set when fileId was provided: single file stream and metadata for download.
        /// </summary>
        public RequestFileStreamDto? File { get; set; }
    }

    /// <summary>
    /// Single file stream result for download.
    /// </summary>
    public class RequestFileStreamDto
    {
        public Stream Stream { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string FileName { get; set; } = null!;
    }
}
