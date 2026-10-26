using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for post equipment service operations.
    /// </summary>
    public interface IPostEquipmentService : IGenericService<PostEquipmentDto, PostEquipment>
    {
        Task<IReadOnlyList<PostEquipmentWithNameDto>> GetByPostIdWithNamesAsync(Guid postId, PageParam? pagination = null);
    }
}