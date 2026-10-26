using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    public interface IPostCallSignService : IGenericService<PostCallSignDto, PostCallSign>
    {
        Task<IEnumerable<PostCallSignDto>> GetAvailableForDropdownAsync(Guid postId, DateOnly dateOfWork);

        Task<PostCallSignDto?> GetNextAvailableAsync(Guid postId, DateOnly dateOfWork);

        Task SoftDeleteByPostIdAsync(Guid postId);

        /// <summary>
        /// Copies valued call-sign catalog rows from source posts onto cloned posts.
        /// </summary>
        Task CloneForPostsAsync(IReadOnlyDictionary<Guid, Guid> oldToNewPostIdMap);
    }
}
