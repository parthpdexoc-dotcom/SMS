using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping ApprovalDetail entities to DTOs and vice-versa (ApprovalDetails table).
    /// </summary>
    public class ApprovalDetailMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalDetailMappingProfile"/> class.
        /// </summary>
        public ApprovalDetailMappingProfile()
        {
            CreateMap<ApprovalDetail, ApprovalDetailDto>();

            CreateMap<ApprovalDetailDto, ApprovalDetail>()
                .ForMember(dest => dest.ApprovalDetailID, opt =>
                    opt.MapFrom(src => src.ApprovalDetailID ?? Guid.Empty));
        }
    }
}
