using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for ExemptedRequester operations.
    /// </summary>
    public class ExemptedRequesterDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the exempted requester record.
        /// </summary>
        public int ExemptedRequestersID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier.
        /// </summary>
        public int? RequesterID { get; set; }

        /// <summary>
        /// Gets or sets the employee type identifier.
        /// </summary>
        public int? EmployeeTypeID { get; set; }

        /// <summary>
        /// Gets or sets the rate before exemption.
        /// </summary>
        public decimal? RateBefore { get; set; }

        /// <summary>
        /// Gets or sets the rate after exemption.
        /// </summary>
        public decimal? RateAfter { get; set; }

        /// <summary>
        /// Gets or sets the effective date of the exemption.
        /// </summary>
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets whether the exemption is enabled.
        /// </summary>
        [MaxLength(1, ErrorMessage = "Enabled cannot exceed 1 character")]
        public string? Enabled { get; set; }

        /// <summary>
        /// Gets or sets the administrator identifier who created or modified the record.
        /// </summary>
        public int? AdminID { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the record was created or last modified.
        /// </summary>
        public DateTime? TimeStamp { get; set; }
    }
}

