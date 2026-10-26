namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Result of payroll processing operation
/// </summary>
public class PayrollProcessingResultDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
    public bool IsSuccess { get; set; }
    public string DateRange { get; set; } = string.Empty;
    public string? GeneratedDocumentPath { get; set; }
    public string? WebsiteDocumentPath { get; set; }
    public List<PayrollDataRecordDto> PayrollDataRecords { get; set; } = new();
    public bool FtpUploadSuccess { get; set; }
    public string? FtpFileName { get; set; }
    public bool SftpUploadSuccess { get; set; }
    public string? SftpFileName { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Creates a payroll processing result with start time and date range
    /// </summary>
    public PayrollProcessingResultDto(DateTime startTime, string dateRange)
    {
        StartTime = startTime;
        DateRange = dateRange;
    }

    /// <summary>
    /// Creates a payroll processing result with all properties
    /// </summary>
    public PayrollProcessingResultDto(DateTime startTime, DateTime endTime, bool isSuccess, string dateRange, 
        string? generatedDocumentPath = null, List<PayrollDataRecordDto>? payrollDataRecords = null,
        bool ftpUploadSuccess = false, string? ftpFileName = null,
        bool sftpUploadSuccess = false, string? sftpFileName = null,
        List<string>? errors = null, List<string>? warnings = null)
    {
        StartTime = startTime;
        EndTime = endTime;
        IsSuccess = isSuccess;
        DateRange = dateRange;
        GeneratedDocumentPath = generatedDocumentPath;
        PayrollDataRecords = payrollDataRecords ?? new();
        FtpUploadSuccess = ftpUploadSuccess;
        FtpFileName = ftpFileName;
        SftpUploadSuccess = sftpUploadSuccess;
        SftpFileName = sftpFileName;
        Errors = errors ?? new();
        Warnings = warnings ?? new();
    }
}
