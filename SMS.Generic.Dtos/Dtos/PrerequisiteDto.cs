using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Prerequisite operations.
    /// </summary>
    public class PrerequisiteDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the prerequisite.
        /// </summary>
        public int PkPrerequisiteID { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the department.
        /// </summary>
        public Guid? FkDepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the title of the prerequisite.
        /// </summary>
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the prerequisite type.
        /// </summary>
        public int? FkPrerequisiteTypeID { get; set; }

        /// <summary>
        /// Gets or sets the comment for the prerequisite.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created this record.
        /// </summary>
        public long? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when this record was created.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last modified this record.
        /// </summary>
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when this record was last modified.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets whether this record is deleted.
        /// </summary>
        public bool? Deleted { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who deleted this record.
        /// </summary>
        public long? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when this record was deleted.
        /// </summary>
        public DateTime? DeletedDate { get; set; }
    }
}

