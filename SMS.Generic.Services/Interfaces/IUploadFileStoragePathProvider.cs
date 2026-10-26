namespace SMS.Generic.Services.Interfaces;

/// <summary>
/// Resolves the base directory for storing uploaded files (folder structure: Base -> DepartmentId -> RequesterId -> RequestId -> files).
/// </summary>
public interface IUploadFileStoragePathProvider
{
    /// <summary>
    /// Gets the full base path for uploaded files (e.g. ContentRoot/Uploaded File).
    /// </summary>
    string GetBasePath();
}
