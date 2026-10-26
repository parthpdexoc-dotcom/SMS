using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for State operations.
    /// </summary>
    public class StateDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the state.
        /// </summary>
        public long StateID { get; set; }

        /// <summary>
        /// Gets or sets the state code.
        /// </summary>
        [MaxLength(2, ErrorMessage = "Code cannot exceed 2 characters")]
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the state name.
        /// </summary>
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string? Name { get; set; }
    }
}

