using Microsoft.Extensions.Configuration;

namespace SMS.Generic.Common.Helpers;

/// <summary>Reads positive minute-based values from the <c>PayrollSettings</c> section.</summary>
public static class PayrollConfigurationExtensions
{
    public static TimeSpan ReadPayrollDuration(this IConfiguration configuration, string key, TimeSpan fallback)
    {
        var raw = configuration[$"PayrollSettings:{key}"];
        return int.TryParse(raw, out var minutes) && minutes > 0
            ? TimeSpan.FromMinutes(minutes)
            : fallback;
    }
}
