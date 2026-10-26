using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestAndRequestRecurrence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "WeekDays",
               schema: "dbo",
               table: "Request");

            migrationBuilder.DropColumn(
               name: "CustomDates",
               schema: "dbo",
               table: "Request");

            migrationBuilder.DropColumn(
                name: "MonthDays",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
               name: "RequestType",
               schema: "dbo",
               table: "Request");

            migrationBuilder.AlterColumn<Guid>(
               name: "RequestRecurrenceID",
               schema: "dbo",
               table: "RequestRecurrence",
               type: "uniqueidentifier",
               nullable: false,
               defaultValueSql: "newid()",
               oldClrType: typeof(Guid),
               oldType: "uniqueidentifier");

            migrationBuilder.RenameColumn(
                name: "RequestRecurrenceID",
                schema: "dbo",
                table: "RequestRecurrence",
                newName: "RequestTypeID");


            migrationBuilder.RenameColumn(
               name: "RecurrenceUnit",
               schema: "dbo",
               table: "RequestRecurrence",
               newName: "RequestType");

            migrationBuilder.RenameColumn(
                name: "Interval",
                schema: "dbo",
                table: "RequestRecurrence",
                newName: "RepeatEvery");

            migrationBuilder.RenameColumn(
                name: "MonthDays",
                schema: "dbo",
                table: "RequestRecurrence",
                newName: "DaysOfMonth");
            migrationBuilder.RenameColumn(
               name: "WeekDays",
               schema: "dbo",
               table: "RequestRecurrence",
               newName: "DaysOfWeek");

            migrationBuilder.RenameTable(
               name: "RequestRecurrence",
               schema: "dbo",
               newName: "RequestType",
               newSchema: "dbo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "WeekDays",
                schema: "dbo",
                table: "Request",
                type: "varchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomDates",
                schema: "dbo",
                table: "Request",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MonthDays",
                schema: "dbo",
                table: "Request",
                type: "nvarchar(max)",
                nullable: true);
            migrationBuilder.AddColumn<int>(
               name: "RequestType",
               schema: "dbo",
               table: "Request",
               type: "int",
               nullable: false,
               defaultValue: 1); // OneTime

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestRecurrenceID",
                schema: "dbo",
                table: "RequestRecurrence",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newid()");

            migrationBuilder.RenameColumn(
                name: "RequestRecurrenceID",
                schema: "dbo",
                table: "RequestRecurrence",
                newName: "RequestTypeID");

            migrationBuilder.RenameColumn(
               name: "RequestType",
               schema: "dbo",
               table: "RequestRecurrence",
               newName: "RecurrenceUnit");

            migrationBuilder.RenameColumn(
               name: "RecurrenceUnit",
               schema: "dbo",
               table: "RequestRecurrence",
               newName: "RequestType");

            migrationBuilder.RenameColumn(
                name: "Interval",
                schema: "dbo",
                table: "RequestRecurrence",
                newName: "RepeatEvery");

            migrationBuilder.RenameColumn(
                name: "MonthDays",
                schema: "dbo",
                table: "RequestRecurrence",
                newName: "DaysOfMonth");
            migrationBuilder.RenameColumn(
               name: "WeekDays",
               schema: "dbo",
               table: "RequestRecurrence",
               newName: "DaysOfWeek");

            migrationBuilder.RenameTable(
               name: "RequestType",
               schema: "dbo",
               newName: "RequestRecurrence",
               newSchema: "dbo");
        }
    }
}
