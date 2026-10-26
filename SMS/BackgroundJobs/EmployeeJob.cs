using System.Collections.Concurrent;
using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SMS.Generic.Common.Audit;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Enums;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Interfaces;
using SMS.Helpers;

namespace SMS.BackgroundJobs;

/// <summary>
/// Scheduled job that reads employee CSV files from FileTransferRootPath,
/// then moves successfully processed files into <c>...\DEPARTMENTS\{EmployeeDataFolderName}\imports\User\</c>.
/// Config: DepartmentPath and FileTransferRootPath. Schedule: ScheduledJobs:EmployeeJobCron.
/// Reads HR_STATUS, CURRENT_ASSIGNMENT, DIVISION from each row. Password is not read from sheet.
/// Entity updates/inserts use <c>HasChanges</c> before <see cref="IEmployeeService.BulkUpsertAsync"/>; while the bulk upsert runs, <see cref="IJobAuditRouting.RouteEmployeeAuditsToJobAuditTable"/> is set so interceptor output goes to <c>EmployeeJob_Audit</c> (<c>EmployeeJobRunAudit</c>) only. Manual employee edits continue to write <c>Employee_Audit</c>.
/// </summary>
/// <remarks>
/// Overrides <see cref="ScheduledJobBase.RunScheduleDepartmentAsync"/>: today's latest due occurrence stays eligible until
/// the source CSV is gone (moved under imports) or there was nothing to import. A CSV still in the transfer folder after shutdown mid-import is retried on the next run.
/// </remarks>
public class EmployeeJob : ScheduledJobBase
{
    private static readonly string UserImportPath = Path.Combine("imports", "User");

    private readonly Configurations _configurations;
    private readonly IConfiguration _configuration;

    public EmployeeJob(
        IServiceScopeFactory scopeFactory,
        ILogger<EmployeeJob> logger,
        Configurations configurations,
        IConfiguration configuration)
        : base(
            scopeFactory,
            logger,
            configuration,
            JobType.Employee)
    {
        _configurations = configurations ?? throw new ArgumentNullException(nameof(configurations));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected override Task ExecuteAsync(IServiceProvider serviceProvider, DepartmentCronJobScheduleDto departmentCron, CancellationToken cancellationToken)
        => RunImportCoreAsync(serviceProvider, departmentCron, cancellationToken);

    protected override Task RunScheduleDepartmentAsync(DepartmentCronJobScheduleDto department, CancellationToken cancellationToken)
        => RunFileImportDepartmentScheduleAsync(
            department,
            UserImportPath,
            _configurations.DepartmentPath,
            _configuration,
            sp => RunImportCoreAsync(sp, department, cancellationToken),
            cancellationToken);

    private async Task<bool> RunImportCoreAsync(IServiceProvider serviceProvider, DepartmentCronJobScheduleDto departmentCron, CancellationToken cancellationToken)
    {
        Logger.LogInformation("EmployeeJob: ExecuteAsync Start.");
        var employeeService = serviceProvider.GetRequiredService<IEmployeeService>();
        var employeeTenureService = serviceProvider.GetRequiredService<IEmployeeTenureService>();
        var departmentService = serviceProvider.GetRequiredService<IDepartmentService>();
        var employeeTypeService = serviceProvider.GetRequiredService<IEmployeeTypeService>();
        var employeeStatusService = serviceProvider.GetRequiredService<IEmployeeStatusService>();
        var departmentDivisionsService = serviceProvider.GetRequiredService<IDepartmentDivisionsService>();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var departmentsRootPath = _configurations.DepartmentPath;
        Directory.CreateDirectory(departmentsRootPath);

        var upsertedCount = 0;
        var previousHttpContext = httpContextAccessor.HttpContext;
        try
        {
            if (departmentCron.DepartmentId == Guid.Empty)
            {
                Logger.LogWarning("EmployeeJob: DepartmentId is empty in cron payload. Skipping run.");
                return true;
            }

            httpContextAccessor.HttpContext = null;
            var department = await departmentService.GetByIdAsync(departmentCron.DepartmentId).ConfigureAwait(false);
            if (department == null)
            {
                Logger.LogWarning("EmployeeJob: no department found for DepartmentId {DepartmentId}. Skipping run.", departmentCron.DepartmentId);
                return true;
            }

            if (string.IsNullOrWhiteSpace(department.EmployeeDataFolderName))
            {
                Logger.LogWarning("EmployeeJob: EmployeeDataFolderName is missing for department {DepartmentName} ({DepartmentId}). Skipping run.", department.DepartmentName, department.DepartmentID);
                return true;
            }

            var folderName = department.EmployeeDataFolderName.Trim();
            var departmentFolderPath = Path.Combine(departmentsRootPath, folderName);
            Directory.CreateDirectory(departmentFolderPath);

            Logger.LogInformation("EmployeeJob: processing department \"{DepartmentName}\" using folder \"{FolderName}\".", department.DepartmentName, folderName);

            var master = await employeeService.GetMasterEmployeeByDepartmentIdAsync(departmentCron.DepartmentId).ConfigureAwait(false);
            if (master != null)
            {
                var claims = BuildMasterUserClaims(master, department);
                var identity = new ClaimsIdentity(claims, "JobMaster", ClaimNames.Username, ClaimTypes.Role);
                var principal = new ClaimsPrincipal(identity);
                httpContextAccessor.HttpContext = new DefaultHttpContext { User = principal };
            }
            else
            {
                httpContextAccessor.HttpContext = null;
            }

            var employeeDataPath = Path.Combine(departmentFolderPath, UserImportPath);
            Directory.CreateDirectory(employeeDataPath);

            var importSourceRoot = _configuration["FileTransferRootPath"]?.Trim();
            if (string.IsNullOrWhiteSpace(importSourceRoot))
            {
                Logger.LogWarning("EmployeeJob: FileTransferRootPath is not configured.");
                return true;
            }

            var sourceUserPath = Path.Combine(importSourceRoot, folderName, UserImportPath);
            if (!Directory.Exists(sourceUserPath))
            {
                Directory.CreateDirectory(sourceUserPath);
                Logger.LogInformation("EmployeeJob: created source user folder for {FolderName}: {SourceUserPath}.", folderName, sourceUserPath);
            }

            Logger.LogInformation("EmployeeJob: reading source folder {FolderName} at {SourceUserPath}.", folderName, sourceUserPath);
            var csvFilePath = Directory.EnumerateFiles(sourceUserPath, "*.csv", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
            if (csvFilePath == null)
            {
                Logger.LogWarning("EmployeeJob: no CSV files found in source folder {Folder}.", sourceUserPath);
                return true;
            }

            Logger.LogInformation("EmployeeJob: reading CSV file {FileName} from {SourceUserPath}.", Path.GetFileName(csvFilePath), sourceUserPath);
            var result = new CommonCsvFileReader().Read(csvFilePath);
            if (result.ErrorMessage != null)
            {
                Logger.LogWarning("EmployeeJob: {Message}", result.ErrorMessage);
                return true;
            }

            var rows = result.Rows ?? new List<Dictionary<string, string>>();
            var rowCount = rows.Count;
            var employeesToUpsert = new List<EmployeeDto>();
            var generatedPasswordsByUsername = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Logger.LogInformation("EmployeeJob: read {RowCount} row(s) from {Folder} (department: {DepartmentName}).", rowCount, employeeDataPath, department.DepartmentName);

            foreach (var row in rows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var dto = mapper.Map<EmployeeDto>(row);
                // Password is never sourced from Excel.
                dto.Password = null;

                var statusName = GetRowValue(row, "HR_STATUS");
                var typeName = GetRowValue(row, "CURRENT_ASSIGNMENT");
                var divisionName = GetRowValue(row, "DIVISION");

                dto.DepartmentID = department.DepartmentID;

                EmployeeStatusDto? status = null;
                if (!string.IsNullOrWhiteSpace(statusName))
                {
                    status = await employeeStatusService.GetByNameAsync(statusName, department.DepartmentID).ConfigureAwait(false);
                    if (status == null)
                    {
                        var newStatusDto = new EmployeeStatusDto(
                            Guid.Empty,
                            department.DepartmentID,
                            statusName.Trim(),
                            statusName.Trim(),
                            status: true);
                        status = await employeeStatusService.UpsertAsync(newStatusDto).ConfigureAwait(false);
                        Logger.LogInformation("EmployeeJob: created new employee status \"{StatusName}\" for department.", statusName);
                    }
                }
                dto.StatusID = status?.EmployeeStatusID;

                EmployeeTypeDto? employeeType = null;
                if (!string.IsNullOrWhiteSpace(typeName))
                {
                    employeeType = await employeeTypeService.GetByNameAsync(typeName, department.DepartmentID).ConfigureAwait(false);
                    if (employeeType == null)
                    {
                        var newTypeDto = new EmployeeTypeDto(
                            Guid.Empty,
                            department.DepartmentID,
                            typeName.Trim(),
                            typeName.Trim(),
                            employeeTypeActive: true,
                            rank: 0,
                            isSystemUser: false);
                        employeeType = await employeeTypeService.UpsertAsync(newTypeDto).ConfigureAwait(false);
                        Logger.LogInformation("EmployeeJob: created new employee type \"{TypeName}\" for department.", typeName);
                    }
                }
                else
                {
                    Logger.LogWarning("EmployeeJob: skipping row - CURRENT_ASSIGNMENT is missing. Employee requires a valid employee type. Username: {Username}.", dto.Username ?? "(blank)");
                    continue;
                }
                dto.EmployeeTypeID = employeeType.EmployeeTypeID;

                if (!string.IsNullOrWhiteSpace(divisionName))
                {
                    var division = await departmentDivisionsService.GetByDepartmentIdAndNameAsync(department.DepartmentID, divisionName).ConfigureAwait(false);
                    if (division == null)
                    {
                        var newDivisionDto = new DepartmentDivisionsDto(
                            null,
                            department.DepartmentID,
                            divisionName.Trim(),
                            divisionName.Trim(),
                            divisionActive: true);
                        division = await departmentDivisionsService.UpsertAsync(newDivisionDto).ConfigureAwait(false);
                        Logger.LogInformation("EmployeeJob: created new division \"{DivisionName}\" for department.", divisionName);
                    }
                    dto.DivisionID = division.DivisionID;
                }

                if (string.IsNullOrWhiteSpace(dto.Username) && !string.IsNullOrWhiteSpace(dto.BadgeNumber))
                    dto.Username = dto.BadgeNumber.Trim();

                if (string.IsNullOrWhiteSpace(dto.Username))
                {
                    Logger.LogDebug("EmployeeJob: skipping row with no Username or BadgeNumber.");
                    continue;
                }

                var existingEmployee = (await employeeService.GetAllAsync(
                    filter: e => (e.Username == dto.Username || e.BadgeNumber == dto.BadgeNumber || e.EmployeeNumber == dto.EmployeeNumber) && e.DepartmentID == dto.DepartmentID,
                    includes: null,
                    pageParam: null).ConfigureAwait(false)).FirstOrDefault();
                if (existingEmployee != null)
                {
                    var currentTenure = (await employeeTenureService.GetAllAsync(
                        x => x.EmployeeID == existingEmployee.EmployeeID && x.IsCurrent,
                        null,
                        null).ConfigureAwait(false)).FirstOrDefault();
                    if (currentTenure != null)
                    {
                        existingEmployee.ImportedDateOfHire = currentTenure.DateOfHire;
                        existingEmployee.ImportedDateOfPromotion = currentTenure.DateOfPromotion;
                    }

                    if (existingEmployee.HasChanges(dto))
                    {
                        existingEmployee.CloneEmployee(dto);
                        employeesToUpsert.Add(existingEmployee);
                    }
                    else
                    {
                        Logger.LogDebug("EmployeeJob: no employee changes detected in {Department} Department. Username: {Username}, BadgeNumber: {BadgeNumber}, EmployeeNumber: {EmployeeNumber}. Existing EmployeeID: {ExistingEmployeeID}",
                            department.DepartmentName, dto.Username, dto.BadgeNumber, dto.EmployeeNumber, existingEmployee.EmployeeID);
                    }

                    continue;
                }

                // New employee only: generate a strong 16-char password
                // (must include upper/lower/number/special).
                string generatedPassword;
                do
                {
                    generatedPassword = PasswordHelper.GenerateSecureRandomPassword(16);
                }
                while (!PasswordHelper.IsValidPassword(generatedPassword));

                dto.Password = generatedPassword;
                if (!string.IsNullOrWhiteSpace(generatedPassword)) 
                dto.IsPasswordReset = true;
                generatedPasswordsByUsername[dto.Username] = generatedPassword;

                Logger.LogInformation("Employee" + JsonConvert.SerializeObject(dto));

                employeesToUpsert.Add(dto);
            }

            if (employeesToUpsert.Any())
            {
                var jobAuditRouting = serviceProvider.GetRequiredService<IJobAuditRouting>();
                jobAuditRouting.RouteEmployeeAuditsToJobAuditTable = true;
                try
                {
                    var upsertedEmployees = (await employeeService.BulkUpsertAsync(employeesToUpsert).ConfigureAwait(false)).ToList();
                    upsertedCount += upsertedEmployees.Count;

                    // Send username/password email only for newly created employees from this run.
                    foreach (var upsertedEmployee in upsertedEmployees)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (string.IsNullOrWhiteSpace(upsertedEmployee.Username))
                            continue;

                        if (generatedPasswordsByUsername.TryGetValue(upsertedEmployee.Username, out var plainTextPassword))
                        {
                            await employeeService.SendEmployeePasswordResetEmailByAdminAsync(
                                isUpdate: false,
                                upsertedEmployee: upsertedEmployee,
                                plainTextPassword: plainTextPassword,
                                bypassRoleCheck: true).ConfigureAwait(false);
                        }
                    }
                }
                finally
                {
                    jobAuditRouting.RouteEmployeeAuditsToJobAuditTable = false;
                }
            }

            ExcelImportFileMover.MoveFilesWithDatePrefix(sourceUserPath, employeeDataPath, Path.GetFileName(csvFilePath), Logger, "EmployeeJob");

            if (ExcelImportFileMover.HasPendingSourceCsv(sourceUserPath))
            {
                Logger.LogWarning(
                    "EmployeeJob: source CSV still present under {SourceUserPath} after import. Occurrence stays eligible for retry.",
                    sourceUserPath);
                return false;
            }
        }
        finally
        {
            httpContextAccessor.HttpContext = previousHttpContext;
        }

        if (upsertedCount == 0)
            Logger.LogInformation("EmployeeJob: no valid rows to upsert.");
        else
            Logger.LogInformation("EmployeeJob: upserted {Count} employee(s).", upsertedCount);

        return true;
    }

    private static List<Claim> BuildMasterUserClaims(EmployeeDto master, DepartmentDto department)
    {
        var fullName = $"{master.FirstName ?? ""} {master.LastName ?? ""}".Trim();
        if (string.IsNullOrWhiteSpace(fullName))
            fullName = "Master System";

        var claims = new List<Claim>
        {
            new(ClaimNames.UserType, "Employee"),
            new(ClaimNames.EmployeeID, master.EmployeeID.ToString()),
            new(ClaimTypes.Name, master.Username ?? "JobMaster"),
            new(ClaimNames.Username, master.Username ?? "JobMaster"),
            new(ClaimNames.FirstName, master.FirstName ?? ""),
            new(ClaimNames.LastName, master.LastName ?? ""),
            new(ClaimNames.FullName, fullName),
            new(ClaimNames.DepartmentID, department.DepartmentID.ToString()),
            new(ClaimNames.DepartmentName, department.DepartmentName ?? ""),
            new(ClaimNames.DepartmentCode, department.DepartmentCode ?? ""),
            new(ClaimNames.BadgeNumber, master.BadgeNumber ?? "")
        };
        if (master.EmployeeTypeID.HasValue)
            claims.Add(new Claim(ClaimNames.EmployeeTypeID, master.EmployeeTypeID.Value.ToString()));
        if (master.StatusID.HasValue)
            claims.Add(new Claim(ClaimNames.StatusID, master.StatusID.Value.ToString()));
        return claims;
    }

    private static string? GetRowValue(Dictionary<string, string> row, string key)
    {
        var pair = row.FirstOrDefault(p => string.Equals(p.Key, key, StringComparison.OrdinalIgnoreCase));
        return pair.Value?.Trim();
    }

}
