using log4net;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace SMS.Logger.Logger;

/// <summary>
/// Implementation of ISMSLogger providing structured logging capabilities using log4net.
/// </summary>
public class SMSLogger : ISMSLogger
{
    private readonly ILog _logger;
    private readonly string _className;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SMSLogger"/> class.
    /// </summary>
    /// <param name="name">The name of the logger (typically the class name).</param>
    /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
    public SMSLogger(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Logger name cannot be null or empty", nameof(name));

        _logger = LogManager.GetLogger(name);
        _className = name;
        _httpContextAccessor = null; // Will be set via dependency injection if needed
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SMSLogger"/> class with HTTP context accessor.
    /// </summary>
    /// <param name="name">The name of the logger (typically the class name).</param>
    /// <param name="httpContextAccessor">The HTTP context accessor for request context.</param>
    /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when httpContextAccessor is null.</exception>
    public SMSLogger(string name, IHttpContextAccessor httpContextAccessor)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Logger name cannot be null or empty", nameof(name));

        _logger = LogManager.GetLogger(name);
        _className = name;
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Formats a log message with timestamp, class name, method name, and line number.
    /// </summary>
    /// <param name="message">The log message to format.</param>
    /// <param name="memberName">The name of the calling method.</param>
    /// <param name="lineNumber">The line number where the log was called.</param>
    /// <returns>A formatted log message.</returns>
    private string BasicLog(string message, [CallerMemberName] string memberName = "", int lineNumber = 0)
    {
        if (string.IsNullOrWhiteSpace(message))
            return string.Empty;

        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        return $"[{timestamp}] [{_className}.{memberName}:{lineNumber}] {message}";
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <exception cref="ArgumentException">Thrown when message is null or empty.</exception>
    public void Info(string message,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or empty", nameof(message));

        try
        {
            var msg = this.BasicLog(message, memberName, lineNumber);
            _logger.Info(msg);
        }
        catch (Exception ex)
        {
            // Log the error to the underlying logger without causing recursion
            _logger.Error($"Failed to log info message: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Logs an informational message with formatted parameters.
    /// </summary>
    /// <param name="format">The format string for the message.</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <param name="args">The arguments to format into the message.</param>
    /// <exception cref="ArgumentException">Thrown when format is null or empty.</exception>
    public void InfoFormat(string format,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        if (string.IsNullOrWhiteSpace(format))
            throw new ArgumentException("Format cannot be null or empty", nameof(format));

        try
        {
            var message = this.BasicLog(String.Format(format, args), memberName, lineNumber);
            _logger.Info(message);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to log info format message: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Serializes a data object to JSON string for logging purposes.
    /// </summary>
    /// <param name="message">The object to serialize.</param>
    /// <returns>A JSON string representation of the object, or the string representation for primitive types.</returns>
    private string SerializeDataObjectToJson(object message)
    {
        if (message == null)
            return "null";

        try
        {
            var messageType = message.GetType();
            
            // For primitive types, return string representation
            if (messageType == typeof(int) || messageType == typeof(double) || 
                messageType == typeof(float) || messageType == typeof(decimal) || 
                messageType == typeof(string))
            {
                return message.ToString() ?? "n/a";
            }

            // For complex objects, serialize to JSON
            return JsonSerializer.Serialize(message, new JsonSerializerOptions 
            { 
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            // Fallback to string representation if JSON serialization fails
            return message.ToString() ?? $"Serialization failed: {ex.Message}";
        }
    }

    /// <summary>
    /// Logs an informational message with an object that will be serialized to JSON.
    /// </summary>
    /// <param name="message">The object to log (will be serialized to JSON).</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <exception cref="ArgumentNullException">Thrown when message is null.</exception>
    public void Info(object message, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            var msg = this.BasicLog(SerializeDataObjectToJson(message), memberName, lineNumber);
            _logger.Info(msg);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to log info object message: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <exception cref="ArgumentException">Thrown when message is null or empty.</exception>
    public void Debug(string message, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or empty", nameof(message));

        try
        {
            var msg = this.BasicLog(message, memberName, lineNumber);
            _logger.Debug(msg);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to log debug message: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Logs an error message with an exception.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <exception cref="ArgumentException">Thrown when message is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when exception is null.</exception>
    public void Error(string message, Exception exception,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        try
        {
            var msg = this.BasicLog(message, memberName, lineNumber);
            _logger.Error(msg, exception);
        }
        catch (Exception ex)
        {
            // Use the underlying logger directly to avoid recursion
            _logger.Error($"Failed to log error message: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Logs an error message with an object that will be serialized to JSON and an exception.
    /// </summary>
    /// <param name="message">The object to log (will be serialized to JSON).</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <exception cref="ArgumentNullException">Thrown when message or exception is null.</exception>
    public void Error(object message, Exception exception, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        try
        {
            var msg = this.BasicLog(SerializeDataObjectToJson(message), memberName, lineNumber);
            _logger.Error(msg, exception);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to log error object message: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Logs an error message with formatted parameters and an exception.
    /// </summary>
    /// <param name="format">The format string for the message.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <param name="args">The arguments to format into the message.</param>
    /// <exception cref="ArgumentException">Thrown when format is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when exception is null.</exception>
    public void ErrorFormat(string format, Exception exception, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0,
        params object[] args)
    {
        if (string.IsNullOrWhiteSpace(format))
            throw new ArgumentException("Format cannot be null or empty", nameof(format));
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        try
        {
            var msg = this.BasicLog(String.Format(format, args), memberName, lineNumber);
            _logger.Error(msg, exception);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to log error format message: {ex.Message}", ex);
        }
    }
}
