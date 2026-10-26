using System.ComponentModel.DataAnnotations;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Models;
namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Base Data Transfer Object with common audit fields.
    /// </summary>
    public class BaseDto : BaseModel
    {
        /// <summary>
        /// Record added timestamp.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// User who added record.
        /// </summary>
        public Guid? CreatedUserID { get; set; }

        /// <summary>
        /// Name of user who added record.
        /// </summary>
        [MaxLength(255)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Edit timestamp.
        /// </summary>
        public DateTime? EditedDate { get; set; }

        /// <summary>
        /// User who edited record.
        /// </summary>
        public Guid? EditedUserID { get; set; }

        /// <summary>
        /// Name of user editing record.
        /// </summary>
        [MaxLength(255)]
        public string? EditedBy { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Guid? DeletedUserID { get; set; }

        public string? DeletedBy { get; set; }

        public string? Message { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Message);

        /// <summary>Display name as <c>LastName, FirstName</c>; omits empty parts.</summary>
        public static string? FormatDisplayName(string? lastName, string? firstName) =>
            PersonNameFormatHelper.FormatLastNameCommaFirstName(lastName, firstName);

        /// <summary>
        /// Sums a decimal value selected from each item. Returns 0 when <paramref name="items"/> is null.
        /// </summary>
        public static decimal Sum<T>(IEnumerable<T>? items, Func<T, decimal> amountSelector) => 
            items?.Sum(amountSelector) ?? 0m;
    }
}

