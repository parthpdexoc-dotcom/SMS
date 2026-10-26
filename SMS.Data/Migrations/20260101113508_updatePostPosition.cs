using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatePostPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostPosition_Department",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropIndex(
                name: "IX_PostPosition_DepartmentID",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "PostPosition");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                schema: "dbo",
                table: "PostPosition",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_DepartmentID",
                schema: "dbo",
                table: "PostPosition",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PostPosition_Department",
                schema: "dbo",
                table: "PostPosition",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
