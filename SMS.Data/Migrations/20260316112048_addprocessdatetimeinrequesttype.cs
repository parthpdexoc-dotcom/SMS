using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addprocessdatetimeinrequesttype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ProcessDate",
                schema: "dbo",
                table: "RequestType",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ProcessTime",
                schema: "dbo",
                table: "RequestType",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessDate",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "ProcessTime",
                schema: "dbo",
                table: "RequestType");
        }
    }
}
