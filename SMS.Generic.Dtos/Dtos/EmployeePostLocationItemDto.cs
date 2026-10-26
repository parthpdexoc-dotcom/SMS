namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// GPS location reading for an assigned employee.
    /// </summary>
    public class EmployeePostLocationItemDto
    {
        public Guid EmployeePostLocationId { get; set; }
        public Guid EmployeeId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime CapturedAt { get; set; }
    }
}
