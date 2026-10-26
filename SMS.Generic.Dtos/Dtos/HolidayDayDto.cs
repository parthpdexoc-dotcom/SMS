using SMS.Generic.Dtos.Interfaces;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for HolidayDay entity.
    /// </summary>
    public class HolidayDayDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the holiday day record.
        /// </summary>
        public Guid? HolidayDayID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (foreign key to Department).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the name of the holiday.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the date of the holiday.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets whether premium rate applies.
        /// </summary>
        public bool? IsPremiumRate { get; set; }

        /// <summary>
        /// Gets or sets whether the holiday is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets whether to pay premium for entire shift.
        /// </summary>
        public bool? PayPremiumForEntireShift { get; set; }

    }
}
