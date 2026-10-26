using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for DepartmentEquipment.
    /// </summary>
    public class DepartmentEquipmentRepository : GenericRepository<DepartmentEquipment, SMSDbContext>, IDepartmentEquipmentRepository
    {
        private readonly ILogger _logger;
        private readonly SMSDbContext _context;
        public DepartmentEquipmentRepository(SMSDbContext context, ILogger<DepartmentEquipmentRepository> logger)
            : base(context, logger)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<List<string>> GetVehicleNamesByDepartmentEquipmentIdsAsync(IEnumerable<Guid> departmentEquipmentIds)
        {
            return await _context.DepartmentEquipments
                .AsSplitQuery()
                .Where(de =>
                    departmentEquipmentIds.Contains(de.DepartmentEquipmentID) &&
                    de.RequiresUtilizationRecorded != true &&
                    de.EquipmentType != null &&
                    de.EquipmentType.IsVehicle == true)
                .Select(de => de.EquipmentType!.EquipementTypeShort!)
                .Distinct()
                .ToListAsync();
        }
    }
}


