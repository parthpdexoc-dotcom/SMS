using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Post position vehicle-specific repository implementation.
    /// </summary>
    public class PostPositionVehicleRepository : GenericRepository<PostPositionVehicle, SMSDbContext>, IPostPositionVehicleRepository
    {
        private readonly SMSDbContext _context;

        public PostPositionVehicleRepository(
            SMSDbContext context,
            ILogger<PostPositionVehicleRepository> logger) : base(context, logger)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<(bool IsVehicle, string Name)>> GetAssignedEquipmentTypeNamesAsync(
            Guid postPositionId,
            CancellationToken cancellationToken = default)
        {
            if (postPositionId == Guid.Empty)
                return Array.Empty<(bool, string)>();

            cancellationToken.ThrowIfCancellationRequested();

            var rows = await (
                    from ppv in _context.PostPositionVehicle.AsNoTracking()
                    join de in _context.DepartmentEquipments.AsNoTracking()
                        on ppv.DepartmentEquipmentID equals de.DepartmentEquipmentID
                    join et in _context.EquipmentTypes.AsNoTracking()
                        on de.EquipmentTypeID equals et.EquipementTypeID
                    where ppv.PostPositionID == postPositionId
                          && !ppv.IsDeleted
                    select new
                    {
                        IsVehicle = et.IsVehicle == true,
                        ShortName = et.EquipementTypeShort,
                        LongName = et.EquipementTypeLong
                    })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return rows
                .Select(row => (row.IsVehicle, Name: (row.ShortName ?? row.LongName ?? string.Empty).Trim()))
                .Where(row => row.Name.Length > 0)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int>>
            GetAllocationCountsByPostAndDateAsync(
                IReadOnlyList<Guid> postIds,
                IReadOnlyList<DateOnly> workDates,
                CancellationToken cancellationToken = default)
        {
            if (postIds.Count == 0 || workDates.Count == 0)
                return new Dictionary<(Guid, DateOnly, Guid), int>();

            cancellationToken.ThrowIfCancellationRequested();

            var rows = await (
                    from ppv in _context.PostPositionVehicle.AsNoTracking()
                    join pp in _context.PostPosition.AsNoTracking()
                        on ppv.PostPositionID equals pp.PostPositionID
                    where !ppv.IsDeleted
                          && ppv.DepartmentEquipmentID != Guid.Empty
                          && pp.DateOfWork.HasValue
                          && postIds.Contains(pp.PostID)
                          && workDates.Contains(pp.DateOfWork.Value)
                          && pp.PostPositionStatus != PostPositionStatus.Transferred
                          && pp.PostPositionStatus != PostPositionStatus.ClosedAndSplit
                          && pp.PostPositionStatus != PostPositionStatus.Cancelled
                          && pp.PostPositionStatus != PostPositionStatus.Declined
                    group ppv by new
                    {
                        pp.PostID,
                        DateOfWork = pp.DateOfWork!.Value,
                        ppv.DepartmentEquipmentID
                    }
                    into grouped
                    select new
                    {
                        grouped.Key.PostID,
                        grouped.Key.DateOfWork,
                        grouped.Key.DepartmentEquipmentID,
                        Count = grouped.Count()
                    })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return rows.ToDictionary(
                row => (row.PostID, row.DateOfWork, row.DepartmentEquipmentID),
                row => row.Count);
        }
    }
}
