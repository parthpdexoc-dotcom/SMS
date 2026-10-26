using System;
using ClosedXML.Excel;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using static SMS.Generic.Dtos.Dtos.RosterDto;

public class RosterTable : BaseExcelGenerator
{
    private readonly RosterTableDto _table;

    public RosterTable(RosterTableDto table)
    {
        _table = table;
    }
    public override int Export(IXLWorksheet worksheet, int currentRow)
    {
        CreateColumnRow(worksheet,currentRow,GetColumns());
        ApplyBottomBorder(worksheet, currentRow, 1, 5);
        currentRow++;
        var columnWidths = GetColumns().Select(x => x.Width).ToList();
        for (int index = 0; index < _table.Rows.Count; index++)
        {
            var row = _table.Rows[index];
            bool isSupervisorFirstRow = index == 0 &&row.IsSupervisorPost;
            CreateTextRow(worksheet,currentRow,CreateRow(row),isSupervisorFirstRow? (int)ExcelStyleEnum.BorderBoldText: (int)ExcelStyleEnum.BorderNormalText, columnWidths);

            if (isSupervisorFirstRow)
                ApplyBottomBorder(worksheet,currentRow,1,5);

            currentRow++;
        }
        ApplyMediumOuterBorder(worksheet,currentRow -_table.Rows.Count - 1,currentRow - 1,1,5);
        return  AddBlankLine(worksheet, currentRow);
    }
    private List<ExcelModel> GetColumns()
    {
        return
        [
            new(){Header = "Unit",Width = 15},
            new(){Header = "Officer",Width = 30},
            new(){Header = "Payroll",Width = 15},
            new(){Header = "Vehicle",Width = 15},
            new(){Header = "Assignment",Width = 40}
        ];
    }
    private ExcelRow CreateRow(RosterInformationDto row)
    {
        return new ExcelRow
        {
            Cells = 
            [
                new(){Text = row.CallUnit,Alignment =XLAlignmentHorizontalValues.Center},
                new(){Text = row.EmployeeName},
                new(){Text = row.EmployeeNumber,Alignment =XLAlignmentHorizontalValues.Center},
                new(){Text = row.VehicleName,Alignment =XLAlignmentHorizontalValues.Center},
                new(){Text = row.PostLocation}
            ]
        };
    }
}