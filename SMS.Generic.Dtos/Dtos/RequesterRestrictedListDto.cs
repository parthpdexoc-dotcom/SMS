using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Entities.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RequesterRestrictedList operations.
    /// </summary>
    public class RequesterRestrictedListDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester restricted list record.
        /// If null or empty, a new Guid will be generated when creating a new record.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("requesterRestrictID")]
        [System.Text.Json.Serialization.JsonPropertyName("requesterRestrictID")]
        public Guid? RequesterRestrictID { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier (foreign key to Requesters).
        /// </summary>
        public Guid? RequesterID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier (foreign key to Employee).
        /// ID of employee NOT allowed to work for this Requester.
        /// </summary>
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the effective start date for this restriction.
        /// </summary>
        public DateOnly EffectiveStartDate { get; set; }

        /// <summary>
        /// Gets or sets the effective end date for this restriction.
        /// </summary>
        public DateOnly EffectiveEndDate { get; set; }

        public EmployeeDto? Employee { get; set; }

        public RequesterDto? Requester { get; set; }
    }

    /// <summary>
    /// Response DTO for RequesterRestrictedList with Requester and Employee names.
    /// </summary>
    public class RequesterRestrictedListResponseDto : RequesterRestrictedListDto
    {
        /// <summary>
        /// Gets or sets the requester name (from navigation property).
        /// </summary>
        public string? RequesterName { get; set; }

        /// <summary>
        /// Gets or sets the employee name (from navigation property, combined FirstName and LastName).
        /// </summary>
        public string? EmployeeName { get; set; }
    }
}

