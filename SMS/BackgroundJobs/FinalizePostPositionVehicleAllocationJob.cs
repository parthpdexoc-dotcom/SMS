using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.BackgroundJobs;

/// <summary>
/// Scheduled job that finalizes historical vehicle/equipment allocations for started assigned post positions
/// by persisting <see cref="SMS.Generic.Entities.Entities.PostPositionVehicle"/> records.
/// Schedule: <c>ScheduledJobs:FinalizePostPositionVehicleAllocationCron</c>.
/// </summary>
public class FinalizePostPositionVehicleAllocationJob : ScheduledJobBase
{
    public FinalizePostPositionVehicleAllocationJob(
        IServiceScopeFactory scopeFactory,
        ILogger<FinalizePostPositionVehicleAllocationJob> logger,
        IConfiguration configuration)
        : base(
            scopeFactory,
            logger,
            configuration,
            JobType.FinalizePostPositionVehicleAllocation)
    {
    }

    protected override async Task ExecuteAsync(
        IServiceProvider serviceProvider,
        DepartmentCronJobScheduleDto department,
        CancellationToken cancellationToken)
    {
        var departmentNow = department.GetDepartmentNow();
        // Freeze assigned vehicle/equipment records 2 hours before shift start
        var asOfTime = departmentNow.AddHours(2);

        Logger.LogInformation(
            "FinalizePostPositionVehicleAllocationJob started for Department {DepartmentId} | TimeZone {TimeZoneId} | DeptNow {DeptNow:O} | Freezing shifts starting on or before {AsOfTime:O} (2h pre-shift freeze)",
            department.DepartmentId,
            department.TimeZoneId,
            departmentNow,
            asOfTime);

        var employeeService = serviceProvider.GetRequiredService<IEmployeeService>();
        var allocationService = serviceProvider.GetRequiredService<IPostPositionVehicleService>();

        var master = await employeeService
            .GetMasterEmployeeByDepartmentIdAsync(department.DepartmentId)
            .ConfigureAwait(false);

        if (master?.EmployeeID == null || master.EmployeeID == Guid.Empty)
        {
            Logger.LogWarning(
                "FinalizePostPositionVehicleAllocationJob skipped for Department {DepartmentId}: master/system employee not found.",
                department.DepartmentId);
            return;
        }

        var masterDisplayName = PersonNameFormatHelper.FormatLastNameCommaFirstName(master.LastName, master.FirstName)
            ?? "Master System";

        var finalizedCount = await allocationService
            .FinalizeStartedAllocationsForDepartmentAsync(
                department.DepartmentId,
                asOfTime,
                master.EmployeeID.Value,
                masterDisplayName,
                cancellationToken)
            .ConfigureAwait(false);

        if (finalizedCount > 0)
        {
            Logger.LogInformation(
                "FinalizePostPositionVehicleAllocationJob finalized {Count} post position(s) for Department {DepartmentId} (2h pre-shift freeze as of {AsOfTime:O}).",
                finalizedCount,
                department.DepartmentId,
                asOfTime);
        }
        else
        {
            Logger.LogDebug(
                "FinalizePostPositionVehicleAllocationJob found no post positions to finalize for Department {DepartmentId} (2h pre-shift freeze as of {AsOfTime:O}).",
                department.DepartmentId,
                asOfTime);
        }
    }
}
