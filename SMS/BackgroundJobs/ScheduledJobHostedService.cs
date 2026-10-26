using SMS.Generic.Enums;

namespace SMS.BackgroundJobs;

/// <summary>
/// Hosted service that every minute calls <see cref="IScheduledJob.RunIfDueAsync"/> on each registered job.
/// Each job (via <see cref="ScheduledJobBase"/>) owns its own cron expression, scope factory, and last-run state.
/// </summary>
public class ScheduledJobHostedService : BackgroundService
{
    private readonly IEnumerable<IScheduledJob> _jobs;
    private readonly ILogger<ScheduledJobHostedService> _logger;

    public ScheduledJobHostedService(
        IEnumerable<IScheduledJob> jobs,
        ILogger<ScheduledJobHostedService> logger)
    {
        _jobs = jobs?
        .OrderBy(x => x.JobType == JobType.PayrollExport)
        .ToList()
        ?? new List<IScheduledJob>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_jobs.Any())
        {
            _logger.LogInformation("No scheduled jobs registered. Scheduler will not run.");
            return;
        }

        _logger.LogInformation("Scheduled job host started with {Count} job(s).", _jobs.Count());

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var job in _jobs)
            {
                try
                {
                    await job.RunIfDueAsync(stoppingToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in scheduled job.");
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken).ConfigureAwait(false);
        }
    }
}
