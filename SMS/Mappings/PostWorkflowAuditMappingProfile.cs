using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities.Audit;

namespace SMS.Mappings;

public sealed class PostWorkflowAuditMappingProfile : Profile
{
    public PostWorkflowAuditMappingProfile()
    {
        CreateMap<PostWorkflowAudit, PostWorkflowAuditDto>();
    }
}
