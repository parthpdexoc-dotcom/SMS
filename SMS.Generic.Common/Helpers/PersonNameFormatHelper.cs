namespace SMS.Generic.Common.Helpers;

/// <summary>
/// Person display-name formatting (comma-separated parts).
/// </summary>
public static class PersonNameFormatHelper
{
    /// <summary>
    /// Joins trimmed <paramref name="lastName"/> then <paramref name="firstName"/> with ", " (e.g. <c>Doe, Jane</c>).
    /// Null or whitespace-only parts are omitted. Returns null when both parts are empty.
    /// </summary>
    public static string? FormatLastNameCommaFirstName(string? lastName, string? firstName)
    {
        var parts = new[] { lastName?.Trim(), firstName?.Trim() }
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
        return parts.Count == 0 ? null : string.Join(", ", parts);
    }
}
