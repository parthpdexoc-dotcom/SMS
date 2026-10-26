using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common.Implementations.Models;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Logger.Logger;
using System.Net;
using System.Reflection;

namespace SMS.Generic.Api
{

    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly ISMSLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when logger is null.</exception>
        protected BaseController(ISMSLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is provided for backward compatibility.
        /// Consider using the constructor that accepts ISMSLogger for better testability.
        /// </remarks>
        protected BaseController()
        {
            _logger = SMSLogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Gets the result for a collection of items.
        /// </summary>
        /// <typeparam name="T">The Base Model</typeparam>
        /// <param name="obj">The collection of objects.</param>
        /// <returns>The http result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
        protected ActionResult GetResult<T>(ICollection<T> obj) where T : BaseModel
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (!obj.Any())
            {
                _logger.Info($"Empty collection provided for {typeof(T).Name}");
                return NoContent();
            }

            _logger.Debug($"Returning {obj.Count} items of type {typeof(T).Name}");
            return Ok(obj);
        }
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <typeparam name="T">The base model.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="errorBody">if set to <c>true</c> [error body].</param>
        /// <returns>
        /// The http result.
        /// </returns>
        protected ActionResult GetResult<T>(SuccessResult<PagedList<T>> result, string routeName, object routeValues, bool errorBody = false) where T : BaseModel
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            _logger.Debug($"Processing paged result for {typeof(T).Name} with status {result.Status}");

            if (!result.IsValid || !result.IsSuccessStatusCode)
            {
                _logger.Info($"Invalid or unsuccessful result for {typeof(T).Name}: {result.Message}");
                return CreateErrorResponse(result, errorBody);
            }

            var pagedResult = this.GetPagedResult<T>(routeName, routeValues, result.Data);
            _logger.Debug($"Successfully processed paged result for {typeof(T).Name} with {result.Data?.Data?.Count ?? 0} items");

            return this.Content<PagedResult<T>>((HttpStatusCode)result.Status, pagedResult);
        }

        /// <summary>
        /// Gets the result for a single item.
        /// </summary>
        /// <typeparam name="T">The type of the result item.</typeparam>
        /// <param name="result">The result item.</param>
        /// <param name="errorBody">Whether to include error body in response.</param>
        /// <returns>The HTTP result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        protected ActionResult GetResult<T>(T result, bool errorBody = false) where T : BaseModel
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return this.GetResult<T>(result, string.Empty, errorBody);
        }

        /// <summary>
        /// Gets the result for a single item with route information.
        /// </summary>
        /// <typeparam name="T">The type of the result item.</typeparam>
        /// <param name="result">The result item.</param>
        /// <param name="routeName">The name of the route (unused).</param>
        /// <param name="errorBody">Whether to include error body in response.</param>
        /// <returns>The HTTP result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        protected ActionResult GetResult<T>(T result, string routeName, bool errorBody = false) where T : BaseModel
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return this.GetResult<T>(result, string.Empty, errorBody);
        }

        /// <summary>
        /// Gets the result for a success result.
        /// </summary>
        /// <typeparam name="T">The class.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The http result.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        protected ActionResult GetResult<T>(SuccessResult<T> result) where T : BaseModel
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            _logger.Debug($"Processing success result for {typeof(T).Name} with status {result.Status}");

            if (!result.IsValid || !result.IsSuccessStatusCode)
            {
                _logger.Info($"Invalid or unsuccessful result for {typeof(T).Name}: {result.Message}");
                return CreateErrorResponse(result, false);
            }

            _logger.Debug($"Successfully processed result for {typeof(T).Name}");
            return this.Content<T>(HttpStatusCode.OK, result.Data);
        }

        /// <summary>
        /// Gets the error result.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <returns>The http result.</returns>
        protected ActionResult GetErrorResult(HttpStatusCode code, string message)
        {
            return this.Content(code, message);
        }

        /// <summary>
        /// Creates an error response for a success result.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="result">The result containing error information.</param>
        /// <param name="includeErrorBody">Whether to include error body in response.</param>
        /// <returns>The error response.</returns>
        private ActionResult CreateErrorResponse<T>(SuccessResult<T> result, bool includeErrorBody) where T : class
        {
            if (includeErrorBody && !string.IsNullOrWhiteSpace(result.Message))
            {
                return this.GetErrorResult((HttpStatusCode)result.Status, result.Message);
            }

            return this.StatusCode(result.Status);
        }

        /// <summary>
        /// Creates an error response for a success result with data and error message.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="result">The result containing error information.</param>
        /// <param name="includeErrorBody">Whether to include error body in response.</param>
        /// <returns>The error response with data and error message.</returns>
        private ActionResult CreateErrorResponseWithData<T>(SuccessResult<T> result, bool includeErrorBody) where T : class
        {
            if (includeErrorBody && !string.IsNullOrWhiteSpace(result.Message))
            {
                return this.Content<SuccessResult<T>>((HttpStatusCode)result.Status, result);
            }

            return this.StatusCode(result.Status);
        }

        /// <summary>
        /// Creates an error response that always returns the full SuccessResult with data and error message.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="result">The result containing error information.</param>
        /// <returns>The error response with full data and error message.</returns>
        protected ActionResult GetErrorResponseWithData<T>(SuccessResult<T> result) where T : class
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                return this.Content<SuccessResult<T>>((HttpStatusCode)result.Status, result);
            }

            return this.StatusCode(result.Status);
        }

        /// <summary>
        /// Validates that the HTTP status code is within a valid range.
        /// </summary>
        /// <param name="statusCode">The status code to validate.</param>
        /// <returns>True if the status code is valid; otherwise, false.</returns>
        private static bool IsValidHttpStatusCode(int statusCode)
        {
            return statusCode >= 100 && statusCode < 600;
        }

        /// <summary>
        /// Contents the specified status code.
        /// </summary>
        /// <typeparam name="T">The type of content in the entity body.</typeparam>
        /// <param name="statusCode">The status code.</param>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="CustomNegotiatedContentResult{T}"/> with the specified values.</returns>
        internal protected ActionResult Content<T>(HttpStatusCode statusCode, T value)
        {
            ////this.SetJsonIgnoreCondition();
            return StatusCode((int)statusCode, value);
        }

        /// <summary>
        /// Gets the paged result.
        /// </summary>
        /// <typeparam name="T">The Base Model</typeparam>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The paged result.
        /// </returns>
        private PagedResult<T> GetPagedResult<T>(string routeName, object routeValues, PagedList<T> obj) where T : BaseModel
        {
            if (obj == null)
            {
                _logger.Info($"Null paged list provided for {typeof(T).Name}");
                return new PagedResult<T>();
            }

            var result = new PagedResult<T>(obj);
            
            // TODO: Implement HATEOAS links when needed
            // SetHateoasLinks(result.Items);
            
            // TODO: Implement pagination links when needed
            // SetPageLinks(routeName, routeValues, result, obj);

            _logger.Debug($"Created paged result for {typeof(T).Name} with {obj.Data?.Count ?? 0} items");
            return result;
        }
        protected ActionResult GetStatusMessageOnly<T>(SuccessResult<T> result) where T : class
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return StatusCode(
                result.Status,
                new
                {
                    status = result.Status,
                    message = result.Message
                }
            );
        }
        /// <summary>
        /// Gets the value of the specified request header.
        /// </summary>
        /// <param name="headerName">The name of the header to retrieve.</param>
        /// <returns>The header value if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown when headerName is null or empty.</exception>
        protected string GetRequestHeaderValue(string headerName)
        {
            if (string.IsNullOrWhiteSpace(headerName))
                throw new ArgumentException("Header name cannot be null or empty", nameof(headerName));

            HttpContext.Request.Headers.TryGetValue(headerName, out var headerValue);
            return headerValue;
        }
    }
}
