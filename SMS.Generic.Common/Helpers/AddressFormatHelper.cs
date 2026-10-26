namespace SMS.Generic.Common.Helpers;

/// <summary>
/// Single-line postal / mailing address formatting for UI and exports.
/// </summary>
public static class AddressFormatHelper
{
    /// <summary>
    /// Joins non-empty address parts with ", " (trimmed). Returns null when every part is null or whitespace.
    /// </summary>
    public static string? BuildSingleLine(
        string? address,
        string? address2,
        string? city,
        string? state,
        string? zip)
    {
        var parts = new[] { address, address2, city, state, zip }
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!.Trim())
            .ToList();
        return parts.Count == 0 ? null : string.Join(", ", parts);
    }

    /// <summary>
    /// Single-line US-style request site: <c>Street, City, ST 12345</c> (commas after street and city; single space between state and ZIP).
    /// </summary>
    public static string? BuildStreetCityStateZipLine(string? address, string? city, string? state, string? zip)
    {
        static string? T(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        var a = T(address);
        var c = T(city);
        var s = T(state);
        var z = T(zip);
        var stateZip = s == null && z == null
            ? null
            : string.Join(" ", new[] { s, z }.Where(x => x != null));
        var segments = new List<string>();
        if (a != null)
            segments.Add(a);
        if (c != null)
            segments.Add(c);
        if (stateZip != null)
            segments.Add(stateZip);
        return segments.Count == 0 ? null : string.Join(", ", segments);
    }
}
