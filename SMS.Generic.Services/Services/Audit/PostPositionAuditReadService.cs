using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces.Audit;

namespace SMS.Generic.Services.Services.Audit;

public sealed class PostPositionAuditReadService : GenericAuditReadService<PostPositionAudit>, IPostPositionAuditReadService
{
    public PostPositionAuditReadService(IPostPositionAuditRepository repository) : base(repository) { }
}
