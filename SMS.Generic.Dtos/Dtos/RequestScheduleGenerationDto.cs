using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    public class RequestScheduleGenerationDto : BaseDto
    {
        public Guid RequestScheduleGenerationID { get; set; }

        public Guid RequestId { get; set; }

        public RequestScheduleGenerationAction Action { get; set; }

        public RequestTypes RequestType { get; set; }

        public DateOnly RequestStartDate { get; set; }

        public DateOnly RequestEndDate { get; set; }

        public DateOnly EffectiveFromDate { get; set; }

        public List<int>? ReleaseBeforeDaysOfWeek { get; set; }

        public List<int>? ReleaseBeforeDaysOfMonth { get; set; }

        public MonthlyMode? ReleaseBeforeMonthlyMode { get; set; }

        public int? ReleaseBeforeWeekIndex { get; set; }

        public TimeOnly? ScheduledTime { get; set; }
    }
}
