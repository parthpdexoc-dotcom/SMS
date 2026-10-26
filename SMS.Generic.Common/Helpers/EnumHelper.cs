using System.ComponentModel;
using System.Reflection;

namespace SMS.Generic.Common.Helpers
{
    /// <summary>
    /// Common helpers for enum values.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Converts any enum value to its string form. If the enum member has a <see cref="DescriptionAttribute"/>,
        /// returns that description; otherwise returns the enum member name (e.g. for template keys like [Keyword]).
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <returns>The description if present, otherwise the enum member name.</returns>
        public static string ToStringValue<T>(T value) where T : Enum
        {
            if (value == null)
                return string.Empty;

            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return value.ToString() ?? string.Empty;

            var desc = field.GetCustomAttribute<DescriptionAttribute>();
            return string.IsNullOrWhiteSpace(desc?.Description) ? value.ToString() ?? string.Empty : desc.Description;
        }
    }
}
