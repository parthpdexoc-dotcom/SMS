using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RequesterAuthorized operations.
    /// </summary>
    public class RequesterAuthorizedDto : BaseDto
    {
        public Guid? RequesterAuthorizedID { get; set; }

        public Guid? RequesterID { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PrimaryPhone
        {
            get => _primaryPhone;
            set => _primaryPhone = SMS.Generic.Common.Helpers.PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        public string? SecondaryPhone
        {
            get => _secondaryPhone;
            set => _secondaryPhone = SMS.Generic.Common.Helpers.PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        public string? PrimaryEmail { get; set; }

        public string? SecondaryEmail { get; set; }

        public bool? Status { get; set; }

        private string? _primaryPhone;
        private string? _secondaryPhone;
    }
}


