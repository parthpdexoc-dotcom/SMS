using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RequesterConfiguration operations.
    /// </summary>
    public class RequesterConfigurationDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester configuration.
        /// </summary>
        public long ReqConID { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier (references Requesters table).
        /// </summary>
        public Guid? RequesterID { get; set; }

        /// <summary>
        /// Gets or sets the open 24 hour value.
        /// </summary>
        public int? Open24Hr { get; set; }

        /// <summary>
        /// Gets or sets the SBA 24 hour value.
        /// </summary>
        public int? Sba24Hr { get; set; }

        /// <summary>
        /// Gets or sets the hours per day.
        /// </summary>
        public int? HourPerDay { get; set; }

        /// <summary>
        /// Gets or sets the hours per week.
        /// </summary>
        public int? HourPerWeek { get; set; }

        /// <summary>
        /// Gets or sets whether supervisor override is enabled.
        /// </summary>
        public bool? SupervisorOverride { get; set; }

        /// <summary>
        /// Gets or sets whether this is the requester default configuration.
        /// </summary>
        public bool? RequesterDefault { get; set; }

        /// <summary>
        /// Gets or sets the list of not eligible employees.
        /// </summary>
        public string? NotEligibleEmployee { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the track history information.
        /// </summary>
        public string? TrackHistory { get; set; }
    }
}

