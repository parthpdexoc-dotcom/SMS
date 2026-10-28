// using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Admin operations. //testing git scenario 28 October 
    /// </summary>
    public class AdminDto : BaseDto
    {
        /// <summary>
        /// Copies all admin fields from <paramref name="source"/> (including password hash) for PATCH workflows.
        /// </summary>
        /// <param name="source">The admin DTO to copy from.</param>
        public void CloneAdmin(AdminDto source)
        {

            //adding some commets to check git scenario 
            if (source == null) return;

            AdminID = source.AdminID;
            AdminTypeID = source.AdminTypeID;
            FirstName = source.FirstName;
            LastName = source.LastName;
            UserName = source.UserName;
            Password = source.Password;
            Status = source.Status;
        }

        /// <summary>
        /// Primary Key - Admin ID (GUID)
        /// </summary>
        public Guid AdminID { get; set; }

        /// <summary>
        /// Admin Type foreign key
        /// </summary>
        public Guid AdminTypeID { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password (BCrypt hashed)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Status of the admin
        /// </summary>
        public bool Status { get; set; }
    }
}
