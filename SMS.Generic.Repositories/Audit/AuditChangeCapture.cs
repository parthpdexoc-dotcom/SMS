using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SMS.Generic.Common.Audit;
using SMS.Generic.Dtos.Audit;
using SMS.Generic.Dtos.Dtos.Audit;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using AuditRowEntity = SMS.Generic.Entities.Entities.Audit.AuditBase;

namespace SMS.Generic.Repositories.Audit;

/// <summary>
/// Builds detached <see cref="AuditBase"/> rows from EF change tracker entries for the audit queue.
/// </summary>
internal static class AuditChangeCapture
{
    private static readonly ConcurrentDictionary<Type, AuditedEntityAttribute?> EntityAttrCache = new();

    private static readonly ConcurrentDictionary<(Type Type, string PropertyName), PropertyInfo?> PropertyCache = new();

    private static readonly ConcurrentDictionary<PropertyInfo, AuditedPropertyAttribute?> PropertyAttrCache = new();

    private static readonly HashSet<Type> AuditRowTypes = new(AuditEntityTypeMap.SourceToAudit.Values);

    /// <summary>
    /// Per-<see cref="DbContext"/> cache for <see cref="DbContext.Find(System.Type,object[])"/> so repeated lookups in one SaveChanges do not round-trip.
    /// </summary>
    private static readonly ConditionalWeakTable<DbContext, ConcurrentDictionary<(Type EntityType, object Key), object?>> FindCacheByContext = new();

    private static readonly ConcurrentDictionary<Type, Func<AuditBase>> AuditRowFactoryCache = new();

    private static object? CachedFind(DbContext ctx, Type entityType, object key)
    {
        var map = FindCacheByContext.GetValue(ctx, static _ => new ConcurrentDictionary<(Type, object), object?>());
        return map.GetOrAdd((entityType, key), t => ctx.Find(t.Item1, t.Item2));
    }

    private static TEntity? CachedFind<TEntity>(DbContext ctx, object key) where TEntity : class =>
        (TEntity?)CachedFind(ctx, typeof(TEntity), key);

    public static CapturePlan PlanBeforeSave(ChangeTracker tracker, IAuditUserContext userContext)
    {
        var modifiedOrDeleted = new List<AuditBase>();
        var addedEntries = new List<EntityEntry>();

        foreach (var entry in tracker.Entries())
        {
            if (entry.Entity is AuditRowEntity)
                continue;

            var clrType = entry.Metadata.ClrType;
            if (!ShouldAudit(clrType))
                continue;

            switch (entry.State)
            {
                case EntityState.Modified:
                    if (BuildModified(entry, userContext) is { } modifiedMsg)
                        modifiedOrDeleted.Add(modifiedMsg);
                    break;
                case EntityState.Deleted:
                    modifiedOrDeleted.Add(BuildDeleted(entry, userContext));
                    break;
                case EntityState.Added:
                    addedEntries.Add(entry);
                    break;
            }
        }

        return new CapturePlan(modifiedOrDeleted, addedEntries);
    }

    public static IReadOnlyList<AuditBase> BuildAfterAdded(IEnumerable<EntityEntry> addedEntries, IAuditUserContext userContext)
    {
        var list = new List<AuditBase>();
        foreach (var entry in addedEntries)
        {
            if (entry.State != EntityState.Unchanged)
                continue;

            var clrType = entry.Metadata.ClrType;
            if (!ShouldAudit(clrType))
                continue;

            list.Add(BuildInserted(entry, userContext));
        }

        return list;
    }

    private static bool ShouldAudit(Type clrType) =>
        EntityAttrCache.GetOrAdd(clrType, static t => t.GetCustomAttribute<AuditedEntityAttribute>()) is not null
        && AuditEntityTypeMap.SourceToAudit.ContainsKey(clrType);

    private static AuditBase? BuildModified(EntityEntry entry, IAuditUserContext ctx)
    {
        var clrType = entry.Metadata.ClrType;
        var auditType = AuditEntityTypeMap.SourceToAudit[clrType];
        var changes = new List<AuditFieldChange>();
        CollectChanges(entry, clrType, AuditAction.UPDATE, changes);

        if (changes.Count == 0)
            return null;

        return CreateMessage(entry, auditType, AuditAction.UPDATE, changes, ctx);
    }

    private static AuditBase BuildDeleted(EntityEntry entry, IAuditUserContext ctx)
    {
        var clrType = entry.Metadata.ClrType;
        var auditType = AuditEntityTypeMap.SourceToAudit[clrType];
        var changes = new List<AuditFieldChange>();
        CollectChanges(entry, clrType, AuditAction.DELETE, changes);
        return CreateMessage(entry, auditType, AuditAction.DELETE, changes, ctx);
    }

    private static AuditBase BuildInserted(EntityEntry entry, IAuditUserContext ctx)
    {
        var clrType = entry.Metadata.ClrType;
        var auditType = AuditEntityTypeMap.SourceToAudit[clrType];
        var changes = new List<AuditFieldChange>();
        CollectChanges(entry, clrType, AuditAction.INSERT, changes);
        return CreateMessage(entry, auditType, AuditAction.INSERT, changes, ctx);
    }

    private static void CollectChanges(EntityEntry entry, Type clrType, AuditAction mode, List<AuditFieldChange> changes)
    {
        var entityAttr = EntityAttrCache.GetOrAdd(clrType, static t => t.GetCustomAttribute<AuditedEntityAttribute>());
        if (entityAttr is null)
            return;

        foreach (var prop in entry.Properties)
        {
            if (prop.Metadata.IsShadowProperty())
                continue;

            if (mode == AuditAction.UPDATE && !prop.IsModified)
                continue;

            var clrProp = GetClrProperty(clrType, prop.Metadata.Name);
            var propAttr = clrProp is null
                ? null
                : PropertyAttrCache.GetOrAdd(clrProp, static p => p.GetCustomAttribute<AuditedPropertyAttribute>());

            if (entityAttr.OnlyAttributedProperties)
            {
                if (propAttr is null)
                    continue;
            }

            object? oldVal = mode == AuditAction.INSERT ? null : prop.OriginalValue;
            object? newVal = mode == AuditAction.DELETE ? null : prop.CurrentValue;

            changes.Add(new AuditFieldChange
            {
                Property = propAttr?.DisplayName ?? prop.Metadata.Name,
                Old = FormatAuditValue(entry, propAttr, oldVal),
                New = FormatAuditValue(entry, propAttr, newVal)
            });
        }
    }

    private static PropertyInfo? GetClrProperty(Type clrType, string propertyName) =>
        PropertyCache.GetOrAdd((clrType, propertyName), static key =>
            key.Type.GetProperty(key.PropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));

    private static object? FormatAuditValue(EntityEntry entry, AuditedPropertyAttribute? propAttr, object? raw)
    {
        if (raw is null)
            return null;

        if (propAttr?.GetSkillNamesByIds == true)
            return FormatSkillIdsAsShortNames(entry.Context, raw);

        if (propAttr?.GetEmployeeNamesByIds == true)
            return FormatEmployeeIdsAsDisplayNames(entry.Context, raw);

        if (propAttr?.GetEmployeeTypeTitlesByIds == true)
            return FormatEmployeeTypeIdsAsTitles(entry.Context, raw);

        if (propAttr is null || propAttr.LookupEntityType is null || string.IsNullOrWhiteSpace(propAttr.LookupDisplayPropertyName))
            return NormalizeValue(raw);

        try
        {
            var related = CachedFind(entry.Context, propAttr.LookupEntityType, raw);
            if (related is null)
                return raw.ToString();

            if (related is RequesterManager requesterManager &&
                string.Equals(propAttr.LookupDisplayPropertyName, nameof(RequesterManager.EmployeeID), StringComparison.OrdinalIgnoreCase))
            {
                var employee = entry.Context.Find<Employee>(requesterManager.EmployeeID);
                return employee.FullName;
            }

            var displayPi = PropertyCache.GetOrAdd(
                (propAttr.LookupEntityType, propAttr.LookupDisplayPropertyName),
                static key => key.Type.GetProperty(key.PropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
            var display = displayPi?.GetValue(related);
            return display?.ToString() ?? raw.ToString();
        }
        catch
        {
            return raw.ToString();
        }
    }

    private static object? FormatSkillIdsAsShortNames(DbContext ctx, object raw)
    {
        if (raw is not List<Guid> ids)
            return NormalizeValue(raw);
        if (ids.Count == 0)
            return null;

        var rows = ctx.Set<Skill>().AsNoTracking()
            .Where(s => ids.Contains(s.SkillID))
            .Select(s => new { s.SkillID, s.SkillShort, s.SkillLong })
            .ToList();

        var map = rows.ToDictionary(
            x => x.SkillID,
            x => string.IsNullOrWhiteSpace(x.SkillShort) ? (x.SkillLong ?? x.SkillID.ToString()) : x.SkillShort!);

        var parts = new List<string>(ids.Count);
        foreach (var id in ids)
            parts.Add(map.GetValueOrDefault(id, id.ToString()));

        return string.Join(", ", parts);
    }

    private static object? FormatEmployeeIdsAsDisplayNames(DbContext ctx, object raw)
    {
        if (raw is not List<Guid> ids)
            return NormalizeValue(raw);
        if (ids.Count == 0)
            return null;

        var map = ctx.Set<Employee>().AsNoTracking()
            .Where(e => ids.Contains(e.EmployeeID))
            .Select(e => new { e.EmployeeID, e.FirstName, e.LastName, e.EmployeeNumber })
            .ToList()
            .ToDictionary(x => x.EmployeeID, x =>
            {
                var n = $"{x.FirstName} {x.LastName}".Trim();
                return n.Length > 0 ? n : !string.IsNullOrEmpty(x.EmployeeNumber) ? x.EmployeeNumber : x.EmployeeID.ToString();
            });

        return string.Join(", ", ids.Select(id => map.GetValueOrDefault(id, id.ToString())));
    }

    private static object? FormatEmployeeTypeIdsAsTitles(DbContext ctx, object raw)
    {
        if (raw is not List<Guid> ids)
            return NormalizeValue(raw);
        if (ids.Count == 0)
            return null;

        var map = ctx.Set<EmployeeType>().AsNoTracking()
            .Where(t => ids.Contains(t.EmployeeTypeID))
            .Select(t => new { t.EmployeeTypeID, t.EmployeeTypeShort, t.EmployeeTypeLong })
            .ToList()
            .Where(t => !string.IsNullOrWhiteSpace(t.EmployeeTypeShort) || !string.IsNullOrWhiteSpace(t.EmployeeTypeLong))
            .ToDictionary(t => t.EmployeeTypeID, t => string.IsNullOrWhiteSpace(t.EmployeeTypeShort) ? t.EmployeeTypeLong! : t.EmployeeTypeShort!);

        var text = string.Join(", ", ids.Where(map.ContainsKey).Select(id => map[id]));
        return text.Length == 0 ? null : text;
    }

    private static AuditBase CreateMessage(
        EntityEntry entry,
        Type auditRowType,
        AuditAction action,
        List<AuditFieldChange> changes,
        IAuditUserContext ctx)
    {
        var row = CreateAuditRow(auditRowType);
        row.EntityId = FormatPrimaryKey(entry);
        row.DisplayName = ResolveDisplayName(entry);
        row.ChangedFields = JsonSerializer.Serialize(changes, AuditJsonSerializerOptions.ChangedFields);
        row.Action = action;
        row.ChangedOn = DateTime.UtcNow;
        row.UserId = ctx.UserId ?? string.Empty;
        row.DepartmentID = ctx.DepartmentID;
        row.CorrelationId = ctx.CorrelationId;
        row.EmployeeId = ResolveAuditEmployeeIdFromPostPosition(entry);
        return row;
    }

    /// <summary>
    /// Worker id from <see cref="PostPosition"/> (or the audited <see cref="Employee"/> row), not from the JWT.
    /// </summary>
    private static Guid? ResolveAuditEmployeeIdFromPostPosition(EntityEntry entry)
    {
        switch (entry.Entity)
        {
            case PostPosition pp:
                if (pp.EmployeeID != Guid.Empty)
                    return pp.EmployeeID;
                var trackedPp = CachedFind<PostPosition>(entry.Context, pp.PostPositionID);
                return trackedPp is { EmployeeID: var e0 } && e0 != Guid.Empty ? e0 : null;

            case ClosedPostPosition cpp:
                var ppForCpp = CachedFind<PostPosition>(entry.Context, cpp.PostPositionID);
                return ppForCpp is { EmployeeID: var e1 } && e1 != Guid.Empty ? e1 : null;

            case ClosedPostPositionEquipment cpe:
                var cppRow = CachedFind<ClosedPostPosition>(entry.Context, cpe.ClosedPostPositionID);
                if (cppRow is null)
                    return null;
                var ppForCpe = CachedFind<PostPosition>(entry.Context, cppRow.PostPositionID);
                return ppForCpe is { EmployeeID: var e2 } && e2 != Guid.Empty ? e2 : null;

            case Post post:
                return entry.Context.Set<PostPosition>().AsNoTracking()
                    .Where(p => p.PostID == post.PostID && p.EmployeeID != Guid.Empty)
                    .OrderBy(p => p.PostPositionID)
                    .Select(p => (Guid?)p.EmployeeID)
                    .FirstOrDefault();

            case Employee em:
                return em.EmployeeID != Guid.Empty ? em.EmployeeID : null;

            default:
                return null;
        }
    }

    private static AuditBase CreateAuditRow(Type auditRowType)
    {
        ArgumentNullException.ThrowIfNull(auditRowType);
        if (!AuditRowTypes.Contains(auditRowType))
            throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUDIT_NO_ROW_MAPPING_FOR_TYPE, auditRowType.FullName));

        return AuditRowFactoryCache.GetOrAdd(auditRowType, CompileAuditRowFactory)();
    }

    private static Func<AuditBase> CompileAuditRowFactory(Type auditRowType)
    {
        var ctor = auditRowType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .FirstOrDefault(c => c.GetParameters().Length == 0);
        if (ctor is null)
            throw new InvalidOperationException($"Audit row type '{auditRowType.FullName}' has no parameterless constructor.");

        var body = Expression.Convert(Expression.New(ctor), typeof(AuditBase));
        return Expression.Lambda<Func<AuditBase>>(body).Compile();
    }

    private static string FormatPrimaryKey(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key is null)
            return string.Empty;

        var parts = key.Properties.Select(p => entry.Property(p.Name).CurrentValue ?? entry.Property(p.Name).OriginalValue);
        return string.Join(",", parts.Select(v => v?.ToString() ?? ""));
    }

    private static string ResolveDisplayName(EntityEntry entry)
    {
    // 1. Post-based entities
        if (TryGetPostTitleContext(entry.Entity, out var postId, out var postNav))
        {
            var title = postNav?.PostTitle;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = CachedFind<Post>(entry.Context, postId)?.PostTitle;
            }
            return !string.IsNullOrWhiteSpace(title)
                ? title.Trim()
                : FormatPrimaryKey(entry);
        }

        if (entry.Entity is RequestSchedule schedule)
        {
            var title = schedule.Request?.Title;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = CachedFind<Request>(entry.Context, schedule.RequestId)?.Title;
            }
            return !string.IsNullOrWhiteSpace(title)
                ? title.Trim()
                : FormatPrimaryKey(entry);
        }
        
        if (entry.Entity is DepartmentEquipment departmentEquipment)
        {
            return ResolveDepartmentEquipmentDisplayName(entry, departmentEquipment);
        }

        if (entry.Entity is PaymentAllocation paymentAllocation)
        {
            return ResolvePaymentAllocationDisplayName(entry, paymentAllocation);
        }

        if (TryResolveDisplayNameFromAuditedScalar(entry, out var auditedDisplay))
        {
            return auditedDisplay;
        }
        return FormatPrimaryKey(entry);
    }

    /// <summary>
    /// Human-readable label for <see cref="PaymentAllocation"/> uses payment number and invoice number when available.
    /// </summary>
    private static string ResolvePaymentAllocationDisplayName(EntityEntry entry, PaymentAllocation paymentAllocation)
    {
        var invoiceNo = paymentAllocation.Invoice?.InvoiceNo;
        if (string.IsNullOrWhiteSpace(invoiceNo))
            invoiceNo = CachedFind<Invoice>(entry.Context, paymentAllocation.InvoicesID)?.InvoiceNo;

        var paymentNumber = paymentAllocation.Payment?.PaymentNumber;
        if (string.IsNullOrWhiteSpace(paymentNumber))
            paymentNumber = CachedFind<Payment>(entry.Context, paymentAllocation.PaymentID)?.PaymentNumber;

        if (!string.IsNullOrWhiteSpace(paymentNumber) && !string.IsNullOrWhiteSpace(invoiceNo))
            return $"Payment {paymentNumber.Trim()} – Invoice {invoiceNo.Trim()}";
        if (!string.IsNullOrWhiteSpace(paymentNumber))
            return $"Payment {paymentNumber.Trim()}";
        if (!string.IsNullOrWhiteSpace(invoiceNo))
            return $"Invoice {invoiceNo.Trim()}";

        return FormatPrimaryKey(entry);
    }

    /// <summary>
    /// Human-readable label for <see cref="DepartmentEquipment"/> uses the equipment type short name, not the row id.
    /// </summary>
    private static string ResolveDepartmentEquipmentDisplayName(EntityEntry entry, DepartmentEquipment departmentEquipment)
    {
        var typeShort = departmentEquipment.EquipmentType?.EquipementTypeShort;
        if (string.IsNullOrWhiteSpace(typeShort))
        {
            var et = CachedFind<EquipmentType>(entry.Context, departmentEquipment.EquipmentTypeID);
            typeShort = et?.EquipementTypeShort;
        }

        if (!string.IsNullOrWhiteSpace(typeShort))
            return typeShort.Trim();

        return FormatPrimaryKey(entry);
    }

    /// <summary>
    /// Uses <see cref="AuditedEntityAttribute.DisplayPropertyName"/> when set: trimmed non-whitespace string, else primary key.
    /// </summary>
    private static bool TryResolveDisplayNameFromAuditedScalar(EntityEntry entry, [NotNullWhen(true)] out string? display)
    {
        display = null;
        var clrType = entry.Metadata.ClrType;
        var propName = EntityAttrCache.GetOrAdd(clrType, static t => t.GetCustomAttribute<AuditedEntityAttribute>())?.DisplayPropertyName;
        if (string.IsNullOrEmpty(propName))
            return false;

        var prop = PropertyCache.GetOrAdd((clrType, propName), static key =>
            key.Type.GetProperty(key.PropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
        if (prop is null)
            return false;

        var raw = prop.GetValue(entry.Entity)?.ToString();
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        display = raw.Trim();
        return true;
    }

    /// <summary>
    /// Shared audit display: entities that link to a post via <c>PostID</c> and optional <c>Post</c> navigation.
    /// </summary>
    private static bool TryGetPostTitleContext(object entity, out Guid postId, out Post? postNav)
    {
        switch (entity)
        {
            case PostPosition pp:
                postId = pp.PostID;
                postNav = pp.Post;
                return postId != Guid.Empty;
            case ClosedPostPosition cpp:
                postId = cpp.PostID;
                postNav = cpp.Post;
                return postId != Guid.Empty;
            case ClosedPostPositionEquipment cpe:
                if (cpe.PostID is not { } pid || pid == Guid.Empty)
                {
                    postId = default;
                    postNav = null;
                    return false;
                }

                postId = pid;
                postNav = cpe.Post;
                return true;
            default:
                postId = default;
                postNav = null;
                return false;
        }
    }

    private static object? NormalizeValue(object? value) =>
        value switch
        {
            null => null,
            DateTime dt => dt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) : dt.ToUniversalTime(),
            DateTimeOffset dto => dto.ToUniversalTime(),
            byte[] bytes => Convert.ToBase64String(bytes),
            Enum e => e.ToString(),
            _ => value
        };

    internal readonly record struct CapturePlan(
        List<AuditBase> ModifiedOrDeleted,
        List<EntityEntry> AddedEntries);
}
