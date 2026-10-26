using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for ActivityType operations.
    /// </summary>
    public class ActivityTypeDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the activity type.
        /// </summary>
        public int ActivityTypeID { get; set; }

        /// <summary>
        /// Gets or sets the title of the activity type.
        /// </summary>
        [MaxLength(10, ErrorMessage = "Title cannot exceed 10 characters")]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the request type identifier.
        /// </summary>
        public int? RequestTypeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public int? DepartmentID { get; set; }
    }
}

