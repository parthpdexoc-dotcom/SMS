using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for post repository operations, extending IGenericRepository.
    /// </summary>
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<Post?> GetPostWithRequestAndPositionsAsync(Guid postId, DateOnly? dateOfWork = null);

        Task<List<Post>> GetPostsByRequestIdAsync(Guid requestId);

        Task<Post?> GetPostWithPositionsByPostIdAsync(
            Guid postId,
            Guid departmentId,
            DateTime shiftWindowStart,
            DateTime shiftWindowEnd,
            CancellationToken cancellationToken = default);
    }
}

