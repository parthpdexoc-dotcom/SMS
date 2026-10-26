using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Estimate operations.
    /// </summary>
    public class EstimateDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the estimate.
        /// </summary>
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
        public DateTime? EstimateCreatedDate { get; set; }

        /// <summary>
        /// Gets or sets whether the estimate is closed.
        /// </summary>
        public bool? CloseEstimate { get; set; }

        /// <summary>
        /// Gets or sets the close estimate date.
        /// </summary>
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

