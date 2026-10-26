using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services;

/// <summary>
/// Generates Word documents from payroll data records
/// </summary>
public class PayrollWordDocumentGenerator : IPayrollWordDocumentGenerator
{
    private readonly ILogger<PayrollWordDocumentGenerator> _logger;

    public PayrollWordDocumentGenerator(ILogger<PayrollWordDocumentGenerator> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a Word document from payroll data records
    /// </summary>
    /// <param name="records">List of payroll data records</param>
    /// <param name="filePath">Output file path</param>
    /// <returns>Path to the generated document</returns>
    public async Task<string> CreateDocumentAsync(List<PayrollDataRecordDto> records, string filePath)
    {
        try
        {
            _logger.LogInformation("Creating Word document with {Count} payroll records", records.Count);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                // Add a main document part
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Add title
                AddTitle(body);
                
                // Add metadata
                AddMetadata(body, records.Count);
                
                // Add table with payroll data
                AddPayrollTable(body, records);
                
                // Add footer
                AddFooter(body);

                mainPart.Document.Save();
            }

            _logger.LogInformation("Successfully created Word document: {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Word document: {FilePath}", filePath);
            throw;
        }
    }

    /// <summary>
    /// Adds a title to the document
    /// </summary>
    private static void AddTitle(Body body)
    {
        var titlePara = body.AppendChild(new Paragraph());
        var titleRun = titlePara.AppendChild(new Run());
        titleRun.AppendChild(new Text("Payroll Report"));
        
        // Style the title
        titlePara.ParagraphProperties = new ParagraphProperties
        {
            Justification = new Justification { Val = JustificationValues.Center },
            ParagraphStyleId = new ParagraphStyleId { Val = "Title" }
        };
        
        titleRun.RunProperties = new RunProperties
        {
            Bold = new Bold(),
            FontSize = new FontSize { Val = "32" }
        };
    }

    /// <summary>
    /// Adds metadata section to the document
    /// </summary>
    private static void AddMetadata(Body body, int recordCount)
    {
        // Add date and record count
        var metaPara = body.AppendChild(new Paragraph());
        var metaRun = metaPara.AppendChild(new Run());
        metaRun.AppendChild(new Text($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Total Records: {recordCount}"));
        
        metaRun.RunProperties = new RunProperties
        {
            Italic = new Italic(),
            FontSize = new FontSize { Val = "20" }
        };
        
        // Add spacing
        body.AppendChild(new Paragraph());
    }

    /// <summary>
    /// Adds a table with payroll data
    /// </summary>
    private static void AddPayrollTable(Body body, List<PayrollDataRecordDto> records)
    {
        // Create table
        var table = body.AppendChild(new Table());
        
        // Define table properties
        table.AppendChild(new TableProperties(
            new TableWidth { Type = TableWidthUnitValues.Pct, Width = "100%" },
            new TableBorders(
                new TopBorder { Val = BorderValues.Nil },
                new LeftBorder { Val = BorderValues.Nil },
                new BottomBorder { Val = BorderValues.Nil },
                new RightBorder { Val = BorderValues.Nil },
                new InsideHorizontalBorder { Val = BorderValues.Nil },
                new InsideVerticalBorder { Val = BorderValues.Nil }
            )
        ));

        // Add table headers dynamically
        var headers = GetTableHeaders();
        AddTableRow(table, headers, isHeader: true);

        // Add data rows
        foreach (var record in records)
        {
            AddTableRow(table, new[]
            {
                record.GetExportEmployeeNumber(),
                record.StartDateTime.ToString("yyyyMMddHHmmss"),
                record.EndDateTime.ToString("yyyyMMddHHmmss"),
                record.Duration.ToString("000.00"),
                record.PayCode
            });
        }
    }

    /// <summary>
    /// Gets dynamic table headers for the payroll data
    /// </summary>
    /// <returns>Array of header names</returns>
    private static string[] GetTableHeaders()
    {
        return new[]
        {
            "Employee Number",
            "Start Date Time", 
            "End Date Time",
            "Duration",
            "Pay Code"
        };
    }

    /// <summary>
    /// Adds a table row with the specified cells
    /// </summary>
    private static void AddTableRow(Table table, string[] cellValues, bool isHeader = false)
    {
        var row = table.AppendChild(new TableRow());

        foreach (var cellValue in cellValues)
        {
            var cell = row.AppendChild(new TableCell());
            var paragraph = cell.AppendChild(new Paragraph());
            var run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(cellValue));

            // Style header cells
            if (isHeader)
            {
                run.RunProperties = new RunProperties
                {
                    Bold = new Bold()
                };
                
                paragraph.ParagraphProperties = new ParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Center }
                };
            }
            else
            {
                // Align data cells to match headers
                paragraph.ParagraphProperties = new ParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Center }
                };
            }

            // Add cell properties
            cell.AppendChild(new TableCellProperties(
                new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "16.67%" }
            ));
        }
    }

    /// <summary>
    /// Adds a footer to the document
    /// </summary>
    private static void AddFooter(Body body)
    {
        // Add spacing before footer
        body.AppendChild(new Paragraph());
        
        var footerPara = body.AppendChild(new Paragraph());
        var footerRun = footerPara.AppendChild(new Run());
        footerRun.AppendChild(new Text($"Report generated by SMS Payroll System - {DateTime.Now:yyyy}"));
        
        footerRun.RunProperties = new RunProperties
        {
            Italic = new Italic(),
            FontSize = new FontSize { Val = "18" }
        };
        
        footerPara.ParagraphProperties = new ParagraphProperties
        {
            Justification = new Justification { Val = JustificationValues.Center }
        };
    }
}
