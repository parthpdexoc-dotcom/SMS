using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a login/logout history record entity in the system.
    /// </summary>
    [Table("LogInOutHistory", Schema = "dbo")]
    public class LogInOutHistory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the login/logout history record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogInOutHistoryID { get; set; }

        /// <summary>
        /// Gets or sets the employee ID associated with this record.
        /// </summary>
        public int? EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the department ID associated with this record.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets whether this is a login event.
        /// </summary>
        public bool? Login { get; set; }

        /// <summary>
        /// Gets or sets whether this is a logout event.
        /// </summary>
        public bool? Logout { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the event occurred.
        /// </summary>
        public DateTime? TimeStamp { get; set; }
    }
}

