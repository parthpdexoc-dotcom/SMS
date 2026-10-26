using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SMS.Helpers
{
    /// <summary>
    /// Helper class for formatting validation error messages.
    /// </summary>
    public static class ValidationErrorHelper
    {
        /// <summary>
        /// Formats validation errors from ModelState into a structured response.
        /// </summary>
        public static object FormatValidationErrors(ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => new
                {
                    field = FormatFieldName(x.Key),
                    message = GetErrorMessage(x.Key, e)
                }))
                .ToList();

            return new
            {
                error = "Validation failed",
                errors = errors
            };
        }

        /// <summary>
        /// Gets a user-friendly error message from a ModelError.
        /// </summary>
        private static string GetErrorMessage(string fieldName, ModelError error)
        {
            // Use custom error message if provided
            if (!string.IsNullOrEmpty(error.ErrorMessage))
            {
                return error.ErrorMessage;
            }

            // Handle exception-based errors
            if (error.Exception != null)
            {
                var exceptionMessage = error.Exception.Message;
                
                // GUID conversion errors
                if (exceptionMessage.Contains("could not be converted to System.Guid", StringComparison.OrdinalIgnoreCase))
                {
                    return $"{FormatFieldName(fieldName)} must be a valid GUID format.";
                }
                
                // Type conversion errors
                if (exceptionMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
                {
                    return $"{FormatFieldName(fieldName)} has an invalid format.";
                }
                
                // Missing required fields
                if (exceptionMessage.Contains("is required", StringComparison.OrdinalIgnoreCase) || 
                    exceptionMessage.Contains("required", StringComparison.OrdinalIgnoreCase))
                {
                    return $"{FormatFieldName(fieldName)} is required.";
                }
                
                // Return formatted exception message
                return $"{FormatFieldName(fieldName)}: {exceptionMessage}";
            }
            
            // Default message
            return $"{FormatFieldName(fieldName)} is invalid.";
        }

        /// <summary>
        /// Formats field name for better readability (removes JSON path prefixes, converts to Title Case).
        /// </summary>
        private static string FormatFieldName(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                return "Field";

            // Remove JSON path prefixes
            fieldName = fieldName.Replace("$.", "", StringComparison.Ordinal)
                                 .Replace("$", "", StringComparison.Ordinal);
            
            // Convert camelCase to Title Case
            if (fieldName.Length > 0 && char.IsLower(fieldName[0]))
            {
                fieldName = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
            }
            
            return fieldName;
        }
    }
}
