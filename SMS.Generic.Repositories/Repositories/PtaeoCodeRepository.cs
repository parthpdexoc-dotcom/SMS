using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for PTAEO_Code entity operations.
    /// </summary>
    public class PtaeoCodeRepository : GenericRepository<PtaeoCode, SMSDbContext>, IPtaeoCodeRepository
    {
        private readonly SMSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PtaeoCodeRepository"/> class.
        /// </summary>
        public PtaeoCodeRepository(
            SMSDbContext context,
            ILogger<PtaeoCodeRepository> logger)
            : base(context, logger)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
        {
            var all = await _context.PtaeoCode.ToListAsync(cancellationToken).ConfigureAwait(false);
            _context.PtaeoCode.RemoveRange(all);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
