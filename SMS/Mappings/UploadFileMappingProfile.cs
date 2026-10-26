using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for UploadFile entity and DTO.
    /// </summary>
    public class UploadFileMappingProfile : Profile
    {
        public UploadFileMappingProfile()
        {
            CreateMap<UploadFile, UploadFileDto>()
                .ForMember(d => d.RequestID, o => o.MapFrom(s => s.RequestID.ToString()))
                .ForMember(d => d.FileUploadedId, o => o.MapFrom(s => s.FileUploadedId.ToString()))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedDate));
        }
    }
}
