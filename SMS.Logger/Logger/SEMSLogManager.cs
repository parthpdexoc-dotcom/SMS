using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

namespace SMS.Logger.Logger
{
    /// <summary>
    /// Thread-safe manager for creating and retrieving SMSLogger instances.
    /// </summary>
    public class SMSLogManager
    {
        private static readonly ConcurrentDictionary<string, SMSLogger> LogMap = new ConcurrentDictionary<string, SMSLogger>();

        /// <summary>
        /// Gets a logger instance for the specified type.
        /// </summary>
        /// <param name="clazz">The type to get a logger for.</param>
        /// <returns>A SMSLogger instance for the specified type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when clazz is null.</exception>
        public static SMSLogger GetLogger(Type clazz)
        {
            if (clazz == null)
                throw new ArgumentNullException(nameof(clazz));

            return LogMap.GetOrAdd(clazz.Name, name => new SMSLogger(name));
        }

        /// <summary>
        /// Gets a logger instance for the specified name.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <returns>A SMSLogger instance for the specified name.</returns>
        /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
        public static SMSLogger GetLogger(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Logger name cannot be null or empty", nameof(name));

            return LogMap.GetOrAdd(name, loggerName => new SMSLogger(loggerName));
        }

        /// <summary>
        /// Gets a logger instance for the specified type with HTTP context accessor.
        /// </summary>
        /// <param name="clazz">The type to get a logger for.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor for request context.</param>
        /// <returns>A SMSLogger instance for the specified type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when clazz or httpContextAccessor is null.</exception>
        public static SMSLogger GetLogger(Type clazz, IHttpContextAccessor httpContextAccessor)
        {
            if (clazz == null)
                throw new ArgumentNullException(nameof(clazz));
            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));

            var key = $"{clazz.Name}_HttpContext";
            return LogMap.GetOrAdd(key, name => new SMSLogger(clazz.Name, httpContextAccessor));
        }

        /// <summary>
        /// Gets a logger instance for the specified name with HTTP context accessor.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor for request context.</param>
        /// <returns>A SMSLogger instance for the specified name.</returns>
        /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when httpContextAccessor is null.</exception>
        public static SMSLogger GetLogger(string name, IHttpContextAccessor httpContextAccessor)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Logger name cannot be null or empty", nameof(name));
            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));

            var key = $"{name}_HttpContext";
            return LogMap.GetOrAdd(key, loggerName => new SMSLogger(name, httpContextAccessor));
        }

        /// <summary>
        /// Clears all cached logger instances. Use with caution as this will recreate loggers on next access.
        /// </summary>
        public static void ClearLoggers()
        {
            LogMap.Clear();
        }

        /// <summary>
        /// Gets the number of cached logger instances.
        /// </summary>
        /// <returns>The number of cached logger instances.</returns>
        public static int GetLoggerCount()
        {
            return LogMap.Count;
        }
    }
}

