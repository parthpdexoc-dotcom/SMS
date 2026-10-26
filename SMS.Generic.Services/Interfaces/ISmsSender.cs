namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Sends an SMS. Failures are logged and do not throw, so email delivery is not blocked.
    /// </summary>
    public interface ISmsSender
    {
        Task SendAsync(string toPhoneNumber, string body, CancellationToken cancellationToken = default);
    }
}
