using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for email communication service operations.
    /// </summary>
    public interface IEmailCommunicationService : IGenericService<EmailCommunicationDto, EmailCommunication>
    {
        /// <summary>
        /// Retrieves an email communication by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The email communication DTO if found; otherwise, null.</returns>
        Task<EmailCommunicationDto?> GetByIdAsync(int id);
    }
}

