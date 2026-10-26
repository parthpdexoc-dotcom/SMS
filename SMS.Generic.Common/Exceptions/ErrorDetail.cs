using SMS.Generic.Enums;

namespace SMS.Generic.Common.Exceptions
{
    public class ErrorDetail
    {
        public ErrorType ErrorType { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string FieldName { get; set; }
        public ErrorDetail()
        {

        }
        public ErrorDetail(string errorMessage, ErrorType errorType, string errorCode, string fieldName)
        {
            ErrorType = errorType;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            FieldName = fieldName;
        }
    }
}
