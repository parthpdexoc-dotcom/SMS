namespace SMS.Generic.Common.Helpers
{
    public static class DateHelper
    {
        public const string DisplayDateFormat = "MMM dd, yyyy";
        public const string RosterDisplayDateFormat = "dddd, MMMM d, yyyy";
        public const string DisplayTime24HourFormat = "HH:mm";

        public static DateOnly GetEndDate(DateOnly startDate, int numberOfDays)
        {
            return startDate.AddDays(numberOfDays);
        }

        /// <summary>
        /// Combines DateOnly and TimeOnly into DateTime
        /// </summary>
        /// <param name="date">The date component</param>
        /// <param name="time">The time component (nullable)</param>
        /// <returns>Combined DateTime</returns>
        public static DateTime CombineDateAndTime(DateOnly date, TimeOnly? time)
        {
            if (!time.HasValue)
                return date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);
            
            return date.ToDateTime(time.Value, DateTimeKind.Local);
        }

        /// <summary>
        /// Calculates end datetime based on start time and duration in minutes
        /// </summary>
        public static DateTime CalculateEndDateTime(DateOnly date, TimeOnly? startTime, int lengthInMinutes)
        {
            var startDateTime = CombineDateAndTime(date, startTime);
            return startDateTime.AddMinutes(lengthInMinutes);
        }

        public static string ToRosterDisplayDate(DateOnly date)
        {
            return date.ToString(RosterDisplayDateFormat, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string ToDisplayDate(DateOnly? date)
        {
            return date?.ToString(DisplayDateFormat, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
        }

        public static string ToDisplayTime24Hour(TimeOnly time)
        {
            return time.ToString(DisplayTime24HourFormat, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a duration in minutes to display hours string (e.g. 90 -> "1.5").
        /// </summary>
        public static string ToDisplayHoursFromMinutes(int durationMinutes)
        {
            var hours = durationMinutes / 60m;
            return hours.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts minutes to "X hrs Y mins" display text (e.g. 435 -> "7 hrs 15 mins", 600 -> "10 hrs").
        /// </summary>
        public static string ToDisplayHoursAndMinutesFromMinutes(int totalMinutes)
        {
            var hours = totalMinutes / 60;
            var minutes = totalMinutes % 60;

            if (minutes == 0)
                return $"{hours} hrs";

            return $"{hours} hrs {minutes} mins";
        }
    }
}
