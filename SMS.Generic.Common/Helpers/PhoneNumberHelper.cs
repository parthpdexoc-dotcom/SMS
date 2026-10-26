using System;
using System.Linq;

namespace SMS.Generic.Common.Helpers
{
    /// <summary>
    /// Helper methods for normalizing phone numbers.
    /// </summary>
    public static class PhoneNumberHelper
    {
        /// <summary>
        /// Returns true when the input has exactly 10 digits (after stripping non-digits),
        /// or when the input is null/whitespace (so other NotEmpty rules can handle requiredness).
        /// </summary>
        public static bool HasValidPhoneDigits(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true;

            var digits = new string(phone.Where(char.IsDigit).ToArray());
            return digits.Length == 10;
        }

        /// <summary>
        /// Normalizes a phone number to XXXXXXXXXX format (10 digits, no dashes) for database storage.
        /// Non-digit characters (e.g. spaces, dashes, parentheses) are stripped before normalization.
        /// If the user provides the number in another format (e.g. XXX-XXX-XXXX or (XXX) XXX-XXXX),
        /// it is converted to the normalized format before saving.
        /// If the cleaned value does not contain exactly 10 digits, the original trimmed input is returned.
        /// </summary>
        /// <param name="phone">The input phone number (any format).</param>
        /// <returns>The normalized 10-digit phone number (XXXXXXXXXX), or the original value if it cannot be normalized.</returns>
        public static string? PhoneNumberFormatHelper(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            var trimmed = phone.Trim();

            // Extract digits only (strips dashes, spaces, parentheses, etc.).
            var digits = new string(trimmed.Where(char.IsDigit).ToArray());

            if (digits.Length != 10)
                return trimmed;

            return digits;
        }

        /// <summary>
        /// True when the value is empty, or E.164: a leading + and 8 to 15 digits.
        /// The country code stays inside the number. This does not split it out.
        /// </summary>
        public static bool HasValidE164(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true;

            var digits = Digits(phone);
            return phone.Trim().StartsWith('+') && digits.Length >= 8 && digits.Length <= 15;
        }

        /// <summary>
        /// Stores an SMS number as + and digits, for example +916355367556.
        /// Spaces and dashes are removed. A value that is not E.164 is returned trimmed so validation can reject it.
        /// </summary>
        public static string? NormalizeE164(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            var trimmed = phone.Trim();
            if (!HasValidE164(trimmed))
                return trimmed;

            return "+" + Digits(trimmed);
        }

        private static string Digits(string phone) =>
            new(phone.Where(char.IsDigit).ToArray());
    }
}

