using AutoMapper;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

public class ExcelExport
{
    private readonly List<BaseExcelGenerator> _excelElements = new();
    private readonly string _sheetName;

    public ExcelExport(string sheetName,List<BaseExcelGenerator> excelElements)
    {
        _sheetName = sheetName;
        _excelElements = excelElements;
    }
    public byte[] Export()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(_sheetName);
        int currentRow = 1;

        foreach (var element in _excelElements)
            currentRow = element.Export(worksheet,currentRow);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

