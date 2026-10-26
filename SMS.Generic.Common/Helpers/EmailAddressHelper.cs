using System.Text.RegularExpressions;

namespace SMS.Generic.Common.Helpers
{
    /// <summary>
    /// Email format validation shared by FluentValidation rules and SMTP sending.
    /// </summary>
    public static class EmailAddressHelper
    {

        private const string DomainLabel = @"[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?";
        private const string LocalPart = @"[A-Za-z0-9](?:[A-Za-z0-9._%+-]*[A-Za-z0-9])?";

        private static readonly Regex EmailPattern = new(
            $@"^(?!.*\.\.)(?!\.)(?!.*\.$)(?!.*@\.)(?!.*@-){LocalPart}@(?:{DomainLabel}\.)+[A-Za-z]{{2,}}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        /// <summary>
        /// Returns true when <paramref name="email"/> is a non-empty, well-formed address.
        /// Allows apostrophe in the local part (e.g. Ryan.O'Shaughnessy@stpete.org).
        /// Rejects + and -, consecutive dots, dot before @, or dot after @ (e.g. Jennifer.@test.com).
        /// </summary>
        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailPattern.IsMatch(email.Trim());
        }

        /// <summary>
        /// For optional FluentValidation fields: null/whitespace passes so <c>NotEmpty</c> can enforce requiredness.
        /// </summary>
        public static bool IsValidEmailAddress(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true;

            return IsValidEmail(email);
        }
    }
}
