using System.ComponentModel;

namespace SMS.Generic.Enums;

/// <summary>
/// Identifies background scheduled (cron) jobs in the SMS host.
/// </summary>
public enum JobType
{
    [Description("Update Released On To Released")]
    UpdateReleasedOnToReleased = 0,

    [Description("Update Preferred To Managed")]
    UpdatePreferredToManaged = 1,

    [Description("PTAEO Code")]
    PtaeoCode = 2,

    [Description("Employee")]
    Employee = 3,

    [Description("Tenure Waiting List ")]
    TenureWaitingList = 4,

    [Description("Payroll Export")]
    PayrollExport = 5,

    [Description("Revert Cancellation Pending On Shift Start")]
    RevertCancellationPendingOnShiftStart = 6,

    [Description("Employee Job Starting Reminder")]
    EmployeeJobStartingReminder = 7,

    [Description("Finalize Post Position Vehicle Allocation")]
    FinalizePostPositionVehicleAllocation = 8
}
