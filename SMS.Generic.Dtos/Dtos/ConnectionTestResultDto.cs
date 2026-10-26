namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Result of connection testing
/// </summary>
public class ConnectionTestResultDto
{
    public bool FtpConnectionSuccess { get; set; }
    public bool SftpConnectionSuccess { get; set; }
    public bool AllConnectionsSuccessful => FtpConnectionSuccess && SftpConnectionSuccess;
}
