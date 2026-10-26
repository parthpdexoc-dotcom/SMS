using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces.Audit;

namespace SMS.Generic.Services.Services.Audit;

public sealed class DepartmentActivityTypeAuditReadService : GenericAuditReadService<DepartmentActivityTypeAudit>, IDepartmentActivityTypeAuditReadService
{
    public DepartmentActivityTypeAuditReadService(IDepartmentActivityTypeAuditRepository repository) : base(repository) { }
}
