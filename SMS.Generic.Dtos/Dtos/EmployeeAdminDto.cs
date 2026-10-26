using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmployeeAdmin operations.
    /// </summary>
    public class EmployeeAdminDto : BaseDto
    {
        /// <summary>
        /// Primary Key (GUID)
        /// </summary>
        public Guid EmployeeAdminID { get; set; }

        /// <summary>
        /// Foreign Key to AdminType
        /// </summary>
        public Guid? AdminTypeID { get; set; }

        /// <summary>
        /// Foreign Key to Employee
        /// </summary>
        public Guid? EmployeeID { get; set; }
    }
}
