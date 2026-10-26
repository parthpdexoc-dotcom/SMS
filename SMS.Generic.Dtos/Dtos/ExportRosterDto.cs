using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for ExportRoster operations.
    /// </summary>
    public class ExportRosterDto
    {
        /// <summary>
        /// Gets or sets the unique identifier (serial number).
        /// </summary>
        public int SL { get; set; }

        /// <summary>
        /// Gets or sets the requester name.
        /// </summary>
        [MaxLength(2147483647, ErrorMessage = "RequesterName cannot exceed maximum length")]
        public string? RequesterName { get; set; }

        /// <summary>
        /// Gets or sets the request name.
        /// </summary>
        [MaxLength(2147483647, ErrorMessage = "RequestName cannot exceed maximum length")]
        public string? RequestName { get; set; }

        /// <summary>
        /// Gets or sets the post day.
        /// </summary>
        [MaxLength(2147483647, ErrorMessage = "PostDay cannot exceed maximum length")]
        public string? PostDay { get; set; }

        /// <summary>
        /// Gets or sets the position name.
        /// </summary>
        [MaxLength(2147483647, ErrorMessage = "PositionName cannot exceed maximum length")]
        public string? PositionName { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        [MaxLength(2147483647, ErrorMessage = "StartTime cannot exceed maximum length")]
        public string? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the hours required.
        /// </summary>
        public decimal? HourRequired { get; set; }

        /// <summary>
        /// Gets or sets the end date and time.
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the post ID.
        /// </summary>
        [MaxLength(2147483647, ErrorMessage = "PostID cannot exceed maximum length")]
        public string? PostID { get; set; }

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        [MaxLength(2147483647, ErrorMessage = "EmployeeNo cannot exceed maximum length")]
        public string? EmployeeNo { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [MaxLength(2147483647, ErrorMessage = "FirstName cannot exceed maximum length")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [MaxLength(2147483647, ErrorMessage = "LastName cannot exceed maximum length")]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets whether the position is filled.
        /// </summary>
        [MaxLength(2147483647, ErrorMessage = "Filled cannot exceed maximum length")]
        public string? Filled { get; set; }

        /// <summary>
        /// Gets or sets the primary contact.
        /// </summary>
        [MaxLength(2147483647, ErrorMessage = "PrimaryContact cannot exceed maximum length")]
        public string? PrimaryContact { get; set; }
    }
}

