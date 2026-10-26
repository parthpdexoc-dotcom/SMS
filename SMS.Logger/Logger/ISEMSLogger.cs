using System.Runtime.CompilerServices;

namespace SMS.Logger.Logger;

/// <summary>
/// Interface for SMS logging functionality providing structured logging capabilities.
/// </summary>
public interface ISMSLogger
{
    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <exception cref="ArgumentException">Thrown when message is null or empty.</exception>
    void Info(string message, [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0);
    
    /// <summary>
    /// Logs an informational message with formatted parameters.
    /// </summary>
    /// <param name="format">The format string for the message.</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <param name="args">The arguments to format into the message.</param>
    /// <exception cref="ArgumentException">Thrown when format is null or empty.</exception>
    void InfoFormat(string format, [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0, params object[] args);

    /// <summary>
    /// Logs an informational message with an object that will be serialized to JSON.
    /// </summary>
    /// <param name="message">The object to log (will be serialized to JSON).</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <exception cref="ArgumentNullException">Thrown when message is null.</exception>
    void Info(object message, [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0);

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <exception cref="ArgumentException">Thrown when message is null or empty.</exception>
    void Debug(string message, [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0);

    /// <summary>
    /// Logs an error message with an exception.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <exception cref="ArgumentException">Thrown when message is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when exception is null.</exception>
    void Error(string message, Exception exception,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0);
    
    /// <summary>
    /// Logs an error message with an object that will be serialized to JSON and an exception.
    /// </summary>
    /// <param name="message">The object to log (will be serialized to JSON).</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="memberName">The name of the calling method (automatically populated).</param>
    /// <param name="lineNumber">The line number where the log was called (automatically populated).</param>
    /// <exception cref="ArgumentNullException">Thrown when message or exception is null.</exception>
    void Error(object message, Exception exception,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0);
    
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
    void ErrorFormat(string format, Exception exception, [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0, params object[] args);
}