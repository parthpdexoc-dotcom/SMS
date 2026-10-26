namespace SMS.Generic.Entities.Entities.Audit;

/// <summary>
/// Maps source entity CLR types to audit row CLR types. Extend when adding new audited tables.
/// </summary>
public static class AuditEntityTypeMap
{
    /// <summary>Business entity type → audit row type (one dedicated audit table per entry).</summary>
    public static IReadOnlyDictionary<Type, Type> SourceToAudit { get; } = new Dictionary<Type, Type>
    {
        [typeof(SMS.Generic.Entities.Entities.Requester)] = typeof(RequesterAudit),
        [typeof(SMS.Generic.Entities.Entities.Post)] = typeof(PostAudit),
        [typeof(SMS.Generic.Entities.Entities.Request)] = typeof(RequestAudit),
        [typeof(SMS.Generic.Entities.Entities.PostPosition)] = typeof(PostPositionAudit),
        [typeof(SMS.Generic.Entities.Entities.Employee)] = typeof(EmployeeAudit),
        [typeof(SMS.Generic.Entities.Entities.EmployeeType)] = typeof(EmployeeTypeAudit),
        [typeof(SMS.Generic.Entities.Entities.Skill)] = typeof(SkillAudit),
        [typeof(SMS.Generic.Entities.Entities.BusinessType)] = typeof(BusinessTypeAudit),
        [typeof(SMS.Generic.Entities.Entities.EmployeeStatus)] = typeof(EmployeeStatusAudit),
        [typeof(SMS.Generic.Entities.Entities.DepartmentResponseArea)] = typeof(DepartmentResponseAreaAudit),
        [typeof(SMS.Generic.Entities.Entities.ClosedPostPosition)] = typeof(ClosedPostPositionAudit),
        [typeof(SMS.Generic.Entities.Entities.ClosedPostPositionEquipment)] = typeof(ClosedPostPositionEquipmentAudit),
        [typeof(SMS.Generic.Entities.Entities.RequestSchedule)] = typeof(RequestScheduleAudit),
        [typeof(SMS.Generic.Entities.Entities.RoutingGroup)] = typeof(RoutingGroupAudit),
        [typeof(SMS.Generic.Entities.Entities.HolidayDay)] = typeof(HolidayDayAudit),
        [typeof(SMS.Generic.Entities.Entities.DepartmentDivisions)] = typeof(DepartmentDivisionsAudit),
        [typeof(SMS.Generic.Entities.Entities.DepartmentActivityType)] = typeof(DepartmentActivityTypeAudit),
        [typeof(SMS.Generic.Entities.Entities.EquipmentType)] = typeof(EquipmentTypeAudit),
        [typeof(SMS.Generic.Entities.Entities.DepartmentEquipment)] = typeof(DepartmentEquipmentAudit),
        [typeof(SMS.Generic.Entities.Entities.DepartmentFee)] = typeof(DepartmentFeeAudit),
        [typeof(SMS.Generic.Entities.Entities.PayCode)] = typeof(PayCodeAudit),
        [typeof(SMS.Generic.Entities.Entities.PaymentType)] = typeof(PaymentTypeAudit),
        [typeof(SMS.Generic.Entities.Entities.Payment)] = typeof(PaymentAudit),
        [typeof(SMS.Generic.Entities.Entities.RefundPayment)] = typeof(RefundPaymentAudit),
        [typeof(SMS.Generic.Entities.Entities.RefundPaymentAllocation)] = typeof(RefundPaymentAllocationAudit),
        [typeof(SMS.Generic.Entities.Entities.PaymentAllocation)] = typeof(PaymentAllocationAudit),
        [typeof(SMS.Generic.Entities.Entities.Invoice)] = typeof(InvoiceAudit),
        [typeof(SMS.Generic.Entities.Entities.InvoiceDetail)] = typeof(InvoiceDetailAudit),
        [typeof(SMS.Generic.Entities.Entities.RequesterLedger)] = typeof(RequesterLedgerAudit),
        [typeof(SMS.Generic.Entities.Entities.Placeholder)] = typeof(PlaceholderAudit),
        [typeof(SMS.Generic.Entities.Entities.EmailTemplate)] = typeof(EmailTemplateAudit)
    };

    public static bool IsAuditedEntityType(Type clrType) => SourceToAudit.ContainsKey(clrType);
}
