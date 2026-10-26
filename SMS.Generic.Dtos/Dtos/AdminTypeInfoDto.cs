using System;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for admin type information used in login operations.
    /// </summary>
    public class AdminTypeInfoDto
    {
        /// <summary>
        /// Gets or sets the admin type identifier.
        /// </summary>
        public Guid AdminTypeID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the admin type.
        /// </summary>
        public string AdminTypeShort { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the long name or description of the admin type.
        /// </summary>
        public string AdminTypeLong { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether this admin type can manage posts.
        /// </summary>
        public bool IsManagePost { get; set; }
    }
}

