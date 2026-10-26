using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a requester location entity in the system.
    /// </summary>
    [Table("RequesterLocations", Schema = "dbo")]
    public class RequesterLocations
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester location.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PkLocationID { get; set; }

        /// <summary>
        /// Gets or sets the foreign key department ID.
        /// </summary>
        public int? FkDepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the foreign key requester ID (references Requesters table).
        /// </summary>
        public Guid? FKRequesterID { get; set; }

        /// <summary>
        /// Navigation property for the requester.
        /// </summary>
        [ForeignKey(nameof(FKRequesterID))]
        public virtual Requester? Requester { get; set; }

        /// <summary>
        /// Gets or sets the department defined ID.
        /// </summary>
        [MaxLength(50)]
        public string? DepartmentDefinedID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [MaxLength(255)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the foreign key response area ID.
        /// </summary>
        public int? FkDepartmentResponseAreaID { get; set; }

        /// <summary>
        /// Gets or sets the first address line.
        /// </summary>
        [Column(TypeName = "text")]
        public string? Address1 { get; set; }

        /// <summary>
        /// Gets or sets the second address line.
        /// </summary>
        [Column(TypeName = "text")]
        public string? Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        [MaxLength(50)]
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [MaxLength(20)]
        public string? State { get; set; }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        [MaxLength(20)]
        public string? Zip { get; set; }

        /// <summary>
        /// Gets or sets whether location check list is required.
        /// </summary>
        public bool? IsLocCheckListRequired { get; set; }

        /// <summary>
        /// Gets or sets the location check list IDs.
        /// </summary>
        [MaxLength(100)]
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

