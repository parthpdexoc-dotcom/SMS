using System;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object containing all admin data needed for JWT token generation during login.
    /// </summary>
    public class AdminLoginInfoDto
    {
        // Admin Info
        /// <summary>
        /// Gets or sets the admin identifier.
        /// </summary>
        public Guid AdminID { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the password (for validation only).
        /// </summary>
        public string? Password { get; set; }

        // AdminType Info
        /// <summary>
        /// Gets or sets the admin type identifier.
        /// </summary>
        public Guid AdminTypeID { get; set; }

        /// <summary>
        /// Gets or sets the admin type short name.
        /// </summary>
        public string? AdminTypeShort { get; set; }

        /// <summary>
        /// Gets or sets the admin type long name.
        /// </summary>
        public string? AdminTypeLong { get; set; }

        /// <summary>
        /// Gets or sets whether the admin's type is a super user (can access AdminType controller etc.).
        /// </summary>
        public bool IsSuperUser { get; set; }

        /// <summary>
        /// Gets or sets the status of the admin.
        /// </summary>
        public bool Status { get; set; }
    }
}

