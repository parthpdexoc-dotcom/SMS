using System;
using ClosedXML.Excel;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

public class RosterHeader : BaseExcelGenerator
{
    public string RequesterName { get; set; }

    public string? Date { get; set; }

    public RosterHeader(string title,string? rightText)
    {
        RequesterName = title;
        Date = rightText;
    }
    public override int Export(IXLWorksheet worksheet,int currentRow)
    {
        CreateMergedCell(worksheet,currentRow,1,4,RequesterName,(int)ExcelStyleEnum.BoldText);
        CreateCell(worksheet,currentRow,5,Date,(int)ExcelStyleEnum.BoldRightAlignedText,XLAlignmentHorizontalValues.Right);
        ApplyBottomBorder(worksheet, currentRow, 1, 5);
        currentRow++;
        return CreateBlankMergedRow(worksheet, currentRow, 1, 5);
        
    }
}