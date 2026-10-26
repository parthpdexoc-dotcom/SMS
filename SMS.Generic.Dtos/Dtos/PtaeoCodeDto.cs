using SMS.Generic.Dtos.Interfaces;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for PTAEO_Code operations.
    /// </summary>
    public class PtaeoCodeDto : IDepartmentScoped
    {
        public PtaeoCodeDto()
        {
        }

        /// <summary>
        /// Copies all fields from <paramref name="other"/> (audit snapshots, inactive marking).
        /// </summary>
        public PtaeoCodeDto(PtaeoCodeDto other)
        {
            ArgumentNullException.ThrowIfNull(other);
            PtaeoCodeID = other.PtaeoCodeID;
            DepartmentID = other.DepartmentID;
            ProjectNumber = other.ProjectNumber;
            TaskNumber = other.TaskNumber;
            AwardNumber = other.AwardNumber;
            FundNumber = other.FundNumber;
            OrganizationName = other.OrganizationName;
            OrganizationNumber = other.OrganizationNumber;
            StartDate = other.StartDate;
            EndDate = other.EndDate;
            Status = other.Status;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the PTAEO code.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("ptaeoCodeID")]
        [System.Text.Json.Serialization.JsonPropertyName("ptaeoCodeID")]
        public Guid? PtaeoCodeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the project number.
        /// </summary>
        public string? ProjectNumber { get; set; }

        /// <summary>
        /// Gets or sets the task number/name.
        /// </summary>
        public string? TaskNumber { get; set; }

        /// <summary>
        /// Gets or sets the award number.
        /// </summary>
        public string? AwardNumber { get; set; }

        /// <summary>
        /// Gets or sets the fund number.
        /// </summary>
        public string? FundNumber { get; set; }

        /// <summary>
        /// Gets or sets the organization name.
        /// </summary>
        public string? OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the Organization number.
        /// </summary>
        public string? OrganizationNumber { get; set; }

        /// <summary>
        /// Effective start date for this PTAEO row.
        /// </summary>
        public DateOnly? StartDate { get; set; }

        /// <summary>
        /// Effective end date for this PTAEO row.
        /// </summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// True when active (1), false when inactive (0). Not read from PTAEO CSV; set by application logic when needed.
        /// </summary>
        public bool Status { get; set; } = true;
    }
}
