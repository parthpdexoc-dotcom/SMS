using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services;

/// <summary>
/// Main orchestrator service for payroll processing workflow
/// </summary>
public class PayrollProcessingService : IPayrollProcessingService
{
    private readonly IPayrollDataService _dataService;
    private readonly ILogger<PayrollProcessingService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public PayrollProcessingService(
    IPayrollDataService dataService,
    // IPayrollWordDocumentGenerator documentGenerator,
    ILogger<PayrollProcessingService> logger,
    IConfiguration configuration,
    IWebHostEnvironment environment)
    {
        _dataService = dataService;
        // _documentGenerator = documentGenerator;
        _logger = logger;
        _configuration = configuration;
        _environment = environment;
    }

    /// <summary>
    /// Local payroll output: <c>{DepartmentPath}</c>\<c>{department folder}</c>\Export\Payroll under the content root (same config key as employee imports).
    /// Department folder comes from payroll row data when set; otherwise the department id string.
    /// </summary>
    private string ResolvePayrollExportDirectory(Guid departmentId, string? departmentExcelFolderName)
    {
        var departmentsRoot = (_configuration["DepartmentPath"] ?? "ExcelFiles\\DEPARTMENTS").Trim();
        var deptFolder = !string.IsNullOrWhiteSpace(departmentExcelFolderName)
            ? departmentExcelFolderName.Trim()
            : departmentId.ToString();
        var relative = Path.Combine(departmentsRoot, deptFolder, "export");
        return Path.IsPathRooted(relative)
            ? relative
            : Path.Combine(_environment.ContentRootPath, relative);
    }

    /// <summary>
    /// Department-specific suffix of every generated payroll CSV, shared by the writer and by
    /// <see cref="FindExportedPayrollFile"/> so the naming convention is defined once.
    /// </summary>
    private string BuildDepartmentFileSuffix(Guid departmentId) => $"_Dept_{departmentId}.csv";

    /// <inheritdoc />
    public string? FindExportedPayrollFile(Guid departmentId, string? departmentFolderName, DateTime generatedOnOrAfterUtc)
    {
        var exportDirectory = ResolvePayrollExportDirectory(departmentId, departmentFolderName);

        if (!Directory.Exists(exportDirectory))
            return null;

        try
        {
            // Matched on write time rather than the name's timestamp, which uses the server clock and can
            // fall on a different calendar day than the department.
            return Directory
                .EnumerateFiles(exportDirectory, $"*{BuildDepartmentFileSuffix(departmentId)}", SearchOption.TopDirectoryOnly)
                .Where(path => File.GetLastWriteTimeUtc(path) >= generatedOnOrAfterUtc)
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not inspect payroll export directory {ExportDirectory} for department {DepartmentId}.",
                exportDirectory, departmentId);
            return null;
        }
    }

    /// <summary>
    /// Website payroll output: <c>{FileTransferRootPath}</c>\<c>{department folder}</c>\export.
    /// If the root is not configured, server deployments infer it as the sibling cts.checkthesite.com folder.
    /// </summary>
    private string? CopyPayrollToOtherDirectory(Guid departmentId, string? departmentExcelFolderName)
    {
        var websiteRoot = _configuration["FileTransferRootPath"]?.Trim();

        var deptFolder = !string.IsNullOrWhiteSpace(departmentExcelFolderName)
            ? departmentExcelFolderName.Trim()
            : departmentId.ToString();

        return Path.Combine(websiteRoot, deptFolder, "export");
    }

    /// <summary>
    /// Writes payroll rows as UTF-8 CSV — same five columns and value formatting as <see cref="PayrollWordDocumentGenerator"/> (Word table).
    /// </summary>
    private static async Task<string> WritePayrollCsvAsync(List<PayrollDataRecordDto> records, string filePath)
    {
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        await using var writer = new StreamWriter(filePath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        await writer.WriteLineAsync("Employee Number,Start Date Time,End Date Time,Hours Work,Paycode,Project,Task,Award,Fund");
        foreach (var r in records)
        {
            var employeeNumber = r.GetExportEmployeeNumber();
            var start = r.StartDateTime.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var end = r.EndDateTime.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var duration = r.Duration.ToString("000.00", CultureInfo.InvariantCulture);
            var line = string.Join(",",
                EscapeCsvField(employeeNumber),
                EscapeCsvField(start),
                EscapeCsvField(end),
                duration,
                EscapeCsvField(r.PayCode),
                EscapeCsvField(r.Project ?? string.Empty),
                EscapeCsvField(r.Task ?? string.Empty),
                EscapeCsvField(r.Award ?? string.Empty),
                EscapeCsvField(r.Fund ?? string.Empty));
            await writer.WriteLineAsync(line);
        }

        return filePath;
    }

    private static string EscapeCsvField(string value)
    {
        if (value.IndexOfAny([',', '"', '\r', '\n']) >= 0)
            return $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
        return value;
    }

    /// <summary>
    /// Processes payroll data from start date through today
    /// </summary>
    /// <param name="startDate">Start date for payroll data</param>
    /// <param name="customFileName">Optional custom filename for the output document</param>
    /// <returns>Processing result with file paths and status</returns>
    public async Task<PayrollProcessingResultDto> ProcessPayrollDataAsync(
        DateOnly startDate,
        DateOnly endDate,
        string? customFileName = null,
        Guid? departmentId = null)
    {
        var result = new PayrollProcessingResultDto(DateTime.UtcNow, $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

        try
        {
            _logger.LogInformation("Starting payroll processing for date range: {StartDate} to {EndDate}", startDate, endDate);

            // Step 1: Get payroll data (repository filters by department when requested)
            result.PayrollDataRecords = await _dataService.GetPayrollDataAsync(startDate, endDate, departmentId);

            if (!result.PayrollDataRecords.Any())
            {
                _logger.LogWarning("No payroll records found for the specified date range");
                result.Warnings.Add("No payroll records found for the specified date range.");
                result.IsSuccess = true;
                result.EndTime = DateTime.UtcNow;
                return result;
            }

            _logger.LogInformation("Retrieved {Count} payroll records", result.PayrollDataRecords.Count);

            // Step 2: Single CSV — department for paths/name comes from parameter when set; otherwise first row (legacy callers without dept scope).
            var baseFileName = customFileName ?? $"PayrollReport_{DateTime.Now:yyyyMMdd_HHmmss}";
            baseFileName = Path.GetFileNameWithoutExtension(baseFileName);
            if (string.IsNullOrWhiteSpace(baseFileName))
                baseFileName = $"PayrollReport_{DateTime.Now:yyyyMMdd_HHmmss}";

            var deptId = departmentId.HasValue && departmentId.Value != Guid.Empty
                ? departmentId.Value
                : result.PayrollDataRecords.First().DepartmentId;

            var folderName = result.PayrollDataRecords.FirstOrDefault()?.DepartmentExcelFolderName;

            var exportDir = ResolvePayrollExportDirectory(deptId, folderName);
            Directory.CreateDirectory(exportDir);

            var fileName = $"{baseFileName}{BuildDepartmentFileSuffix(deptId)}";
            var documentPath = Path.Combine(exportDir, fileName);

            var generatedPath = await WritePayrollCsvAsync(result.PayrollDataRecords, documentPath);

            _logger.LogInformation("Generated payroll CSV for department {DepartmentId}: {DocumentPath}", deptId, generatedPath);

            var websiteExportDir = CopyPayrollToOtherDirectory(deptId, folderName);
            if (!string.IsNullOrWhiteSpace(websiteExportDir))
            {
                try
                {
                    Directory.CreateDirectory(websiteExportDir);
                    var websiteDocumentPath = Path.Combine(websiteExportDir, "Payroll.csv");
                    File.Copy(generatedPath, websiteDocumentPath, overwrite: true);
                    result.WebsiteDocumentPath = websiteDocumentPath;

                    _logger.LogInformation("Copied payroll CSV for department {DepartmentId} to website export path: {DocumentPath}",
                        deptId, websiteDocumentPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to copy payroll CSV for department {DepartmentId} to website export path {ExportDirectory}",
                        deptId, websiteExportDir);
                    result.Errors.Add($"Website payroll export failed: {ex.Message}");
                }
            }

            /* DOCX — restore IPayrollWordDocumentGenerator + DI, then replace CSV block above with:
            var fileName = $"{baseFileName}_Dept_{deptId}.docx";
            var documentPath = Path.Combine(exportDir, fileName);
            var generatedPath = await _documentGenerator.CreateDocumentAsync(result.PayrollDataRecords, documentPath);
            _logger.LogInformation("Generated Word document for department {DepartmentId}: {DocumentPath}", deptId, generatedPath);
            */

            result.GeneratedDocumentPath = generatedPath;

            result.IsSuccess = !result.Errors.Any();
            result.EndTime = DateTime.UtcNow;

            _logger.LogInformation("Payroll processing completed. Success: {Success}, Duration: {Duration}ms",
                result.IsSuccess, result.Duration.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            result.EndTime = DateTime.UtcNow;
            result.IsSuccess = false;
            result.Errors.Add($"Processing failed: {ex.Message}");

            _logger.LogError(ex, "Payroll processing failed for date range: {DateRange}", result.DateRange);
            return result;
        }
    }

    /// <summary>
    /// Processes payroll data for today
    /// </summary>
    /// <param name="customFileName">Optional custom filename for the output document</param>
    /// <returns>Processing result with file paths and status</returns>
    public async Task<PayrollProcessingResultDto> ProcessTodayPayrollDataAsync(string? customFileName = null)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return await ProcessPayrollDataAsync(today,today, customFileName);
    }

    /// <summary>
    /// Processes payroll data for yesterday
    /// </summary>
    /// <param name="customFileName">Optional custom filename for the output document</param>
    /// <returns>Processing result with file paths and status</returns>
    public async Task<PayrollProcessingResultDto> ProcessYesterdayPayrollDataAsync( DateOnly yesterday,DateOnly today, string? customFileName = null, Guid? departmentId = null)
    {
        return await ProcessPayrollDataAsync(yesterday,today, customFileName, departmentId);
    }

    /// <summary>
    /// Tests all service connections
    /// </summary>
    public async Task<ConnectionTestResultDto> TestConnectionsAsync()
    {
        var result = new ConnectionTestResultDto();

        try
        {
            // Remote FTP/SFTP export removed; payroll files are written under DepartmentPath only.
            result.FtpConnectionSuccess = true;
            result.SftpConnectionSuccess = true;
            _logger.LogInformation("Connection test skipped (payroll uses local export paths only).");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection testing failed");
            throw;
        }
    }

    /// <summary>
    /// Tests department-based payroll data retrieval and grouping
    /// </summary>
    /// <param name="startDate">Start date for test data</param>
    /// <param name="endDate">End date for test data</param>
    /// <returns>Test results with department information</returns>
    public async Task<Dictionary<Guid, int>> TestDepartmentDataRetrievalAsync(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            _logger.LogInformation("Testing department-based payroll data retrieval for {StartDate} to {EndDate}", startDate, endDate);

            // Get payroll data
            var payrollData = await _dataService.GetPayrollDataAsync(startDate,endDate);
            
            if (!payrollData.Any())
            {
                _logger.LogWarning("No payroll data found for testing");
                return new Dictionary<Guid, int>();
            }

            // Group by department
            var departmentGroups = payrollData.GroupBy(r => r.DepartmentId)
                                              .ToDictionary(g => g.Key, g => g.Count());

            _logger.LogInformation("Found {TotalRecords} records across {DepartmentCount} departments", 
                payrollData.Count, departmentGroups.Count);

            // Log department details
            foreach (var department in departmentGroups)
            {
                _logger.LogInformation("Department {DepartmentId}: {RecordCount} records", department.Key, department.Value);
            }

            return departmentGroups;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Department data retrieval test failed");
            throw;
        }
    }

    /// <summary>
    /// Tests department-based document generation without upload
    /// </summary>
    /// <param name="startDate">Start date for test data</param>
    /// <param name="endDate">End date for test data</param>
    /// <returns>List of generated document paths</returns>
    public async Task<List<string>> TestDepartmentDocumentGenerationAsync(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            _logger.LogInformation("Testing department-based document generation for {StartDate} to {EndDate}", startDate, endDate);

            // Get payroll data
            var payrollData = await _dataService.GetPayrollDataAsync(startDate,endDate);
            
            if (!payrollData.Any())
            {
                _logger.LogWarning("No payroll data found for document generation test");
                return new List<string>();
            }

            // Group by department
            var departmentGroups = payrollData.GroupBy(r => r.DepartmentId).ToList();
            var baseFileName = $"TestPayrollReport_{DateTime.Now:yyyyMMdd_HHmmss}";
            var generatedDocuments = new List<string>();

            foreach (var departmentGroup in departmentGroups)
            {
                var groupDepartmentId = departmentGroup.Key;
                var departmentRecords = departmentGroup.ToList();
                var folderName = departmentRecords.FirstOrDefault()?.DepartmentExcelFolderName;

                var exportDir = ResolvePayrollExportDirectory(groupDepartmentId, folderName);
                Directory.CreateDirectory(exportDir);

                var fileName = $"{baseFileName}_Dept_{groupDepartmentId}.csv";
                var documentPath = Path.Combine(exportDir, fileName);

                var generatedPath = await WritePayrollCsvAsync(departmentRecords, documentPath);
                generatedDocuments.Add(generatedPath);

                _logger.LogInformation("Generated test payroll CSV for department {DepartmentId}: {DocumentPath} ({RecordCount} records)",
                    groupDepartmentId, generatedPath, departmentRecords.Count);

                /* DOCX test — restore generator DI + field/ctor, then:
                var fileName = $"{baseFileName}_Dept_{groupDepartmentId}.docx";
                var documentPath = Path.Combine(exportDir, fileName);
                var generatedPath = await _documentGenerator.CreateDocumentAsync(departmentRecords, documentPath);
                generatedDocuments.Add(generatedPath);
                _logger.LogInformation("Generated test document for department {DepartmentId}: {DocumentPath} ({RecordCount} records)",
                    groupDepartmentId, generatedPath, departmentRecords.Count);
                */
            }

            _logger.LogInformation("Test payroll file generation completed. Generated {DocumentCount} files", generatedDocuments.Count);
            return generatedDocuments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Department document generation test failed");
            throw;
        }
    }
}
