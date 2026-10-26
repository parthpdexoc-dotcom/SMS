namespace SMS.Generic.Common.Helpers;

/// <summary>
/// Rounds monetary amounts to standard currency precision (2 decimal places).
/// </summary>
public static class CurrencyRoundHelper
{
    /// <summary>Rounds to two decimal places using away-from-zero midpoint rounding.</summary>
    public static decimal RoundTwo(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);

    /// <summary>Rounds a nullable amount to two decimal places when present.</summary>
    public static decimal? RoundTwo(decimal? value) =>
        value.HasValue ? RoundTwo(value.Value) : null;
}
