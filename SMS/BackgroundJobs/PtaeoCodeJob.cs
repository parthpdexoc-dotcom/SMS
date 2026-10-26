using AutoMapper;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Interfaces.Audit;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Interfaces;
using SMS.Helpers;
using System.Collections.Concurrent;
using System.Globalization;

namespace SMS.BackgroundJobs;

/// <summary>
/// Scheduled job that reads PTAEO CSV files from FileTransferRootPath, syncs PTAEO code rows by composite key,
/// then moves successfully processed CSV files into DepartmentPath imports
/// (project, task, award, fund, organization number): inserts/updates active rows from CSV and marks DB-only rows inactive.
/// Enqueues <see cref="PtaeoJobRunAudit"/> rows via <see cref="IAuditQueueService.EnqueueManyAsync"/> only when a row’s data or status actually changes.
/// </summary>
/// <remarks>
/// Overrides <see cref="ScheduledJobBase.RunScheduleDepartmentAsync"/>: today's latest due occurrence stays eligible until
/// the source CSV is gone (moved under imports) or there was nothing to import. A CSV still in the transfer folder after shutdown mid-import is retried on the next run.
/// </remarks>
public class PtaeoCodeJob : ScheduledJobBase
{
    private static readonly StringComparer KeyComparer = StringComparer.OrdinalIgnoreCase;
    private static readonly string PtaeoImportPath = Path.Combine("imports", "Ptaeo");

    private readonly Configurations _configurations;
    private readonly IConfiguration _configuration;
    private readonly IJobAuditService _jobAuditService;
    private readonly IAuditQueueService _auditQueueService;

    public PtaeoCodeJob(
        IServiceScopeFactory scopeFactory,
        ILogger<PtaeoCodeJob> logger,
        Configurations configurations,
        IConfiguration configuration,
        IJobAuditService jobAuditService,
        IAuditQueueService auditQueue)
        : base(
            scopeFactory,
            logger,
            configuration,
            JobType.PtaeoCode)
    {
        _configurations = configurations ?? throw new ArgumentNullException(nameof(configurations));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _jobAuditService = jobAuditService ?? throw new ArgumentNullException(nameof(jobAuditService));
        _auditQueueService = auditQueue ?? throw new ArgumentNullException(nameof(auditQueue));
    }

    protected override Task ExecuteAsync(IServiceProvider serviceProvider, DepartmentCronJobScheduleDto departmentCron, CancellationToken cancellationToken)
        => RunImportCoreAsync(serviceProvider, departmentCron, cancellationToken);

    protected override Task RunScheduleDepartmentAsync(DepartmentCronJobScheduleDto department, CancellationToken cancellationToken)
        => RunFileImportDepartmentScheduleAsync(
            department,
            PtaeoImportPath,
            _configurations.DepartmentPath,
            _configuration,
            sp => RunImportCoreAsync(sp, department, cancellationToken),
            cancellationToken);

    private async Task<bool> RunImportCoreAsync(IServiceProvider serviceProvider, DepartmentCronJobScheduleDto department, CancellationToken cancellationToken)
    {
        var ptaeoCodeService = serviceProvider.GetRequiredService<IPtaeoCodeService>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var departmentService = serviceProvider.GetRequiredService<IDepartmentService>();

        var departmentsRootPath = _configurations.DepartmentPath;
        Directory.CreateDirectory(departmentsRootPath);

        var allRows = new List<Dictionary<string, string>>();
        var readablePtaeoFolderPaths = new List<string>();
        var processedPtaeoFiles = new List<(string SourcePtaeoPath, string DestinationPtaeoPath, string FileName)>();
        var importSourceRoot = _configuration["FileTransferRootPath"]?.Trim();
        if (string.IsNullOrWhiteSpace(importSourceRoot))
        {
            Logger.LogWarning("PtaeoCodeJob: FileTransferRootPath is not configured.");
            return true;
        }

        Directory.CreateDirectory(importSourceRoot);
        var departmentDetails = await departmentService.GetByIdAsync(department.DepartmentId).ConfigureAwait(false);
        if (departmentDetails == null)
        {
            Logger.LogWarning("PtaeoCodeJob: department {DepartmentId} was not found.", department.DepartmentId);
            return true;
        }

        var folderName = departmentDetails.EmployeeDataFolderName?.Trim();
        if (string.IsNullOrWhiteSpace(folderName))
        {
            Logger.LogWarning("PtaeoCodeJob: EmployeeDataFolderName is missing for department {DepartmentId}.", department.DepartmentId);
            return true;
        }

        var sourceDepartmentPath = Path.Combine(importSourceRoot, folderName);
        var sourcePtaeoPath = Path.Combine(sourceDepartmentPath, PtaeoImportPath);
        if (!Directory.Exists(sourcePtaeoPath))
        {
            Directory.CreateDirectory(sourcePtaeoPath);
            Logger.LogInformation("PtaeoCodeJob: created source PTAEO folder for {FolderName}: {SourcePtaeoPath}.", folderName, sourcePtaeoPath);
        }

        Logger.LogInformation("PtaeoCodeJob: reading source folder {FolderName} at {SourcePtaeoPath}.", folderName, sourcePtaeoPath);
        var csvFilePath = Directory.EnumerateFiles(sourcePtaeoPath, "*.csv", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();
        if (csvFilePath == null)
        {
            Logger.LogWarning("PtaeoCodeJob: no CSV files found in source folder {Folder}.", sourcePtaeoPath);
            return true;
        }

        Logger.LogInformation("PtaeoCodeJob: reading CSV file {FileName} from {SourcePtaeoPath}.", Path.GetFileName(csvFilePath), sourcePtaeoPath);
        var result = new CommonCsvFileReader().Read(csvFilePath);

        if (result.ErrorMessage != null)
        {
            Logger.LogWarning("PtaeoCodeJob: {Message}", result.ErrorMessage);
            return true;
        }

        readablePtaeoFolderPaths.Add(sourcePtaeoPath);
        var destinationPtaeoPath = Path.Combine(departmentsRootPath, folderName, PtaeoImportPath);
        Logger.LogInformation("PtaeoCodeJob: CSV file {FileName} read successfully; it will move to {DestinationPtaeoPath} after processing.", Path.GetFileName(csvFilePath), destinationPtaeoPath);
        processedPtaeoFiles.Add((sourcePtaeoPath, destinationPtaeoPath, Path.GetFileName(csvFilePath)));
        if (result.Rows is { Count: > 0 } r)
            allRows.AddRange(r);

        if (readablePtaeoFolderPaths.Count == 0)
        {
            Logger.LogWarning("PtaeoCodeJob: no readable PTAEO CSV files under source root {Path}.", importSourceRoot);
            return true;
        }

        var rowCount = allRows.Count;
        Logger.LogInformation(
            "PtaeoCodeJob: read {RowCount} row(s) from {PtaeoFolderCount} department PTAEO folder(s) under {Root}.",
            rowCount,
            readablePtaeoFolderPaths.Count,
            departmentsRootPath);

        var changedOn = DateTime.UtcNow;

        var existingList = (await ptaeoCodeService.GetAllAsync(x => x.DepartmentID == department.DepartmentId).ConfigureAwait(false)).ToList();

        if (rowCount == 0)
        {
            await SyncMarkAllInactiveAsync(ptaeoCodeService, existingList, changedOn, cancellationToken)
                .ConfigureAwait(false);
            return CompleteAndVerifyFileMove(processedPtaeoFiles, sourcePtaeoPath);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var uniqueCsv = MapUniquePtaeoRowsForDepartment(allRows, mapper, department.DepartmentId);
        var csvKeySet = uniqueCsv.Select(d => BuildCompositeKey(d)).ToHashSet(KeyComparer);

        var existingGrouped = existingList
            .GroupBy(e => BuildCompositeKey(e), KeyComparer)
            .ToDictionary(g => g.Key, g => g.ToList(), KeyComparer);

        var bulkList = new List<PtaeoCodeDto>();
        var auditActions = new List<AuditAction>();
        var auditPreviousRows = new List<PtaeoCodeDto?>();

        foreach (var e in existingList)
        {
            var key = BuildCompositeKey(e);
            if (!csvKeySet.Contains(key))
            {
                if (!e.Status)
                    continue;

                var previous = new PtaeoCodeDto(e);
                var afterInactive = new PtaeoCodeDto(e);
                afterInactive.Status = false;
                bulkList.Add(afterInactive);
                auditActions.Add(AuditAction.UPDATE);
                auditPreviousRows.Add(previous);
            }
        }

        foreach (var csv in uniqueCsv)
        {
            var k = BuildCompositeKey(csv);
            if (existingGrouped.TryGetValue(k, out var matches) && matches.Count > 0)
            {
                foreach (var m in matches)
                {
                    var u = mapper.Map<PtaeoCodeDto>(csv);
                    u.PtaeoCodeID = m.PtaeoCodeID;
                    u.Status = true;
                    if (PtaeoRowContentEquals(m, u))
                        continue;

                    bulkList.Add(u);
                    auditActions.Add(AuditAction.UPDATE);
                    auditPreviousRows.Add(new PtaeoCodeDto(m));
                }
            }
            else
            {
                var ins = mapper.Map<PtaeoCodeDto>(csv);
                ins.PtaeoCodeID = null;
                ins.Status = true;
                bulkList.Add(ins);
                auditActions.Add(AuditAction.INSERT);
                auditPreviousRows.Add(null);
            }
        }

        if (bulkList.Count == 0)
        {
            Logger.LogInformation("PtaeoCodeJob: nothing to persist.");
            return CompleteAndVerifyFileMove(processedPtaeoFiles, sourcePtaeoPath);
        }

        if (bulkList.Count != auditActions.Count || bulkList.Count != auditPreviousRows.Count)
            throw new InvalidOperationException("PtaeoCodeJob: bulk list, audit actions, and previous-row counts must match.");

        cancellationToken.ThrowIfCancellationRequested();

        var upserted = (await ptaeoCodeService.BulkUpsertAsync(bulkList).ConfigureAwait(false)).ToList();
        if (upserted.Count != auditActions.Count)
            Logger.LogWarning(
                "PtaeoCodeJob: upserted count {Upserted} differs from expected {Expected}; audit order may be off.",
                upserted.Count,
                auditActions.Count);

        var take = Math.Min(upserted.Count, auditActions.Count);
        var auditRows = new List<AuditBase>(take);
        for (var i = 0; i < take; i++)
        {
            var afterDto = BuildAuditAfterDto(bulkList[i], upserted[i]);
            auditRows.Add(
                (AuditBase)_jobAuditService.CreatePtaeoAudit(
                    afterDto,
                    auditActions[i],
                    isDelete: false,
                    changedOn,
                    auditPreviousRows[i]));
        }

        await _auditQueueService.EnqueueManyAsync(auditRows, cancellationToken).ConfigureAwait(false);

        Logger.LogInformation(
            "PtaeoCodeJob: persisted {Count} PTAEO change(s) (CSV distinct keys: {Csv}).",
            upserted.Count,
            uniqueCsv.Count);
        return CompleteAndVerifyFileMove(processedPtaeoFiles, sourcePtaeoPath);
    }

    private async Task SyncMarkAllInactiveAsync(
        IPtaeoCodeService ptaeoCodeService,
        List<PtaeoCodeDto> existingList,
        DateTime changedOn,
        CancellationToken cancellationToken)
    {
        if (existingList.Count == 0)
        {
            Logger.LogInformation("PtaeoCodeJob: no CSV rows and no existing PTAEO codes.");
            return;
        }

        var bulkList = new List<PtaeoCodeDto>();
        var auditActions = new List<AuditAction>();
        var auditPreviousRows = new List<PtaeoCodeDto?>();
        foreach (var e in existingList)
        {
            if (!e.Status)
                continue;

            var previous = new PtaeoCodeDto(e);
            var afterInactive = new PtaeoCodeDto(e);
            afterInactive.Status = false;
            bulkList.Add(afterInactive);
            auditActions.Add(AuditAction.UPDATE);
            auditPreviousRows.Add(previous);
        }

        if (bulkList.Count == 0)
        {
            Logger.LogInformation("PtaeoCodeJob: CSV empty — all PTAEO row(s) already inactive; nothing to persist.");
            return;
        }

        var upserted = (await ptaeoCodeService.BulkUpsertAsync(bulkList).ConfigureAwait(false)).ToList();
        var take = Math.Min(upserted.Count, auditActions.Count);
        var auditRows = new List<AuditBase>(take);
        for (var i = 0; i < take; i++)
        {
            var afterDto = BuildAuditAfterDto(bulkList[i], upserted[i]);
            auditRows.Add(
                (AuditBase)_jobAuditService.CreatePtaeoAudit(
                    afterDto,
                    auditActions[i],
                    isDelete: false,
                    changedOn,
                    auditPreviousRows[i]));
        }

        await _auditQueueService.EnqueueManyAsync(auditRows, cancellationToken).ConfigureAwait(false);
        Logger.LogInformation("PtaeoCodeJob: CSV empty — marked {Count} active PTAEO row(s) inactive.", upserted.Count);
    }

    /// <summary>Audit after-state from bulk list; PK merged from upsert when assigned.</summary>
    private static PtaeoCodeDto BuildAuditAfterDto(PtaeoCodeDto bulkRow, PtaeoCodeDto upsertedRow)
    {
        var dto = new PtaeoCodeDto(bulkRow);
        var id = upsertedRow.PtaeoCodeID;
        if (id.HasValue && id.Value != Guid.Empty)
            dto.PtaeoCodeID = id;

        return dto;
    }

    /// <summary>Maps CSV rows to department-scoped PTAEO DTOs; last row wins for duplicate composite keys.</summary>
    private static List<PtaeoCodeDto> MapUniquePtaeoRowsForDepartment(
        List<Dictionary<string, string>> rows,
        IMapper mapper,
        Guid departmentId)
    {
        var byKey = new Dictionary<string, PtaeoCodeDto>(KeyComparer);
        foreach (var row in rows)
        {
            var dto = mapper.Map<PtaeoCodeDto>(row);
            dto.DepartmentID = departmentId;
            byKey[BuildCompositeKey(dto)] = dto;
        }

        return byKey.Values.ToList();
    }

    /// <summary>
    /// Composite identity: ProjectNumber, TaskNumber, AwardNumber, FundNumber, OrganizationNumber (trimmed, case-insensitive).
    /// </summary>
    private static string BuildCompositeKey(PtaeoCodeDto d) =>
        string.Join(
            '\u001e',
            NormalizePart(d.ProjectNumber),
            NormalizePart(d.TaskNumber),
            NormalizePart(d.AwardNumber),
            NormalizePart(d.FundNumber),
            NormalizePart(d.OrganizationNumber));

    private static string NormalizePart(string? s) => (s ?? string.Empty).Trim().ToUpperInvariant();

    /// <summary>True when imported row matches existing DB row (all comparable fields including status).</summary>
    private static bool PtaeoRowContentEquals(PtaeoCodeDto existing, PtaeoCodeDto incoming)
    {
        return NormalizePart(existing.ProjectNumber) == NormalizePart(incoming.ProjectNumber)
            && existing.DepartmentID == incoming.DepartmentID
            && NormalizePart(existing.TaskNumber) == NormalizePart(incoming.TaskNumber)
            && NormalizePart(existing.AwardNumber) == NormalizePart(incoming.AwardNumber)
            && NormalizePart(existing.FundNumber) == NormalizePart(incoming.FundNumber)
            && NormalizePart(existing.OrganizationNumber) == NormalizePart(incoming.OrganizationNumber)
            && NormalizePart(existing.OrganizationName) == NormalizePart(incoming.OrganizationName)
            && existing.StartDate == incoming.StartDate
            && existing.EndDate == incoming.EndDate
            && existing.Status == incoming.Status;
    }

    private void MoveProcessedCsvFiles(IReadOnlyCollection<(string SourcePtaeoPath, string DestinationPtaeoPath, string FileName)> processedFiles)
    {
        foreach (var file in processedFiles)
            ExcelImportFileMover.MoveFilesWithDatePrefix(
                file.SourcePtaeoPath,
                file.DestinationPtaeoPath,
                file.FileName,
                Logger,
                "PtaeoCodeJob");
    }

    private bool CompleteAndVerifyFileMove(
        IReadOnlyCollection<(string SourcePtaeoPath, string DestinationPtaeoPath, string FileName)> processedFiles,
        string sourcePtaeoPath)
    {
        MoveProcessedCsvFiles(processedFiles);
        if (ExcelImportFileMover.HasPendingSourceCsv(sourcePtaeoPath))
        {
            Logger.LogWarning(
                "PtaeoCodeJob: source CSV still present under {SourcePtaeoPath} after import. Occurrence stays eligible for retry.",
                sourcePtaeoPath);
            return false;
        }
        return true;
    }
}
