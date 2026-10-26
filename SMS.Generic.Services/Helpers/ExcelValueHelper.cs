using System.Text.Json;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using SMS.Generic.Common.Constants;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Services.Interfaces;
public static class ExcelValueHelper
{
    public static string GetValue(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "-"
            : value;
    }
}