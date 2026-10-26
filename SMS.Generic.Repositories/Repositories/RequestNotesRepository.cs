using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Implementations.Models;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Request-specific repository implementation.
    /// </summary>
    public class RequestNotesRepository : GenericRepository<RequestNotes, SMSDbContext>, IRequestNotesRepository
    {

        private readonly SMSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestNotesRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequestNotesRepository(
            SMSDbContext context,
            ILogger<RequestNotesRepository> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}

