using SMS.Generic.Dtos.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RequesterConfig operations.
    /// </summary>
    public class RequesterConfigDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester configuration.
        /// If null or empty, a new Guid will be generated when creating a new record.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("requesterConfigID")]
        [System.Text.Json.Serialization.JsonPropertyName("requesterConfigID")]
        public Guid? RequesterConfigID { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier.
        /// </summary>
        public Guid RequesterID { get; set; }

        /// <summary>
        /// Gets or sets whether skills are enabled.
        /// </summary>
        public bool? EnableSkills { get; set; }

        /// <summary>
        /// Gets or sets the defined skills.
        /// </summary>
        public string? DefinedSkills { get; set; }

        /// <summary>
        /// Gets or sets whether requester manager is enabled.
        /// </summary>
        public bool? EnableRequesterManager { get; set; }

        /// <summary>
        /// Gets or sets the defined requester manager.
        /// </summary>
        public string? DefineRequesterManager { get; set; }

        /// <summary>
        /// Gets or sets whether pre-employment paperwork is enabled.
        /// </summary>
        public bool? EnablePreEmploymentPaperwork { get; set; }

        /// <summary>
        /// Gets or sets whether activity reporting is enabled.
        /// </summary>
        public bool? EnableActivityReporting { get; set; }

        /// <summary>
        /// Gets or sets whether restricted employee list is enabled.
        /// </summary>
        public bool? EnableRestrictedEmployeeList { get; set; }

        /// <summary>
        /// Gets or sets whether parent child invoicing is enabled.
        /// </summary>
        public bool? EnableParentChildInvoicing { get; set; }

        /// <summary>
        /// Gets or sets whether exempt from invoicing is enabled.
        /// </summary>
        public bool? EnableExemptFromInvoicing { get; set; }

        /// <summary>
        /// Gets or sets whether exempt from payroll is enabled.
        /// </summary>
        public bool? EnableExemptFromPayroll { get; set; }

        /// <summary>
        /// Gets or sets whether exempt from hours rules is enabled.
        /// </summary>
        public bool? EnableExemptFromHoursRules { get; set; }
    }
}
