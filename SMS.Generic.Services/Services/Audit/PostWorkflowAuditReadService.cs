using AutoMapper;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services.Audit;

public sealed class PostWorkflowAuditReadService : IPostWorkflowAuditReadService
{
    private readonly IPostWorkflowAuditRepository _repository;
    private readonly IMapper _mapper;
    private readonly ITokenParser _tokenParser;

    public PostWorkflowAuditReadService(
        IPostWorkflowAuditRepository repository,
        IMapper mapper,
        ITokenParser tokenParser)
    {
        _repository = repository;
        _mapper = mapper;
        _tokenParser = tokenParser;
    }

    /// <inheritdoc />
    public async Task<PagedList<PostWorkflowAuditDto>> GetPagedByEmployeeIdAsync(
        PageParam? pageParam,
        PostWorkflowAttemptStatus? status = null,
        Guid? employeeId = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        employeeId ??= _tokenParser.GetEmployeeID();
        if (!employeeId.HasValue || employeeId.Value == Guid.Empty)
            throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.VALID_EMPLOYEE_ID_REQUIRED));

        pageParam ??= new PageParam();
        if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            pageParam.SortBy = nameof(PostWorkflowAudit.RecordedDate);
        if (string.IsNullOrWhiteSpace(pageParam.SortDirection))
            pageParam.SortDirection = "desc";

        var rows = await _repository.GetAllAsync(
            x => x.EmployeeId == employeeId.Value && (!status.HasValue || x.Status == status.Value),
            null,
            pageParam).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        var total = await _repository
            .GetFilteredCountAsync(x => x.EmployeeId == employeeId.Value && (!status.HasValue || x.Status == status.Value))
            .ConfigureAwait(false);
        var dtos = _mapper.Map<List<PostWorkflowAuditDto>>(rows);
        return new PagedList<PostWorkflowAuditDto>(dtos, pageParam, total);
    }
}
