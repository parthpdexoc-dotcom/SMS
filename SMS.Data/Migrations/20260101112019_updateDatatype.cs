using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateDatatype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                schema: "dbo",
                table: "Request",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MonthDays",
                schema: "dbo",
                table: "Request",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                schema: "dbo",
                table: "Request",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PostID1",
                schema: "dbo",
                table: "PostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_PostID1",
                schema: "dbo",
                table: "PostPosition",
                column: "PostID1");

            migrationBuilder.AddForeignKey(
                name: "FK_PostPosition_Post_PostID1",
                schema: "dbo",
                table: "PostPosition",
                column: "PostID1",
                principalSchema: "dbo",
                principalTable: "Post",
                principalColumn: "PostID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostPosition_Post_PostID1",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropIndex(
                name: "IX_PostPosition_PostID1",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "PostID1",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                schema: "dbo",
                table: "Request",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "MonthDays",
                schema: "dbo",
                table: "Request",
                type: "varchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                schema: "dbo",
                table: "Request",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
