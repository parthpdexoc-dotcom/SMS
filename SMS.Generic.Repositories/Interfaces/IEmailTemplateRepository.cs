using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for email template repository operations, extending IGenericRepository.
    /// </summary>
    public interface IEmailTemplateRepository : IGenericRepository<EmailTemplate>
    {
        /// <summary>
        /// Gets the first email template matching the given email type.
        /// </summary>
        /// <param name="emailType">The email type to find.</param>
        /// <returns>The email template if found; otherwise, null.</returns>
        Task<EmailTemplate?> GetByEmailTypeAsync(EmailType emailType);
    }
}
