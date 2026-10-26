using System.Linq.Expressions;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Entities.Entities.Audit;

namespace SMS.Generic.Services.Interfaces.Audit;

/// <summary>Read-only access for persisted <see cref="AuditBase"/> rows of type <typeparamref name="TAudit"/>.</summary>
public interface IGenericAuditReadService<TAudit> where TAudit : AuditBase
{
    Task<IEnumerable<TAudit>> GetAllAsync(
        Expression<Func<TAudit, bool>>? filter = null,
        Expression<Func<TAudit, object>>[]? includes = null,
        PageParam? pageParam = null);

    Task<int> GetFilteredCountAsync(
        Expression<Func<TAudit, bool>>? predicate = null,
        Expression<Func<TAudit, object>>[]? includes = null);
}
