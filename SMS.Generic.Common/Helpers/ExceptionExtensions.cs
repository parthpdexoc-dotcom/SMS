namespace SMS.Generic.Common.Helpers;

public static class ExceptionExtensions
{
    public static string GetUserFacingMessage(this Exception exception) =>
        exception is WebApiException webApiException
            ? webApiException.ErrorMessage
            : exception.Message;
}
