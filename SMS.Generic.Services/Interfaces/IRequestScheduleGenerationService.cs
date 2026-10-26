using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    public interface IRequestScheduleGenerationService
        : IGenericService<RequestScheduleGenerationDto, RequestScheduleGeneration>
    {
        Task<PagedList<RequestScheduleGenerationDto>> GetByRequestIdAsync(Guid requestId, PageParam? pageParam);
    }
}
