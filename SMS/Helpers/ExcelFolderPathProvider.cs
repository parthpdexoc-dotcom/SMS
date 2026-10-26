using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SMS.Generic.Services.Interfaces;

namespace SMS.Helpers;

/// <summary>
/// Resolves the configured Excel folder path from app settings and content root.
/// </summary>
public class ExcelFolderPathProvider : IExcelFolderPathProvider
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    public ExcelFolderPathProvider(IWebHostEnvironment env, IConfiguration configuration)
    {
        _env = env;
        _configuration = configuration;
    }

    /// <inheritdoc />
    public string GetPath()
    {
        var folderName = _configuration["ExcelFolder"] ?? "ExcelFiles";
        return Path.IsPathRooted(folderName)
            ? folderName
            : Path.Combine(_env.ContentRootPath, folderName);
    }
}

public static class ExcelImportFileMover
{
    public static void MoveFilesWithDatePrefix(
        string? sourceDirectory,
        string destinationDirectory,
        string searchPattern,
        ILogger logger,
        string logPrefix)
    {
        if (string.IsNullOrWhiteSpace(sourceDirectory) || !Directory.Exists(sourceDirectory))
            return;

        Directory.CreateDirectory(destinationDirectory);

        var datePrefix = DateTime.Today.ToString("MMddyyyy", CultureInfo.InvariantCulture);
        foreach (var sourceFile in Directory.EnumerateFiles(sourceDirectory, searchPattern, SearchOption.TopDirectoryOnly)
                     .OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            var destPath = Path.Combine(destinationDirectory, $"{datePrefix}_{Path.GetFileName(sourceFile)}");
            destPath = EnsureUniqueDestinationPath(destPath);
            try
            {
                File.Move(sourceFile, destPath, overwrite: false);
                logger.LogInformation("{LogPrefix}: moved import file {Source} -> {Destination}", logPrefix, sourceFile, destPath);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "{LogPrefix}: failed to move import file {Source} -> {Destination}", logPrefix, sourceFile, destPath);
            }
        }
    }

    private static string EnsureUniqueDestinationPath(string destPath)
    {
        if (!File.Exists(destPath))
            return destPath;
        var directory = Path.GetDirectoryName(destPath) ?? string.Empty;
        var name = Path.GetFileNameWithoutExtension(destPath);
        var ext = Path.GetExtension(destPath);
        for (var n = 1; n < 10_000; n++)
        {
            var candidate = Path.Combine(directory, $"{name}_{n}{ext}");
            if (!File.Exists(candidate))
                return candidate;
        }
        return Path.Combine(directory, $"{name}_{Guid.NewGuid():N}{ext}");
    }

    public static bool HasPendingSourceCsv(string? sourceDirectory)
        => !string.IsNullOrWhiteSpace(sourceDirectory)
           && Directory.Exists(sourceDirectory)
           && Directory.EnumerateFiles(sourceDirectory, "*.csv", SearchOption.TopDirectoryOnly).Any();

    public static bool HasArchivedImportForDate(string? archiveDirectory, DateOnly businessDate)
    {
        if (string.IsNullOrWhiteSpace(archiveDirectory) || !Directory.Exists(archiveDirectory))
            return false;

        var datePrefix = businessDate.ToString("MMddyyyy", CultureInfo.InvariantCulture) + "_";
        return Directory.EnumerateFiles(archiveDirectory, "*.csv", SearchOption.TopDirectoryOnly)
            .Any(path => Path.GetFileName(path).StartsWith(datePrefix, StringComparison.OrdinalIgnoreCase));
    }

    public static async Task<(bool Success, string SourcePath, string ArchivePath)> ResolveDepartmentImportPathsAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        string? departmentsRootPath,
        Guid departmentId,
        string relativeSubPath)
    {
        var importSourceRoot = configuration["FileTransferRootPath"]?.Trim();
        if (string.IsNullOrWhiteSpace(importSourceRoot) || string.IsNullOrWhiteSpace(departmentsRootPath))
            return (false, string.Empty, string.Empty);

        var department = await serviceProvider.GetRequiredService<IDepartmentService>()
            .GetByIdAsync(departmentId)
            .ConfigureAwait(false);
        if (department == null || string.IsNullOrWhiteSpace(department.EmployeeDataFolderName))
            return (false, string.Empty, string.Empty);

        var folderName = department.EmployeeDataFolderName.Trim();
        var sourcePath = Path.Combine(importSourceRoot, folderName, relativeSubPath);
        var archivePath = Path.Combine(departmentsRootPath, folderName, relativeSubPath);
        return (true, sourcePath, archivePath);
    }
}
