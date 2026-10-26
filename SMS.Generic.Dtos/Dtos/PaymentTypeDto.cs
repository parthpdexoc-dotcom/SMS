using SMS.Generic.Dtos.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for PaymentType operations.
    /// </summary>
    public class PaymentTypeDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Primary Key - Payment Type ID (GUID)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("paymentTypeId")]
        [System.Text.Json.Serialization.JsonPropertyName("paymentTypeId")]
        public Guid? PaymentTypeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (auto-populated from token, not required in request).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Payment type name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Whether this payment type is the default for the department.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Returns true when another default payment type already exists in the department.
        /// </summary>
        public bool IsDefaultAlreadyExist(IEnumerable<PaymentTypeDto> departmentPaymentTypes, out string? existingDefaultName)
        {
            existingDefaultName = null;
            if (!IsDefault)
                return false;

            var currentId = PaymentTypeID.GetValueOrDefault(Guid.Empty);
            var existingDefault = departmentPaymentTypes
                .FirstOrDefault(x => x.IsDefault && x.PaymentTypeID.GetValueOrDefault(Guid.Empty) != currentId);

            if (existingDefault == null)
                return false;

            existingDefaultName = existingDefault.Name;
            return true;
        }
    }
}
