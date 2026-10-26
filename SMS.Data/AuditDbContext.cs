using Microsoft.EntityFrameworkCore;
using SMS.Generic.Entities.Entities.Audit;

namespace SMS.Data;

/// <summary>
/// EF Core context for the dedicated audit database (separate from main business data).
/// </summary>
public sealed class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
    {
    }

    public DbSet<RequesterAudit> RequesterAudits => Set<RequesterAudit>();
    public DbSet<PostAudit> PostAudits => Set<PostAudit>();
    public DbSet<RequestAudit> RequestAudits => Set<RequestAudit>();
    public DbSet<PostPositionAudit> PostPositionAudits => Set<PostPositionAudit>();
    public DbSet<EmployeeAudit> EmployeeAudits => Set<EmployeeAudit>();
    public DbSet<EmployeeTypeAudit> EmployeeTypeAudits => Set<EmployeeTypeAudit>();
    public DbSet<SkillAudit> SkillAudits => Set<SkillAudit>();
    public DbSet<BusinessTypeAudit> BusinessTypeAudits => Set<BusinessTypeAudit>();
    public DbSet<EmployeeStatusAudit> EmployeeStatusAudits => Set<EmployeeStatusAudit>();
    public DbSet<DepartmentResponseAreaAudit> DepartmentResponseAreaAudits => Set<DepartmentResponseAreaAudit>();
    public DbSet<PostWorkflowAudit> PostWorkflowAudits => Set<PostWorkflowAudit>();
    public DbSet<PostPositionFlowAudit> PostPositionFlowAudits => Set<PostPositionFlowAudit>();
    public DbSet<ClosedPostPositionAudit> ClosedPostPositionAudits => Set<ClosedPostPositionAudit>();
    public DbSet<ClosedPostPositionEquipmentAudit> ClosedPostPositionEquipmentAudits => Set<ClosedPostPositionEquipmentAudit>();
    public DbSet<RequestScheduleAudit> RequestScheduleAudits => Set<RequestScheduleAudit>();
    public DbSet<RoutingGroupAudit> RoutingGroupAudits => Set<RoutingGroupAudit>();
    public DbSet<HolidayDayAudit> HolidayDayAudits => Set<HolidayDayAudit>();
    public DbSet<DepartmentDivisionsAudit> DepartmentDivisionsAudits => Set<DepartmentDivisionsAudit>();
    public DbSet<DepartmentActivityTypeAudit> DepartmentActivityTypeAudits => Set<DepartmentActivityTypeAudit>();
    public DbSet<EquipmentTypeAudit> EquipmentTypeAudits => Set<EquipmentTypeAudit>();
    public DbSet<DepartmentEquipmentAudit> DepartmentEquipmentAudits => Set<DepartmentEquipmentAudit>();
    public DbSet<DepartmentFeeAudit> DepartmentFeeAudits => Set<DepartmentFeeAudit>();
    public DbSet<PayCodeAudit> PayCodeAudits => Set<PayCodeAudit>();
    public DbSet<PaymentTypeAudit> PaymentTypeAudits => Set<PaymentTypeAudit>();
    public DbSet<PaymentAudit> PaymentAudits => Set<PaymentAudit>();
    public DbSet<RefundPaymentAudit> RefundPaymentAudits => Set<RefundPaymentAudit>();
    public DbSet<RefundPaymentAllocationAudit> RefundPaymentAllocationAudits => Set<RefundPaymentAllocationAudit>();
    public DbSet<PaymentAllocationAudit> PaymentAllocationAudits => Set<PaymentAllocationAudit>();
    public DbSet<InvoiceAudit> InvoiceAudits => Set<InvoiceAudit>();
    public DbSet<InvoiceDetailAudit> InvoiceDetailAudits => Set<InvoiceDetailAudit>();
    public DbSet<RequesterLedgerAudit> RequesterLedgerAudits => Set<RequesterLedgerAudit>();
    public DbSet<PlaceholderAudit> PlaceholderAudits => Set<PlaceholderAudit>();
    public DbSet<EmailTemplateAudit> EmailTemplateAudits => Set<EmailTemplateAudit>();

    public DbSet<PtaeoJobRunAudit> PtaeoJobRunAudits => Set<PtaeoJobRunAudit>();
    public DbSet<EmployeeJobRunAudit> EmployeeJobRunAudits => Set<EmployeeJobRunAudit>();
    public DbSet<WaitingPostPositionJobRunAudit> WaitingPostPositionJobRunAudits => Set<WaitingPostPositionJobRunAudit>();
    public DbSet<UpdateReleasedOnToReleasedJobRunAudit> UpdateReleasedOnToReleasedJobRunAudits => Set<UpdateReleasedOnToReleasedJobRunAudit>();
    public DbSet<UpdatePreferredToManagedJobRunAudit> UpdatePreferredToManagedJobRunAudits => Set<UpdatePreferredToManagedJobRunAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapAuditTable<RequesterAudit>(modelBuilder, "Requester_Audit");
        MapAuditTable<PostAudit>(modelBuilder, "Post_Audit");
        MapAuditTable<RequestAudit>(modelBuilder, "Request_Audit");
        MapAuditTable<PostPositionAudit>(modelBuilder, "PostPosition_Audit");
        MapAuditTable<EmployeeAudit>(modelBuilder, "Employee_Audit");
        MapAuditTable<EmployeeTypeAudit>(modelBuilder, "EmployeeType_Audit");
        MapAuditTable<SkillAudit>(modelBuilder, "Skill_Audit");
        MapAuditTable<BusinessTypeAudit>(modelBuilder, "BusinessType_Audit");
        MapAuditTable<EmployeeStatusAudit>(modelBuilder, "EmployeeStatus_Audit");
        MapAuditTable<DepartmentResponseAreaAudit>(modelBuilder, "DepartmentResponseArea_Audit");
        MapAuditTable<ClosedPostPositionAudit>(modelBuilder, "ClosedPostPosition_Audit");
        MapAuditTable<ClosedPostPositionEquipmentAudit>(modelBuilder, "ClosedPostPositionEquipment_Audit");
        MapAuditTable<RequestScheduleAudit>(modelBuilder, "RequestSchedule_Audit");
        MapAuditTable<RoutingGroupAudit>(modelBuilder, "RoutingGroup_Audit");
        MapAuditTable<HolidayDayAudit>(modelBuilder, "HolidayDay_Audit");
        MapAuditTable<DepartmentDivisionsAudit>(modelBuilder, "DepartmentDivisions_Audit");
        MapAuditTable<DepartmentActivityTypeAudit>(modelBuilder, "DepartmentActivityType_Audit");
        MapAuditTable<EquipmentTypeAudit>(modelBuilder, "EquipmentType_Audit");
        MapAuditTable<DepartmentEquipmentAudit>(modelBuilder, "DepartmentEquipment_Audit");
        MapAuditTable<DepartmentFeeAudit>(modelBuilder, "DepartmentFee_Audit");
        MapAuditTable<PayCodeAudit>(modelBuilder, "PayCode_Audit");
        MapAuditTable<PaymentTypeAudit>(modelBuilder, "PaymentType_Audit");
        MapAuditTable<PaymentAudit>(modelBuilder, "Payments_Audit");
        MapAuditTable<RefundPaymentAudit>(modelBuilder, "RefundPayments_Audit");
        MapAuditTable<RefundPaymentAllocationAudit>(modelBuilder, "RefundPaymentAllocations_Audit");
        MapAuditTable<PaymentAllocationAudit>(modelBuilder, "PaymentAllocations_Audit");
        MapAuditTable<InvoiceAudit>(modelBuilder, "Invoices_Audit");
        MapAuditTable<InvoiceDetailAudit>(modelBuilder, "InvoiceDetails_Audit");
        MapAuditTable<RequesterLedgerAudit>(modelBuilder, "RequesterLedgers_Audit");
        MapAuditTable<PlaceholderAudit>(modelBuilder, "Placeholder_Audit");
        MapAuditTable<EmailTemplateAudit>(modelBuilder, "EmailTemplate_Audit");

        MapAuditTable<PtaeoJobRunAudit>(modelBuilder, "Ptaeo_Audit");
        MapAuditTable<EmployeeJobRunAudit>(modelBuilder, "EmployeeJob_Audit");
        MapAuditTable<WaitingPostPositionJobRunAudit>(modelBuilder, "WaitingPostPosition_Audit");
        MapAuditTable<UpdateReleasedOnToReleasedJobRunAudit>(modelBuilder, "UpdateReleasedOnToReleased_Audit");
        MapAuditTable<UpdatePreferredToManagedJobRunAudit>(modelBuilder, "UpdatePreferredToManaged_Audit");

        modelBuilder.Entity<PostWorkflowAudit>(entity =>
        {
            entity.ToTable("PostWorkflow_Audit", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.PostTitle).HasMaxLength(200);
            entity.Property(e => e.RequestTitle).HasMaxLength(200);
            entity.Property(e => e.EmployeeName).HasMaxLength(200);
            entity.Property(e => e.FailureReason).HasMaxLength(512);
            entity.Property(e => e.CorrelationId).HasMaxLength(128);
        });

        modelBuilder.Entity<PostPositionFlowAudit>(entity =>
        {
            entity.ToTable("PostPositionFlow_Audit", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.DetailsJson).HasColumnType("nvarchar(max)");
            entity.Property(e => e.PostTitle).HasMaxLength(200);
            entity.Property(e => e.EmployeeName).HasMaxLength(200);
            entity.Property(e => e.PerformedByName).HasMaxLength(200);
            entity.HasIndex(e => e.PostPositionId);
        });
    }

    private static void MapAuditTable<T>(ModelBuilder modelBuilder, string tableName) where T : AuditBase
    {
        modelBuilder.Entity<T>(entity =>
        {
            entity.ToTable(tableName, "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ChangedFields).HasColumnType("nvarchar(max)");
            // DisplayName is nvarchar(512) in the database; SQL Server rejects longer values before triggers run.
            // Use Substring, not range syntax: value-converter lambdas are expression trees (no System.Range).
            entity.Property(e => e.DisplayName)
                .HasMaxLength(512)
                .HasConversion(v => v.Length <= 512 ? v : v.Substring(0, 512), v => v);
        });
    }
}
