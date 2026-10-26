namespace SMS.Generic.Dtos.Dtos
{
    public class EmployeePostLocationDto : BaseDto
    {
        public Guid? EmployeePostLocationID { get; set; }

        public Guid PostPositionID { get; set; }

        public Guid EmployeeID { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public DateTime CapturedAt { get; set; }
    }
}
