using Newtonsoft.Json;
using SMS.Generic.Common.Enums;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using System.Text.Json.Serialization;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for TenureWaitingPosition operations.
    /// </summary>
    public class WaitingPostPositionDto : BaseDto, IEmployeeScoped
    {
        public WaitingPostPositionDto()
        {
        }

        public WaitingPostPositionDto(PostDto post, Guid employeeId, DateOnly dateOfWork)
        {
            if (post?.PostID == null || post.PostID == Guid.Empty)
            {
                throw new ArgumentException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.WAITING_JOB_POSITION_JOB_ID_REQUIRED),
                    nameof(post));
            }

            PostID = post.PostID.Value;
            EmployeeID = employeeId;
            DateOfWork = dateOfWork;
            StartTime = post.PostStartTime;
            LengthOfWork = post.PostLength;
            EquipmentRequired = post.PostRequiresEquipment;
            WaitingPostPositionPayRate = post.PostPayRate;
            IsPremiumPaidEvent = post.IsPremiumPaidEvent;
            PostPositionStatus = PostPositionStatus.Waiting;
            ReleaseAs = post.ReleaseAs;

            PostDepartmentDefinedFieldValue1 = post.PostDepartmentDefinedValue1;
            PostDepartmentDefinedFieldValue2 = post.PostDepartmentDefinedValue2;
            PostDepartmentDefinedFieldValue3 = post.PostDepartmentDefinedValue3;
            PostDepartmentDefinedFieldValue4 = post.PostDepartmentDefinedValue4;
            PostDepartmentDefinedFieldValue5 = post.PostDepartmentDefinedValue5;
            PostDepartmentDefinedFieldValue6 = post.PostDepartmentDefinedValue6;
            PostDepartmentDefinedFieldValue7 = post.PostDepartmentDefinedValue7;
            PostDepartmentDefinedFieldValue8 = post.PostDepartmentDefinedValue8;
            PostDepartmentDefinedFieldValue9 = post.PostDepartmentDefinedValue9;
            PostDepartmentDefinedFieldValue10 = post.PostDepartmentDefinedValue10;
        }

        [JsonProperty("waitingPostPositionId")]
        [JsonPropertyName("waitingPostPositionId")]
        public Guid? WaitingPostPositionID { get; set; }

        public Guid? PostID { get; set; }
        public Guid? RequestId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid EmployeeID { get; set; }

        /// <summary>First and last name when <see cref="Employee"/> is loaded (e.g. job/API with includes).</summary>
        public string? AssignedEmployeeName { get; set; }

        public DateOnly DateOfWork { get; set; }
        /// <summary>
        /// Waiting-list queue order (1-based) while <see cref="PostPositionStatus.Waiting"/>; 0 when assigned (<see cref="PostPositionStatus.Applied"/>).
        /// </summary>
        public int OrderNumber { get; set; }
        /// <summary>
        /// True when the employee has already been applied/assigned from this tenure waiting-list row.
        /// </summary>
        public bool HasAppliedInTenure { get; set; }
        public TimeOnly StartTime { get; set; }
        public int LengthOfWork { get; set; }
        public bool EquipmentRequired { get; set; }
        public string? CallSign { get; set; }
        public Guid? PostCallSignID { get; set; }
        public decimal WaitingPostPositionPayRate { get; set; }
        public bool IsPremiumPaidEvent { get; set; }
        public PostPositionStatus PostPositionStatus { get; set; }
        public ProcessingStatus ProcessingStatus { get; set; } = ProcessingStatus.Pending;
        public int? StatusCode { get; set; }
        public string? ResultMessage { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public ReleaseAs ReleaseAs { get; set; } = 0;
        public Guid? ParentPostPositionId { get; set; }
        public bool IsSplit { get; set; } = false;
        public Guid? SplitPostPositionID { get; set; }
        public string? PostDepartmentDefinedFieldValue1 { get; set; }
        public string? PostDepartmentDefinedFieldValue2 { get; set; }
        public string? PostDepartmentDefinedFieldValue3 { get; set; }
        public string? PostDepartmentDefinedFieldValue4 { get; set; }
        public string? PostDepartmentDefinedFieldValue5 { get; set; }
        public string? PostDepartmentDefinedFieldValue6 { get; set; }
        public string? PostDepartmentDefinedFieldValue7 { get; set; }
        public string? PostDepartmentDefinedFieldValue8 { get; set; }
        public string? PostDepartmentDefinedFieldValue9 { get; set; }
        public string? PostDepartmentDefinedFieldValue10 { get; set; }
        public int TotalPostPositions { get; set; }
        public PostDto Post { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public RequestDto Request { get; set; }
        public bool IsEmployeeIdMatch(Guid? employeeId)
        {
            return EmployeeID == employeeId;
        }
        public bool IsPostIdMatch(Guid? postId)
        {
            return PostID == postId;
        }
        public bool IsSameWorkDate(DateOnly dateOfWork)
        {
            return DateOfWork == dateOfWork;
        }
        public bool IsSameRequest(Guid requestId)
        {
            return RequestId.HasValue && RequestId.Value == requestId;
        }
        public bool IsPostPositionStatusWaiting()
        {
            return PostPositionStatus == PostPositionStatus.Waiting;
        }
        public bool IsPostPositionStatusSpecialEvent()
        {
            return PostPositionStatus == PostPositionStatus.SpecialEvent;
        }

        public bool IsWaitingPostPositionAlreadyTakenByEmployee(Guid employeeId, DateOnly dateOfWork,Guid postId)
        {
            return IsEmployeeIdMatch(employeeId) && IsSameWorkDate(dateOfWork) && IsPostIdMatch(postId) && IsPostPositionStatusWaiting();
        }
        public bool IsSpecialEventAlreadyTaken(Guid requestId,Guid employeeId,DateOnly postDate,HashSet<Guid?>? requestPostIds)
        {
            // 🔥 Common base condition (avoid duplication)
            bool isAlreadyTaken =
                IsSameRequest(requestId) &&
                IsEmployeeIdMatch(employeeId) &&
                IsSameWorkDate(postDate);

            if (!isAlreadyTaken)
                return false;

            // ✅ Case 1: Special Event
            if (IsPostPositionStatusSpecialEvent())
                return true;

            // ✅ Case 2: Same Request Post (ignore status)
            if (requestPostIds != null && requestPostIds.Contains(PostID))
                return true;

            return false;
        }

    }

    /// <summary>
    /// Waiting list row with employee display fields (for manager views).
    /// </summary>
    public class WaitingPostPositionResponseDto : WaitingPostPositionDto
    {
        public new Guid EmployeeID
        {
            get => base.EmployeeID;
            set => base.EmployeeID = value;
        }
        public string? BadgeNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PrimaryPhone { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? EmployeeTypeName { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmployeeStatusName { get; set; }
    }
}

