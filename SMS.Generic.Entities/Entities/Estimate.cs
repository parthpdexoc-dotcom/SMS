using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an estimate entity in the system.
    /// </summary>
    [Table("Estimate", Schema = "dbo")]
    public class Estimate
    {
        /// <summary>
        /// Gets or sets the unique identifier for the estimate.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EstimateID { get; set; }

        /// <summary>
        /// Gets or sets the estimate number.
        /// </summary>
        public string? EstimateNo { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier.
        /// </summary>
        public int? RequesterID { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        public int? RequestID { get; set; }

        /// <summary>
        /// Gets or sets the estimate created date.
        /// </summary>
        [Column(TypeName = "smalldatetime")]
        public DateTime? EstimateCreatedDate { get; set; }

        /// <summary>
        /// Gets or sets whether the estimate is closed.
        /// </summary>
        public bool? CloseEstimate { get; set; }

        /// <summary>
        /// Gets or sets the close estimate date.
        /// </summary>
        [Column(TypeName = "smalldatetime")]
        public DateTime? CloseEstimateDate { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets whether the estimate is active.
        /// </summary>
        public bool? IsActive { get; set; }
    }
}

