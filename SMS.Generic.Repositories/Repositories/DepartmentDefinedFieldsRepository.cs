using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for DepartmentEquipment.
    /// </summary>
    public class DepartmentDefinedFieldsRepository : GenericRepository<DepartmentDefinedFields, SMSDbContext>, IDepartmentDefinedFieldsRepository
    {
        public DepartmentDefinedFieldsRepository(SMSDbContext context, ILogger<DepartmentDefinedFieldsRepository> logger)
            : base(context, logger)
        {
        }
    }
}


