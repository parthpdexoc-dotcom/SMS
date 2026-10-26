namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Job / Post with assigned employees and their GPS locations.
    /// </summary>
    public class PostWithEmployeeLocationDto
    {
        public Guid PostId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public TimeOnly PostStartTime { get; set; }
        public TimeOnly PostEndTime { get; set; }
        public string PostLocation { get; set; } = string.Empty;
        public int TotalPositions { get; set; }
        public List<PostPositionEmployeeLocationDto> PostPositions { get; set; } = new();
    }
}
