using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Audit;
using SMS.Generic.Dtos.Dtos.Audit;
using SMS.Generic.Entities.Entities.Audit;

namespace SMS.Generic.Dtos.Extensions;

/// <summary>
/// Converts audit read-model timestamps from UTC to department-local clock for API responses.
/// Applies only to <see cref="AuditBase.ChangedOn"/> and Tier A/B whitelisted <see cref="AuditBase.ChangedFields"/> properties.
/// </summary>
public static class AuditTimezoneConverter
{
    private static readonly Regex DateOnlyPattern = new(
        @"^\d{4}-\d{2}-\d{2}$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    /// <summary>
    /// Tier A (audit timestamps) + Tier B (visible business/processing datetimes in Audit Log UI).
    /// Values match <see cref="SMS.Generic.Common.Audit.AuditedPropertyAttribute"/> display names.
    /// </summary>
    private static readonly HashSet<string> EligibleChangedFieldProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        AuditTimezoneFields.CreatedDateProperty,
        AuditTimezoneFields.EditedDateProperty,
        AuditTimezoneFields.DeletedAtProperty,
        AuditTimezoneFields.CreatedDateTime,
        AuditTimezoneFields.EditedDateTime,
        AuditTimezoneFields.DeletedAt,
        AuditTimezoneFields.ProcessedAt,
        AuditTimezoneFields.PaymentDate,
        AuditTimezoneFields.AllocationDate,
        AuditTimezoneFields.CloseDateTime,
        AuditTimezoneFields.WorkStartDate,
    };

    public static void Apply(PagedList<AuditBase>? page, string? departmentTimeZoneId)
    {
        if (page?.Data == null || page.Data.Count == 0 || string.IsNullOrWhiteSpace(departmentTimeZoneId))
            return;

        var zone = departmentTimeZoneId.ToSafeTimeZone();
        foreach (var row in page.Data)
            Apply(row, zone);
    }

    public static void Apply(AuditBase row, TimeZoneInfo departmentZone)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(departmentZone);

        row.ChangedOn = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(row.ChangedOn, DateTimeKind.Utc),
            departmentZone);

        if (!string.IsNullOrEmpty(row.ChangedFields))
            row.ChangedFields = ConvertChangedFieldsJson(row.ChangedFields, departmentZone);
    }

    private static string ConvertChangedFieldsJson(string json, TimeZoneInfo departmentZone)
    {
        try
        {
            var changes = JsonSerializer.Deserialize<List<AuditFieldChange>>(json, AuditJsonSerializerOptions.ChangedFields);
            if (changes == null || changes.Count == 0)
                return json;

            var anyConverted = false;

            foreach (var change in changes)
            {
                if (!EligibleChangedFieldProperties.Contains(change.Property))
                    continue;

                var oldConverted = ConvertAuditFieldValue(change.Old, departmentZone);
                var newConverted = ConvertAuditFieldValue(change.New, departmentZone);

                if (!ReferenceEquals(oldConverted, change.Old) || !ReferenceEquals(newConverted, change.New))
                    anyConverted = true;

                change.Old = oldConverted;
                change.New = newConverted;
            }

            return anyConverted
                ? JsonSerializer.Serialize(changes, AuditJsonSerializerOptions.ChangedFields)
                : json;
        }
        catch (JsonException)
        {
            return json;
        }
    }

    private static object? ConvertAuditFieldValue(object? value, TimeZoneInfo departmentZone)
    {
        if (value is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                var converted = ConvertAuditDateTimeString(element.GetString(), departmentZone);
                return converted ?? element.GetString();
            }

            if (element.ValueKind == JsonValueKind.Null)
                return null;

            return element.ToString();
        }

        if (value is string s)
            return ConvertAuditDateTimeString(s, departmentZone) ?? s;

        return value;
    }

    private static string? ConvertAuditDateTimeString(string? value, TimeZoneInfo departmentZone)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();

        // Exclude DateOnly business dates (e.g. "2026-06-01") from timezone conversion.
        if (DateOnlyPattern.IsMatch(trimmed))
            return trimmed;

        if (!DateTime.TryParse(
                trimmed,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var utc))
            return null;

        var local = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utc, DateTimeKind.Utc),
            departmentZone);

        return local.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
    }
}
