using ClosedXML.Excel;
using SMS.Generic.Enums;

public abstract class BaseExcelGenerator
{
    public List<ExcelStyleConfig> Styles =
    [
        // 0 - Normal Text
        new ExcelStyleConfig(),
        // 1 - Bold Text
        new ExcelStyleConfig{IsBold = true},
        // 2 - Bold Centered Text
        new ExcelStyleConfig{IsBold = true,HorizontalAlignment =XLAlignmentHorizontalValues.Center},
        // 3 - Bold Right Aligned Text
        new ExcelStyleConfig{IsBold = true,HorizontalAlignment =XLAlignmentHorizontalValues.Right},
        // 4 - Bold Underlined Centered Text
        new ExcelStyleConfig{IsBold = true,IsUnderline = true,FontSize =ExcelConstants.SectionTitleFontSize,HorizontalAlignment =XLAlignmentHorizontalValues.Center},
        // 5 - Gray Background Bold Centered Text
        new ExcelStyleConfig{IsBold = true,BackgroundColor =XLColor.LightGray,HorizontalAlignment =XLAlignmentHorizontalValues.Center,BorderStyle =XLBorderStyleValues.Thin},
        // 6 - Border Bold Text
        new ExcelStyleConfig{IsBold = true,BorderStyle =XLBorderStyleValues.Thin},
        // 7 - Border Normal Text
        new ExcelStyleConfig{BorderStyle =XLBorderStyleValues.Thin},
        // 8 - Border Bold Centered Text
        new ExcelStyleConfig{IsBold = true,BorderStyle =XLBorderStyleValues.Thin,HorizontalAlignment =XLAlignmentHorizontalValues.Center}
    ];

    public ExcelStyleConfig GetStyleByIndex(int styleIndex)
    {
        return Styles[styleIndex];
    }

    public abstract int Export(IXLWorksheet worksheet, int currentRow);

    protected void CreateCell(IXLWorksheet worksheet, int row, int column, string? text, int styleIndex, XLAlignmentHorizontalValues? alignment = null)
    {
        var cell = worksheet.Cell(row, column);
        cell.Value = text;
        ApplyStyle(worksheet.Range(row, column, row, column), GetStyleByIndex(styleIndex));

        if (alignment.HasValue)
        {
            cell.Style.Alignment.Horizontal = alignment.Value;
        }
    }
    protected void CreateTextRow(IXLWorksheet worksheet,int rowNumber,ExcelRow row,int styleIndex, List<double>? columnWidths = null)
    {
        for (int column = 0;column < row.Cells.Count;column++)
        {
            CreateCell(worksheet,rowNumber,column + 1,row.Cells[column].Text,styleIndex,row.Cells[column].Alignment);
        }
        worksheet.Row(rowNumber).AdjustToContents();
        if (columnWidths != null)
            worksheet.Row(rowNumber).Height = GetRowHeight(row.Cells.Select(x => x.Text).ToList(),columnWidths);
    }
    protected void CreateMergedCell(IXLWorksheet worksheet, int row, int startColumn, int endColumn, string? text, int styleIndex)
    {
        var range = worksheet.Range(row, startColumn, row, endColumn);
        range.Merge();
        range.Value = text;
        ApplyStyle(range, GetStyleByIndex(styleIndex));
    }
    protected void CreateColumnRow(IXLWorksheet worksheet,int row,List<ExcelModel> columns)
    {
        for (int column = 0;column < columns.Count;column++)
        {
            CreateCell(worksheet,row,column + 1,columns[column].Header,(int)ExcelStyleEnum.GrayBackgroundBoldCenteredText,XLAlignmentHorizontalValues.Center);
            SetColumnWidth(worksheet,column + 1,columns[column].Width);
        }
    }
    protected void ApplyStyle(IXLRange range, ExcelStyleConfig style)
    {
        range.Style.Font.Bold = style.IsBold;
        range.Style.Font.Italic = style.IsItalic;
        range.Style.Font.Underline = style.IsUnderline ? XLFontUnderlineValues.Single : XLFontUnderlineValues.None;
        range.Style.Font.FontSize = style.FontSize;
        range.Style.Font.FontName = style.FontName;
        range.Style.Font.FontColor = style.FontColor;
        range.Style.Fill.BackgroundColor = style.BackgroundColor;
        if (style.HorizontalAlignment.HasValue)
        {
            range.Style.Alignment.Horizontal =
                style.HorizontalAlignment.Value;
        }

        if (style.VerticalAlignment.HasValue)
        {
            range.Style.Alignment.Vertical =
                style.VerticalAlignment.Value;
        }
        range.Style.Alignment.WrapText = style.WrapText;
        ApplyBorder(range, style);
    }

    private void ApplyBorder(IXLRange range, ExcelStyleConfig style)
    {
        range.Style.Border.TopBorder = style.BorderStyle;
        range.Style.Border.BottomBorder = style.BorderStyle;
        range.Style.Border.LeftBorder = style.BorderStyle;
        range.Style.Border.RightBorder = style.BorderStyle;
        range.Style.Border.OutsideBorder = style.BorderStyle;
    }

    protected void ApplyBottomBorder(IXLWorksheet worksheet,int row,int startColumn,int endColumn)
    {
        worksheet.Range(row,startColumn,row,endColumn).Style.Border.BottomBorder =XLBorderStyleValues.Medium;
    }

    protected void SetColumnWidth(IXLWorksheet worksheet,int column,double width)
    {
        worksheet.Column(column).Width = width;
    }

    protected int CreateBlankRow(IXLWorksheet worksheet, int row)
    {
        worksheet.Row(row).Height = ExcelConstants.BlankRowHeight;
        return row + 1;
    }
    protected void ApplyMediumOuterBorder(IXLWorksheet worksheet,int startRow,int endRow,int startColumn,int endColumn)
    {
        worksheet.Range(startRow,startColumn,endRow,endColumn).Style.Border.OutsideBorder =XLBorderStyleValues.Medium;
    }
    protected void ApplyThinBorder(IXLWorksheet worksheet,int startRow,int endRow,int startColumn,int endColumn)
    {
        worksheet.Range(startRow,startColumn,endRow,endColumn).Style.Border.OutsideBorder =XLBorderStyleValues.Thin;
    }
    protected int AddBlankLine(IXLWorksheet worksheet,int currentRow)
    {
        worksheet.Row(currentRow).Height = ExcelConstants.BlankRowHeight;
        return currentRow + 1;
    }
    protected int CreateBlankMergedRow(IXLWorksheet worksheet,int row,int startColumn,int endColumn)
    {
        CreateMergedCell(worksheet,row,startColumn,endColumn,string.Empty,(int)ExcelStyleEnum.NormalText);
        worksheet.Row(row).Height = ExcelConstants.BlankRowHeight;
        return row + 1;
    }
    protected double GetRowHeight(List<string?> values,List<double> columnWidths)
    {
        double maxHeight = ExcelConstants.DefaultRowHeight;
        for (int index = 0; index < values.Count;index++)
        {
            var value = values[index];

            if (string.IsNullOrWhiteSpace(value))
                continue;

            double width = columnWidths[index];

            int charactersPerLine = (int)(width * 1.2);

            int lineCount = (int)Math.Ceiling((double)value.Length /charactersPerLine);
            double calculatedHeight =Math.Max(ExcelConstants.DefaultRowHeight,lineCount *ExcelConstants.DefaultRowHeight);

            if (calculatedHeight > maxHeight)
                maxHeight =calculatedHeight;
        }

        return maxHeight;
    }
}