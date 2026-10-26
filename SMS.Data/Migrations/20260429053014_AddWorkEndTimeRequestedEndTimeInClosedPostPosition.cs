using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkEndTimeRequestedEndTimeInClosedPostPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "RequestedEndTime",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "WorkEndTime",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestedEndTime",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "WorkEndTime",
                schema: "dbo",
                table: "ClosedPostPosition");
        }
    }
}
