using Microsoft.Extensions.Configuration;

namespace SMS.Generic.Common.Helpers;

/// <summary>
/// Central configuration access. Values are read from appsettings via IConfiguration.
/// </summary>
public class Configurations
{
    public Configurations(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    private string GetConfigurationValue(string key)
    {
        return Convert.ToString(Configuration[key]) ?? string.Empty;
    }
    /// <summary>
    /// Cron expression for UpdateReleasedOnToReleasedJob (e.g. "0 6 * * *" = daily at 6:00 AM).
    /// Key: ScheduledJobs:UpdateReleasedOnToReleasedCron
    /// </summary>
    public string UpdateReleasedOnToReleasedCron => GetConfigurationValue("ScheduledJobs:UpdateReleasedOnToReleasedCron");
    /// <summary>
    /// Cron expression for UpdatePreferredToManagedJob (e.g. "*/1 * * * *" = every minute).
    /// Key: ScheduledJobs:UpdatePreferredToManagedCron
    /// </summary>
    public string UpdatePreferredToManagedCron => GetConfigurationValue("ScheduledJobs:UpdatePreferredToManagedCron");

    /// <summary>
    /// Cron expression for PtaeoCodeJob. Key: ScheduledJobs:PtaeoCodeJobCron
    /// </summary>
    public string PtaeoCodeJobCron => GetConfigurationValue("ScheduledJobs:PtaeoCodeJobCron");

    /// <summary>
    /// Excel folder path (relative to content root or absolute). Key: ExcelFolder. Default: ExcelFiles
    /// </summary>
    public string ExcelFolder => GetConfigurationValue("ExcelFolder");

    /// <summary>
    /// PTAEO Excel folder path (relative to content root or absolute). Key: DepartmentPath. Default: ExcelFiles\Departments
    /// </summary>
    public string DepartmentPath => GetConfigurationValue("DepartmentPath");

    /// <summary>
    /// Cron expression for EmployeeJob. Key: ScheduledJobs:EmployeeJobCron
    /// </summary>
    public string EmployeeJobCron => GetConfigurationValue("ScheduledJobs:EmployeeJobCron");

    /// <summary>
    /// Cron expression for TenureWaitingListTestJob (testing job). Key: ScheduledJobs:TenureWaitingListTestJobCron
    /// Default should usually be "*/1 * * * *" (every minute) for test.
    /// </summary>
    public string TenureWaitingListTestJobCron => GetConfigurationValue("ScheduledJobs:TenureWaitingListTestJobCron");

    /// <summary>
    /// Cron expression for PayrollExportJob (e.g. "0 6 * * *" = daily at 6:00 AM UTC). Key: ScheduledJobs:PayrollExportJobCron
    /// </summary>
    public string FileTransferRootPath => GetConfigurationValue("ScheduledJobs:FileTransferRootPath");

    /// <summary>
    /// Root folder containing department subfolders; each subfolder name must match Department.EmployeeDataFolderName and contain the Excel sheet(s).
    /// Key: EmployeeExcelDepartmentsFolder, else falls back to DepartmentPath.
    /// </summary>
    public string EmployeeExcelDepartmentsFolder
    {
        get
        {
            var v = (GetConfigurationValue("EmployeeExcelDepartmentsFolder") ?? string.Empty).Trim();
            if (v.Length > 0) return v;
            return (GetConfigurationValue("DepartmentPath") ?? string.Empty).Trim();
        }
    }

    /// <summary>
    /// Default Department ID for employees imported from Excel when not otherwise determined. Key: DefaultEmployeeDepartmentID
    /// </summary>
    public string DefaultEmployeeDepartmentID => GetConfigurationValue("DefaultEmployeeDepartmentID");

    /// <summary>
    /// Default Employee Type ID for employees imported from Excel. Key: DefaultEmployeeTypeID
    /// </summary>
    public string DefaultEmployeeTypeID => GetConfigurationValue("DefaultEmployeeTypeID");

    /// <summary>
    /// Default Employee Status ID for employees imported from Excel. Key: DefaultEmployeeStatusID
    /// </summary>
    public string DefaultEmployeeStatusID => GetConfigurationValue("DefaultEmployeeStatusID");

    /// <summary>
    /// Default password for new employees imported from Excel (will be hashed). Key: DefaultEmployeePassword
    /// </summary>
    public string DefaultEmployeePassword => GetConfigurationValue("DefaultEmployeePassword");
}
