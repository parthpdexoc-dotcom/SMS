namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for BlockDate operations.
    /// </summary>
    public class BlockDateDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the block date.
        /// </summary>
        public Guid BlockDateId { get; set; }

        /// <summary>
        /// Gets or sets the request identifier (foreign key to Request).
        /// </summary>
        public Guid? RequestID { get; set; }

        /// <summary>
        /// Gets or sets the post identifier (foreign key to Post).
        /// </summary>
        public Guid? PostID { get; set; }

        /// <summary>
        /// Gets or sets the start date and time (combines StartDate + StartTime).
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the end date and time (combines EndDate + EndTime).
        /// </summary>
        public DateTime EndDateTime { get; set; }

        public BlockDateDto CloneForPost(Guid newPostId)
        {
            return new BlockDateDto
            {
                BlockDateId = Guid.Empty,
                RequestID = RequestID,
                PostID = newPostId,
                StartDateTime = StartDateTime,
                EndDateTime = EndDateTime
            };
        }
    }
}
