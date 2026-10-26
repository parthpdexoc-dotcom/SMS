using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for email process service operations.
    /// </summary>
    public interface IEmailProcessService : IGenericService<EmailProcessDto, EmailProcess>
    {
        /// <summary>
        /// Retrieves an email process by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The email process DTO if found; otherwise, null.</returns>
        Task<EmailProcessDto?> GetByIdAsync(int id);
    }
}

