using AutoMapper;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    public class RequestScheduleGenerationService
        : GenericService<RequestScheduleGenerationDto, RequestScheduleGeneration>, IRequestScheduleGenerationService
    {
        public RequestScheduleGenerationService(
            IRequestScheduleGenerationRepository repository,
            IMapper mapper,
            ITokenParser tokenParser)
            : base(repository, mapper, nameof(RequestScheduleGeneration.RequestScheduleGenerationID), tokenParser)
        {
        }

        public Task<PagedList<RequestScheduleGenerationDto>> GetByRequestIdAsync(Guid requestId, PageParam? pageParam) =>
            GetAllPagedAsync(x => x.RequestId == requestId, pageParam);
    }
}
