using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an activity type entity in the system.
    /// </summary>
    [Table("ActivityType", Schema = "dbo")]
    public class ActivityType
    {
        /// <summary>
        /// Gets or sets the unique identifier for the activity type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityTypeID { get; set; }

        /// <summary>
        /// Gets or sets the title of the activity type.
        /// </summary>
        [MaxLength(10)]
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

