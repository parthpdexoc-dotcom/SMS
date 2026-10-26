using System;
using ClosedXML.Excel;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

public class RosterSection : BaseExcelGenerator
{
    public string PostTitle;

    public string? PostDescription;

    public string? PayCode;

    public RosterSection(string title, string? description, string? payCode)
    {
        PostTitle = title;
        PostDescription = description;
        PayCode = payCode;
    }
    public override int Export(IXLWorksheet worksheet, int currentRow)
    {
        CreateMergedCell(worksheet, currentRow, 1, 5, PostTitle, (int)ExcelStyleEnum.BoldUnderlinedCenteredText);
        currentRow++;
        CreateMergedCell(worksheet, currentRow, 1, 4, PostDescription, (int)ExcelStyleEnum.BoldText);
        if (!string.IsNullOrWhiteSpace(PayCode))
            CreateCell(worksheet,currentRow,5,PayCode,(int)ExcelStyleEnum.BoldRightAlignedText,XLAlignmentHorizontalValues.Right);
        return currentRow + 1;
    }
}