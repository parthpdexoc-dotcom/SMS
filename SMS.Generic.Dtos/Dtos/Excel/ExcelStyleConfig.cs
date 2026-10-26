using ClosedXML.Excel;

public class ExcelStyleConfig
{
    public bool IsBold { get; set; }
    public bool IsItalic { get; set; }
    public bool IsUnderline { get; set; }

    public double FontSize { get; set; }
        = ExcelConstants.DefaultFontSize;

    public string FontName { get; set; }
        = "Calibri";

    public XLColor FontColor { get; set; }
        = XLColor.Black;

    public XLColor BackgroundColor { get; set; }
        = XLColor.NoColor;

    public XLAlignmentHorizontalValues? HorizontalAlignment { get; set; }

    public XLAlignmentVerticalValues? VerticalAlignment { get; set; }
        = XLAlignmentVerticalValues.Center;

    public bool WrapText { get; set; }
        = true;

    public XLBorderStyleValues BorderStyle { get; set; }
        = XLBorderStyleValues.None;
}