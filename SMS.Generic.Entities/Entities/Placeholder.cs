using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    [AuditedEntity(DisplayPropertyName = nameof(FullName), OnlyAttributedProperties = true)]
    [Table("Placeholder", Schema = "dbo")]
    public class Placeholder : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PlaceholderID { get; set; }

        [MaxLength(100)]
        [AuditedProperty("First name")]
        public string FirstName { get; set; } = null!;

        [MaxLength(100)]
        [AuditedProperty("Last name")]
        public string LastName { get; set; } = null!;

        /// <summary>Audit/display label: <c>LastName, FirstName</c>.</summary>
        [NotMapped]
        public string FullName =>
            PersonNameFormatHelper.FormatLastNameCommaFirstName(LastName, FirstName) ?? string.Empty;

        [MaxLength(50)]
        [AuditedProperty("Contact number")]
        public string? ContactNumber { get; set; }

        /// <summary>
        /// Badge suffix only (up to 5 digits). Payroll export applies the <c>9900</c> prefix.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Employee number")]
        public string? EmployeeNumber { get; set; }

        [Required]
        public Guid DepartmentID { get; set; }

        [AuditedProperty("Status")]
        public PlaceholderStatus PlaceholderStatus { get; set; } = PlaceholderStatus.Visible;

        /// <summary>
        /// When true, assigning this placeholder to another post creates a new placeholder with the same name.
        /// </summary>
        public bool IsUniquePerPosition { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public Department? Department { get; set; }

        [ForeignKey("CreatedUserID")]
        public Employee? CreatedByEmployee { get; set; }

        [ForeignKey("EditedUserID")]
        public Employee? EditedByEmployee { get; set; }

        public ICollection<PostPosition> PostPositions { get; set; } = new List<PostPosition>();
    }
}
