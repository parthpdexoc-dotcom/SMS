using System;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Department location DTO containing geographic coordinates and address information
    /// for showcasing on maps (Master Maps, Employee Location, etc.).
    /// </summary>
    public class DepartmentLocationDto
    {
        public Guid DepartmentID { get; set; }
        public string? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? TimeZoneId { get; set; }
        public bool? Active { get; set; }
    }
}
