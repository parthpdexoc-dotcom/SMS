using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for closed post position equipment service operations.
    /// </summary>
    public interface IClosedPostPositionEquipmentService : IGenericService<ClosedPostPositionEquipmentDto, ClosedPostPositionEquipment>
    {
        /// <summary>
        /// Gets the closed post position and its related equipment records for a given post position ID.
        /// </summary>
        /// <param name="postPositionId">The post position ID.</param>
        /// <returns>The closed post position with its equipment records.</returns>
        Task<ClosedPostPositionDetailsDto?> GetClosedDetailsByPostPositionIdAsync(Guid postPositionId);

        /// <summary>
        /// Gets all closed post position records and related equipment for a given post position ID.
        /// </summary>
        /// <param name="postPositionId">The post position ID.</param>
        /// <returns>List of closed post position detail records.</returns>
        Task<List<ClosedPostPositionDetailsDto>> GetClosedDetailsListByPostPositionIdAsync(Guid postPositionId);

        /// <summary>
        /// Splits and closes a post position, creating closed records and updating post position status.
        /// </summary>
        /// <param name="request">Split/close request payload.</param>
        /// <returns>The newly created post position.</returns>
        Task<PostPositionDto> SplitAndCloseAsync(SplitPostPositionRequestDto request);

        /// <summary>
        /// Closes a post position, creating closed records and updating post position status.
        /// </summary>
        /// <param name="request">Close request payload.</param>
        /// <returns>The updated post position.</returns>
        Task<PostPositionDto> CloseAsync(SplitPostPositionRequestDto request);

        /// <summary>
        /// Marks a post position as unclosed.
        /// </summary>
        /// <param name="postPositionId">Post position id.</param>
        /// <returns>The number of updated records.</returns>
        Task<int> UncloseAsync(Guid postPositionId);
    }
}
