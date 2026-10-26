using SMS.Generic.Models.Constants;
using System.ComponentModel;
using System.Reflection;

namespace SMS.Helpers
{
    public static class ValidationMessageHelper
    {
        public static string GetMessage(string errorCode, params object[] args)
        {
            var field = typeof(ModelValidationErrorCodes)
                .GetField(errorCode);

            var description = field?
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description;

            return description == null
                ? errorCode
                : string.Format(description, args);
        }
    }
}
