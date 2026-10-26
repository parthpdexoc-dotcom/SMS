using SMS.Generic.Dtos.Interfaces;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for DepartmentAdmin operations.
    /// </summary>
    public class DepartmentAdminDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the department admin.
        /// Optional: If not provided or empty, a new record will be created.
        /// If provided, the record will be updated if it exists, otherwise returns not found.
        /// </summary>
        public Guid? DepartmentAdminID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier. Can be sent in the payload; when omitted or empty, it is taken from the token.
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the admin type identifier.
        /// </summary>
        public Guid AdminTypeID { get; set; }
    }
}
