using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Invoice-specific repository implementation.
    /// </summary>
    public class InvoiceRepository : GenericRepository<Invoice, SMSDbContext>, IInvoiceRepository
    {
        private readonly SMSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public InvoiceRepository(
            SMSDbContext context,
            ILogger<InvoiceRepository> logger) : base(context, logger)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Invoice>> GetByProjectNumberAsync(
            string projectNumber,
            Guid? departmentId,
            CancellationToken cancellationToken = default)
        {
            var requestQuery = _context.Requests.AsNoTracking()
                .Where(r => !r.IsDeleted && r.ProjectNumber == projectNumber);

            if (departmentId.HasValue && departmentId.Value != Guid.Empty)
                requestQuery = requestQuery.Where(r => r.DepartmentID == departmentId.Value);

            var requestId = await requestQuery
                .OrderByDescending(r => r.FiscalYear)
                .ThenByDescending(r => r.CreatedDate)
                .Select(r => r.RequestID)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (requestId == Guid.Empty)
                return Array.Empty<Invoice>();

            var invoiceQuery = _context.Invoices.AsNoTracking()
                .Where(i => !i.IsDeleted && i.RequestId == requestId);

            if (departmentId.HasValue && departmentId.Value != Guid.Empty)
                invoiceQuery = invoiceQuery.Where(i => i.DepartmentID == departmentId.Value);

            return await invoiceQuery
                .Include(i => i.InvoiceDetails!)
                .Include(i => i.Request!)
                .Include(i => i.Requester!)
                .OrderByDescending(i => i.DateCreated)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<(IReadOnlyList<RequesterInvoiceCountDto> Data, long TotalRecords)> GetRequesterInvoiceCountsAsync(
            Guid? departmentId,
            Guid? requesterId,
            bool? isPaid,
            string? searchTerm,
            int? dueDateRangeDays,
            DateTime departmentToday,
            PageParam pageParam,
            CancellationToken cancellationToken = default)
        {
            pageParam ??= new PageParam();
            searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();

            var page = pageParam.GetPage() ?? PageParam.MinPage;
            var pageSize = pageParam.GetPageSize() ?? pageParam.GetLimit() ?? PageParam.MinLimit;
            if (page < 1) page = PageParam.MinPage;
            if (pageSize < 1) pageSize = PageParam.MinLimit;

            var filtered =
                from i in _context.Invoices.AsNoTracking()
                join r in _context.Requester.AsNoTracking() on i.RequesterID equals r.RequesterID
                where !i.IsDeleted
                    && i.RequesterID.HasValue
                    && i.RequesterID.Value != Guid.Empty
                select new { Invoice = i, Requester = r };

            if (departmentId.HasValue && departmentId.Value != Guid.Empty)
                filtered = filtered.Where(x => x.Invoice.DepartmentID == departmentId.Value);

            if (requesterId.HasValue && requesterId.Value != Guid.Empty)
                filtered = filtered.Where(x => x.Invoice.RequesterID == requesterId.Value);

            if (isPaid.HasValue)
                filtered = filtered.Where(x => x.Invoice.IsPaid == isPaid.Value);

            if (searchTerm != null)
                filtered = filtered.Where(x =>
                    (x.Requester.Name != null && x.Requester.Name.Contains(searchTerm))
                    || (x.Invoice.InvoiceNo != null && x.Invoice.InvoiceNo.Contains(searchTerm)));

            if (dueDateRangeDays is 30 or 60 or 90)
            {
                var today = departmentToday.Date;
                var minDueDate = today.AddDays(-dueDateRangeDays.Value);
                filtered = filtered.Where(x =>
                    x.Invoice.DueDate.HasValue
                    && x.Invoice.DueDate.Value.Date >= minDueDate
                    && x.Invoice.DueDate.Value.Date <= today);
            }

            var grouped = filtered
                .GroupBy(x => new { RequesterId = x.Requester.RequesterID, RequesterName = x.Requester.Name })
                .Select(g => new RequesterInvoiceCountDto
                {
                    RequesterId = g.Key.RequesterId,
                    RequesterName = g.Key.RequesterName ?? string.Empty,
                    InvoicesCount = g.Count()
                });

            var totalRecords = await grouped
                .LongCountAsync(cancellationToken)
                .ConfigureAwait(false);

            var data = await grouped
                .OrderBy(x => x.RequesterName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return (data, totalRecords);
        }

        /// <inheritdoc />
        public async Task<IReadOnlySet<Guid>> GetRequesterIdsWithUnpaidInvoicesDueTillDateAsync(
            IReadOnlyCollection<Guid> requesterIds,
            Guid? departmentId,
            DateTime departmentToday,
            CancellationToken cancellationToken = default)
        {
            var today = departmentToday.Date;
            var query = _context.Invoices.AsNoTracking()
                .Where(i => !i.IsDeleted
                    && !i.IsPaid
                    && i.RequesterID.HasValue
                    && requesterIds.Contains(i.RequesterID.Value)
                    && i.DueDate.HasValue
                    && i.DueDate.Value.Date <= today);

            if (departmentId.HasValue && departmentId.Value != Guid.Empty)
                query = query.Where(i => i.DepartmentID == departmentId.Value);

            var ids = await query
                .Select(i => i.RequesterID!.Value)
                .Distinct()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return ids.ToHashSet();
        }

        /// <inheritdoc />
        public async Task<string> GetNextInvoiceNumberForPrefixAsync(
            string prefix,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return prefix;

            var query = _context.Invoices
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(i => i.InvoiceNo != null && i.InvoiceNo.StartsWith(prefix));

            if (departmentId.HasValue && departmentId.Value != Guid.Empty)
                query = query.Where(i => i.DepartmentID == departmentId);

            var existing = await query
                .Select(i => i.InvoiceNo)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return InvoiceNumberFormat.FormatNext(prefix, existing);
        }
    }
}

