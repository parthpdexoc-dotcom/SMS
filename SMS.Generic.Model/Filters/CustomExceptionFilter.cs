using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SMS.Generic.Common;
using SMS.Generic.Common.Extensions;
using SMS.Generic.Common.Implementations.Responses;
using System.Net;

namespace SMS.Generic.Models.Filter;

public class CustomExceptionFilter : Filter, IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        var (httpCode, errorCode, errorMessage, shouldTrack) = context.Exception switch
        {
            WebApiException e => (e.HttpStatusCode, e.ErrorCode, e.ErrorMessage, e.HttpStatusCode == HttpStatusCode.InternalServerError),
            _ => (HttpStatusCode.InternalServerError, (int)HttpStatusCode.InternalServerError, context.Exception.Message, true)
        };

        context.Result = new ObjectResult(new SuccessResult<WebApiError>
        {
            Data = default!,
            Status = errorCode,
            Message = errorMessage
        })
        {
            StatusCode = (int)httpCode,
            DeclaredType = typeof(WebApiError)
        };

        if (shouldTrack)
            TrackException(context.Exception);

        return Task.CompletedTask;
    }
}