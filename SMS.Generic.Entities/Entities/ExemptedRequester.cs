using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an exempted requester entity in the system.
    /// </summary>
    [Table("ExemptedRequester", Schema = "dbo")]
    public class ExemptedRequester
    {
        /// <summary>
        /// Gets or sets the unique identifier for the exempted requester record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [Column(TypeName = "smallmoney")]
        public decimal? RateBefore { get; set; }

        /// <summary>
        /// Gets or sets the rate after exemption.
        /// </summary>
        [Column(TypeName = "smallmoney")]
        public decimal? RateAfter { get; set; }

        /// <summary>
        /// Gets or sets the effective date of the exemption.
        /// </summary>
        [Column(TypeName = "smalldatetime")]
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets whether the exemption is enabled.
        /// </summary>
        [MaxLength(1)]
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

