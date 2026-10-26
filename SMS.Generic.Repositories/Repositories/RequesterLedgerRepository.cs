using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    public class RequesterLedgerRepository : GenericRepository<RequesterLedger, SMSDbContext>, IRequesterLedgerRepository
    {
        private readonly SMSDbContext _context;

        public RequesterLedgerRepository(
            SMSDbContext context,
            ILogger<RequesterLedgerRepository> logger) : base(context, logger)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<RequesterLedger?> GetLatestLedgerEntryByRequesterIdAsync(Guid requesterId,DateTime? beforeDateTime = null,CancellationToken cancellationToken = default)
        {
            if (requesterId == Guid.Empty)
                return null;

            var query = _context.RequesterLedgers
                .AsNoTracking()
                .Include(l => l.Requester)
                .Where(l => l.RequesterID == requesterId && !l.IsDeleted);

            if (beforeDateTime.HasValue)
                query = query.Where(l => l.TransactionDateTime < beforeDateTime.Value);

            return await query
                .OrderByDescending(l => l.TransactionDateTime)
                .ThenByDescending(l => l.CreatedDate)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RequesterLedger>> GetLedgerEntriesForStatementAsync(Guid requesterId,DateTime? startDateTime,DateTime? endDateTime,CancellationToken cancellationToken = default)
        {
            if (requesterId == Guid.Empty)
                return Array.Empty<RequesterLedger>();

            var query = _context.RequesterLedgers
                .AsNoTracking()
                .Include(l => l.Requester)
                .Where(l => l.RequesterID == requesterId && !l.IsDeleted);

            if (endDateTime.HasValue)
                query = query.Where(l => l.TransactionDateTime <= endDateTime.Value);

            if (startDateTime.HasValue)
            {
                var periodStart = startDateTime.Value;
                query = query.Where(l =>
                    l.TransactionDateTime >= periodStart
                    || l.RequesterLedgerID == _context.RequesterLedgers
                        .Where(x => x.RequesterID == requesterId && !x.IsDeleted && x.TransactionDateTime < periodStart)
                        .OrderByDescending(x => x.TransactionDateTime)
                        .ThenByDescending(x => x.CreatedDate)
                        .Select(x => x.RequesterLedgerID)
                        .FirstOrDefault());
            }

            return await query
                .OrderBy(l => l.TransactionDateTime)
                .ThenBy(l => l.CreatedDate)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
