using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <inheritdoc />
    public class ViewDetailsEquipmentRepository : IViewDetailsEquipmentRepository
    {
        private readonly SMSDbContext _context;
        private readonly ILogger<ViewDetailsEquipmentRepository> _logger;

        public ViewDetailsEquipmentRepository(SMSDbContext context, ILogger<ViewDetailsEquipmentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ViewDetailsEquipmentQueryResult> LoadAsync(
            Guid requestId,
            Guid? postId,
            DateOnly? dateOfWork,
            bool loadRankContext,
            CancellationToken cancellationToken = default)
        {
            if (requestId == Guid.Empty)
                return new ViewDetailsEquipmentQueryResult();

            cancellationToken.ThrowIfCancellationRequested();

            if (!dateOfWork.HasValue)
            {
                return new ViewDetailsEquipmentQueryResult
                {
                    RequestEquipment = new List<RequestEquipment>(),
                    PostEquipment = new List<PostEquipment>()
                };
            }

            var workDate = dateOfWork.Value;

            var postDepartmentEquipmentIdsByPostId = await LoadPostDepartmentEquipmentIdsByPostIdAsync(
                requestId,
                cancellationToken).ConfigureAwait(false);

            var requestEquipment = new List<RequestEquipment>();

            var postEquipmentQuery = _context.PostEquipment
                .AsNoTracking()
                .AsSplitQuery()
                .Include(pe => pe.DepartmentEquipment!)
                .ThenInclude(de => de.EquipmentType)
                .Where(pe =>
                    _context.Post.Any(p => p.PostID == pe.PostID && p.RequestID == requestId)
                    && pe.StartDate <= workDate
                    && pe.EndDate >= workDate);

            if (postId is Guid postFilterId && postFilterId != Guid.Empty)
                postEquipmentQuery = postEquipmentQuery.Where(pe => pe.PostID == postFilterId);

            var postEquipment = await postEquipmentQuery.ToListAsync(cancellationToken).ConfigureAwait(false);

            if (!loadRankContext)
            {
                return new ViewDetailsEquipmentQueryResult
                {
                    RequestEquipment = requestEquipment,
                    PostEquipment = postEquipment,
                    PostDepartmentEquipmentIdsByPostId = postDepartmentEquipmentIdsByPostId
                };
            }

            var positionQuery = _context.PostPosition
                .AsNoTracking()
                .AsSplitQuery()
                .Where(pp =>
                    pp.Post != null
                    && pp.Post.RequestID == requestId
                    && !pp.IsPlaceholder
                    && pp.EmployeeID != null
                    && pp.EmployeeID != Guid.Empty
                    // Load Cancelled/Declined for history View Details reconstruction.
                    // Live ranking still excludes them via IsFilledCurrentAssignment;
                    // history path includes them only when historicalViewerStatus matches.
                    && pp.PostPositionStatus != PostPositionStatus.Transferred);

            positionQuery = positionQuery.Where(pp => pp.DateOfWork == workDate);

            var requestPositions = await positionQuery
                .Include(pp => pp.Employee!)
                .ThenInclude(e => e.EmployeeType)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var employeeIds = requestPositions
                .Select(pp => pp.EmployeeID!.Value)
                .Distinct()
                .ToList();

            var tenures = employeeIds.Count == 0
                ? new List<EmployeeTenure>()
                : await _context.EmployeeTenure
                    .AsNoTracking()
                    .Where(t => employeeIds.Contains(t.EmployeeID))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

            return new ViewDetailsEquipmentQueryResult
            {
                RequestEquipment = requestEquipment,
                PostEquipment = postEquipment,
                RequestPositions = requestPositions,
                EmployeeTenures = tenures,
                PostDepartmentEquipmentIdsByPostId = postDepartmentEquipmentIdsByPostId
            };
        }

        private async Task<Dictionary<Guid, HashSet<Guid>>> LoadPostDepartmentEquipmentIdsByPostIdAsync(
            Guid requestId,
            CancellationToken cancellationToken)
        {
            var rows = await _context.PostEquipment
                .AsNoTracking()
                .Where(pe => _context.Post.Any(p => p.PostID == pe.PostID && p.RequestID == requestId))
                .Select(pe => new { pe.PostID, pe.DepartmentEquipmentID })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return rows
                .GroupBy(x => x.PostID)
                .ToDictionary(g => g.Key, g => g.Select(x => x.DepartmentEquipmentID).ToHashSet());
        }

        /// <inheritdoc />
        public async Task<List<EmployeeTenure>> GetTenuresForEmployeesAsync(
            IEnumerable<Guid> employeeIds,
            CancellationToken cancellationToken = default)
        {
            var ids = employeeIds.Where(id => id != Guid.Empty).Distinct().ToList();
            if (ids.Count == 0)
                return new List<EmployeeTenure>();

            cancellationToken.ThrowIfCancellationRequested();
            return await _context.EmployeeTenure
                .AsNoTracking()
                .Where(t => ids.Contains(t.EmployeeID))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
