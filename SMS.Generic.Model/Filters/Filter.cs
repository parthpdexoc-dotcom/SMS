using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SMS.Generic.Common.Exceptions;
using SMS.Generic.Common.Extensions;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Logger.Logger;
using System.Net;
using System.Reflection; // adding comment for testing 

namespace SMS.Generic.Models.Filter
{
    public class Filter : ResultFilterAttribute
    {
        private ISMSLogger _logger = SMSLogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType!);

        protected void TrackException(Exception ex)
        {
            _logger.Error("Unhandled exception. trace_id {TraceIdentifier}", ex);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);

            if (context.ModelState.IsValid) return;
            var allErrors = context.ModelState
                .Where(ms => ms.Value?.ValidationState == ModelValidationState.Invalid)
                .SelectMany(ms => ms.Value!.Errors.Select(err => new ErrorDetail
                (
                    errorMessage: ModelValidationErrorCodes.GetErrorMessage(err.ErrorMessage),
                    errorType: ErrorType.Error,
                    errorCode: err.ErrorMessage,
                    fieldName: ms.Key
                )))
                .ToList();

            var ErrorMessage = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_REQUEST);
            var webApiError = new SuccessResult<WebApiError>()
            {
                Data = null!,
                Status = (int)HttpStatusCode.BadRequest,
                Message = ErrorMessage,
                Errors = allErrors
            };

            context.Result = new ObjectResult(webApiError)
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                DeclaredType = typeof(WebApiError)
            };
        }
    }
}
