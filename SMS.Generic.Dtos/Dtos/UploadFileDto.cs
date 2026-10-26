namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data transfer object for uploaded file (API response).
    /// </summary>
    public class UploadFileDto
    {
        /// <summary>
        /// Original file name.
        /// </summary>
        public string FileName { get; set; } = null!;

        /// <summary>
        /// File type/extension (e.g. pdf, xlsx, txt, docx, jpg, png).
        /// </summary>
        public string TypeOfFile { get; set; } = null!;

        /// <summary>
        /// Parent entity ID this file belongs to (string for JSON).
        /// </summary>
        public string RequestID { get; set; } = null!;

        /// <summary>
        /// Unique ID for the uploaded file record (GUID string).
        /// </summary>
        public string FileUploadedId { get; set; } = null!;

        /// <summary>
        /// Optional: file size in bytes.
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// Optional: when the file was uploaded (UTC).
        /// </summary>
        public DateTime? CreatedAt { get; set; }
    }
}
