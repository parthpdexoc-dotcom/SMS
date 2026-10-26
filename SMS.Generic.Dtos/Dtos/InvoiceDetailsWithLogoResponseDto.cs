namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Response for GET invoice details by invoice ID when department logo is requested.
    /// </summary>
    public class InvoiceDetailsWithLogoResponseDto
    {
        /// <summary>
        /// Invoice detail items.
        /// </summary>
        public IEnumerable<InvoiceDetailDto> Items { get; set; } = null!;

        /// <summary>
        /// Department logo when requested (optional query departmentId). Null when not requested or not found.
        /// </summary>
        public DepartmentLogoDto? Logo { get; set; }
        public bool IsUpdated { get; set; }
    }

    /// <summary>
    /// Department logo as base64 for embedding in JSON (e.g. for invoice header).
    /// </summary>
    public class DepartmentLogoDto
    {
        /// <summary>
        /// MIME content type (e.g. image/png, image/jpeg).
        /// </summary>
        public string ContentType { get; set; } = null!;

        /// <summary>
        /// Image bytes as base64 string. Use in img src as "data:{ContentType};base64,{Base64}".
        /// </summary>
        public string Base64 { get; set; } = null!;
    }
}
