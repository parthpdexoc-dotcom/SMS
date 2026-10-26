using Microsoft.AspNetCore.Hosting;
using SMS.Generic.Services.Interfaces;

namespace SMS.Helpers;

/// <summary>
/// Resolves the base path for uploaded files from app settings and content root.
/// Folder structure: Uploaded File -> DepartmentId -> RequesterId -> RequestId -> FileUniqueId_FileName.ext
/// </summary>
public class UploadFileStoragePathProvider : IUploadFileStoragePathProvider
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    public const string ConfigKey = "UploadedFilesFolder";
    public const string DefaultFolderName = "Uploaded File";

    public UploadFileStoragePathProvider(IWebHostEnvironment env, IConfiguration configuration)
    {
        _env = env;
        _configuration = configuration;
    }

    /// <inheritdoc />
    public string GetBasePath()
    {
        var folderName = (_configuration[ConfigKey] ?? DefaultFolderName)?.Trim();
        if (string.IsNullOrEmpty(folderName))
            folderName = DefaultFolderName;
        return Path.IsPathRooted(folderName)
            ? folderName
            : Path.Combine(_env.ContentRootPath, folderName);
    }
}
