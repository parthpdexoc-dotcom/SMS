using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces.Audit;

namespace SMS.Generic.Services.Services.Audit;

/// <summary>
/// Resolves the typed audit read service for a <see cref="AuditHistoryTable"/> lazily (only the service for the chosen table is created in this scope).
/// </summary>
public sealed class AuditHistoryReadServiceResolver : IAuditHistoryReadServiceResolver
{
    private readonly IServiceProvider _services;

    public AuditHistoryReadServiceResolver(IServiceProvider services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc />
    public Task<PagedList<AuditBase>> GetPagedAsync(
        AuditHistoryTable table,
        string? entityId,
        Guid departmentId,
        Guid employeeId,
        PageParam pageParam,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken) =>
        table switch
        {
            AuditHistoryTable.Requester => GetPagedAsync(_services.GetRequiredService<IRequesterAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.Post => GetPagedAsync(_services.GetRequiredService<IPostAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.Request => GetPagedAsync(_services.GetRequiredService<IRequestAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.PostPosition => GetPagedAsync(_services.GetRequiredService<IPostPositionAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.Employee => GetPagedAsync(_services.GetRequiredService<IEmployeeAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.EmployeeType => GetPagedAsync(_services.GetRequiredService<IEmployeeTypeAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.Skill => GetPagedAsync(_services.GetRequiredService<ISkillAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.BusinessType => GetPagedAsync(_services.GetRequiredService<IBusinessTypeAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.EmployeeStatus => GetPagedAsync(_services.GetRequiredService<IEmployeeStatusAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.DepartmentResponseArea => GetPagedAsync(_services.GetRequiredService<IDepartmentResponseAreaAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.ClosedPostPosition => GetPagedAsync(_services.GetRequiredService<IClosedPostPositionAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.ClosedPostPositionEquipment => GetPagedAsync(_services.GetRequiredService<IClosedPostPositionEquipmentAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.RequestSchedule => GetPagedAsync(_services.GetRequiredService<IRequestScheduleAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.RoutingGroup => GetPagedAsync(_services.GetRequiredService<IRoutingGroupAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.HolidayDay => GetPagedAsync(_services.GetRequiredService<IHolidayDayAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.DepartmentDivisions => GetPagedAsync(_services.GetRequiredService<IDepartmentDivisionsAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.DepartmentActivityType => GetPagedAsync(_services.GetRequiredService<IDepartmentActivityTypeAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.EquipmentType => GetPagedAsync(_services.GetRequiredService<IEquipmentTypeAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.DepartmentEquipment => GetPagedAsync(_services.GetRequiredService<IDepartmentEquipmentAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.DepartmentFee => GetPagedAsync(_services.GetRequiredService<IDepartmentFeeAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.PayCode => GetPagedAsync(_services.GetRequiredService<IPayCodeAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.PaymentType => GetPagedAsync(_services.GetRequiredService<IPaymentTypeAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.Payment => GetPagedAsync(_services.GetRequiredService<IPaymentAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.PaymentAllocation => GetPagedAsync(_services.GetRequiredService<IPaymentAllocationAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.RequesterLedger => GetPagedAsync(_services.GetRequiredService<IRequesterLedgerAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.Invoice => GetPagedAsync(_services.GetRequiredService<IInvoiceAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.InvoiceDetail => GetPagedAsync(_services.GetRequiredService<IInvoiceDetailAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.Placeholder => GetPagedAsync(_services.GetRequiredService<IPlaceholderAuditReadService>(), entityId, departmentId, employeeId, pageParam, startDate, endDate, cancellationToken),
            AuditHistoryTable.EmailTemplate => GetEmailTemplatePageAsync(_services.GetRequiredService<IEmailTemplateAuditReadService>(), entityId, employeeId, pageParam, startDate, endDate, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(table), table, null)
        };

    /// <inheritdoc />
    public Task<PagedList<AuditBase>> SearchInTableAsync(
        AuditHistoryTable table,
        Expression<Func<AuditBase, bool>> searchFilter,
        PageParam pageParam,
        CancellationToken cancellationToken) =>
        table switch
        {
            AuditHistoryTable.Requester => SearchInRepository(_services.GetRequiredService<IRequesterAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.Post => SearchInRepository(_services.GetRequiredService<IPostAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.Request => SearchInRepository(_services.GetRequiredService<IRequestAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.PostPosition => SearchInRepository(_services.GetRequiredService<IPostPositionAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.Employee => SearchInRepository(_services.GetRequiredService<IEmployeeAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.EmployeeType => SearchInRepository(_services.GetRequiredService<IEmployeeTypeAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.Skill => SearchInRepository(_services.GetRequiredService<ISkillAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.BusinessType => SearchInRepository(_services.GetRequiredService<IBusinessTypeAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.EmployeeStatus => SearchInRepository(_services.GetRequiredService<IEmployeeStatusAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.DepartmentResponseArea => SearchInRepository(_services.GetRequiredService<IDepartmentResponseAreaAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.ClosedPostPosition => SearchInRepository(_services.GetRequiredService<IClosedPostPositionAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.ClosedPostPositionEquipment => SearchInRepository(_services.GetRequiredService<IClosedPostPositionEquipmentAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.RequestSchedule => SearchInRepository(_services.GetRequiredService<IRequestScheduleAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.RoutingGroup => SearchInRepository(_services.GetRequiredService<IRoutingGroupAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.HolidayDay => SearchInRepository(_services.GetRequiredService<IHolidayDayAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.DepartmentDivisions => SearchInRepository(_services.GetRequiredService<IDepartmentDivisionsAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.DepartmentActivityType => SearchInRepository(_services.GetRequiredService<IDepartmentActivityTypeAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.EquipmentType => SearchInRepository(_services.GetRequiredService<IEquipmentTypeAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.DepartmentEquipment => SearchInRepository(_services.GetRequiredService<IDepartmentEquipmentAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.DepartmentFee => SearchInRepository(_services.GetRequiredService<IDepartmentFeeAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.PayCode => SearchInRepository(_services.GetRequiredService<IPayCodeAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.PaymentType => SearchInRepository(_services.GetRequiredService<IPaymentTypeAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.Payment => SearchInRepository(_services.GetRequiredService<IPaymentAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.PaymentAllocation => SearchInRepository(_services.GetRequiredService<IPaymentAllocationAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.RequesterLedger => SearchInRepository(_services.GetRequiredService<IRequesterLedgerAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.Invoice => SearchInRepository(_services.GetRequiredService<IInvoiceAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.InvoiceDetail => SearchInRepository(_services.GetRequiredService<IInvoiceDetailAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.Placeholder => SearchInRepository(_services.GetRequiredService<IPlaceholderAuditReadService>(), searchFilter, pageParam, cancellationToken),
            AuditHistoryTable.EmailTemplate => SearchInRepository(_services.GetRequiredService<IEmailTemplateAuditReadService>(), searchFilter, pageParam, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(table), table, null)
        };

    private static async Task<PagedList<AuditBase>> GetEmailTemplatePageAsync(
        IEmailTemplateAuditReadService readService,
        string? entityId,
        Guid employeeId,
        PageParam pageParam,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var hasEntity = !string.IsNullOrWhiteSpace(entityId);
        var hasEmployee = employeeId != Guid.Empty;
        var hasStartDate = startDate.HasValue;
        var hasEndDate = endDate.HasValue;
        var eid = entityId ?? string.Empty;

        Expression<Func<EmailTemplateAudit, bool>> filter = r =>
            (!hasEntity || r.EntityId == eid)
            && (!hasEmployee || r.EmployeeId == employeeId)
            && (!hasStartDate || r.ChangedOn.Date >= startDate!.Value.Date)
            && (!hasEndDate || r.ChangedOn.Date <= endDate!.Value.Date);

        var rows = await readService.GetAllAsync(filter, includes: null, pageParam).ConfigureAwait(false);
        var total = await readService.GetFilteredCountAsync(filter).ConfigureAwait(false);
        return new PagedList<AuditBase>(rows.Cast<AuditBase>().ToList(), pageParam, total);
    }

    private static async Task<PagedList<AuditBase>> GetPagedAsync<TAudit>(
        IGenericAuditReadService<TAudit> readService,
        string? entityId,
        Guid departmentId,
        Guid employeeId,
        PageParam pageParam,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken)
        where TAudit : AuditBase
    {
        cancellationToken.ThrowIfCancellationRequested();
        var filter = BuildFilter<TAudit>(entityId, departmentId, employeeId, startDate, endDate);
        var rows = await readService.GetAllAsync(filter, includes: null, pageParam).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        var total = await readService.GetFilteredCountAsync(filter).ConfigureAwait(false);
        var data = rows.Cast<AuditBase>().ToList();
        return new PagedList<AuditBase>(data, pageParam, total);
    }

    private static Expression<Func<TAudit, bool>> BuildFilter<TAudit>(string? entityId, Guid departmentId, Guid employeeId, DateTime? startDate, DateTime? endDate)
        where TAudit : AuditBase
    {
        var hasEntity = !string.IsNullOrWhiteSpace(entityId);
        var hasEmployee = employeeId != Guid.Empty;
        var hasStartDate = startDate.HasValue;
        var hasEndDate = endDate.HasValue;

        if (hasEntity && hasEmployee)
        {
            var eid = entityId!;
            return r => r.EntityId == eid && r.DepartmentID == departmentId && r.EmployeeId == employeeId &&
                        (!hasStartDate || r.ChangedOn.Date >= startDate!.Value.Date) &&
                        (!hasEndDate || r.ChangedOn.Date <= endDate!.Value.Date);
        }

        if (hasEntity)
        {
            var eid = entityId!;
            return r => r.EntityId == eid && r.DepartmentID == departmentId &&
                        (!hasStartDate || r.ChangedOn.Date >= startDate!.Value.Date) &&
                        (!hasEndDate || r.ChangedOn.Date <= endDate!.Value.Date);
        }

        if (hasEmployee)
            return r => r.DepartmentID == departmentId && r.EmployeeId == employeeId &&
                        (!hasStartDate || r.ChangedOn.Date >= startDate!.Value.Date) &&
                        (!hasEndDate || r.ChangedOn.Date <= endDate!.Value.Date);

        return r => r.DepartmentID == departmentId &&
                    (!hasStartDate || r.ChangedOn.Date >= startDate!.Value.Date) &&
                    (!hasEndDate || r.ChangedOn.Date <= endDate!.Value.Date);
    }

    private static async Task<PagedList<AuditBase>> SearchInRepository<TAudit>(
        IGenericAuditReadService<TAudit> readService,
        Expression<Func<AuditBase, bool>> searchFilter,
        PageParam pageParam,
        CancellationToken cancellationToken)
        where TAudit : AuditBase
    {
        cancellationToken.ThrowIfCancellationRequested();

        var parameter = Expression.Parameter(typeof(TAudit), "x");
        var convertedFilter = Expression.Lambda<Func<TAudit, bool>>(
            Expression.Invoke(searchFilter, parameter), parameter);

        var data = await readService.GetAllAsync(convertedFilter, includes: null, pageParam).ConfigureAwait(false);
        var total = await readService.GetFilteredCountAsync(convertedFilter).ConfigureAwait(false);

        return new PagedList<AuditBase>(data.Cast<AuditBase>().ToList(), pageParam, total);
    }
}
