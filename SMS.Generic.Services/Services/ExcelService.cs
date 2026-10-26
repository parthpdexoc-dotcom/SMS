using System.Globalization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Interfaces;
using static SMS.Generic.Dtos.Dtos.PostDto;
using static SMS.Generic.Dtos.Dtos.RosterDto;

namespace SMS.Generic.Services.Services;

/// <summary>
/// Service that reads uploaded Excel files and returns DTOs, and creates simple single-sheet .xlsx workbooks.
/// </summary>
public class ExcelService : IExcelService
{
    private const uint TwoDecimalNumberStyleIndex = 2U;
    private const uint PaddedCodeNumberStyleIndex = 3U;
    private const uint IntegerNumberStyleIndex = 4U;
    private const uint DateMmDdYyyyStyleIndex = 5U;
    private const uint PaddedCodeNumberFormatId = 164U;
    private const uint DateMmDdYyyyFormatId = 165U;

    public ExcelService()
    {
    }

    /// <inheritdoc />
    public ExcelFileDto Read(Stream stream, string? fileName = null)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        using var document = SpreadsheetDocument.Open(stream, false);
        var workbookPart = document.WorkbookPart;
        if (workbookPart == null)
            return new ExcelFileDto { FileName = fileName ?? string.Empty };

        var sharedStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
        var sheets = workbookPart.Workbook?.Descendants<Sheet>().ToList() ?? new List<Sheet>();
        var sheetList = new List<ExcelSheetDto>();

        foreach (var sheet in sheets)
        {
            var sheetPart = GetWorksheetPart(workbookPart, sheet);
            var sheetData = sheetPart?.Worksheet?.GetFirstChild<SheetData>();
            if (sheetData == null)
                continue;

            var sheetName = sheet.Name?.Value ?? $"Sheet{sheetList.Count + 1}";
            var rows = EnumerateRows(sheetData, sharedStringPart);
            sheetList.Add(ExcelSheetDto.FromRows(sheetName, rows));
        }

        return new ExcelFileDto
        {
            FileName = fileName ?? string.Empty,
            Sheets = sheetList
        };
    }

    /// <inheritdoc />
    public ExcelFolderReadResult ReadFromFolder(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            return new ExcelFolderReadResult { ErrorMessage = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EXCEL_FOLDER_PATH_REQUIRED) };

        if (!Directory.Exists(folderPath))
            return new ExcelFolderReadResult { ErrorMessage = $"Excel folder not found: {folderPath}. Create the folder and add .xlsx files." };

        var files = Directory.GetFiles(folderPath, "*.xlsx", SearchOption.TopDirectoryOnly);
        var allRows = new List<Dictionary<string, string>>();

        foreach (var filePath in files.OrderBy(f => f))
        {
            using var stream = File.OpenRead(filePath);
            var excelFile = Read(stream, Path.GetFileName(filePath));
            allRows.AddRange(ToFlatRows(excelFile));
        }

        return new ExcelFolderReadResult { Rows = allRows };
    }

    /// <inheritdoc />
    public MemoryStream CreateWorkbookSingleSheet(
        string sheetName,
        IReadOnlyList<string> headers,
        IEnumerable<IReadOnlyList<object?>> dataRows)
    {
        ArgumentNullException.ThrowIfNull(headers);

        var rowsList = new List<IReadOnlyList<object?>>();
        foreach (var dataRow in dataRows)
            rowsList.Add(PadRow(dataRow.ToList(), headers.Count));

        var stream = new MemoryStream();
        using (var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
        {
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = CreateSingleSheetTableStylesheet();
            stylesPart.Stylesheet.Save();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            var columnWidths = ComputeColumnWidthsFromContent(headers, rowsList);
            worksheetPart.Worksheet = new Worksheet(
                new SheetFormatProperties { DefaultRowHeight = 15D },
                BuildColumnsFromWidths(columnWidths),
                sheetData);

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            sheets.Append(new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1U,
                Name = SanitizeSheetName(sheetName),
            });

            // Style 0 = body Calibri 11; style 1 = column headers Calibri 11 bold.
            uint rowIndex = 1U;
            sheetData.AppendChild(BuildRow(rowIndex++, PadRow(headers.Select(h => (object?)h).ToList(), headers.Count), 1U));

            foreach (var list in rowsList)
                sheetData.AppendChild(BuildRow(rowIndex++, list, 0U));

            if (rowIndex > 2U)
            {
                worksheetPart.Worksheet.AppendChild(new IgnoredErrors(
                    new IgnoredError
                    {
                        SequenceOfReferences = new ListValue<StringValue> { InnerText = $"D2:D{rowIndex - 1}" },
                        NumberStoredAsText = true,
                    }));
            }

            workbookPart.Workbook.Save();
        }

        stream.Position = 0;
        return stream;
    }

    /// <summary>Body + header row fonts for <see cref="CreateWorkbookSingleSheet"/> (Calibri 11; headers bold).</summary>
    private static Stylesheet CreateSingleSheetTableStylesheet()
    {
        var fonts = new Fonts(
            new Font(new FontSize { Val = 11D }, new Color { Theme = 1U }, new FontName { Val = "Calibri" }),
            new Font(
                new Bold { Val = true },
                new FontSize { Val = 11D },
                new Color { Theme = 1U },
                new FontName { Val = "Calibri" },
                new FontFamilyNumbering { Val = 2 }));

        var fills = new Fills(
            new Fill(new PatternFill { PatternType = PatternValues.None }),
            new Fill(new PatternFill { PatternType = PatternValues.Gray125 }));

        var borders = new Borders(new Border(
            new LeftBorder(),
            new RightBorder(),
            new TopBorder(),
            new BottomBorder(),
            new DiagonalBorder()));

        var cellStyleFormats = new CellStyleFormats(new CellFormat
        {
            NumberFormatId = 0U,
            FontId = 0U,
            FillId = 0U,
            BorderId = 0U,
        });

        var numberingFormats = new NumberingFormats(
            new NumberingFormat
            {
                NumberFormatId = PaddedCodeNumberFormatId,
                FormatCode = "000000",
            },
            new NumberingFormat
            {
                NumberFormatId = DateMmDdYyyyFormatId,
                FormatCode = "mm/dd/yyyy",
            });

        var cellFormats = new CellFormats(
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 0U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
            },
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 1U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyFont = true,
            },
            new CellFormat
            {
                NumberFormatId = 2U,
                FontId = 0U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyNumberFormat = true,
            },
            new CellFormat
            {
                NumberFormatId = PaddedCodeNumberFormatId,
                FontId = 0U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyNumberFormat = true,
            },
            new CellFormat
            {
                NumberFormatId = 1U,
                FontId = 0U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyNumberFormat = true,
            },
            new CellFormat
            {
                NumberFormatId = DateMmDdYyyyFormatId,
                FontId = 0U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyNumberFormat = true,
            });

        var cellStyles = new CellStyles(new CellStyle
        {
            Name = "Normal",
            FormatId = 0U,
            BuiltinId = 0U,
        })
        {
            Count = 1U,
        };

        fonts.Count = 2U;
        fills.Count = 2U;
        borders.Count = 1U;
        numberingFormats.Count = 2U;
        cellStyleFormats.Count = 1U;
        cellFormats.Count = 6U;

        return new Stylesheet(numberingFormats, fonts, fills, borders, cellStyleFormats, cellFormats, cellStyles);
    }

    /// <inheritdoc />
    public MemoryStream CreateWorkbookSingleSheetWithTitleAndStyledHeader(
        string sheetName,
        string reportTitle,
        IReadOnlyList<string> headers,
        IEnumerable<IReadOnlyList<object?>> dataRows)
    {
        ArgumentNullException.ThrowIfNull(headers);
        ArgumentNullException.ThrowIfNull(reportTitle);

        var stream = new MemoryStream();
        using (var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
        {
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = CreateWatchCommanderReportStylesheet();
            stylesPart.Stylesheet.Save();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            sheets.Append(new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1U,
                Name = SanitizeSheetName(sheetName),
            });

            var columnCount = headers.Count;
            var lastColLetter = ExcelCellDto.GetColumnLetter(columnCount - 1);
            var mergeReference = $"A1:{lastColLetter}1";

            uint rowIndex = 1U;
            sheetData.AppendChild(BuildRow(rowIndex++, PadRow([(object?)reportTitle], columnCount), 2U));
            sheetData.AppendChild(BuildRow(rowIndex++, PadRow(headers.Select(h => (object?)h).ToList(), columnCount), 1U));

            foreach (var dataRow in dataRows)
            {
                var list = dataRow.ToList();
                // Style 0 = body: Calibri 11 from stylesheet (unstyled cells may not pick workbook font in all viewers).
                sheetData.AppendChild(BuildRow(rowIndex++, PadRow(list, columnCount), 0U));
            }

            worksheetPart.Worksheet = new Worksheet(
                new SheetFormatProperties { DefaultRowHeight = 15D },
                BuildWatchCommanderColumnDefinitions(columnCount),
                sheetData,
                new MergeCells(new MergeCell { Reference = mergeReference }) { Count = 1U });

            workbookPart.Workbook.Save();
        }

        stream.Position = 0;
        return stream;
    }

    /// <inheritdoc />
    public MemoryStream CreateRosterInformationWorkbook(DateOnly dateOfWork, RosterResponseDto roster)
    {
        ArgumentNullException.ThrowIfNull(roster);

        const int columnCount = 5;
        var titleLine = RosterInformationExcelBuilder.BuildTitleLine(roster.RequesterName);
        var excelDate = dateOfWork.ToDateTime(TimeOnly.MinValue).ToOADate();

        var stream = new MemoryStream();
        using (var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
        {
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = CreateRosterInformationStylesheet();
            stylesPart.Stylesheet.Save();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            var mergeCells = new List<MergeCell>();

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            sheets.Append(new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1U,
                Name = RosterInformationExcelBuilder.SheetName,
            });

            uint rowIndex = 1U;

            // Row 1: department + request code (A:C) and work date (E).
            sheetData.AppendChild(BuildRosterRow(
                rowIndex,
                PadRow([(object?)titleLine, null, null, null, excelDate], columnCount),
                [1U, 1U, 1U, null, 2U],
                useNumberForLastDate: true));
            mergeCells.Add(new MergeCell { Reference = "A1:C1" });
            rowIndex++;

            // Row 2: spacer.
            sheetData.AppendChild(BuildRosterRow(rowIndex, PadRow([null, null, null, null, null], columnCount), null));
            mergeCells.Add(new MergeCell { Reference = $"A{rowIndex}:E{rowIndex}" });
            rowIndex++;

            foreach (var table in roster.Tables)
            {
                if (table.Rows.Count == 0)
                    continue;

                var postTitle = string.IsNullOrWhiteSpace(table.PostTitle) ? "-" : table.PostTitle.Trim();
                var sectionMeta = table.PostDescription;

                sheetData.AppendChild(BuildRosterRow(
                    rowIndex,
                    PadRow([(object?)postTitle, null, null, null, null], columnCount),
                    Enumerable.Repeat<uint?>(3U, columnCount).ToList()));
                mergeCells.Add(new MergeCell { Reference = $"A{rowIndex}:E{rowIndex}" });
                rowIndex++;

                var payCodeDisplay = string.IsNullOrWhiteSpace(table.PayCodeValue)
                    ? null
                    : table.PayCodeValue.Trim();
                sheetData.AppendChild(BuildRosterRow(
                    rowIndex,
                    PadRow([(object?)sectionMeta, null, null, null, (object?)payCodeDisplay], columnCount),
                    [4U, 4U, 4U, 4U, RosterPayCodeCellStyleIndex]));
                mergeCells.Add(new MergeCell { Reference = $"A{rowIndex}:D{rowIndex}" });
                rowIndex++;

                sheetData.AppendChild(BuildRosterRow(
                    rowIndex,
                    PadRow(RosterInformationExcelBuilder.ColumnHeaders.Cast<object?>().ToList(), columnCount),
                    Enumerable.Repeat<uint?>(5U, columnCount).ToList()));
                rowIndex++;

                for (var rowIdx = 0; rowIdx < table.Rows.Count; rowIdx++)
                {
                    var rosterRow = table.Rows[rowIdx];
                    var dataStyleIndex = rowIdx == 0 && rosterRow.IsSupervisorPost
                        ? RosterSupervisorDataCellStyleIndex
                        : RosterDataCellStyleIndex;
                    sheetData.AppendChild(BuildRosterRow(
                        rowIndex,
                        PadRow(RosterInformationExcelBuilder.ToDataRow(rosterRow).ToList(), columnCount),
                        Enumerable.Repeat<uint?>(dataStyleIndex, columnCount).ToList()));
                    rowIndex++;
                }

                sheetData.AppendChild(BuildRosterRow(rowIndex, PadRow([null, null, null, null, null], columnCount), null));
                mergeCells.Add(new MergeCell { Reference = $"A{rowIndex}:E{rowIndex}" });
                rowIndex++;
            }

            worksheetPart.Worksheet = new Worksheet(
                new SheetFormatProperties { DefaultRowHeight = 15D },
                BuildRosterColumnDefinitions(),
                sheetData,
                new MergeCells(mergeCells) { Count = (uint)mergeCells.Count });

            workbookPart.Workbook.Save();
        }

        stream.Position = 0;
        return stream;
    }

    private static Row BuildRosterRow(
        uint rowIndex,
        IReadOnlyList<object?> cells,
        IReadOnlyList<uint?>? styleIndices,
        bool useNumberForLastDate = false)
    {
        var row = new Row { RowIndex = rowIndex };
        for (var i = 0; i < cells.Count; i++)
        {
            var cellRef = ExcelCellDto.GetColumnLetter(i) + rowIndex;
            var styleIndex = styleIndices != null && i < styleIndices.Count ? styleIndices[i] : null;
            var value = cells[i];

            if (useNumberForLastDate && i == cells.Count - 1 && value is double dateSerial)
            {
                row.AppendChild(CreateNumberCell(cellRef, dateSerial, styleIndex));
                continue;
            }

            row.AppendChild(CreateInlineStringCell(cellRef, value, styleIndex));
        }

        return row;
    }

    private static Cell CreateNumberCell(string cellReference, double value, uint? styleIndex = null)
    {
        var cell = new Cell
        {
            CellReference = cellReference,
            DataType = CellValues.Number,
            CellValue = new CellValue(value.ToString(CultureInfo.InvariantCulture)),
        };
        if (styleIndex.HasValue)
            cell.StyleIndex = styleIndex.Value;
        return cell;
    }

    private static Columns BuildRosterColumnDefinitions()
    {
        double[] columnWidths = [7, 21, 11, 11, 47];
        var columns = new Columns();
        for (var i = 0; i < columnWidths.Length; i++)
        {
            var col = (uint)(i + 1);
            columns.Append(new Column
            {
                Min = col,
                Max = col,
                Width = columnWidths[i],
                CustomWidth = true,
            });
        }

        return columns;
    }

    private const uint RosterDataCellStyleIndex = 6U;
    private const uint RosterSupervisorDataCellStyleIndex = 7U;
    private const uint RosterPayCodeCellStyleIndex = 8U;

    private static Stylesheet CreateRosterInformationStylesheet()
    {
        const string fontFamily = "Calibri";
        const double defaultFontSize = 11D;
        const double titleFontSize = 10D;
        const double postTitleFontSize = 12D;
        const double locationFontSize = 9D;
        const double headerFontSize = 11D;

        const string headerFillColor = "FFF3F3F3";
        const string headerBorderColor = "FFCFCFCF";
        const string dataBorderColor = "FFD0D0D0";
        const string borderBottomColor = "FF999999";

        const string dateNumberFormat = "dddd, mmmm dd, yyyy";
        const uint dateNumberFormatId = 164U;

        var fonts = new Fonts(
            new Font(new FontSize { Val = defaultFontSize }, new Color { Theme = 1U }, new FontName { Val = fontFamily }),
            new Font(new Bold(), new FontSize { Val = titleFontSize }, new FontName { Val = fontFamily }),
            new Font(
                new Bold(),
                new Underline(),
                new FontSize { Val = postTitleFontSize },
                new FontName { Val = fontFamily }),
            new Font(new Bold(), new FontSize { Val = locationFontSize }, new FontName { Val = fontFamily }),
            new Font(new Bold(), new FontSize { Val = headerFontSize }, new FontName { Val = fontFamily }),
            new Font(new Bold(), new FontSize { Val = defaultFontSize }, new FontName { Val = fontFamily }));

        var fills = new Fills(
            new Fill(new PatternFill { PatternType = PatternValues.None }),
            new Fill(new PatternFill { PatternType = PatternValues.Gray125 }),
            new Fill(new PatternFill(
                new ForegroundColor { Rgb = headerFillColor },
                new BackgroundColor { Rgb = headerFillColor })
            {
                PatternType = PatternValues.Solid,
            }));

        var borders = new Borders(
            new Border(
                new LeftBorder(),
                new RightBorder(),
                new TopBorder(),
                new BottomBorder(),
                new DiagonalBorder()),
            new Border(
                new LeftBorder { Style = BorderStyleValues.Thin, Color = new Color { Rgb = headerBorderColor } },
                new RightBorder { Style = BorderStyleValues.Thin, Color = new Color { Rgb = headerBorderColor } },
                new TopBorder { Style = BorderStyleValues.Thin, Color = new Color { Rgb = headerBorderColor } },
                new BottomBorder { Style = BorderStyleValues.Medium, Color = new Color { Rgb = borderBottomColor } },
                new DiagonalBorder()),
            new Border(
                new LeftBorder { Style = BorderStyleValues.Thin, Color = new Color { Rgb = dataBorderColor } },
                new RightBorder { Style = BorderStyleValues.Thin, Color = new Color { Rgb = dataBorderColor } },
                new TopBorder { Style = BorderStyleValues.Thin, Color = new Color { Rgb = dataBorderColor } },
                new BottomBorder { Style = BorderStyleValues.Medium, Color = new Color { Rgb = borderBottomColor } },
                new DiagonalBorder()));

        var numberingFormats = new NumberingFormats(
            new NumberingFormat
            {
                NumberFormatId = dateNumberFormatId,
                FormatCode = dateNumberFormat,
            });

        var cellStyleFormats = new CellStyleFormats(new CellFormat
        {
            NumberFormatId = 0U,
            FontId = 0U,
            FillId = 0U,
            BorderId = 0U,
        });

        var cellFormats = new CellFormats(
            new CellFormat { NumberFormatId = 0U, FontId = 0U, FillId = 0U, BorderId = 0U, FormatId = 0U },
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 1U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyFont = true,
                ApplyAlignment = true,
                Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center },
            },
            new CellFormat
            {
                NumberFormatId = dateNumberFormatId,
                FontId = 1U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyNumberFormat = true,
                ApplyFont = true,
                ApplyAlignment = true,
                Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Right, Vertical = VerticalAlignmentValues.Center },
            },
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 2U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyFont = true,
                ApplyAlignment = true,
                Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center },
            },
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 3U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyFont = true,
                ApplyAlignment = true,
                Alignment = new Alignment
                {
                    Horizontal = HorizontalAlignmentValues.Left,
                    Vertical = VerticalAlignmentValues.Center,
                    WrapText = true,
                },
            },
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 4U,
                FillId = 2U,
                BorderId = 1U,
                FormatId = 0U,
                ApplyFont = true,
                ApplyFill = true,
                ApplyBorder = true,
                ApplyAlignment = true,
                Alignment = new Alignment
                {
                    Horizontal = HorizontalAlignmentValues.Center,
                    Vertical = VerticalAlignmentValues.Center,
                    WrapText = true,
                },
            },
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 0U,
                FillId = 0U,
                BorderId = 2U,
                FormatId = 0U,
                ApplyBorder = true,
                ApplyAlignment = true,
                Alignment = new Alignment
                {
                    Horizontal = HorizontalAlignmentValues.Center,
                    Vertical = VerticalAlignmentValues.Center,
                    WrapText = true,
                },
            },
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 5U,
                FillId = 0U,
                BorderId = 2U,
                FormatId = 0U,
                ApplyFont = true,
                ApplyBorder = true,
                ApplyAlignment = true,
                Alignment = new Alignment
                {
                    Horizontal = HorizontalAlignmentValues.Center,
                    Vertical = VerticalAlignmentValues.Center,
                    WrapText = true,
                },
            },
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 3U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyFont = true,
                ApplyAlignment = true,
                Alignment = new Alignment
                {
                    Horizontal = HorizontalAlignmentValues.Right,
                    Vertical = VerticalAlignmentValues.Center,
                    WrapText = true,
                },
            });

        fonts.Count = 6U;
        fills.Count = 3U;
        borders.Count = 3U;
        numberingFormats.Count = 1U;
        cellStyleFormats.Count = 1U;
        cellFormats.Count = 9U;

        return new Stylesheet(numberingFormats, fonts, fills, borders, cellStyleFormats, cellFormats);
    }

    private static Stylesheet CreateWatchCommanderReportStylesheet()
    {
        // Excel expects fill[0]=none and fill[1]=Gray125; custom fills start at index 2 (header blue = fill 2).
        // Font 1 = column headers: Calibri 11 pt bold, white (explicit Bold Val for Excel compatibility).
        var fonts = new Fonts(
            new Font(new FontSize { Val = 11D }, new Color { Theme = 1U }, new FontName { Val = "Calibri" }),
            new Font(
                new FontSize { Val = 11D },
                new Color { Rgb = "FFFFFFFF" },
                new FontName { Val = "Calibri" },
                new FontFamilyNumbering { Val = 2 },
                new Bold { Val = true }),
            new Font(new Bold(), new FontSize { Val = 16D }, new Color { Theme = 1U }, new FontName { Val = "Calibri" }));

        var fills = new Fills(
            new Fill(new PatternFill { PatternType = PatternValues.None }),
            new Fill(new PatternFill { PatternType = PatternValues.Gray125 }),
            new Fill(new PatternFill(
                new ForegroundColor { Rgb = "FF0070C0" },
                new BackgroundColor { Rgb = "FF0070C0" })
            {
                PatternType = PatternValues.Solid,
            }));

        var borders = new Borders(new Border(
            new LeftBorder(),
            new RightBorder(),
            new TopBorder(),
            new BottomBorder(),
            new DiagonalBorder()));

        var cellStyleFormats = new CellStyleFormats(new CellFormat
        {
            NumberFormatId = 0U,
            FontId = 0U,
            FillId = 0U,
            BorderId = 0U,
        });

        var cellFormats = new CellFormats(
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 0U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
            },
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 1U,
                FillId = 2U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyFont = true,
                ApplyFill = true,
                ApplyAlignment = true,
                Alignment = new Alignment
                {
                    Horizontal = HorizontalAlignmentValues.Center,
                    Vertical = VerticalAlignmentValues.Center,
                },
            },
            new CellFormat
            {
                NumberFormatId = 0U,
                FontId = 2U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyFont = true,
                ApplyAlignment = true,
                Alignment = new Alignment
                {
                    Horizontal = HorizontalAlignmentValues.Center,
                    Vertical = VerticalAlignmentValues.Center,
                },
            });

        var cellStyles = new CellStyles(new CellStyle
        {
            Name = "Normal",
            FormatId = 0U,
            BuiltinId = 0U,
        })
        {
            Count = 1U,
        };

        fonts.Count = 3U;
        fills.Count = 3U;
        borders.Count = 1U;
        cellStyleFormats.Count = 1U;
        cellFormats.Count = 3U;

        return new Stylesheet(fonts, fills, borders, cellStyleFormats, cellFormats, cellStyles);
    }

    /// <summary>
    /// Width per column (Excel character units) from the longest rendered cell text in the header and data rows.
    /// </summary>
    private static IReadOnlyList<double> ComputeColumnWidthsFromContent(
        IReadOnlyList<string> headers,
        IReadOnlyList<IReadOnlyList<object?>> dataRows)
    {
        var n = headers.Count;
        var maxLens = new int[n];
        for (var j = 0; j < n; j++)
            maxLens[j] = headers[j]?.Length ?? 0;

        foreach (var row in dataRows)
        {
            var count = Math.Min(row.Count, n);
            for (var j = 0; j < count; j++)
            {
                var text = FormatCellValue(row[j]);
                if (text.Length > maxLens[j])
                    maxLens[j] = text.Length;
            }
        }

        var widths = new double[n];
        for (var j = 0; j < n; j++)
            widths[j] = EstimateExcelColumnWidth(maxLens[j]);
        return widths;
    }

    /// <summary>Maps max visible character count to Excel <c>Column.Width</c> (heuristic; true AutoFit is client-only).</summary>
    private static double EstimateExcelColumnWidth(int maxCharCount)
    {
        if (maxCharCount <= 0)
            return 10D;
        // Slightly wider than raw char count so typical Calibri 11 content is not clipped.
        var w = maxCharCount * 1.12d + 3.5d;
        return Math.Clamp(w, 9D, 85D);
    }

    private static Columns BuildColumnsFromWidths(IReadOnlyList<double> widths)
    {
        var columns = new Columns();
        for (var i = 0; i < widths.Count; i++)
        {
            var col = (uint)(i + 1);
            columns.Append(new Column
            {
                Min = col,
                Max = col,
                Width = widths[i],
                CustomWidth = true,
            });
        }

        return columns;
    }

    /// <summary>Approximate column widths (Excel character units) so labels and data are not clipped as heavily.</summary>
    private static Columns BuildWatchCommanderColumnDefinitions(int columnCount)
    {
        // Order: Date, Officer Name, Payroll, Call Sign, Officer Type, Start Time, Hours, End Time, Client, Location, Vehicle.
        ReadOnlySpan<double> defaultWidths = stackalloc double[] { 12, 26, 10, 12, 18, 12, 10, 10, 24, 38, 10 };
        var columns = new Columns();
        for (var i = 0; i < columnCount; i++)
        {
            var w = i < defaultWidths.Length ? defaultWidths[i] : 14D;
            var col = (uint)(i + 1);
            columns.Append(new Column
            {
                Min = col,
                Max = col,
                Width = w,
                CustomWidth = true,
            });
        }

        return columns;
    }

    private static List<object?> PadRow(IReadOnlyList<object?> cells, int columnCount)
    {
        var padded = new List<object?>(columnCount);
        for (var i = 0; i < columnCount; i++)
        {
            padded.Add(i < cells.Count ? cells[i] : null);
        }

        return padded;
    }

    private static string SanitizeSheetName(string sheetName)
    {
        var s = string.IsNullOrWhiteSpace(sheetName) ? "Sheet1" : sheetName.Trim();
        foreach (var c in new[] { '\\', '/', '*', '[', ']', ':', '?' })
        {
            s = s.Replace(c, '_');
        }

        return s.Length <= 31 ? s : s[..31];
    }

    private static Row BuildRow(uint rowIndex, IReadOnlyList<object?> cells, uint? textStyleIndex = null)
    {
        var row = new Row { RowIndex = rowIndex };
        for (var i = 0; i < cells.Count; i++)
        {
            var cellRef = ExcelCellDto.GetColumnLetter(i) + rowIndex;
            row.AppendChild(CreateCell(cellRef, cells[i], textStyleIndex));
        }

        return row;
    }

    private static Cell CreateCell(string cellReference, object? value, uint? textStyleIndex = null)
    {
        return value switch
        {
            ExcelPaddedCodeValue c => CreateNumberCell(cellReference, c.Value, PaddedCodeNumberStyleIndex),
            ExcelIntegerValue i => CreateNumberCell(cellReference, i.Value, IntegerNumberStyleIndex),
            DateOnly date => CreateNumberCell(cellReference, date.ToDateTime(TimeOnly.MinValue).ToOADate(), DateMmDdYyyyStyleIndex),
            decimal m => CreateNumberCell(cellReference, (double)m, TwoDecimalNumberStyleIndex),
            double d => CreateNumberCell(cellReference, d, TwoDecimalNumberStyleIndex),
            float f => CreateNumberCell(cellReference, f, TwoDecimalNumberStyleIndex),
            int i => CreateNumberCell(cellReference, i, TwoDecimalNumberStyleIndex),
            long l => CreateNumberCell(cellReference, l, TwoDecimalNumberStyleIndex),
            _ => CreateInlineStringCell(cellReference, value, textStyleIndex),
        };
    }

    private static Cell CreateInlineStringCell(string cellReference, object? value, uint? styleIndex = null)
    {
        var text = FormatCellValue(value);
        var cell = new Cell(new InlineString(new Text(text)))
        {
            DataType = CellValues.InlineString,
            CellReference = cellReference,
        };
        if (styleIndex.HasValue)
            cell.StyleIndex = styleIndex.Value;
        return cell;
    }

    private static string FormatCellValue(object? value)
    {
        return value switch
        {
            null => string.Empty,
            ExcelPaddedCodeValue c => c.Value.ToString("000000", CultureInfo.InvariantCulture),
            ExcelIntegerValue i => i.Value.ToString(CultureInfo.InvariantCulture),
            DateOnly d => d.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
            TimeOnly t => t.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            bool b => (b ? "Yes" : "No"),
            Guid g => g.ToString("D", CultureInfo.InvariantCulture),
            decimal m => m.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),
            float f => f.ToString(CultureInfo.InvariantCulture),
            int i => i.ToString(CultureInfo.InvariantCulture),
            long l => l.ToString(CultureInfo.InvariantCulture),
            uint ui => ui.ToString(CultureInfo.InvariantCulture),
            ulong ul => ul.ToString(CultureInfo.InvariantCulture),
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty,
        };
    }

    private static List<Dictionary<string, string>> ToFlatRows(ExcelFileDto excelFile)
    {
        var firstSheet = excelFile.Sheets?.FirstOrDefault();
        if (firstSheet?.Rows == null)
            return new List<Dictionary<string, string>>();

        return firstSheet.Rows
            .Select(row => row.Cells.ToDictionary(
                kv => kv.Key,
                kv => kv.Value?.ToString() ?? string.Empty,
                StringComparer.OrdinalIgnoreCase))
            .ToList();
    }

    private static WorksheetPart? GetWorksheetPart(WorkbookPart workbookPart, Sheet sheet)
    {
        if (sheet.Id?.Value == null)
            return null;
        var part = workbookPart.GetPartById(sheet.Id.Value);
        return part as WorksheetPart;
    }

    private static IEnumerable<IReadOnlyList<object?>> EnumerateRows(SheetData sheetData, SharedStringTablePart? sharedStringPart)
    {
        foreach (var row in sheetData.Elements<Row>())
        {
            var cells = GetOrderedCellValues(row, sharedStringPart);
            yield return cells;
        }
    }

    private static IReadOnlyList<object?> GetOrderedCellValues(Row row, SharedStringTablePart? sharedStringPart)
    {
        var cellsWithIndex = row.Elements<Cell>()
            .Select(c => (Index: ExcelCellDto.GetColumnIndex(c.CellReference?.Value), Cell: c))
            .Where(x => x.Index >= 0)
            .OrderBy(x => x.Index)
            .ToList();

        if (cellsWithIndex.Count == 0)
            return Array.Empty<object?>();

        var maxCol = cellsWithIndex.Max(x => x.Index);
        var result = new object?[maxCol + 1];
        foreach (var (index, cell) in cellsWithIndex)
            result[index] = ExcelCellDto.GetValue(cell, sharedStringPart);
        return result;
    }
}
