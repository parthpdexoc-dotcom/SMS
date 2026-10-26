using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a fitness standards entity in the system.
    /// </summary>
    [Table("FitStandards", Schema = "dbo")]
    public class FitStandards
    {
        /// <summary>
        /// Gets or sets the unique identifier for the fitness standard.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FitStandardID { get; set; }

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        public Guid? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the FF code.
        /// </summary>
        [MaxLength(3)]
        public string? FfCode { get; set; }

        /// <summary>
        /// Gets or sets the effective date.
        /// </summary>
        [Column(TypeName = "smalldatetime")]
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the vertical jump measurement.
        /// </summary>
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? VerticalJump { get; set; }

        /// <summary>
        /// Gets or sets the bench press count.
        /// </summary>
        public short? BenchPress { get; set; }

        /// <summary>
        /// Gets or sets the agility run time.
        /// </summary>
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? AgilityRun { get; set; }

        /// <summary>
        /// Gets or sets the sit-ups count.
        /// </summary>
        public short? SitUps { get; set; }

        /// <summary>
        /// Gets or sets the 300 meter run time.
        /// </summary>
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? Meter300 { get; set; }

        /// <summary>
        /// Gets or sets the push-ups count.
        /// </summary>
        public short? PushUps { get; set; }

        /// <summary>
        /// Gets or sets the 1.5 mile run time.
        /// </summary>
        [Column(TypeName = "decimal(8, 2)")]
        public decimal? Mile1_5 { get; set; }

        /// <summary>
        /// Gets or sets whether the fitness standard is active.
        /// </summary>
        public bool? Active { get; set; }

        /// <summary>
        /// Gets or sets whether the fitness standard is visible.
        /// </summary>
        public bool? Visible { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        [Column(TypeName = "smalldatetime")]
        public DateTime? TimeStamp { get; set; }
    }
}

