using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentIdInPtaeoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                schema: "dbo",
                table: "PtaeoCode",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("992fab13-e61b-4afd-95d1-7dd53b928038"));

            migrationBuilder.CreateIndex(
                name: "IX_PtaeoCode_DepartmentID",
                schema: "dbo",
                table: "PtaeoCode",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PtaeoCode_Department_DepartmentID",
                schema: "dbo",
                table: "PtaeoCode",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PtaeoCode_Department_DepartmentID",
                schema: "dbo",
                table: "PtaeoCode");

            migrationBuilder.DropIndex(
                name: "IX_PtaeoCode_DepartmentID",
                schema: "dbo",
                table: "PtaeoCode");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "PtaeoCode");
        }
    }
}
