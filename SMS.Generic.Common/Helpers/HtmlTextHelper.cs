using System.Net;
using System.Text.RegularExpressions;

namespace SMS.Generic.Common.Helpers
{
    /// <summary>
    /// Converts HTML content to readable plain text (e.g. email plain-text parts, history storage).
    /// </summary>
    public static class HtmlTextHelper
    {
        public static string ConvertHtmlToPlainText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            var normalized = html
                .Replace("<br>", "\n", StringComparison.OrdinalIgnoreCase)
                .Replace("<br/>", "\n", StringComparison.OrdinalIgnoreCase)
                .Replace("<br />", "\n", StringComparison.OrdinalIgnoreCase)
                .Replace("</p>", "\n\n", StringComparison.OrdinalIgnoreCase)
                .Replace("</div>", "\n", StringComparison.OrdinalIgnoreCase)
                .Replace("</li>", "\n", StringComparison.OrdinalIgnoreCase);

            normalized = Regex.Replace(normalized, "<[^>]+>", string.Empty, RegexOptions.Singleline);
            normalized = WebUtility.HtmlDecode(normalized);
            normalized = normalized.Replace("\r\n", "\n", StringComparison.Ordinal);
            normalized = Regex.Replace(normalized, "\n{3,}", "\n\n");

            return normalized.Trim();
        }
    }
}
