using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RequesterLocations operations.
    /// </summary>
    public class RequesterLocationsDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester location.
        /// </summary>
        public long PkLocationID { get; set; }

        /// <summary>
        /// Gets or sets the foreign key department ID.
        /// </summary>
        public Guid? FkDepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the foreign key requester ID (references Requesters table).
        /// </summary>
        public Guid? FKRequesterID { get; set; }

        /// <summary>
        /// Gets or sets the department defined ID.
        /// </summary>
        [MaxLength(50, ErrorMessage = "DepartmentDefinedID cannot exceed 50 characters")]
        public string? DepartmentDefinedID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the foreign key response area ID.
        /// </summary>
        public Guid? FkDepartmentResponseAreaID { get; set; }

        /// <summary>
        /// Gets or sets the first address line.
        /// </summary>
        public string? Address1 { get; set; }

        /// <summary>
        /// Gets or sets the second address line.
        /// </summary>
        public string? Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [MaxLength(20, ErrorMessage = "State cannot exceed 20 characters")]
        public string? State { get; set; }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        [MaxLength(20, ErrorMessage = "Zip cannot exceed 20 characters")]
        public string? Zip { get; set; }

        /// <summary>
        /// Gets or sets whether location check list is required.
        /// </summary>
        public bool? IsLocCheckListRequired { get; set; }

        /// <summary>
        /// Gets or sets the location check list IDs.
        /// </summary>
        [MaxLength(100, ErrorMessage = "LocCheckListIDs cannot exceed 100 characters")]
        public string? LocCheckListIDs { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public short? Status { get; set; }

        /// <summary>
        /// Gets or sets the user ID who created this record.
        /// </summary>
        public long? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user ID who last modified this record.
        /// </summary>
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets whether this record is deleted.
        /// </summary>
        public bool? Deleted { get; set; }

        /// <summary>
        /// Gets or sets the user ID who deleted this record.
        /// </summary>
        public long? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the deletion date.
        /// </summary>
        public DateTime? DeletedDate { get; set; }
    }
}

