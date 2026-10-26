namespace SMS.Generic.Dtos.Dtos
{
    public class PostCallSignDto : BaseDto
    {
        public Guid? PostCallSignID { get; set; }

        public Guid PostID { get; set; }

        public string? CallSign { get; set; }

        public PostCallSignDto CloneForPost(Guid newPostId)
        {
            return new PostCallSignDto
            {
                PostCallSignID = Guid.Empty,
                PostID = newPostId,
                CallSign = CallSign
            };
        }
    }
}
