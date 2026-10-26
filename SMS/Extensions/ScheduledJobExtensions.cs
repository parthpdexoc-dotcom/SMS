using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMS.BackgroundJobs;

namespace SMS.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering scheduled (cron) jobs.
/// To add a new job: implement <see cref="IScheduledJob"/>, register it as singleton and as IScheduledJob here.
/// Cron format: minute hour day month day-of-week (e.g. "0 6 * * *" = daily at 6:00 AM).
/// </summary>
public static class ScheduledJobExtensions
{
    /// <summary>
    /// Registers the scheduled job host and all <see cref="IScheduledJob"/> implementations.
    /// Call this from Program.cs to enable daily/hourly/custom cron jobs.
    /// </summary>
    public static IServiceCollection AddScheduledJobs(this IServiceCollection services, IConfiguration configuration)
    {
        // Each job (ScheduledJobBase) owns its own cron, scope factory, and last-run state.
        // Register each job as singleton and as IScheduledJob (ScheduledJobHostedService resolves IEnumerable<IScheduledJob>)
        services.AddSingleton<UpdateReleasedOnToReleasedJob>();
        services.AddSingleton<IScheduledJob>(sp => sp.GetRequiredService<UpdateReleasedOnToReleasedJob>());

        services.AddSingleton<UpdatePreferredToManagedJob>();
        services.AddSingleton<IScheduledJob>(sp => sp.GetRequiredService<UpdatePreferredToManagedJob>());

        services.AddSingleton<PtaeoCodeJob>();
        services.AddSingleton<IScheduledJob>(sp => sp.GetRequiredService<PtaeoCodeJob>());

        services.AddSingleton<EmployeeJob>();
        services.AddSingleton<IScheduledJob>(sp => sp.GetRequiredService<EmployeeJob>());

        services.AddSingleton<TenureWaitingPostPositionJob>();
        services.AddSingleton<IScheduledJob>(sp => sp.GetRequiredService<TenureWaitingPostPositionJob>());

        services.AddSingleton<PayrollExportJob>();
        services.AddSingleton<IScheduledJob>(sp => sp.GetRequiredService<PayrollExportJob>());

        services.AddSingleton<RevertCancellationPendingOnShiftStartJob>();
        services.AddSingleton<IScheduledJob>(sp => sp.GetRequiredService<RevertCancellationPendingOnShiftStartJob>());

        services.AddSingleton<EmployeeJobStartingReminderJob>();
        services.AddSingleton<IScheduledJob>(sp => sp.GetRequiredService<EmployeeJobStartingReminderJob>());

        services.AddSingleton<FinalizePostPositionVehicleAllocationJob>();
        services.AddSingleton<IScheduledJob>(sp => sp.GetRequiredService<FinalizePostPositionVehicleAllocationJob>());

        services.AddHostedService<ScheduledJobHostedService>();
        return services;
    }
}
