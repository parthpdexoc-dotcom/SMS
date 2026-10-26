using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Mappings
{
    public class PostPositionMappingProfile : Profile
    {
        public PostPositionMappingProfile()
        {
            CreateMap<PostPosition, PostPositionDto>()
                .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src => src.EmployeeID ?? Guid.Empty))
                .ForMember(dest => dest.PostStartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.PostEndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.CallSign, opt => opt.MapFrom(src =>
                    src.CallSign ?? (src.PostCallSign == null ? null : src.PostCallSign.CallSign)))
                .ForMember(dest => dest.JobRank, opt => opt.MapFrom(src =>
                    src.Post != null && src.Post.EmployeeType != null
                        ? EmployeeTypeDto.FormatJobRankFromEmployeeTypeShort(
                            src.Post.EmployeeType.EmployeeTypeShort)
                        : null))
                .ForMember(dest => dest.PlaceholderDetail, opt => opt.MapFrom(src => src.Placeholder))
                .ForMember(dest => dest.HoursWorked, opt => opt.MapFrom(src =>
                    src.ClosedPostPositions.Select(x => (int?)x.HoursWorked).FirstOrDefault()))
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee))
                .ReverseMap()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.PostStartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.PostEndTime))
                .ForMember(dest => dest.Placeholder, opt => opt.Ignore())
                .ForMember(dest => dest.PostCallSign, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src =>
                    src.IsPlaceholder || src.EmployeeID == Guid.Empty ? (Guid?)null : src.EmployeeID))
                .AfterMap((src, dest) =>
                {
                    if (src.HoursWorked == null)
                        return;

                    dest.ClosedPostPositions ??= new List<ClosedPostPosition>();
                    dest.ClosedPostPositions.Clear();
                    dest.ClosedPostPositions.Add(new ClosedPostPosition
                    {
                        PostPositionID = dest.PostPositionID,
                        HoursWorked = src.HoursWorked.Value
                    });
                });

            CreateMap<PostPositionDto, PostPositionResponseDto>();
            CreateMap<PostPosition, PostPositionResponseDto>()
                .ForMember(dest => dest.CallSign, opt => opt.MapFrom(src =>
                    src.CallSign ?? (src.PostCallSign == null ? null : src.PostCallSign.CallSign)))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                    src.Employee != null
                        ? src.Employee.FullName
                        : PlaceholderDto.FormatDisplayName(src.Placeholder)))
                .ForMember(dest => dest.BadgeNumber, opt => opt.MapFrom(src =>
                    src.Employee != null
                        ? src.Employee.BadgeNumber
                        : (src.Placeholder != null ? src.Placeholder.EmployeeNumber : null)))
                .ForMember(dest => dest.IsUniquePerPosition, opt => opt.MapFrom(src =>
                    src.Placeholder != null && src.Placeholder.IsUniquePerPosition))
                .ForMember(dest => dest.PlaceholderDetail, opt => opt.MapFrom(src => src.Placeholder))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src =>
                    src.Employee != null ? src.Employee.FirstName : (src.Placeholder != null ? src.Placeholder.FirstName : null)))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src =>
                    src.Employee != null ? src.Employee.LastName : (src.Placeholder != null ? src.Placeholder.LastName : null)))
                .ForMember(dest => dest.PrimaryPhone, opt => opt.MapFrom(src =>
                    src.Employee != null ? src.Employee.PrimaryPhone : (src.Placeholder != null ? src.Placeholder.ContactNumber : null)))
                .ForMember(dest => dest.PrimaryEmail, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.PrimaryEmail : null))
                .ForMember(dest => dest.EmployeeTypeName, opt => opt.MapFrom(src =>
                    src.Employee != null && src.Employee.EmployeeType != null ? src.Employee.EmployeeType.EmployeeTypeShort : null))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src =>
                    src.Employee != null && src.Employee.Department != null ? src.Employee.Department.DepartmentName : null))
                .ForMember(dest => dest.EmployeeStatusName, opt => opt.MapFrom(src =>
                    src.Employee != null && src.Employee.Status != null ? src.Employee.Status.StatusShort : null))
                .ForMember(dest => dest.PostStartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.PostEndTime, opt => opt.MapFrom(src => src.EndTime))
                .ReverseMap()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.PostStartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.PostEndTime))
                .ForMember(dest => dest.Placeholder, opt => opt.Ignore());

            CreateMap<PostPosition, PostTypeResponseDto>()
                .ForMember(dest => dest.CallSign, opt => opt.MapFrom(src =>
                    src.PostCallSign == null ? null : src.PostCallSign.CallSign))
                .ForMember(dest => dest.PostTitle, opt => opt.MapFrom(src => src.Post != null ? (src.Post.PostTitle ?? string.Empty) : string.Empty))
                .ForMember(dest => dest.PostLocation, opt => opt.MapFrom(src => src.Post != null ? (src.Post.PostLocation ?? string.Empty) : string.Empty))
                .ForMember(dest => dest.RequestID, opt => opt.MapFrom(src => src.Post != null ? src.Post.RequestID : null))
                .ForMember(dest => dest.PostRequiresVehicle, opt => opt.MapFrom(src => src.Post != null && src.Post.PostRequiresVehicle))
                .ForMember(dest => dest.PostDescription, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDescription : null))
                .ForMember(dest => dest.PostPayRate, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostPayRate : 0))
                .ForMember(dest => dest.PostPremiumRate, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostPremiumRate : 0))
                .ForMember(dest => dest.PostIsFlexible, opt => opt.MapFrom(src => src.Post != null && src.Post.PostIsFlexible))
                .ForMember(dest => dest.PostRequiresEquipment, opt => opt.MapFrom(src => src.Post != null && src.Post.PostRequiresEquipment))
                .ForMember(dest => dest.IsSkillsRequired, opt => opt.MapFrom(src => src.Post != null && src.Post.IsSkillsRequired))
                .ForMember(dest => dest.RankEligible, opt => opt.MapFrom(src => src.Post != null && src.Post.RankEligible))
                .ForMember(dest => dest.JobRank, opt => opt.MapFrom(src =>
                    src.Post != null && src.Post.EmployeeType != null
                        ? EmployeeTypeDto.FormatJobRankFromEmployeeTypeShort(
                            src.Post.EmployeeType.EmployeeTypeShort)
                        : null))
                .ForMember(dest => dest.RequestTitle, opt => opt.MapFrom(src =>
                    src.Post != null && src.Post.Request != null ? src.Post.Request.Title : null))
                .ForMember(dest => dest.RequesterID, opt => opt.MapFrom(src =>
                    src.Post != null && src.Post.Request != null ? (Guid?)src.Post.Request.RequesterID : null))
                .ForMember(dest => dest.RequesterTitle, opt => opt.MapFrom(src =>
                    src.Post != null && src.Post.Request != null && src.Post.Request.Requester != null
                        ? src.Post.Request.Requester.Name
                        : null))
                .ForMember(dest => dest.PayCodeValue, opt => opt.MapFrom(src =>
                    src.Post != null && src.Post.PayCode != null ? src.Post.PayCode.PayCodeValue : null))
                .ForMember(dest => dest.IsPostRequiredDocument, opt => opt.MapFrom(src => src.Post != null && src.Post.IsPostRequiredDocument))
                .ForMember(dest => dest.TotalPostPositions, opt => opt.MapFrom(src => src.Post != null ? src.Post.TotalPostPositions : 0))
                .ForMember(dest => dest.ReleaseAs, opt => opt.MapFrom(src =>
                    src.Post != null && src.Post.Request != null
                        ? src.Post.Request.RequestSchedules
                            .OrderByDescending(s => s.EditedDate ?? s.CreatedDate)
                            .ThenByDescending(s => s.EndDate)
                            .Select(s => (ReleaseAs?)s.ReleaseAs)
                            .FirstOrDefault() ?? ReleaseAs.Open
                        : ReleaseAs.Open))
                .ForMember(dest => dest.WasUnclosed, opt => opt.MapFrom(src => src.WasUnclosed))
                .ForMember(dest => dest.PostLength, opt => opt.MapFrom(src => src.LengthOfWork > 0 ? src.LengthOfWork : (src.Post != null ? src.Post.PostLength : 0)))
                .ForMember(dest => dest.PostStartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.PostEndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.CancelledBy, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                    src.Employee != null
                        ? src.Employee.FullName
                        : PlaceholderDto.FormatDisplayName(src.Placeholder)))
                .ReverseMap()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.PostStartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.PostEndTime));

            CreateMap<PostPositionDto, PostTypeResponseDto>()
                .ForMember(dest => dest.PostTitle, opt => opt.MapFrom(src => src.Post != null ? (src.Post.PostTitle ?? string.Empty) : string.Empty))
                .ForMember(dest => dest.PostLocation, opt => opt.MapFrom(src => src.Post != null ? (src.Post.PostLocation ?? string.Empty) : string.Empty))
                .ForMember(dest => dest.RequestID, opt => opt.MapFrom(src => src.Post != null ? src.Post.RequestID : null))
                .ForMember(dest => dest.PostRequiresVehicle, opt => opt.MapFrom(src => src.Post != null && src.Post.PostRequiresVehicle))
                .ForMember(dest => dest.PostDescription, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDescription : null))
                .ForMember(dest => dest.PostPayRate, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostPayRate : 0))
                .ForMember(dest => dest.PostPremiumRate, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostPremiumRate : 0))
                .ForMember(dest => dest.PostIsFlexible, opt => opt.MapFrom(src => src.Post != null && src.Post.PostIsFlexible))
                .ForMember(dest => dest.PostRequiresEquipment, opt => opt.MapFrom(src => src.Post != null && src.Post.PostRequiresEquipment))
                .ForMember(dest => dest.IsSkillsRequired, opt => opt.MapFrom(src => src.Post != null && src.Post.IsSkillsRequired))
                .ForMember(dest => dest.RankEligible, opt => opt.MapFrom(src => src.Post != null && src.Post.RankEligible))
                .ForMember(dest => dest.JobRank, opt => opt.MapFrom(src =>
                    !string.IsNullOrWhiteSpace(src.JobRank)
                        ? src.JobRank
                        : (src.Post != null && !string.IsNullOrWhiteSpace(src.Post.JobRank)
                            ? src.Post.JobRank
                            : null)))
                .ForMember(dest => dest.RequestTitle, opt => opt.MapFrom(src =>
                    src.Post != null && src.Post.Request != null ? src.Post.Request.Title : null))
                .ForMember(dest => dest.RequesterID, opt => opt.MapFrom(src =>
                    src.Post != null && src.Post.Request != null ? src.Post.Request.RequesterID : null))
                .ForMember(dest => dest.RequesterTitle, opt => opt.MapFrom(src =>
                    src.Post != null && src.Post.Request != null && src.Post.Request.Requester != null
                        ? src.Post.Request.Requester.Name
                        : null))
                .ForMember(dest => dest.PayCodeValue, opt => opt.MapFrom(src =>
                    src.Post != null && src.Post.PayCode != null ? src.Post.PayCode.PayCodeValue : null))
                .ForMember(dest => dest.IsPostRequiredDocument, opt => opt.MapFrom(src => src.Post != null && src.Post.IsPostRequiredDocument))
                .ForMember(dest => dest.TotalPostPositions, opt => opt.MapFrom(src => src.Post != null ? src.Post.TotalPostPositions : 0))
                .ForMember(dest => dest.PostLength, opt => opt.MapFrom(src => src.LengthOfWork > 0 ? src.LengthOfWork : (src.Post != null ? src.Post.PostLength : 0)))
                .ForMember(dest => dest.PostEndTime, opt => opt.MapFrom(src => src.PostEndTime))
                .ForMember(dest => dest.WasUnclosed, opt => opt.MapFrom(src => src.WasUnclosed))
                .ForMember(dest => dest.ReleaseAs, opt => opt.MapFrom(src => src.Post != null ? src.Post.ReleaseAs : ReleaseAs.Open))
                .ForMember(dest => dest.CancelledBy, opt => opt.MapFrom(src => src.Employee == null ? null : src.Employee.GetFullName()))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                    src.Employee != null
                        ? src.Employee.GetFullName()
                        : (src.PlaceholderDetail != null ? src.PlaceholderDetail.GetPlaceHolderName() : null)))
                .ReverseMap();

            CreateMap<PostPosition, PostPositionWithPostTitleDto>()
                .ForMember(dest => dest.PostTitle, opt => opt.MapFrom(src => src.Post != null ? (src.Post.PostTitle ?? string.Empty) : string.Empty))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                    src.Employee != null
                        ? src.Employee.FullName
                        : PlaceholderDto.FormatDisplayName(src.Placeholder) ?? string.Empty))
                .ForMember(dest => dest.PostPosition, opt => opt.MapFrom(src => src));

            CreateMap<PostPositionResponseDto, PostPositionInRequestListDto>()
                .ForMember(dest => dest.PostPositionId, opt =>
                {
                    opt.PreCondition(src => src.PostPositionID.HasValue);
                    opt.MapFrom(src => src.PostPositionID!.Value);
                })
                .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeTypeName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PrimaryPhone))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PostPositionStatus));

            CreateMap<PostPosition, EmployeePostListingItemDto>()
                .ForMember(dest => dest.RequestID, opt => opt.MapFrom(src => src.Post != null ? src.Post.RequestID : null))
                .ForMember(dest => dest.PostID, opt => opt.MapFrom(src => src.PostID))
                .ForMember(dest => dest.PostPositionID, opt => opt.MapFrom(src => src.PostPositionID))
                .ForMember(dest => dest.PostPositionStatus, opt => opt.MapFrom(src => src.PostPositionStatus))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostTitle : null))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostLocation : null))
                .ForMember(dest => dest.PostLength, opt => opt.MapFrom(src => src.LengthOfWork > 0 ? src.LengthOfWork : (src.Post != null ? src.Post.PostLength : 0)))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.DateOfWork))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime ?? (src.Post != null ? (TimeOnly?)src.Post.PostStartTime : null)))
                .ForMember(dest => dest.PostEndTime, opt => opt.MapFrom(src => src.EndTime ?? (src.Post != null ? (TimeOnly?)src.Post.PostEndTime : null)))
                .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.CancelledBy, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
                .ForMember(dest => dest.RequestTitle, opt => opt.MapFrom(src => src.Post != null && src.Post.Request != null ? src.Post.Request.Title : null))
                .ForMember(dest => dest.RequesterID, opt => opt.MapFrom(src => src.Post != null && src.Post.Request != null ? (Guid?)src.Post.Request.RequesterID : null));
        }
    }
}
