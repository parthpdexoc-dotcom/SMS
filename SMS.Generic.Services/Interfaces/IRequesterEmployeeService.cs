using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for RequesterEmployee service operations.
    /// </summary>
    public interface IRequesterEmployeeService : IGenericService<RequesterEmployeeDto, RequesterEmployee>
    {
    Task<PagedList<RequestEmployeeResponseDto>> GetAllRequestEmployeeWithDetailsAsync(PageParam? pageParam);
    }
}
