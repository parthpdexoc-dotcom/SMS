using SMS.Generic.Dtos.Interfaces;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for PayCode operations.
    /// </summary>
    public class PayCodeDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the pay code.
        /// Optional: If not provided or empty, a new record will be created.
        /// If provided, the record will be updated if it exists, otherwise returns not found.
        /// </summary>
        public Guid? PayCodeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (auto-populated from token, not required in request).
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the pay code value.
        /// </summary>
        public string? PayCodeValue { get; set; }

        /// <summary>
        /// Gets or sets the pay code description.
        /// </summary>
        public string? PayCodeDescription { get; set; }

        /// <summary>
        /// Gets or sets the status of the pay code.
        /// </summary>
        public bool PayCodeStatus { get; set; }
    }
}

