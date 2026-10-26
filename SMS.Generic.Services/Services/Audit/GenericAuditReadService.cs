using System.Linq.Expressions;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces.Audit;

namespace SMS.Generic.Services.Services.Audit;

/// <summary>Default read-only audit access: delegates to <see cref="IGenericRepository{TAudit}"/>.</summary>
public class GenericAuditReadService<TAudit> : IGenericAuditReadService<TAudit> where TAudit : AuditBase
{
    private readonly IGenericRepository<TAudit> _repository;

    public GenericAuditReadService(IGenericRepository<TAudit> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public Task<IEnumerable<TAudit>> GetAllAsync(
        Expression<Func<TAudit, bool>>? filter = null,
        Expression<Func<TAudit, object>>[]? includes = null,
        PageParam? pageParam = null) =>
        _repository.GetAllAsync(filter, includes, pageParam);

    /// <inheritdoc />
    public Task<int> GetFilteredCountAsync(
        Expression<Func<TAudit, bool>>? predicate = null,
        Expression<Func<TAudit, object>>[]? includes = null) =>
        _repository.GetFilteredCountAsync(predicate, includes);
}
