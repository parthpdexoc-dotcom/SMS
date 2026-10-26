using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for post service operations.
    /// </summary>
    public interface IPostService : IGenericService<PostDto, Post>
    {
        Task<PostDto?> GetPostWithRequestAsync(Guid postId);
        Task<PagedList<PostDto>> GetPostByRequestIdAsync(Guid requestId, PageParam? pagination = null);
        /// <summary>
        /// Gets all posts for a request with hierarchy levels set (no position counts or paging).
        /// </summary>
        Task<IEnumerable<PostDto>> GetAllPostsByRequestIdAsync(Guid requestId);
        Task<PostPositionDto> AssignToPostMeAsync(Guid postId, Guid employeeId, DateOnly dateOfWork, bool isPlaceHolder = false, PlaceholderDto? placeholder = null, Guid? sourcePostPositionId = null, bool enableInvoiceAndPayroll = false, decimal? waitingPostPositionPayRate = null);
        /// <summary>
        /// Gets all posts where IsSupervisorPost is true (for Parent Post dropdown).
        /// When requestId is provided, returns only supervisor posts for that request.
        /// </summary>
        Task<IEnumerable<PostDto>> GetSupervisorPostsAsync(Guid? requestId = null);
        /// <summary>
        /// Builds recursive hierarchy and sets Level property on posts.
        /// </summary>
        /// <param name="posts">List of posts to process</param>
        /// <returns>Ordered list of posts with Level property set</returns>
        List<PostDto> BuildPostHierarchy(List<PostDto> posts);
        /// <summary>
        /// Gets eligible employees for a post by post ID.
        /// Filters employees based on rank, available time, skills, and not already assigned.
        /// </summary>
        /// <param name="postId">The post ID.</param>
        /// <returns>A collection of eligible employees.</returns>
        Task<IEnumerable<EmployeeDto>> GetEligibleEmployeesByPostIdAsync(Guid postId);
        Task<PostDto> ClonePostAsync(Guid postId);

        /// <summary>
        /// Copies valued call-sign catalog rows from source posts onto cloned posts.
        /// </summary>
        Task ClonePostCallSignsAsync(IReadOnlyDictionary<Guid, Guid> oldToNewPostIdMap);

        /// <summary>
        /// Copies equipment and vehicle assignments from source posts onto cloned posts.
        /// </summary>
        Task ClonePostEquipmentAsync(IReadOnlyDictionary<Guid, Guid> oldToNewPostIdMap);

        /// <summary>
        /// Aligns premium flag and pay rate on all posts under a request with the request setting.
        /// </summary>
        Task SyncPremiumPaidEventFromRequestAsync(Guid requestId, bool isPremiumPaidEvent);

        Task NotifyJobsPostedAsync(Guid? requestId = null, params Guid[] postIds);

        /// <summary>
        /// Saves vehicle equipment and sends one combined schedule update email when required.
        /// </summary>
        Task<T> ExecuteVehicleEquipmentChangeAsync<T>(Guid postId, bool isVehicle, Func<Task<T>> saveAsync);

        /// <summary>
        /// Gets filtered posts for a request (Daily Workspace).
        /// </summary>
        Task<PagedList<PostInRequestListDto>> GetPostsByRequestFilterAsync(
            RequestPostsFilterDto filter,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets assigned employees with position details and latest GPS coordinates for a post,
        /// resolving current department date and shifts starting within 30 minutes before or after current department time.
        /// </summary>
        Task<PagedList<PostPositionEmployeeLocationDto>> GetEmployeesWithLocationsByPostIdAsync(
            Guid postId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Emails assigned employees on future (not-started) jobs when post-form vehicles/equipment change.
        /// </summary>
        Task NotifyFutureAssigneesOfVehicleEquipmentChangeAsync(Guid postId);
    }
}

