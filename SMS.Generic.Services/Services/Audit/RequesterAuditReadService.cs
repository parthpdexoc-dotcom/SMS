using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces.Audit;

namespace SMS.Generic.Services.Services.Audit;

public sealed class RequesterAuditReadService : GenericAuditReadService<RequesterAudit>, IRequesterAuditReadService
{
    public RequesterAuditReadService(IRequesterAuditRepository repository) : base(repository) { }
}
