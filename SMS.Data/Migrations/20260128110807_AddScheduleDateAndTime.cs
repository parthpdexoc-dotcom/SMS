using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleDateAndTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseTime",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                schema: "dbo",
                table: "Request");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleDateAndTime",
                schema: "dbo",
                table: "Request",
                type: "datetime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduleDateAndTime",
                schema: "dbo",
                table: "Request");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ReleaseTime",
                schema: "dbo",
                table: "Request",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ScheduleDate",
                schema: "dbo",
                table: "Request",
                type: "date",
                nullable: true);
        }
    }
}
