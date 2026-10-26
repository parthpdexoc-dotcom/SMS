using SMS.Generic.Enums;

namespace SMS.BackgroundJobs;

/// <summary>
/// Contract for a scheduled job. The host calls <see cref="RunIfDueAsync"/> every tick;
/// the implementation (usually <see cref="ScheduledJobBase"/>) decides whether to run based on cron and last run.
/// </summary>
public interface IScheduledJob
{
    /// <summary>
    /// Stable job identifier (not the CLR type name).
    /// </summary>
    JobType JobType { get; }

    /// <summary>
    /// Checks if the job is due (cron + last run); if so, runs the job and updates last-run state.
    /// </summary>
    Task RunIfDueAsync(CancellationToken cancellationToken = default);
}
