using System.Net;

namespace SMS.Generic.Common
{
    public class WebApiException : Exception
    {
        private readonly HttpStatusCode _httpStatusCode = HttpStatusCode.BadRequest;
        private readonly int _errorCode;
        private readonly string _errorMessage;

        public WebApiException(
            HttpStatusCode httpStatusCode,
            string errorCode,
            string errorMessage,
            object? responseData = null,
            decimal? refundRequired = null,
            Guid? invoiceId = null,
            Guid? closedPostPositionId = null,
            object? conflictPayload = null) : base(message: errorCode)
        {
            _httpStatusCode = httpStatusCode;
            _errorCode = int.Parse(errorCode);
            _errorMessage = errorMessage;
            RefundRequired = refundRequired;
            InvoiceId = invoiceId;
            ResponseData = responseData;
            ClosedPostPositionId = closedPostPositionId;
            ConflictPayload = conflictPayload;
        }

        public int ErrorCode => _errorCode;

        public string ErrorMessage => _errorMessage;

        public HttpStatusCode HttpStatusCode => _httpStatusCode;

        /// <summary>Overpaid amount to refund before the operation can proceed (invoice total decrease conflicts).</summary>
        public decimal? RefundRequired { get; }

        /// <summary>Invoice affected by the refund requirement.</summary>
        public Guid? InvoiceId { get; }

        public object? ResponseData { get; }

        /// <summary>Closed post position being edited; used to highlight on Modify Invoice after a refund conflict.</summary>
        public Guid? ClosedPostPositionId { get; }

        /// <summary>Submitted edit form payload preserved when an invoice refund conflict blocks the edit.</summary>
        public object? ConflictPayload { get; }
    }
}
