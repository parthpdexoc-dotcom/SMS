using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStartDateEndDateForRequestAndPostEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                schema: "dbo",
                table: "RequestEquipment",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                schema: "dbo",
                table: "RequestEquipment",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                schema: "dbo",
                table: "PostEquipment",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                schema: "dbo",
                table: "PostEquipment",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "dbo",
                table: "RequestEquipment");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "dbo",
                table: "RequestEquipment");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "dbo",
                table: "PostEquipment");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "dbo",
                table: "PostEquipment");
        }
    }
}
