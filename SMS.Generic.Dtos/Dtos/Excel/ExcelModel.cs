using ClosedXML.Excel;

public class ExcelModel
{
    public string Header { get; set; } = string.Empty;
    public double Width { get; set; }
}
public class ExcelRow
{
    public List<ExcelCellModel> Cells { get; set; } = new();
}
public class ExcelCellModel
{
    public string? Text { get; set; }
    public ExcelStyleConfig Style { get; set; } = new();
    public XLAlignmentHorizontalValues? Alignment { get; set; }
}