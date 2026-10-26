using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeTypeIdAndRemoveRequiredRankPostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeTypeIDs",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "RequiredRank",
                schema: "dbo",
                table: "Post");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeTypeID",
                schema: "dbo",
                table: "Post",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_EmployeeTypeID",
                schema: "dbo",
                table: "Post",
                column: "EmployeeTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_EmployeeType_EmployeeTypeID",
                schema: "dbo",
                table: "Post",
                column: "EmployeeTypeID",
                principalSchema: "dbo",
                principalTable: "EmployeeType",
                principalColumn: "EmployeeTypeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_EmployeeType_EmployeeTypeID",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_EmployeeTypeID",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "EmployeeTypeID",
                schema: "dbo",
                table: "Post");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeTypeIDs",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredRank",
                schema: "dbo",
                table: "Post",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
