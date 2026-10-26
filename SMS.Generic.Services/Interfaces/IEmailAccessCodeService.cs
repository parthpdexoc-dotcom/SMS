using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for email access code service operations.
    /// </summary>
    public interface IEmailAccessCodeService : IGenericService<EmailAccessCodeDto, EmailAccessCode>
    {
        /// <summary>
        /// Retrieves an email access code by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The email access code DTO if found; otherwise, null.</returns>
        Task<EmailAccessCodeDto?> GetByIdAsync(int id);
    }
}

