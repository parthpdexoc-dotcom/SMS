using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a posts on duty entity in the system.
    /// </summary>
    [Table("PostsOnDuty", Schema = "dbo")]
    public class PostsOnDuty
    {
        /// <summary>
        /// Gets or sets the unique identifier for the posts on duty.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostsOnDutyID { get; set; }

        /// <summary>
        /// Gets or sets the division ID.
        /// </summary>
        public int? DivisionID { get; set; }

        /// <summary>
        /// Gets or sets the admin on duty ID.
        /// </summary>
        [MaxLength(50)]
        public string? AdminOnDutyID { get; set; }

        /// <summary>
        /// Gets or sets the unit name.
        /// </summary>
        [MaxLength(35)]
        public string? UnitName { get; set; }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Gets or sets the post name.
        /// </summary>
        [MaxLength(45)]
        public string? PostName { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public int? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public int? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the Monday value.
        /// </summary>
        public int? Monday { get; set; }

        /// <summary>
        /// Gets or sets the Tuesday value.
        /// </summary>
        public int? Tueday { get; set; }

        /// <summary>
        /// Gets or sets the Wednesday value.
        /// </summary>
        public int? Wedday { get; set; }

        /// <summary>
        /// Gets or sets the Thursday value.
        /// </summary>
        public int? Thuday { get; set; }

        /// <summary>
        /// Gets or sets the Friday value.
        /// </summary>
        public int? Friday { get; set; }

        /// <summary>
        /// Gets or sets the Saturday value.
        /// </summary>
        public int? Satday { get; set; }

        /// <summary>
        /// Gets or sets the Sunday value.
        /// </summary>
        public int? Sunday { get; set; }

        /// <summary>
        /// Gets or sets the rotation value.
        /// </summary>
        public int? Rotation { get; set; }

        /// <summary>
        /// Gets or sets the four three value.
        /// </summary>
        public int? Fourthree { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [Column(TypeName = "smalldatetime")]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        [MaxLength(12)]
        public string? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the place holder value.
        /// </summary>
        public int? PlaceHolder { get; set; }

        /// <summary>
        /// Gets or sets the list order.
        /// </summary>
        [MaxLength(250)]
        public string? ListOrder { get; set; }

        /// <summary>
        /// Gets or sets the original start date.
        /// </summary>
        [Column(TypeName = "smalldatetime")]
        public DateTime? OriginalStartDate { get; set; }
    }
}

