using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for FitStandards operations.
    /// </summary>
    public class FitStandardsDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the fitness standard.
        /// </summary>
        public int FitStandardID { get; set; }

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        public Guid? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the FF code.
        /// </summary>
        [MaxLength(3, ErrorMessage = "FfCode cannot exceed 3 characters")]
        public string? FfCode { get; set; }

        /// <summary>
        /// Gets or sets the effective date.
        /// </summary>
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the vertical jump measurement.
        /// </summary>
        public decimal? VerticalJump { get; set; }

        /// <summary>
        /// Gets or sets the bench press count.
        /// </summary>
        public short? BenchPress { get; set; }

        /// <summary>
        /// Gets or sets the agility run time.
        /// </summary>
        public decimal? AgilityRun { get; set; }

        /// <summary>
        /// Gets or sets the sit-ups count.
        /// </summary>
        public short? SitUps { get; set; }

        /// <summary>
        /// Gets or sets the 300 meter run time.
        /// </summary>
        public decimal? Meter300 { get; set; }

        /// <summary>
        /// Gets or sets the push-ups count.
        /// </summary>
        public short? PushUps { get; set; }

        /// <summary>
        /// Gets or sets the 1.5 mile run time.
        /// </summary>
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
        public DateTime? TimeStamp { get; set; }
    }
}

