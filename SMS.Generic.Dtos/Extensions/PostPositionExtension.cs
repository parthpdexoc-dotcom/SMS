using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using System.Linq;

namespace SMS.Generic.Dtos.Extensions;
public static class PostPositionExtension
{
    /* =====================================================
       PER-DAY AGGREGATION (SOURCE OF TRUTH)
       ===================================================== */

    /// <summary>
    /// Counts all post positions per work date.
    /// </summary>
    public static Dictionary<DateOnly, int> CountPostsPerDay(this IEnumerable<PostPositionDto> posts)
        => posts
            .GroupBy(p => p.DateOfWork!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

    /// <summary>
    /// Counts filtered post positions per work date.
    /// </summary>
    public static Dictionary<DateOnly, int> CountPostsPerDay(this IEnumerable<PostPositionDto> posts,Func<PostPositionDto, bool> predicate)
        => posts
            .Where(predicate)
            .GroupBy(p => p.DateOfWork!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

    /// <summary>
    /// Calculates worked hours per work date.
    /// </summary>
    public static Dictionary<DateOnly, decimal> CalculateHoursPerDay(this IEnumerable<PostPositionDto> posts)
        => posts
            .GroupBy(p => p.DateOfWork!.Value)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(p =>
                p.PostPositionStatus == PostPositionStatus.ClosedAndSplit || p.PostPositionStatus == PostPositionStatus.Closed
                    ? (p.HoursWorked ?? 0)
                    : p.LengthOfWork
            ) / 60m);

    public static List<DateOnly> ExtractWorkDates(this IEnumerable<DateOnly?> dates)
        => dates
            .Where(d => d.HasValue)
            .Select(d => d!.Value)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

}