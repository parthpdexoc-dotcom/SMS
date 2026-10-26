using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using SMS.Generic.Common.Enums;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Data
{
    /// <summary>
    /// Database context for SMS application (main business database / MainDb).
    /// </summary>
    public class SMSDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SMSDbContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public SMSDbContext(DbContextOptions<SMSDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Department DbSet.
        /// </summary>
        public DbSet<Department> Department { get; set; }

        /// <summary>
        /// Gets or sets the Admin DbSet.
        /// </summary>
        public DbSet<Admin> Admin { get; set; }

        public DbSet<DepartmentDivisions> DepartmentDivisions { get; set; }

        ///// <summary>
        ///// Gets or sets the AdminType DbSet.
        ///// </summary>
        public DbSet<AdminType> AdminType { get; set; }

        ///// <summary>
        ///// Gets or sets the Employee DbSet.
        ///// </summary>
        public DbSet<Employee> Employee { get; set; }

        ///// <summary>
        ///// Gets or sets the EmployeeAdmin DbSet.
        ///// </summary>
        public DbSet<EmployeeAdmin> EmployeeAdmin { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }

        /// <summary>
        /// Gets or sets the Equipment DbSet.
        /// </summary>
        public DbSet<Equipment> Equipment { get; set; }

        /// <summary>
        /// Gets or sets the BusinessType DbSet.
        /// </summary>
        public DbSet<BusinessType> BusinessType { get; set; }

        /// <summary>
        /// Gets or sets the PayCode DbSet.
        /// </summary>
        public DbSet<PayCode> PayCode { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentAdmin DbSet.
        /// </summary>
        public DbSet<DepartmentAdmin> DepartmentAdmin { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentPolicy DbSet.
        /// </summary>
        public DbSet<DepartmentPolicy> DepartmentPolicy { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentResponseArea DbSet.
        /// </summary>
        public DbSet<DepartmentResponseArea> DepartmentResponseArea { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentInvoiceConfig DbSet.
        /// </summary>
        public DbSet<DepartmentInvoiceConfig> DepartmentInvoiceConfig { get; set; }

        /// <summary>
        /// Gets or sets the Invoice DbSet.
        /// </summary>
        public DbSet<Invoice> Invoices { get; set; }

        /// <summary>
        /// Gets or sets the InvoiceDetail DbSet.
        /// </summary>
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentFee DbSet.
        /// </summary>
        public DbSet<DepartmentFee> DepartmentFee { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentCronJobManagement DbSet.
        /// </summary>
        public DbSet<DepartmentCronJobManagement> DepartmentCronJobManagement { get; set; } = null!;

        ///// <summary>
        ///// Gets or sets the Posts DbSet.
        ///// </summary>
        public DbSet<Post> Post { get; set; }
        public DbSet<PostPosition> PostPosition { get; set; } = null!;
        public DbSet<EmployeePostLocation> EmployeePostLocation { get; set; } = null!;
        public DbSet<Placeholder> Placeholder { get; set; } = null!;
        public DbSet<WaitingPostPosition> WaitingPostPosition { get; set; } = null!;
        public DbSet<PostEquipment> PostEquipment { get; set; } = null!;
        public DbSet<PostPositionVehicle> PostPositionVehicle { get; set; } = null!;
        public DbSet<PostCallSign> PostCallSign { get; set; } = null!;
        public DbSet<RequesterQuestionnaire> RequesterQuestionnaire { get; set; } = null!;
        public DbSet<RequestEquipment> RequestEquipment { get; set; } = null!;
        public DbSet<RequestDepartmentFee> RequestDepartmentFee { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UploadFile DbSet.
        /// </summary>
        public DbSet<UploadFile> UploadFile { get; set; } = null!;
        public DbSet<ClosedPostPosition> ClosedPostPosition { get; set; } = null!;
        public DbSet<ClosedPostPositionEquipment> ClosedPostPositionEquipment { get; set; } = null!;
        public DbSet<CancelledPostPosition> CancelledPostPosition { get; set; } = null!;
        public DbSet<RoutingGroup> RoutingGroup { get; set; } = null!;
        public DbSet<DepartmentConfiguration> DepartmentConfiguration { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Requests DbSet.
        /// </summary>
        public DbSet<Request> Requests { get; set; }

        /// <summary>
        /// Gets or sets the RequestSchedule DbSet.
        /// </summary>
        public DbSet<RequestSchedule> RequestSchedule { get; set; }

        public DbSet<RequestScheduleGeneration> RequestScheduleGeneration { get; set; }

        /// <summary>
        /// Gets or sets the Requester DbSet.
        /// </summary>
        public DbSet<Requester> Requester { get; set; }

        /// <summary>
        /// Gets or sets the RequesterConfig DbSet.
        /// </summary>
        public DbSet<RequesterConfig> RequesterConfig { get; set; }

        /// <summary>
        /// Gets or sets the RequesterRestrictedList DbSet.
        /// </summary>
        public DbSet<RequesterRestrictedList> RequesterRestrictedList { get; set; }

        /// <summary>
        /// Gets or sets the RequesterEmployee DbSet.
        /// </summary>
        public DbSet<RequesterEmployee> RequesterEmployee { get; set; }

        /// <summary>
        /// Gets or sets the HolidayDay DbSet.
        /// </summary>
        public DbSet<HolidayDay> HolidayDay { get; set; }

        /// <summary>
        /// Gets or sets the RequesterAuthorized DbSet.
        /// </summary>
        public DbSet<RequesterAuthorized> RequesterAuthorized { get; set; }

        /// <summary>
        /// Gets or sets the EmployeeTenure DbSet.
        /// </summary>
        public DbSet<EmployeeTenure> EmployeeTenure { get; set; }

        /// <summary>
        /// Gets or sets the EmployeeSkill DbSet.
        /// </summary>
        public DbSet<EmployeeSkill> EmployeeSkill { get; set; }

        /// <summary>
        /// Gets or sets the EmployeeDeviceToken DbSet.
        /// </summary>
        public DbSet<EmployeeDeviceToken> EmployeeDeviceToken { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DepartmentEquipment DbSet.
        /// </summary>
        public DbSet<DepartmentEquipment> DepartmentEquipments { get; set; }

        /// <summary>
        /// Gets or sets the VehicleApprovalRouting DbSet.
        /// </summary>
        public DbSet<VehicleApprovalRouting> VehicleApprovalRouting { get; set; } = null!;

        /// <summary>
        /// Gets or sets the RequestApprovalRouting DbSet.
        /// </summary>
        public DbSet<RequestApprovalRouting> RequestApprovalRouting { get; set; } = null!;

        /// <summary>
        /// Gets or sets the PermitApprovalRouting DbSet.
        /// </summary>
        public DbSet<PermitApprovalRouting> PermitApprovalRouting { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ApprovalDetail DbSet (table: ApprovalDetails).
        /// </summary>
        public DbSet<ApprovalDetail> ApprovalDetail { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Skill DbSet.
        /// </summary>
        public DbSet<Skill> Skill { get; set; }

        /// <summary>
        /// Gets or sets the PaymentType DbSet.
        /// </summary>
        public DbSet<SMS.Generic.Entities.Entities.PaymentType> PaymentType { get; set; } = null!;

        public DbSet<Payment> Payment { get; set; }

        /// <summary>
        /// Refund records (same shape as <see cref="Payment"/>).
        /// </summary>
        public DbSet<RefundPayment> RefundPayment { get; set; }

        /// <summary>
        /// Allocations between a <see cref="Payment"/> and a <see cref="RefundPayment"/>.
        /// </summary>
        public DbSet<RefundPaymentAllocation> RefundPaymentAllocations { get; set; }

        /// <summary>
        /// Gets or sets the PaymentAllocation DbSet.
        /// </summary>
        public DbSet<PaymentAllocation> PaymentAllocations { get; set; }

        /// <summary>
        /// Requester ledger entries (bank-statement style credit/debit rows).
        /// </summary>
        public DbSet<RequesterLedger> RequesterLedgers { get; set; }

        /// <summary>
        /// Gets or sets the EmailTemplate DbSet.
        /// </summary>
        public DbSet<EmailTemplate> EmailTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentActivityType DbSet.
        /// </summary>
        public DbSet<DepartmentActivityType> DepartmentActivityType { get; set; }

        /// <summary>
        /// Gets or sets the EmployeeType DbSet.
        /// </summary>
        public DbSet<EmployeeType> EmployeeType { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentDefinedFields DbSet.
        /// </summary>
        public DbSet<DepartmentDefinedFields> DepartmentDefinedFields { get; set; }
        public DbSet<PostNotes> PostNotes { get; set; }
        public DbSet<RequestNotes> RequestNotes { get; set; }
        public DbSet<NotificationHistory> NotificationHistory { get; set; } = null!;
        public DbSet<NotificationHistoryRecipient> NotificationHistoryRecipients { get; set; } = null!;
        public DbSet<NotificationHistoryAttachment> NotificationHistoryAttachments { get; set; } = null!;
        public DbSet<BlockDate> BlockDate { get; set; }

        /// <summary>
        /// Gets or sets the 
        /// DbSet.
        /// </summary>
        public DbSet<Questionnaire> Questionnaire { get; set; }


        public DbSet<PostDocument> PostDocuments { get; set; }

        /// <summary>
        /// Gets or sets the Question DbSet.
        /// </summary>
        public DbSet<Question> Question { get; set; }

        /// <summary>
        /// Gets or sets the Option DbSet.
        /// </summary>
        public DbSet<Option> Option { get; set; }

        /// <summary>
        /// Gets or sets the PTAEO_Code DbSet.
        /// </summary>
        public DbSet<PtaeoCode> PtaeoCode { get; set; }

        /// <summary>
        /// Gets or sets the Response DbSet.
        /// </summary>
        public DbSet<Response> Response { get; set; }

        /// <summary>
        /// Gets or sets the Answers DbSet.
        /// </summary>
        public DbSet<Answers> Answers { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentQuestionnaire DbSet.
        /// </summary>
        public DbSet<DepartmentQuestionnaire> DepartmentQuestionnaire { get; set; }

        /// <summary>
        /// Configures the model that was discovered from the entity types.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ApplySoftDeleteQueryFilters(modelBuilder);

            // Sequences for *Code columns (int sequence; column is nvarchar(20) default FORMAT(..., '000000') e.g. 000001). Migrations should use e.g.:
            // migrationBuilder.CreateSequence<int>(name: "Seq_EmployeeCode", schema: "dbo", startValue: 1L, incrementBy: 1);
            modelBuilder.HasSequence<int>("Seq_EmployeeCode", "dbo").StartsAt(1L).IncrementsBy(1);
            modelBuilder.HasSequence<int>("Seq_PostCode", "dbo").StartsAt(1L).IncrementsBy(1);
            modelBuilder.HasSequence<int>("Seq_RequestCode", "dbo").StartsAt(1L).IncrementsBy(1);
            modelBuilder.HasSequence<int>("Seq_RequesterCode", "dbo").StartsAt(1L).IncrementsBy(1);
            modelBuilder.HasSequence<int>("Seq_PostpositionCode", "dbo").StartsAt(1L).IncrementsBy(1);

            // PostNotes → Post (no collection in Post)
            modelBuilder.Entity<PostNotes>(entity =>
            {
                entity.HasOne(pn => pn.Post)
                    .WithMany()
                    .HasForeignKey(pn => pn.PostID)
                    .OnDelete(DeleteBehavior.Cascade);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedUserID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                    .WithMany()
                    .HasForeignKey(e => e.EditedUserID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //postDocuments

            modelBuilder.Entity<PostDocument>(entity =>
            {
                entity.ToTable("PostDocument", "dbo");
                entity.HasKey(e => e.PostDocumentId);
                entity.Property(e => e.PostDocumentId)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.HasOne(e => e.Post)
                      .WithMany(p => p.PostDocuments)
                      .HasForeignKey(e => e.PostID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // RequestNotes → Request (no collection in Request)
            modelBuilder.Entity<RequestNotes>(entity =>
            {
                entity.HasOne(rn => rn.Request)
                    .WithMany()
                    .HasForeignKey(rn => rn.RequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedUserID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                    .WithMany()
                    .HasForeignKey(e => e.EditedUserID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<NotificationHistory>(entity =>
            {
                entity.HasMany(h => h.Recipients)
                    .WithOne(r => r.NotificationHistory)
                    .HasForeignKey(r => r.NotificationHistoryID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(h => h.Attachments)
                    .WithOne(a => a.NotificationHistory)
                    .HasForeignKey(a => a.NotificationHistoryID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.DepartmentID);
                entity.HasIndex(e => e.RequestID);
                entity.HasIndex(e => e.RequesterID);
                entity.HasIndex(e => e.PostID);
                entity.HasIndex(e => e.PostPositionID);
                entity.HasIndex(e => e.SentAt);
            });

            // BlockDate → Request and Post (no collections in Request/Post)
            modelBuilder.Entity<BlockDate>(entity =>
            {
                // Foreign Key to Request table
                entity.HasOne(bd => bd.Request)
                    .WithMany()
                    .HasForeignKey(bd => bd.RequestID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Post table
                entity.HasOne(bd => bd.Post)
                    .WithMany(p => p.BlockDates)
                    .HasForeignKey(bd => bd.PostID)
                    .OnDelete(DeleteBehavior.Restrict);


                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedUserID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                    .WithMany()
                    .HasForeignKey(e => e.EditedUserID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for UserDeletedID
                //entity.HasOne(e => e.UserDeletedEmployee)
                //    .WithMany()
                //    .HasForeignKey(e => e.UserDeletedID)
                //    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Department entity
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department", "dbo");
                entity.HasKey(e => e.DepartmentID);
                entity.Property(e => e.DepartmentID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.Property(e => e.TimeZoneId)
                      .HasMaxLength(100)
                      .IsUnicode()
                      .IsRequired()
                      .HasDefaultValue("Eastern Standard Time");

                // Foreign Key to Admin table for CreatedUserID
                entity.HasOne(e => e.CreatedByAdmin)
                      .WithMany(a => a.CreatedDepartments)
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Admin table for EditedUserID
                entity.HasOne(e => e.EditedByAdmin)
                      .WithMany(a => a.EditedDepartments)
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin", "dbo");
                entity.HasKey(e => e.AdminID);
                entity.Property(e => e.AdminID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                // Foreign Key to Admin table for CreatedUserID
                entity.HasOne<Admin>()
                      .WithMany(a => a.CreatedAdmins)
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Admin table for EditedUserID
                entity.HasOne<Admin>()
                      .WithMany(a => a.EditedAdmins)
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to AdminType table for AdminTypeID
                entity.HasOne(a => a.AdminType)
                      .WithMany()
                      .HasForeignKey(a => a.AdminTypeID)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<AdminType>(entity =>
            {
                entity.ToTable("AdminType", "dbo");
                entity.HasKey(e => e.AdminTypeID);
                entity.Property(e => e.AdminTypeID)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

                // Default Status to active/true (bit = 1)
                entity.Property(e => e.Status)
                      .HasColumnType("bit")
                      .HasDefaultValue(true);

                entity.Property(e => e.IsManagePost)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.IsSuperUser)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.IsSuperUserType)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.IsChild)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                // Self-referencing Foreign Key: ParentID -> AdminTypeID
                entity.HasOne(e => e.Parent)
                      .WithMany()
                      .HasForeignKey(e => e.ParentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Admin table for CreatedUserID
                entity.HasOne(e => e.CreatedByAdmin)
                      .WithMany(a => a.CreatedAdminTypes)
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Admin table for EditedUserID
                entity.HasOne(e => e.EditedByAdmin)
                      .WithMany(a => a.EditedAdminTypes)
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee", "dbo");
                entity.HasKey(e => e.EmployeeID);
                entity.Property(e => e.EmployeeID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.Property(e => e.EmployeeCode)
                      .HasMaxLength(20)
                      .IsRequired()
                      .HasDefaultValueSql("FORMAT(NEXT VALUE FOR [dbo].[Seq_EmployeeCode], '000000')")
                      .ValueGeneratedOnAdd()
                      .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.HasIndex(e => e.EmployeeCode).IsUnique();
                var activeSession = entity.Property(e => e.ActiveSessionId).Metadata;
                activeSession.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                activeSession.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to EmployeeType table for EmployeeTypeID
                entity.HasOne(e => e.EmployeeType)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeTypeID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to EmployeeStatus table for StatusID
                entity.HasOne(e => e.Status)
                      .WithMany()
                      .HasForeignKey(e => e.StatusID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to DepartmentDivisions table for DivisionID
                entity.HasOne(e => e.Division)
                      .WithMany()
                      .HasForeignKey(e => e.DivisionID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure decimal property for HourlySalary
                entity.Property(e => e.HourlySalary)
                      .HasColumnType("decimal(18,5)");

                // Configure string properties max lengths
                entity.Property(e => e.HrCostCenterCode)
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(e => e.EmployeeDepartmentDefinedField1)
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.Property(e => e.EmployeeDepartmentDefinedField2)
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.Property(e => e.EmployeeDepartmentDefinedField3)
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.Property(e => e.EmployeeDepartmentDefinedField4)
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.Property(e => e.IsApproved)
                      .HasColumnType("bit")
                      .HasDefaultValue(true);

                entity.Property(e => e.IsSubsidizedVehicle)
                      .HasColumnType("bit");

                entity.Property(e => e.SmsTelephoneNumber)
                      .HasMaxLength(25)
                      .HasColumnType("nvarchar(25)");

                entity.Property(e => e.HasSmsConsent)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.NotifyWhenJobsPosted)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.NotifyJobStartingIn24Hours)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.NotifyJobStartingIn1Hour)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.HasMany(e => e.EmployeeSkills)
                     .WithOne(es => es.Employee)
                     .HasForeignKey(es => es.EmployeeID)
                     .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<EmployeeAdmin>(entity =>
            {
                entity.ToTable("EmployeeAdmin", "dbo");
                entity.HasKey(e => e.EmployeeAdminID);
                entity.Property(e => e.EmployeeAdminID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                // Foreign Key to Employee table for EmployeeID
                entity.HasOne(e => e.Employee)
                      .WithMany(e => e.EmployeeAdmins)
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to AdminType table for AdminTypeID
                entity.HasOne(e => e.AdminType)
                      .WithMany()
                      .HasForeignKey(e => e.AdminTypeID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne<Employee>()
                      .WithMany(e => e.CreatedEmployeeAdmins)
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne<Employee>()
                      .WithMany(e => e.EditedEmployeeAdmins)
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure EmployeeStatus entity
            modelBuilder.Entity<EmployeeStatus>(entity =>
            {
                entity.ToTable("EmployeeStatus", "dbo");
                entity.HasKey(e => e.EmployeeStatusID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.EmployeeStatusID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd(); // Add this line

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string properties max lengths
                entity.Property(e => e.StatusShort).HasMaxLength(100);
                entity.Property(e => e.StatusLong).HasMaxLength(255);
            });

            // Configure Skill entity
            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("Skill", "dbo");
                entity.HasKey(e => e.SkillID);
                entity.Property(e => e.SkillID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string properties max lengths and types
                entity.Property(e => e.SkillShort)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.SkillLong)
                      .HasMaxLength(200)
                      .HasColumnType("varchar(200)");
                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.EditedBy).HasMaxLength(255);
            });

            // Configure PaymentType entity
            modelBuilder.Entity<SMS.Generic.Entities.Entities.PaymentType>(entity =>
            {
                entity.ToTable("PaymentType", "dbo");
                entity.HasKey(e => e.PaymentTypeID);
                entity.Property(e => e.PaymentTypeID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.Property(e => e.DepartmentID).IsRequired();

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                // Foreign Key to Employee table for DeletedUserID
                entity.HasOne(e => e.DeletedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.DeletedUserID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnType("varchar(100)");
                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.EditedBy).HasMaxLength(255);
                entity.Property(e => e.DeletedBy).HasMaxLength(255);
            });

            // Configure EmailTemplate entity
            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.ToTable("EmailTemplate", "dbo");
                entity.HasKey(e => e.EmailTemplateID);
                entity.Property(e => e.EmailTemplateID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.Property(e => e.Title)
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");
                entity.Property(e => e.Subject)
                      .HasMaxLength(500)
                      .HasColumnType("nvarchar(500)");
                entity.Property(e => e.Content)
                      .HasColumnType("nvarchar(max)");

                entity.Property(e => e.EmailType)
                      .HasColumnType("int");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.EditedBy).HasMaxLength(255);
                entity.Property(e => e.DeletedBy).HasMaxLength(255);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasIndex(e => e.Title)
                      .IsUnique()
                      .HasDatabaseName("IX_EmailTemplate_Title");
            });

            // Configure DepartmentActivityType entity
            modelBuilder.Entity<DepartmentActivityType>(entity =>
            {
                entity.ToTable("DepartmentActivityType", "dbo");

                // Primary Key
                entity.HasKey(e => e.DepartmentActivityTypeId);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.DepartmentActivityTypeId)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string properties max lengths
                entity.Property(e => e.ActivityTypeShort)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.ActivityTypeLong)
                      .HasMaxLength(200)
                      .HasColumnType("varchar(200)");
                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.EditedBy).HasMaxLength(255);
            });

            // Configure EmployeeType entity
            modelBuilder.Entity<EmployeeType>(entity =>
            {
                entity.ToTable("EmployeeType", "dbo");

                // Primary Key
                entity.HasKey(e => e.EmployeeTypeID);

                entity.Property(e => e.EmployeeTypeID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.EmployeeTypeShort)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");

                entity.Property(e => e.EmployeeTypeLong)
                      .HasMaxLength(200)
                      .HasColumnType("varchar(200)");

                entity.Property(e => e.PayRate)
                      .HasColumnType("money");
                entity.Property(e => e.PremiumPayRate)
                      .HasColumnType("money");
                entity.Property(e => e.HolidayPayRate)
                      .HasColumnType("money");

                entity.Property(e => e.Rank)
                      .HasColumnType("int");

                entity.Property(e => e.IsSupervisor)
                      .HasColumnType("bit");

                entity.Property(e => e.IsEligible)
                      .HasColumnType("bit");

                entity.PrimitiveCollection(e => e.SameRankIds)
                      .HasColumnType("nvarchar(max)");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.EditedBy).HasMaxLength(255);
            });

            // Configure BusinessType entity
            modelBuilder.Entity<BusinessType>(entity =>
            {
                entity.ToTable("BusinessType", "dbo");
                entity.HasKey(e => e.BusinessTypeID);
                entity.Property(e => e.BusinessTypeID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("NEWID()");

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string properties max lengths and types
                entity.Property(e => e.BusinessTypeShort)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.BusinessTypeLong)
                      .HasMaxLength(200)
                      .HasColumnType("varchar(200)");
                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.EditedBy).HasMaxLength(255);
            });

            // Configure PayCode entity
            modelBuilder.Entity<PayCode>(entity =>
            {
                entity.ToTable("PayCode", "dbo");
                entity.HasKey(e => e.PayCodeID);
                entity.Property(e => e.PayCodeID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("NEWID()");

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string properties max lengths and types
                entity.Property(e => e.PayCodeValue)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.PayCodeDescription)
                      .HasMaxLength(1000)
                      .HasColumnType("varchar(1000)");
                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.EditedBy).HasMaxLength(255);
            });

            // Configure DepartmentPolicy entity
            modelBuilder.Entity<DepartmentPolicy>(entity =>
            {
                entity.ToTable("DepartmentPolicy", "dbo");

                entity.HasKey(e => e.DepartmentPolicyID);

                entity.Property(e => e.DepartmentPolicyID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("NEWID()");

                // ✅ One-to-one
                entity.HasOne(dp => dp.Department)
                      .WithOne(d => d.DepartmentPolicy)
                      .HasForeignKey<DepartmentPolicy>(dp => dp.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.PostsTakenPerDay).HasDefaultValue(2);
                entity.Property(e => e.HoursPerDayWorked).HasDefaultValue(3);
                entity.Property(e => e.HoursPerWeekWorked).HasDefaultValue(20);
                entity.Property(e => e.HoursPerMonthWorked).HasDefaultValue(60);
                entity.Property(e => e.DeptOpenPostsTakenPerDay).HasDefaultValue(2);
                entity.Property(e => e.TenurePostsAllowedPerWeek).HasDefaultValue(3);
                entity.Property(e => e.TenurePostsAllowedPerDay).HasDefaultValue(2);
                entity.Property(e => e.DeptOpenPostsTakenPerWeek).HasDefaultValue(5);

                entity.Property(e => e.MinHoursPaid).HasDefaultValue(4);
                entity.Property(e => e.PositionTakenOnSameday).HasDefaultValue((short)1);
                entity.Property(e => e.NumberOfEmployeesBeforeSupervisorRequired).HasDefaultValue(5);

                entity.Property(e => e.IsPostsTakenPerDay).HasDefaultValue(false);
                entity.Property(e => e.IsHoursPerDayWorked).HasDefaultValue(false);
                entity.Property(e => e.IsHoursPerWeekWorked).HasDefaultValue(false);
                entity.Property(e => e.IsHoursPerMonthWorked).HasDefaultValue(false);
                entity.Property(e => e.IsDeptOpenPostsTakenPerDay).HasDefaultValue(false);
                entity.Property(e => e.IsTenurePostsAllowedPerWeek).HasDefaultValue(false);
                entity.Property(e => e.IsTenurePostsAllowedPerDay).HasDefaultValue(false);
                entity.Property(e => e.IsDeptOpenPostsTakenPerWeek).HasDefaultValue(false);
                entity.Property(e => e.IsMinHoursPaid).HasDefaultValue(false);
                entity.Property(e => e.IsPositionTakenOnSameday).HasDefaultValue(false);

                entity.Property(e => e.OverlapJobConflictConfirmation).HasDefaultValue(false);
                entity.Property(e => e.SupervisorsCanViewLowerEmpTypeJobs).HasDefaultValue(false);
                entity.Property(e => e.RestrictWebLoginForNonAdminEmployee).HasDefaultValue(false);

                // Configure Tenure Policy
                entity.Property(p => p.TenureType)
                      .HasConversion<int>();

                entity.Property(p => p.TenureRulesJson)
                      .HasColumnType("nvarchar(max)");

                // Configure relationship to DepartmentConfiguration
                // Both entities share the same DepartmentID (foreign key to Department)
                // This allows EF Core to join them using the shared DepartmentID
                // Note: DepartmentConfiguration.DepartmentID remains FK to Department, this is just for query navigation
                entity.HasOne(dp => dp.DepartmentConfiguration)
                      .WithOne()
                      .HasPrincipalKey<DepartmentPolicy>(dp => dp.DepartmentID)
                      .HasForeignKey<DepartmentConfiguration>(dc => dc.DepartmentID)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure EquipmentType entity
            modelBuilder.Entity<EquipmentType>(entity =>
            {
                entity.ToTable("EquipmentType", "dbo");
                entity.HasKey(e => e.EquipementTypeID);
                entity.Property(e => e.EquipementTypeID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("NEWID()");

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedUser)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedUser)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DepartmentResponseArea>(entity =>
            {
                entity.ToTable("DepartmentResponseArea", "dbo");
                entity.HasKey(e => e.DepartmentResponseAreaID);
                entity.Property(e => e.DepartmentResponseAreaID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string properties max lengths and types
                entity.Property(e => e.DepartmentResponseAreaShort)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.DepartmentResponseAreaLong)
                      .HasMaxLength(200)
                      .HasColumnType("varchar(200)");
                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.EditedBy).HasMaxLength(255);
            });

            // Configure Document entity if needed
            modelBuilder.Entity<DepartmentDivisions>(entity =>
            {
                entity.ToTable("DepartmentDivisions", "dbo");
                entity.HasKey(e => e.DivisionID);
                entity.Property(e => e.DivisionID)
                     .ValueGeneratedOnAdd()
                     .HasDefaultValueSql("NEWID()");
                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedUser)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedUser)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Equipment entity
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.ToTable("Equipement", "dbo");
                entity.HasKey(e => e.EquipementID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.EquipementID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to EquipmentType table for EquipementTypeID
                entity.HasOne(e => e.EquipementType)
                      .WithMany()
                      .HasForeignKey(e => e.EquipementTypeID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Admin table for CreatedUserID (AddedByID)
                entity.HasOne(e => e.CreateByAdmin)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Admin table for EditedUserID (UserEditingID)
                entity.HasOne(e => e.EditedbyAdmin)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string properties max lengths
                entity.Property(e => e.EquipementShort)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.EquipementLong)
                      .HasMaxLength(200)
                      .HasColumnType("varchar(200)");

                // Configure money type for MileageCharge
                entity.Property(e => e.MileageCharge)
                      .HasColumnType("decimal");

                // Configure default values according to schema
                entity.Property(e => e.ChargeMileage)
                      .HasDefaultValue(false); // Default: FALSE

                entity.Property(e => e.IsReimburseable)
                      .HasDefaultValue(false); // Default: FALSE

                entity.Property(e => e.ReimburseTo)
                      .HasDefaultValue(0); // Default: city (assuming 0 = city, 1 = officer)
            });

            // Configure Requester entity
            modelBuilder.Entity<Requester>(entity =>
            {
                entity.ToTable("Requester", "dbo");
                entity.HasKey(e => e.RequesterID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.RequesterID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.RequesterCode)
                      .HasMaxLength(20)
                      .IsRequired()
                      .HasDefaultValueSql("FORMAT(NEXT VALUE FOR [dbo].[Seq_RequesterCode], '000000')")
                      .ValueGeneratedOnAdd()
                      .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.HasIndex(e => e.RequesterCode).IsUnique();

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to BusinessType table for BusinessTypeID
                entity.HasOne(e => e.BusinessType)
                      .WithMany()
                      .HasForeignKey(e => e.BusinessTypeID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to DepartmentQuestionnaire table for DepartmentQuestionnaireId
                entity.HasOne(e => e.DepartmentQuestionnaire)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentQuestionnaireId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Requester table for ParentID (self-referencing)
                // ParentID is a foreign key that references RequesterID (primary key) in the same table
                entity.HasOne(e => e.Parent)
                      .WithMany(e => e.Children)
                      .HasForeignKey(e => e.ParentID)  // ParentID is the foreign key
                      .HasPrincipalKey(e => e.RequesterID)  // References RequesterID (primary key)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string properties max lengths
                entity.Property(e => e.DepartmentDefinedNumber)
                      .HasMaxLength(25)
                      .HasColumnType("varchar(25)");
                entity.Property(e => e.Name)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.Address)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.Address2)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.City)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.State)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.Zip)
                      .HasMaxLength(10)
                      .HasColumnType("varchar(10)");
                entity.Property(e => e.Email)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.Phone)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.PrimaryContact)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.Fax)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.SecondaryContact)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.SecondaryPhone)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.InvoiceBillingName)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.InvoiceContact)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.InvoiceAddress)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.InvoiceAddress2)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.InvoiceCity)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.InvoiceState)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.InvoiceZip)
                      .HasMaxLength(10)
                      .HasColumnType("varchar(10)");
                entity.Property(e => e.InvoicePhone)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.InvoicePhoneExt)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.InvoiceEmail)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.InvoiceFax)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.LiquorLicense)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.BillingNotes)
                      .HasMaxLength(200)
                      .HasColumnType("varchar(200)");
                entity.Property(e => e.Comments)
                      .HasMaxLength(200)
                      .HasColumnType("varchar(200)");
                entity.Property(e => e.NotificationRecipients)
                      .HasMaxLength(200)
                      .HasColumnType("varchar(200)");

                // Configure default values according to schema
                entity.Property(e => e.IsLiquorLicenseRequired)
                      .HasDefaultValue(false);
                entity.HasMany(r => r.RequesterManagers)
                        .WithOne(rm => rm.Requester)
                        .HasForeignKey(rm => rm.RequesterID)
                        .OnDelete(DeleteBehavior.Restrict);

            });

            // Configure RequesterConfig entity
            modelBuilder.Entity<RequesterConfig>(entity =>
            {
                entity.ToTable("RequesterConfig", "dbo");
                entity.HasKey(e => e.RequesterConfigID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.RequesterConfigID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Requester table for RequesterID
                entity.HasOne(e => e.Requester)
                      .WithMany()
                      .HasForeignKey(e => e.RequesterID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string properties max lengths
                entity.Property(e => e.DefinedSkills)
                      .HasMaxLength(255)
                      .HasColumnType("varchar(255)");

                entity.Property(e => e.DefineRequesterManager)
                      .HasMaxLength(255)
                      .HasColumnType("varchar(255)");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.EditedBy).HasMaxLength(255);

                // Configure default values for boolean fields
                entity.Property(e => e.EnableSkills)
                      .HasDefaultValue(false);

                entity.Property(e => e.EnableRequesterManager)
                      .HasDefaultValue(false);

                entity.Property(e => e.EnablePreEmploymentPaperwork)
                      .HasDefaultValue(false);

                entity.Property(e => e.EnableActivityReporting)
                      .HasDefaultValue(false);

                entity.Property(e => e.EnableRestrictedEmployeeList)
                      .HasDefaultValue(false);

                entity.Property(e => e.EnableParentChildInvoicing)
                      .HasDefaultValue(false);

                entity.Property(e => e.EnableExemptFromInvoicing)
                      .HasDefaultValue(false);

                entity.Property(e => e.EnableExemptFromPayroll)
                      .HasDefaultValue(false);

                entity.Property(e => e.EnableExemptFromHoursRules)
                      .HasDefaultValue(false);
            });

            // Configure RequesterRestrictedList entity
            modelBuilder.Entity<RequesterRestrictedList>(entity =>
            {
                entity.ToTable("RequesterRestrictedList", "dbo");
                entity.HasKey(e => e.RequesterRestrictID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.RequesterRestrictID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Requester table for RequesterID
                entity.HasOne(e => e.Requester)
                      .WithMany()
                      .HasForeignKey(e => e.RequesterID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Employee table for EmployeeID
                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Admin table for EnteredByAdminID
                entity.HasOne(e => e.EnteredByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Admin table for UpdatedByAdminID
                entity.HasOne(e => e.UpdatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure date types
                entity.Property(e => e.EffectiveStartDate)
                      .HasColumnType("date");

                entity.Property(e => e.EffectiveEndDate)
                      .HasColumnType("date");
            });

            // Configure RequesterAuthorized entity
            modelBuilder.Entity<RequesterAuthorized>(entity =>
            {
                entity.ToTable("RequesterAuthorized", "dbo");
                entity.HasKey(e => e.RequesterAuthorizedID);

                entity.Property(e => e.RequesterAuthorizedID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Requester)
                      .WithMany()
                      .HasForeignKey(e => e.RequesterID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.AddedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.FirstName)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.LastName)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.PrimaryPhone)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.SecondaryPhone)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.PrimaryEmail)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");
                entity.Property(e => e.SecondaryEmail)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)");

                entity.Property(e => e.Status)
                      .HasColumnType("bit");
            });

            // Configure EmployeeTenure entity
            modelBuilder.Entity<EmployeeTenure>(entity =>
            {
                entity.ToTable("EmployeeTenure", "dbo");
                entity.HasKey(e => e.EmployeeTenureID);

                entity.Property(e => e.EmployeeTenureID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.EmployeeType)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeTypeID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.AddedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.DateOfHire)
                      .HasColumnType("date");

                entity.Property(e => e.DateOfPromotion)
                      .HasColumnType("date");

                entity.Property(e => e.IsCurrent)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);
            });

            // Configure DepartmentEquipment entity
            modelBuilder.Entity<DepartmentEquipment>(entity =>
            {
                entity.ToTable("DepartmentEquipment", "dbo");
                entity.HasKey(e => e.DepartmentEquipmentID);

                entity.Property(e => e.DepartmentEquipmentID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.EquipmentType)
                      .WithMany()
                      .HasForeignKey(e => e.EquipmentTypeID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.AddedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Charge)
                      .HasColumnType("money");

                entity.Property(e => e.MileageCharge)
                      .HasColumnType("money");

                entity.Property(e => e.IsDefault)
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.EquipmentStatus)
                      .HasDefaultValue(true);
            });

            // Configure RequesterManager entity
            modelBuilder.Entity<RequesterManager>(entity =>
            {
                entity.ToTable("RequesterManager", "dbo");
                entity.HasKey(e => e.RequesterManagerID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.RequesterManagerID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Requester)
                      .WithMany()
                      .HasForeignKey(e => e.RequesterID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.Property(e => e.IsEmployeeShared)
                      .HasDefaultValue(false);

                entity.Property(e => e.IsRequestShared)
                      .HasDefaultValue(false);

                entity.HasOne(e => e.AddedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rm => rm.Requester)
                      .WithMany(r => r.RequesterManagers)
                      .HasForeignKey(rm => rm.RequesterID)
                      .OnDelete(DeleteBehavior.Restrict);

                // 🔥 Manager → Employees (IMPORTANT)
                entity.HasMany(rm => rm.RequesterEmployees)
                      .WithOne(re => re.RequesterManager)
                      .HasForeignKey(re => re.RequesterManagerID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure RequesterEmployee entity
            modelBuilder.Entity<RequesterEmployee>(entity =>
            {
                entity.ToTable("RequesterEmployee", "dbo");
                entity.HasKey(e => e.RequesterEmployeeID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.RequesterEmployeeID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.IsPreferred)
                      .HasDefaultValue(false);

                // Foreign Key to RequesterManager table for RequesterManagerID
                entity.HasOne(e => e.RequesterManager)
                      .WithMany()
                      .HasForeignKey(e => e.RequesterManagerID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Employee table for EmployeeID
                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.AddedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(re => re.RequesterManager)
                      .WithMany(rm => rm.RequesterEmployees)
                      .HasForeignKey(re => re.RequesterManagerID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure EmployeeDeviceToken entity
            modelBuilder.Entity<EmployeeDeviceToken>(entity =>
            {
                entity.ToTable("EmployeeDeviceToken", "dbo");
                entity.HasKey(e => e.EmployeeDeviceTokenID);
                entity.Property(e => e.EmployeeDeviceTokenID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();
            });

            // Configure HolidayDay entity
            modelBuilder.Entity<HolidayDay>(entity =>
            {
                entity.ToTable("HolidayDay", "dbo");
                entity.HasKey(e => e.HolidayDayID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.HolidayDayID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.AddedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string properties max lengths
                entity.Property(e => e.Name)
                      .HasMaxLength(100)
                      .HasColumnType("nvarchar(100)");

                // Configure date property
                entity.Property(e => e.Date)
                      .HasColumnType("date");
            });

            // Configure Request entity
            modelBuilder.Entity<Request>(entity =>
            {
                entity.ToTable("Request", "dbo");
                entity.HasKey(e => e.RequestID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.RequestID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.RequestCode)
                      .HasMaxLength(20)
                      .IsRequired()
                      .HasDefaultValueSql("FORMAT(NEXT VALUE FOR [dbo].[Seq_RequestCode], '000000')")
                      .ValueGeneratedOnAdd()
                      .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.HasIndex(e => e.RequestCode).IsUnique();

                entity.HasOne(e => e.Requester)
                      .WithMany()
                      .HasForeignKey(e => e.RequesterID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RequesterManager)
                      .WithMany()
                      .HasForeignKey(e => e.RequesterManagerID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RequestType)
                      .WithOne(rt => rt.Request)
                      .HasForeignKey<RequestType>(rt => rt.RequestID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.LiquorLicenseNumber)
                      .HasMaxLength(50);

                entity.Property(e => e.PermitNumber)
                      .HasMaxLength(50);

                entity.HasOne(e => e.DepartmentResponseArea)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentResponseAreaID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.IsHoursPolicyExempt)
                      .HasColumnType("bit")
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.IsPremiumPaidEvent)
                      .HasColumnType("bit")
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.EnableFees)
                      .HasColumnType("bit")
                      .HasDefaultValue(true)
                      .IsRequired();

                entity.Property(e => e.AlcoholPermitRequired)
                      .HasColumnType("bit")
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.SpecialRate)
                      .HasColumnType("money")
                      .HasDefaultValue(0m)
                      .IsRequired();

                entity.Property(e => e.ProjectNumber)
                      .HasMaxLength(50);

                entity.HasIndex(e => new { e.DepartmentID, e.RequesterID });
                entity.HasIndex(e => new { e.DepartmentID, e.StartDate, e.EndDate });
            });
            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post", "dbo");
                entity.HasKey(p => p.PostID);
                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.PostID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.PostCode)
                      .HasMaxLength(20)
                      .IsRequired()
                      .HasDefaultValueSql("FORMAT(NEXT VALUE FOR [dbo].[Seq_PostCode], '000000')")
                      .ValueGeneratedOnAdd()
                      .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.HasIndex(e => e.PostCode).IsUnique();

                // Request
                entity.HasOne(p => p.Request)
                      .WithMany(r => r.Posts) // 👈 THIS is the key fix
                      .HasForeignKey(p => p.RequestID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Self-reference
                entity.HasOne(p => p.ParentPost)
                      .WithMany(p => p.ChildPosts)
                      .HasForeignKey(p => p.PostParentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // DepartmentActivityType
                entity.HasOne(p => p.DepartmentActivityType)
                      .WithMany()
                      .HasForeignKey(p => p.DepartmentActivityTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Audit → Employee
                entity.HasOne(p => p.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(p => p.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(p => p.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // EmployeeType relation
                entity.HasOne(p => p.EmployeeType)
                      .WithMany() // or .WithMany(e => e.Posts) if you add collection there
                      .HasForeignKey(p => p.EmployeeTypeID)
                      .OnDelete(DeleteBehavior.Restrict);

                // DepartmentResponseArea relation
                entity.HasOne(p => p.DepartmentResponseArea)
                      .WithMany()
                      .HasForeignKey(p => p.PostResponseAreaId)
                      .OnDelete(DeleteBehavior.Restrict);

                // PayCode relation
                entity.HasOne(p => p.PayCode)
                      .WithMany()
                      .HasForeignKey(p => p.PayCodeID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure IsParent property
                entity.Property(e => e.IsParent)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.IsAlcoholServedOrConsumed)
                      .HasColumnType("bit")
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.IsPremiumPaidEvent)
                      .HasColumnType("bit")
                      .HasDefaultValue(false)
                      .IsRequired();
            });

            modelBuilder.Entity<PostPosition>(entity =>
            {
                entity.ToTable("PostPosition", "dbo");

                entity.HasKey(e => e.PostPositionID);

                entity.Property(e => e.PostPositionID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.Property(e => e.PostPositionCode)
                      .HasMaxLength(20)
                      .IsRequired()
                      .HasDefaultValueSql("FORMAT(NEXT VALUE FOR [dbo].[Seq_PostpositionCode], '000000')")
                      .ValueGeneratedOnAdd()
                      .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.HasIndex(e => e.PostPositionCode).IsUnique();

                entity.HasIndex(e => new { e.DateOfWork, e.PostID });

                // 🔹 FK → Post
                entity.HasOne(e => e.Post)
                      .WithMany(p => p.PostPositions)
                      .HasForeignKey(e => e.PostID)
                      .OnDelete(DeleteBehavior.Restrict);

                // 🔹 FK → Employee
                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict);

                // 🔹 AUDIT → Employee
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.PostPositionStatus)
                      .HasConversion<int>()
                      .HasDefaultValue(PostPositionStatus.Assigned)
                      .HasSentinel((PostPositionStatus)0)
                      .IsRequired();

                entity.Property(e => e.ReasonForCancel)
                      .HasMaxLength(500);

                entity.Property(e => e.PostPositionPayRate)
                      .HasColumnType("money")
                      .IsRequired();

                entity.Property(e => e.IsPremiumPaidEvent)
                      .HasColumnType("bit")
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.IsAllocationFinalized)
                      .HasColumnType("bit")
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.HasOne(e => e.ParentPostPosition)
                      .WithMany()
                      .HasForeignKey(e => e.ParentPostPositionId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.SplitPostPosition)
                      .WithMany()
                      .HasForeignKey(e => e.SplitPostPositionID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.ClosedPostPositions)
                      .WithOne(e => e.PostPosition)
                      .HasForeignKey(e => e.PostPositionID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Placeholder)
                      .WithMany(p => p.PostPositions)
                      .HasForeignKey(e => e.PlaceholderID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PostCallSign)
                      .WithMany()
                      .HasForeignKey(e => e.PostCallSignID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Placeholder>(entity =>
            {
                entity.ToTable("Placeholder", "dbo");
                entity.HasKey(e => e.PlaceholderID);
                entity.Property(e => e.PlaceholderID).ValueGeneratedOnAdd().HasDefaultValueSql("newid()");
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ContactNumber).HasMaxLength(50);
                entity.Property(e => e.EmployeeNumber).HasMaxLength(50);
                entity.Property(e => e.DepartmentID).IsRequired();
                entity.Property(e => e.IsUniquePerPosition).HasDefaultValue(false);
                entity.Property(e => e.PlaceholderStatus)
                      .HasConversion<int>()
                      .HasDefaultValue(PlaceholderStatus.Visible)
                      .HasSentinel((PlaceholderStatus)(-1))
                      .IsRequired();

                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<WaitingPostPosition>(entity =>
            {
                entity.ToTable("WaitingPostPosition", "dbo");

                entity.HasKey(e => e.WaitingPostPositionID);

                entity.Property(e => e.WaitingPostPositionID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.Property(e => e.DateOfWork)
                      .HasColumnType("date")
                      .IsRequired();

                // 🔹 FK → Post (OPTIONAL)
                entity.HasOne(e => e.Post)
                      .WithMany(p => p.WaitingPostPosition)
                      .HasForeignKey(e => e.PostID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                // 🔹 FK → Request (OPTIONAL)
                entity.HasOne(e => e.Request)
                      .WithMany(r => r.WaitingPostPosition) // ✅ better navigation
                      .HasForeignKey(e => e.RequestID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                // 🔹 FK → Employee
                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict);

                // 🔹 AUDIT
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.PostPositionStatus)
                      .HasConversion<int>()
                      .HasDefaultValue(PostPositionStatus.Assigned)
                      .HasSentinel((PostPositionStatus)0)
                      .IsRequired();

                entity.Property(e => e.OrderNumber).IsRequired();

                entity.Property(e => e.ProcessingStatus)
                      .HasDefaultValue(ProcessingStatus.Pending);

                entity.Property(e => e.EquipmentRequired)
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.CallSign)
                      .HasMaxLength(50);

                entity.Property(e => e.WaitingPostPositionPayRate)
                      .HasColumnType("money")
                      .IsRequired();

                entity.Property(e => e.IsPremiumPaidEvent)
                      .HasColumnType("bit")
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.HasOne(e => e.PostCallSign)
                      .WithMany()
                      .HasForeignKey(e => e.PostCallSignID)
                      .OnDelete(DeleteBehavior.Restrict);

                // ✅✅ IMPORTANT: REMOVE OLD INDEX (if exists in model)

                // ✅ Post-based unique index — only active Waiting (10) rows; cancelled/historical rows are excluded
                entity.HasIndex(e => new
                {
                    e.EmployeeID,
                    e.PostID,
                    e.DateOfWork,
                })
                .IsUnique()
                .HasFilter("[PostID] IS NOT NULL AND [PostPositionStatus] = 10");

                // ✅ Request-based unique index — only active SpecialEvent (15) rows; cancelled/historical rows are excluded
                entity.HasIndex(e => new
                {
                    e.EmployeeID,
                    e.RequestID,
                    e.DateOfWork,
                })
                .IsUnique()
                .HasFilter("[RequestID] IS NOT NULL AND [PostPositionStatus] = 15");

                // ✅ Prevent invalid rows (VERY IMPORTANT)
                entity.HasCheckConstraint(
                    "CK_WaitingPostPosition_PostOrRequest",
                    "[PostID] IS NOT NULL OR [RequestID] IS NOT NULL"
                );
            });
            // RequestEquipment relationships
            modelBuilder.Entity<RequestEquipment>(entity =>
            {
                entity.HasOne(e => e.Request)
                      .WithMany()
                      .HasForeignKey(e => e.RequestID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DepartmentEquipment)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentEquipmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.IsVehicle)
                      .HasDefaultValue(false)
                      .IsRequired();
            });

            // UploadFile relationships
            modelBuilder.Entity<UploadFile>(entity =>
            {
                entity.HasKey(e => e.FileUploadedId);

                entity.HasOne(e => e.Request)
                      .WithMany()
                      .HasForeignKey(e => e.RequestID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TypeOfFile).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FileContent).HasMaxLength(-1);
            });

            // PostCallSign relationships
            modelBuilder.Entity<PostCallSign>(entity =>
            {
                entity.HasOne(e => e.Post)
                      .WithMany()
                      .HasForeignKey(e => e.PostID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // PostEquipment relationships
            modelBuilder.Entity<PostEquipment>(entity =>
            {
                entity.HasOne(e => e.Post)
                      .WithMany()
                      .HasForeignKey(e => e.PostID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DepartmentEquipment)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentEquipmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // PostPositionVehicle relationships
            modelBuilder.Entity<PostPositionVehicle>(entity =>
            {
                entity.HasOne(e => e.PostPosition)
                      .WithMany()
                      .HasForeignKey(e => e.PostPositionID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DepartmentEquipment)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentEquipmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            // ClosedPostPositionEquipment relationships
            modelBuilder.Entity<ClosedPostPositionEquipment>(entity =>
            {
                entity.ToTable("ClosedPostPositionEquipment", "dbo");
                entity.HasKey(e => e.ClosedPostPositionEquipmentID);

                entity.HasOne(e => e.ClosedPostPosition)
                      .WithMany()
                      .HasForeignKey(e => e.ClosedPostPositionID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DepartmentEquipment)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentEquipmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Post)
                      .WithMany()
                      .HasForeignKey(e => e.PostID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.EquipmentUsedIsVehicle)
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.IsWorked)
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.IsUnPaidBreakTime)
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.PayRate).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ReimbursePayRate).HasColumnType("decimal(18, 2)");
            });

            // ClosedPostPosition configuration
            modelBuilder.Entity<ClosedPostPosition>(entity =>
            {
                entity.ToTable("ClosedPostPosition", "dbo");
                entity.HasKey(e => e.ClosedPostPositionID);

                entity.Property(e => e.Comment).HasMaxLength(1000);
                entity.Property(e => e.CloseBy).HasMaxLength(255);

                entity.HasOne(e => e.Post)
                      .WithMany()
                      .HasForeignKey(e => e.PostID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.QuestionaireResponse)
                      .WithMany()
                      .HasForeignKey(e => e.QuestionaireResponseID)
                      .HasPrincipalKey(r => r.ResponseId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Self-referencing FK: ParentClosedPostPositionID -> ClosedPostPositionID
                entity.HasOne(e => e.ParentClosedPostPosition)
                      .WithMany()
                      .HasForeignKey(e => e.ParentClosedPostPositionID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.IsMinimumDue).HasDefaultValue(false).IsRequired();
                entity.Property(e => e.QuestionaireRequired).HasDefaultValue(false).IsRequired();
            });

            // CancelledPostPosition configuration
            modelBuilder.Entity<CancelledPostPosition>(entity =>
            {
                entity.ToTable("CancelledPostPosition", "dbo");
                entity.HasKey(e => e.CancelledPostPositionID);
                entity.Property(e => e.CancelledPostPositionID).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.CancelledBy).HasMaxLength(255);
                entity.Property(e => e.IPAddressCancelledBy).HasMaxLength(255);
                entity.Property(e => e.ReimburseStatus).HasMaxLength(100);
                entity.Property(e => e.PayRate).HasColumnType("smallmoney");
                entity.Property(e => e.ReimbursePayRate).HasColumnType("smallmoney");
                entity.Property(e => e.Compensation).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Mileage).HasColumnType("decimal(18, 2)");

                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Post)
                      .WithMany()
                      .HasForeignKey(e => e.PostID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PostPosition)
                      .WithMany()
                      .HasForeignKey(e => e.PostPositionID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DepartmentEquipment)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentEquipmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.QuestionaireResponse)
                      .WithMany()
                      .HasForeignKey(e => e.QuestionaireResponseID)
                      .HasPrincipalKey(r => r.ResponseId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.QuestionaireRequired).HasDefaultValue(false).IsRequired();
            });

            modelBuilder.Entity<DepartmentDefinedFields>(entity =>
            {
                entity.HasKey(x => x.DepartmentDefinedFieldID);

                entity.HasOne(x => x.Department)
                      .WithMany()
                      .HasForeignKey(x => x.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.RequestDepartmentDefinedFieldInterfaceLabel1).HasMaxLength(100);
                entity.Property(x => x.RequestDepartmentDefinedFieldInterfaceLabel2).HasMaxLength(100);
                entity.Property(x => x.RequestDepartmentDefinedFieldInterfaceLabel3).HasMaxLength(100);
                entity.Property(x => x.RequestDepartmentDefinedFieldInterfaceLabel4).HasMaxLength(100);
                entity.Property(x => x.RequestDepartmentDefinedFieldInterfaceLabel5).HasMaxLength(100);
                entity.Property(x => x.RequestDepartmentDefinedFieldInterfaceLabel6).HasMaxLength(100);
                entity.Property(x => x.RequestDepartmentDefinedFieldInterfaceLabel7).HasMaxLength(100);
                entity.Property(x => x.RequestDepartmentDefinedFieldInterfaceLabel8).HasMaxLength(100);
                entity.Property(x => x.RequestDepartmentDefinedFieldInterfaceLabel9).HasMaxLength(100);
                entity.Property(x => x.RequestDepartmentDefinedFieldInterfaceLabel10).HasMaxLength(100);

                entity.Property(x => x.PostDepartmentDefinedFieldInterfaceLabel1).HasMaxLength(100);
                entity.Property(x => x.PostDepartmentDefinedFieldInterfaceLabel2).HasMaxLength(100);
                entity.Property(x => x.PostDepartmentDefinedFieldInterfaceLabel3).HasMaxLength(100);
                entity.Property(x => x.PostDepartmentDefinedFieldInterfaceLabel4).HasMaxLength(100);
                entity.Property(x => x.PostDepartmentDefinedFieldInterfaceLabel5).HasMaxLength(100);
                entity.Property(x => x.PostDepartmentDefinedFieldInterfaceLabel6).HasMaxLength(100);
                entity.Property(x => x.PostDepartmentDefinedFieldInterfaceLabel7).HasMaxLength(100);
                entity.Property(x => x.PostDepartmentDefinedFieldInterfaceLabel8).HasMaxLength(100);
                entity.Property(x => x.PostDepartmentDefinedFieldInterfaceLabel9).HasMaxLength(100);
                entity.Property(x => x.PostDepartmentDefinedFieldInterfaceLabel10).HasMaxLength(100);
            });

            // Configure RoutingGroup entity
            modelBuilder.Entity<RoutingGroup>(entity =>
            {
                entity.HasKey(e => e.RoutingGroupId);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.RoutingGroupId)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Configure string properties max lengths
                entity.Property(e => e.ApproverLong)
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.Property(e => e.ApproverShort)
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(e => e.ApproverEmail)
                      .HasMaxLength(150)
                      .HasColumnType("nvarchar(150)");

                entity.Property(e => e.ApprovalType)
                      .HasDefaultValue(ApprovalType.SpecialEventApproval);
            });

            // Configure VehicleApprovalRouting entity
            modelBuilder.Entity<VehicleApprovalRouting>(entity =>
            {
                entity.HasKey(e => e.VehicleApprovalRouteID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.VehicleApprovalRouteID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to RoutingGroup table for ApproverID
                entity.HasOne(e => e.Approver)
                      .WithMany()
                      .HasForeignKey(e => e.ApproverID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to DepartmentEquipment table for DepartmentEquipmentID
                entity.HasOne(e => e.DepartmentEquipment)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentEquipmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Create unique index on DepartmentEquipmentID and Rank
                entity.HasIndex(e => new { e.DepartmentEquipmentID, e.Rank })
                      .IsUnique()
                      .HasDatabaseName("IX_VehicleApprovalRouting_DepartmentEquipmentID_Rank");
            });

            // Configure RequestApprovalRouting entity
            modelBuilder.Entity<RequestApprovalRouting>(entity =>
            {
                entity.HasKey(e => e.RequestApprovalRouteID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.RequestApprovalRouteID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to RoutingGroup table for ApproverID
                entity.HasOne(e => e.Approver)
                      .WithMany()
                      .HasForeignKey(e => e.ApproverID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Request table for RequestID
                entity.HasOne(e => e.Request)
                      .WithMany()
                      .HasForeignKey(e => e.RequestID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Create unique index on RequestID and Rank
                entity.HasIndex(e => new { e.RequestID, e.Rank })
                      .IsUnique()
                      .HasDatabaseName("IX_RequestApprovalRouting_RequestID_Rank");
            });

            // Configure PermitApprovalRouting entity
            modelBuilder.Entity<PermitApprovalRouting>(entity =>
            {
                entity.HasKey(e => e.PermitApprovalRoutingID);

                entity.Property(e => e.PermitApprovalRoutingID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Request)
                      .WithMany()
                      .HasForeignKey(e => e.RequestID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.Approver)
                      .WithMany()
                      .HasForeignKey(e => e.RoutingGroupId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ApprovalDetail entity (table: ApprovalDetails)
            modelBuilder.Entity<ApprovalDetail>(entity =>
            {
                entity.HasKey(e => e.ApprovalDetailID);

                entity.Property(e => e.ApprovalDetailID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.Request)
                      .WithMany()
                      .HasForeignKey(e => e.RequestID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.VehicleApproverComments)
                      .HasMaxLength(1000);

                entity.Property(e => e.Type)
                      .HasDefaultValue(ApprovalType.SpecialEventApproval);
            });

            // Configure DepartmentConfiguration entity
            modelBuilder.Entity<DepartmentConfiguration>(entity =>
            {
                entity.HasKey(e => e.DeptConfigID);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.DeptConfigID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(dc => dc.Department)
                      .WithOne(d => d.DepartmentConfiguration)
                      .HasForeignKey<DepartmentConfiguration>(dc => dc.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);
                // DepartmentID must be unique for one-to-one relationship with Department
                entity.HasIndex(dc => dc.DepartmentID).IsUnique();
                // Configure smallmoney properties
                entity.Property(e => e.DefaultPayRate)
                      .HasColumnType("smallmoney");

                entity.Property(e => e.AdminFeeRate)
                      .HasColumnType("smallmoney");

                entity.Property(e => e.DefaultVehicleRate)
                      .HasColumnType("smallmoney");

                entity.Property(e => e.EnableGeneralPolicy)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.EnableTenurePolicy)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.EnablePayrollCronJob)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.ShowPtaeoCode)
                      .HasColumnType("bit")
                      .HasDefaultValue(true);

                entity.Property(e => e.ShowPayCode)
                      .HasColumnType("bit")
                      .HasDefaultValue(true);

                entity.Property(e => e.EnableEmployeeLocation)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.DepartmentHomeLocation)
                      .HasMaxLength(500);

                entity.Property(e => e.DepartmentHomeLatitude)
                      .HasColumnType("decimal(18, 8)");

                entity.Property(e => e.DepartmentHomeLongitude)
                      .HasColumnType("decimal(18, 8)");

                entity.Property(e => e.ShowRestrictWebLoginForNormalEmployee)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.EnableNormalEmployeeOneSession)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);
            });

            // Configure DepartmentFee entity
            modelBuilder.Entity<DepartmentFee>(entity =>
            {
                entity.HasKey(e => e.FeeId);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.FeeId)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Employee table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.IsAdditionalFees)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.IsActive)
                      .HasColumnType("bit")
                      .HasDefaultValue(true);

                entity.Property(e => e.DefaultFeeConditions)
                      .HasColumnType("int");

                // Configure smallmoney properties
                entity.Property(e => e.DefaultFee)
                      .HasColumnType("smallmoney");

                entity.Property(e => e.AdditionalFee)
                      .HasColumnType("smallmoney");

                // Configure string properties max lengths
                entity.Property(e => e.FeeShort)
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(e => e.FeeLong)
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.Property(e => e.IsDefault)
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.EffectiveStartDate)
                      .HasColumnType("date");

                entity.Property(e => e.EffectiveEndDate)
                      .HasColumnType("date");
            });

            modelBuilder.Entity<RequestDepartmentFee>(entity =>
            {
                entity.HasKey(e => e.RequestDepartmentFeeID);

                entity.Property(e => e.RequestDepartmentFeeID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Request)
                      .WithMany()
                      .HasForeignKey(e => e.RequestID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DepartmentFee)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentFeeID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EmployeePostLocation>(entity =>
            {
                entity.ToTable("EmployeePostLocation", "dbo");
                entity.HasKey(e => e.EmployeePostLocationID);

                entity.Property(e => e.EmployeePostLocationID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.Latitude)
                      .HasColumnType("decimal(9,6)")
                      .IsRequired();

                entity.Property(e => e.Longitude)
                      .HasColumnType("decimal(9,6)")
                      .IsRequired();

                entity.Property(e => e.CapturedAt)
                      .HasColumnType("datetime2")
                      .IsRequired();

                entity.HasIndex(e => new { e.PostPositionID, e.CapturedAt });

                entity.HasOne(e => e.PostPosition)
                      .WithMany()
                      .HasForeignKey(e => e.PostPositionID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure DepartmentCronJobManagement entity
            modelBuilder.Entity<DepartmentCronJobManagement>(entity =>
            {
                entity.ToTable("DepartmentCronJobManagement", "dbo");
                entity.HasKey(e => e.DepartmentCronJobManagementId);
                entity.Property(e => e.DepartmentCronJobManagementId)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.Property(e => e.Job)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(e => e.CronExpression)
                      .HasMaxLength(128)
                      .HasColumnType("nvarchar(128)")
                      .IsRequired();

                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign key to Admin for CreatedUserID / EditedUserID
                entity.HasOne(e => e.CreatedByAdmin)
                      .WithMany(a => a.CreatedDepartmentCronJobManagements)
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByAdmin)
                      .WithMany(a => a.EditedDepartmentCronJobManagements)
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.DepartmentID);
                entity.HasIndex(e => new { e.DepartmentID, e.Job })
                      .IsUnique();
            });

            // Configure Invoice entity
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.InvoicesID);
                entity.Property(e => e.InvoicesID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.HasOne(e => e.Requester)
                      .WithMany()
                      .HasForeignKey(e => e.RequesterID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DeletedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.DeletedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure InvoiceDetail entity
            modelBuilder.Entity<InvoiceDetail>(entity =>
            {
                entity.HasKey(e => e.InvoiceDetailID);
                entity.Property(e => e.InvoiceDetailID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.Property(e => e.FinancialImpactType)
                      .HasConversion<int>()
                      .HasDefaultValue(FinancialImpactType.None)
                      .IsRequired();

                entity.HasOne(e => e.Invoice)
                      .WithMany(e => e.InvoiceDetails)
                      .HasForeignKey(e => e.InvoicesID)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

                entity.HasOne(e => e.ClosePostPosition)
                      .WithMany()
                      .HasForeignKey(e => e.ClosePostPositionID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DeletedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.DeletedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.SpecialRate)
                      .HasColumnType("decimal(18, 2)");
            });

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DeletedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.DeletedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.PaymentNumber)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.HasOne(e => e.PaymentTypeMaster)
                      .WithMany()
                      .HasForeignKey(e => e.PaymentTypeID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure RequesterLedger entity
            modelBuilder.Entity<RequesterLedger>(entity =>
            {
                entity.HasKey(e => e.RequesterLedgerID);
                entity.Property(e => e.RequesterLedgerID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.Property(e => e.Description)
                      .HasMaxLength(1000)
                      .IsRequired();

                entity.Property(e => e.SourceNumber)
                      .HasMaxLength(30);

                entity.Property(e => e.OpeningBalance)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.Balance)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.TransactionType)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(e => e.SourceType)
                      .HasConversion<int>()
                      .IsRequired();

                entity.HasOne(e => e.Requester)
                      .WithMany()
                      .HasForeignKey(e => e.RequesterID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DeletedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.DeletedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure RefundPayment entity
            modelBuilder.Entity<RefundPayment>(entity =>
            {
                entity.HasKey(e => e.RefundPaymentID);
                entity.Property(e => e.RefundPaymentID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DeletedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.DeletedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.RefundPaymentNumber)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.HasOne(e => e.PaymentTypeMaster)
                      .WithMany()
                      .HasForeignKey(e => e.PaymentTypeID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure RefundPaymentAllocation entity
            modelBuilder.Entity<RefundPaymentAllocation>(entity =>
            {
                entity.ToTable("RefundPaymentAllocations", "dbo");
                entity.HasKey(e => e.RefundPaymentAllocationID);
                entity.Property(e => e.RefundPaymentAllocationID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.Property(e => e.AmountApplied)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.AllocationDate)
                      .HasColumnType("datetime2")
                      .HasDefaultValueSql("GETDATE()")
                      .IsRequired();

                entity.Property(e => e.TransactionType)
                      .HasConversion<int>()
                      .HasDefaultValue(RefundPaymentAllocationTransactionType.Debit)
                      .IsRequired();


                entity.HasOne(e => e.Payment)
                      .WithMany(p => p.RefundPaymentAllocations)
                      .HasForeignKey(e => e.PaymentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.RefundPayment)
                      .WithMany(r => r.RefundPaymentAllocations)
                      .HasForeignKey(e => e.RefundPaymentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DeletedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.DeletedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure PaymentAllocation entity
            modelBuilder.Entity<PaymentAllocation>(entity =>
            {
                entity.ToTable("PaymentAllocations", "dbo");
                entity.HasKey(e => e.PaymentAllocationID);
                entity.Property(e => e.PaymentAllocationID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("newid()");

                entity.Property(e => e.AmountApplied)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.AllocationDate)
                      .HasColumnType("datetime2")
                      .HasDefaultValueSql("GETDATE()")
                      .IsRequired();

                entity.Property(e => e.TransactionType)
                      .HasConversion<int>()
                      .HasDefaultValue(PaymentAllocationTransactionType.Debit)
                      .IsRequired();

                entity.HasOne(e => e.Invoice)
                      .WithMany(i => i.PaymentAllocations)
                      .HasForeignKey(e => e.InvoicesID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.Payment)
                      .WithMany(p => p.PaymentAllocations)
                      .HasForeignKey(e => e.PaymentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DeletedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.DeletedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RequestType>(entity =>
            {
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DeletedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.DeletedUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Questionnaire entity
            modelBuilder.Entity<Questionnaire>(entity =>
            {
                entity.HasKey(e => e.QuestionnaireId);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.QuestionnaireId)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Self-referencing: one Questionnaire can reference another as Source (prevent cascade delete)
                entity.HasOne(e => e.SourceQuestionnaire)
                      .WithMany(e => e.DerivedQuestionnaires)
                      .HasForeignKey(e => e.SourceQuestionnaireId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure QuestionnaireType enum as int
                entity.Property(e => e.QuestionnaireType)
                      .HasConversion<int>()
                      .HasDefaultValue(QuestionnaireType.Cancel)
                      .IsRequired();

                // Configure string properties max lengths
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.Property(e => e.Description)
                      .HasMaxLength(1000)
                      .HasColumnType("nvarchar(1000)");

                entity.Property(e => e.IsActive)
                      .IsRequired();
            });

            // Configure Question entity
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.QuestionId);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.QuestionId)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Questionnaire table for QuestionnaireId
                entity.HasOne(e => e.Questionnaire)
                      .WithMany()
                      .HasForeignKey(e => e.QuestionnaireId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

                // Configure IsRequired default value
                entity.Property(e => e.IsRequired)
                      .HasDefaultValue(false);
            });

            // Configure PostQuestionnaire entity
            modelBuilder.Entity<RequesterQuestionnaire>(entity =>
            {
                entity.HasKey(e => e.RequesterQuestionnaireId);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.RequesterQuestionnaireId)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Post table
                entity.HasOne(e => e.Requester)
                      .WithMany()
                      .HasForeignKey(e => e.RequesterId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Foreign Key to Questionnaire table
                entity.HasOne(e => e.Questionnaire)
                      .WithMany()
                      .HasForeignKey(e => e.QuestionnaireId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();
            });

            // Configure Option entity
            modelBuilder.Entity<Option>(entity =>
            {
                entity.HasKey(e => e.OptionId);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.OptionId)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Question table
                entity.HasOne(e => e.Question)
                      .WithMany()
                      .HasForeignKey(e => e.QuestionId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Configure string properties max lengths
                entity.Property(e => e.OptionText)
                      .HasMaxLength(500)
                      .HasColumnType("nvarchar(500)");

                entity.Property(e => e.DisplayOrder)
                      .IsRequired();
            });

            // Configure PTAEO_Code entity
            modelBuilder.Entity<PtaeoCode>(entity =>
            {
                entity.ToTable("PtaeoCode", "dbo");
                entity.HasKey(e => e.PtaeoCodeID);

                entity.Property(e => e.PtaeoCodeID)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.Property(e => e.ProjectNumber).HasMaxLength(50);
                entity.Property(e => e.TaskNumber).HasMaxLength(100);
                entity.Property(e => e.AwardNumber).HasMaxLength(50);
                entity.Property(e => e.FundNumber).HasMaxLength(50);
                entity.Property(e => e.OrganizationNumber).HasMaxLength(50);
                entity.Property(e => e.StartDate).HasColumnType("date");
                entity.Property(e => e.EndDate).HasColumnType("date");
                entity.Property(e => e.Status)
                      .HasColumnType("bit")
                      .HasDefaultValue(true);
            });

            // Configure Response entity
            modelBuilder.Entity<Response>(entity =>
            {
                entity.HasKey(e => e.ResponseId);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.ResponseId)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Questionnaire table
                entity.HasOne(e => e.Questionnaire)
                      .WithMany()
                      .HasForeignKey(e => e.QuestionnaireId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID (SubmittedBy)
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to PostPosition table
                entity.HasOne(e => e.PostPosition)
                      .WithMany()
                      .HasForeignKey(e => e.PostPositionId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string properties
                entity.Property(e => e.SubmitterEmail)
                      .IsRequired()
                      .HasMaxLength(200)
                      .HasColumnType("nvarchar(200)");

                entity.Property(e => e.SubmittedOn)
                      .HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.EditedBy).HasMaxLength(255);
            });

            // Configure Answers entity
            modelBuilder.Entity<Answers>(entity =>
            {
                entity.HasKey(e => e.AnswerId);

                // GUID generated by SQL Server using NEWID() default constraint
                entity.Property(e => e.AnswerId)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Response table
                entity.HasOne(e => e.Response)
                      .WithMany()
                      .HasForeignKey(e => e.ResponseId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

                // Foreign Key to Question table
                entity.HasOne(e => e.Question)
                      .WithMany()
                      .HasForeignKey(e => e.QuestionId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                // Configure string properties
                entity.Property(e => e.Answer)
                      .HasColumnType("nvarchar(max)");
            });

            // Configure DepartmentQuestionnaire entity
            modelBuilder.Entity<DepartmentQuestionnaire>(entity =>
            {
                entity.ToTable("DepartmentQuestionnaire", "dbo");
                entity.HasKey(e => e.DepartmentQuestionnaireId);

                entity.Property(e => e.DepartmentQuestionnaireId)
                      .HasDefaultValueSql("newid()")
                      .ValueGeneratedOnAdd();

                // Foreign Key to Department table
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Questionnaire table
                entity.HasOne(e => e.Questionnaire)
                      .WithMany()
                      .HasForeignKey(e => e.QuestionnaireId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Admin table for CreatedUserID
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CreatedBy).HasMaxLength(200);
                entity.Property(e => e.EditedBy).HasMaxLength(200);
            });

            // Configure DepartmentAdmin entity
            modelBuilder.Entity<DepartmentAdmin>(entity =>
            {
                entity.ToTable("DepartmentAdmin", "dbo");
                entity.HasKey(e => e.DepartmentAdminID);
                entity.Property(e => e.DepartmentAdminID)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("NEWID()");

                // Foreign Key to Department table for DepartmentID
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EmployeeID
                entity.HasOne(e => e.Employee)
                      .WithMany(emp => emp.DepartmentAdmins)
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to AdminType table for AdminTypeID
                entity.HasOne(e => e.AdminType)
                      .WithMany()
                      .HasForeignKey(e => e.AdminTypeID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for CreatedUserID (audit - who added the record)
                entity.HasOne(e => e.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Employee table for EditedUserID (audit - who last edited)
                entity.HasOne(e => e.EditedByEmployee)
                      .WithMany()
                      .HasForeignKey(e => e.EditedUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.EditedBy).HasMaxLength(255);
            });

            modelBuilder.Entity<RequestSchedule>(entity =>
            {
                entity.HasIndex(e => new { e.RequestId, e.RequestStatus });
            });

            modelBuilder.Entity<RequestScheduleGeneration>(entity =>
            {
                entity.HasOne(e => e.Request)
                    .WithMany()
                    .HasForeignKey(e => e.RequestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.IsOwned() || entityType.ClrType == null)
                {
                    continue;
                }

                var isDeletedProperty = entityType.FindProperty("IsDeleted");
                if (isDeletedProperty == null || isDeletedProperty.ClrType != typeof(bool))
                {
                    continue;
                }

                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var isDeletedExpression = Expression.Call(
                    typeof(EF),
                    nameof(EF.Property),
                    new[] { typeof(bool) },
                    parameter,
                    Expression.Constant("IsDeleted"));
                var filter = Expression.Equal(isDeletedExpression, Expression.Constant(false));
                var lambda = Expression.Lambda(filter, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}