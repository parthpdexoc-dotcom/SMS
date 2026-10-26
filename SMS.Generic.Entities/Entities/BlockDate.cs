using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a block date entity in the system.
    /// </summary>
    [Table("BlockDate", Schema = "dbo")]
    public class BlockDate : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the block date (Primary Key).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BlockDateId { get; set; }

        /// <summary>
        /// Gets or sets the request identifier (Foreign Key to Request).
        /// </summary>
        public Guid RequestID { get; set; }

        /// <summary>
        /// Gets or sets the post identifier (Foreign Key to Post).
        /// </summary>
        public Guid PostID { get; set; }

        /// <summary>
        /// Gets or sets the start date and time (combines StartDate + StartTime).
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the end date and time (combines EndDate + EndTime).
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date when this record was deleted.
        /// </summary>
        //public DateTime? DeletedDate { get; set; } 

        /// <summary>
        /// Gets or sets the name of user who deleted the record.
        /// </summary>
        //[MaxLength(255)]
        //public string? DeletedBy { get; set; } not exist in the base entity

        /// <summary>
        /// Gets or sets the user identifier who deleted the record (Foreign Key).
        /// </summary>
        //public Guid? UserDeletedID { get; set; } not exist in base entity

        /// <summary>
        /// Navigation property for the request.
        /// </summary>
        [ForeignKey(nameof(RequestID))]
        public virtual Request? Request { get; set; }

        /// <summary>
        /// Navigation property for the post.
        /// </summary>
        [ForeignKey(nameof(PostID))]
        public virtual Post? Post { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this block date.
        /// </summary>
        [ForeignKey("CreatedUserID")]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who edited this block date.
        /// </summary>
        [ForeignKey("EditedUserID")]
        public virtual Employee? EditedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who deleted this block date.
        /// </summary>
        //[ForeignKey(nameof(UserDeletedID))]
        //public virtual Employee? UserDeletedEmployee { get; set; }
    }
}
