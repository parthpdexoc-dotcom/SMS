using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    [Table("DepartmentDefinedFields", Schema = "dbo")]
    public class DepartmentDefinedFields
    {
        [Key]
        public Guid DepartmentDefinedFieldID { get; set; }

        [Required]
        public Guid DepartmentID { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public Department Department { get; set; } = null!;

        [MaxLength(100)]
        public string? RequestDepartmentDefinedFieldInterfaceLabel1 { get; set; }

        [MaxLength(100)]
        public string? RequestDepartmentDefinedFieldInterfaceLabel2 { get; set; }

        [MaxLength(100)]
        public string? RequestDepartmentDefinedFieldInterfaceLabel3 { get; set; }

        [MaxLength(100)]
        public string? RequestDepartmentDefinedFieldInterfaceLabel4 { get; set; }

        [MaxLength(100)]
        public string? RequestDepartmentDefinedFieldInterfaceLabel5 { get; set; }

        [MaxLength(100)]
        public string? RequestDepartmentDefinedFieldInterfaceLabel6 { get; set; }

        [MaxLength(100)]
        public string? RequestDepartmentDefinedFieldInterfaceLabel7 { get; set; }

        [MaxLength(100)]
        public string? RequestDepartmentDefinedFieldInterfaceLabel8 { get; set; }

        [MaxLength(100)]
        public string? RequestDepartmentDefinedFieldInterfaceLabel9 { get; set; }

        [MaxLength(100)]
        public string? RequestDepartmentDefinedFieldInterfaceLabel10 { get; set; }


        [MaxLength(100)]
        public string? PostDepartmentDefinedFieldInterfaceLabel1 { get; set; }

        [MaxLength(100)]
        public string? PostDepartmentDefinedFieldInterfaceLabel2 { get; set; }

        [MaxLength(100)]
        public string? PostDepartmentDefinedFieldInterfaceLabel3 { get; set; }

        [MaxLength(100)]
        public string? PostDepartmentDefinedFieldInterfaceLabel4 { get; set; }

        [MaxLength(100)]
        public string? PostDepartmentDefinedFieldInterfaceLabel5 { get; set; }

        [MaxLength(100)]
        public string? PostDepartmentDefinedFieldInterfaceLabel6 { get; set; }

        [MaxLength(100)]
        public string? PostDepartmentDefinedFieldInterfaceLabel7 { get; set; }

        [MaxLength(100)]
        public string? PostDepartmentDefinedFieldInterfaceLabel8 { get; set; }

        [MaxLength(100)]
        public string? PostDepartmentDefinedFieldInterfaceLabel9 { get; set; }

        [MaxLength(100)]
        public string? PostDepartmentDefinedFieldInterfaceLabel10 { get; set; }
    }
}
