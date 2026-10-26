using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using System.Linq;

namespace SMS.Generic.Dtos.Extensions;
public static class JobTypeExtensions
{
    public static string GetAppSettingCronKey(this JobType jobType) =>
       jobType switch
       {
           JobType.UpdateReleasedOnToReleased => "UpdateReleasedOnToReleasedCron",
           JobType.UpdatePreferredToManaged => "UpdatePreferredToManagedCron",
           JobType.PtaeoCode => "PtaeoCodeJobCron",
           JobType.Employee => "EmployeeJobCron",
           JobType.TenureWaitingList => "TenureWaitingListTestJobCron",
           JobType.PayrollExport => "PayrollExportJobCron",
           JobType.RevertCancellationPendingOnShiftStart => "RevertCancellationPendingOnShiftStartCron",
           JobType.EmployeeJobStartingReminder => "EmployeeJobStartingReminderCron",
           JobType.FinalizePostPositionVehicleAllocation => "FinalizePostPositionVehicleAllocationCron",
           _ => throw new ArgumentOutOfRangeException(nameof(jobType), jobType, null)
       };
}