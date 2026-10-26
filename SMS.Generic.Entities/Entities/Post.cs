using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    [AuditedEntity(DisplayPropertyName = nameof(PostTitle), OnlyAttributedProperties = true)]
    [Table("Post", Schema = "dbo")]
    public class Post : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PostID { get; set; }

        /// <summary>
        /// Unique code (nvarchar(20)) from dbo.Seq_PostCode; default is six-digit zero-padded (e.g. 000001).
        /// </summary>
        [MaxLength(20)]
        [AuditedProperty("Post code")]
        public string PostCode { get; set; } = null!;

        // 🔹 REQUIRED FOREIGN KEYS
        [AuditedProperty("Request", LookupEntityType = typeof(Request), LookupDisplayPropertyName = nameof(Request.Title))]
        public Guid? RequestID { get; set; }
        [AuditedProperty("Parent job", LookupEntityType = typeof(Post), LookupDisplayPropertyName = nameof(Post.PostTitle))]
        public Guid? PostParentID { get; set; }
        [AuditedProperty("Department activity type", LookupEntityType = typeof(DepartmentActivityType), LookupDisplayPropertyName = nameof(DepartmentActivityType.ActivityTypeLong))]
        public Guid? DepartmentActivityTypeId { get; set; }

        // 🔹 NAVIGATION PROPERTIES
        public Request? Request { get; set; }

        [AuditedProperty("Parent job", LookupEntityType = typeof(Post), LookupDisplayPropertyName = nameof(Post.PostTitle))]
        public Post? ParentPost { get; set; }
        [AuditedProperty("Department activity type", LookupEntityType = typeof(DepartmentActivityType), LookupDisplayPropertyName = nameof(DepartmentActivityType.ActivityTypeShort))]
        public DepartmentActivityType? DepartmentActivityType { get; set; }

        [ForeignKey(nameof(PostResponseAreaId))]
        [AuditedProperty("Department response area", LookupEntityType = typeof(DepartmentResponseArea), LookupDisplayPropertyName = nameof(DepartmentResponseArea.DepartmentResponseAreaShort))]
        public virtual DepartmentResponseArea? DepartmentResponseArea { get; set; }

        public ICollection<Post> ChildPosts { get; set; } = new List<Post>();
        public ICollection<BlockDate> BlockDates { get; set; } = new List<BlockDate>();
        public ICollection<PostDocument> PostDocuments { get; set; } = new List<PostDocument>();

        // Audit → Employee (uses BaseEntity.CreatedUserID / EditedUserID)
        [ForeignKey("CreatedUserID")]
        public Employee? CreatedByEmployee { get; set; }

        [ForeignKey("EditedUserID")]
        public Employee? EditedByEmployee { get; set; }

        // 🔹 BUSINESS FIELDS (REQUIRED)
        [AuditedProperty("Supervisor job")]
        public bool IsSupervisorPost { get; set; }
        [AuditedProperty("Is parent")]
        public bool IsParent { get; set; }
        [AuditedProperty("Total job positions")]
        public int TotalPostPositions { get; set; }
        [AuditedProperty("Rank eligible")]
        public bool RankEligible { get; set; }

        [MaxLength(60)]
        [Required]
        [AuditedProperty("Job title")]
        public string PostTitle { get; set; } = null!;

        // 🔹 OPTIONAL BUSINESS FIELDS
        [MaxLength(1000)]
        [Required]
        [AuditedProperty("Job description")]
        public string PostDescription { get; set; }

        [Required]
        [AuditedProperty("Job start time")]
        public TimeOnly PostStartTime { get; set; }
        [Required]
        [AuditedProperty("Job end time")]
        public TimeOnly PostEndTime { get; set; }
        [Required]
        [AuditedProperty("Job length")]
        public int PostLength { get; set; }

        [AuditedProperty("Job flexible")]
        public bool PostIsFlexible { get; set; }

        [Column(TypeName = "money")]
        [Required]
        [AuditedProperty("Job pay rate")]
        public decimal PostPayRate { get; set; }

        [Column(TypeName = "money")]
        [Required]
        [AuditedProperty("Job premium rate")]
        public decimal PostPremiumRate { get; set; }

        [AuditedProperty("Job requires vehicle")]
        public bool PostRequiresVehicle { get; set; }
        [AuditedProperty("Job requires equipment")]
        public bool PostRequiresEquipment { get; set; }
        [AuditedProperty("Skills required")]
        public bool IsSkillsRequired { get; set; }

        [AuditedProperty("Pay code", LookupEntityType = typeof(PayCode), LookupDisplayPropertyName = nameof(PayCode.PayCodeValue))]
        public Guid? PayCodeID { get; set; }

        [ForeignKey(nameof(PayCodeID))]
        public virtual PayCode? PayCode { get; set; }

        /// <summary>
        /// Gets or sets whether a skill is required.
        /// </summary>
        public List<Guid>? SkillIDs { get; set; }
        public Guid? EmployeeTypeID { get; set; }
        // 🔹 Navigation Property
        public EmployeeType? EmployeeType { get; set; }
        [MaxLength(200)]
        public string? PostNotes { get; set; }

        [MaxLength(200)]
        [Required]
        [AuditedProperty("Job location")]
        public string PostLocation { get; set; }

        [AuditedProperty("Job required document")]
        public bool IsPostRequiredDocument { get; set; }

        /// <summary>
        /// Minimum hours required for this job.
        /// </summary>
        [AuditedProperty("Job minimum hours")]
        public int PostMinimumHoursRequired { get; set; }

        /// <summary>
        /// Whether this job is exempt from hours requirements.
        /// </summary>
        [AuditedProperty("Exempt hours")]
        public bool IsExemptHours { get; set; }

        /// <summary>
        /// Department response area ID (references DepartmentResponseArea table).
        /// </summary>
        [AuditedProperty("Job response area", LookupEntityType = typeof(DepartmentResponseArea), LookupDisplayPropertyName = nameof(DepartmentResponseArea.DepartmentResponseAreaLong))]
        public Guid? PostResponseAreaId { get; set; }

        /// <summary>
        /// Whether the job requires a uniform.
        /// </summary>
        [AuditedProperty("Job uniform")]
        public bool PostUniform { get; set; }

        /// <summary>
        /// Whether alcohol is served or consumed at this job.
        /// </summary>
        [AuditedProperty("Alcohol served or consumed")]
        public bool IsAlcoholServedOrConsumed { get; set; }

        /// <summary>
        /// Whether this job is a premium paid event.
        /// </summary>
        [AuditedProperty("Premium paid event")]
        public bool IsPremiumPaidEvent { get; set; } = false;

        /// <summary>
        /// Required rank for this job.
        /// </summary>
        [AuditedProperty("Required rank")]
        public int RequiredRank { get; set; }

        /// <summary>
        /// Project number (e.g. 20419).
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Project number")]
        public string? ProjectNumber { get; set; }

        /// <summary>
        /// Task number/name (e.g. Police).
        /// </summary>
        [MaxLength(100)]
        [AuditedProperty("Task number")]
        public string? TaskNumber { get; set; }

        /// <summary>
        /// Award number (e.g. 82187).
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Award number")]
        public string? AwardNumber { get; set; }

        /// <summary>
        /// Fund number (e.g. 0001).
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Fund number")]
        public string? FundNumber { get; set; }

        /// <summary>
        /// Organization number (e.g. 1401385).
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Organization number")]
        public string? OrganizationNumber { get; set; }

        /// <summary>
        /// Gets or sets the first name (job contact; mirrors Request).
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("First name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Last name")]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the primary phone.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Primary phone")]
        public string? PrimaryPhone { get; set; }

        /// <summary>
        /// Gets or sets the cell phone.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Cell phone")]
        public string? CellPhone { get; set; }

        /// <summary>
        /// Gets or sets the primary email.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Primary email")]
        public string? PrimaryEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to define call sign.
        /// </summary>
        [AuditedProperty("Define call sign")]
        public bool DefineCallSign { get; set; }

        // 🔹 DEPARTMENT DEFINED (OPTIONAL)
        [MaxLength(200)] [AuditedProperty("Dept defined 1")] public string? PostDepartmentDefinedValue1 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 2")] public string? PostDepartmentDefinedValue2 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 3")] public string? PostDepartmentDefinedValue3 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 4")] public string? PostDepartmentDefinedValue4 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 5")] public string? PostDepartmentDefinedValue5 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 6")] public string? PostDepartmentDefinedValue6 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 7")] public string? PostDepartmentDefinedValue7 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 8")] public string? PostDepartmentDefinedValue8 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 9")] public string? PostDepartmentDefinedValue9 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 10")] public string? PostDepartmentDefinedValue10 { get; set; }

        public ICollection<PostPosition> PostPositions { get; set; } = new List<PostPosition>();
        public ICollection<WaitingPostPosition> WaitingPostPosition { get; set; } = new List<WaitingPostPosition>();
    }
}

