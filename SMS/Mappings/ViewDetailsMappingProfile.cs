using AutoMapper;
using SMS.Generic.Dtos.Dtos;

namespace SMS.Mappings
{
    /// <summary>
    /// Maps full DTOs into the minimal <see cref="ViewDetailsDto"/> contract for the View Details modal.
    /// </summary>
    public class ViewDetailsMappingProfile : Profile
    {
        public ViewDetailsMappingProfile()
        {
            CreateMap<PostDto, ViewDetailsPostInfo>()
                .ForMember(d => d.PostId, o => o.MapFrom(s => s.PostID!.Value))
                .ForMember(d => d.RequestId, o => o.MapFrom(s => s.RequestID))
                .ForMember(d => d.PayCodeId, o => o.MapFrom(s => s.PayCodeID))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName))
                .ForMember(d => d.PrimaryEmail, o => o.MapFrom(s => s.PrimaryEmail))
                .ForMember(d => d.PrimaryPhone, o => o.MapFrom(s => s.PrimaryPhone));

            CreateMap<PostTypeResponseDto, ViewDetailsPostInfo>()
                .ForMember(d => d.PostId, o => o.MapFrom(s => s.PostID))
                .ForMember(d => d.PostStartTime, o => o.MapFrom(s => s.PostStartTime ?? default))
                .ForMember(d => d.PostEndTime, o => o.MapFrom(s => s.PostEndTime ?? default))
                .ForMember(d => d.RequestId, o => o.MapFrom(s => s.RequestID));

            CreateMap<RequestDto, ViewDetailsRequestInfo>()
                .ForMember(d => d.RequestId, o => o.MapFrom(s => s.RequestID))
                .ForMember(d => d.RequesterId, o => o.MapFrom(s => s.RequesterID));

            CreateMap<RequesterDto, ViewDetailsRequesterInfo>()
                .ForMember(d => d.RequesterId, o => o.MapFrom(s => s.RequesterID!.Value))
                .ForMember(d => d.Address, o => o.MapFrom(s => FormatRequesterAddress(s)));

            CreateMap<PostPositionDto, ViewDetailsPostPositionInfo>()
                .ForMember(d => d.PostPositionId, o => o.MapFrom(s => s.PostPositionID!.Value))
                .ForMember(d => d.EmployeeFirstName, o => o.MapFrom(s => s.Employee != null ? s.Employee.FirstName : null))
                .ForMember(d => d.EmployeeLastName, o => o.MapFrom(s => s.Employee != null ? s.Employee.LastName : null));

            CreateMap<UploadFileDto, ViewDetailsAttachment>()
                .ForMember(d => d.RequestId, o => o.MapFrom(s => s.RequestID));

            CreateMap<PostNotesDto, ViewDetailsLatestPostNote>()
                .ForMember(d => d.PostId, o => o.MapFrom(s => s.PostID));
        }

        private static string? FormatRequesterAddress(RequesterDto r)
        {
            var parts = new[]
            {
                r.Address,
                r.Address2,
                r.City,
                r.State,
                r.Zip
            }.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!.Trim()).ToList();

            return parts.Count == 0 ? null : string.Join(", ", parts);
        }
    }
}
