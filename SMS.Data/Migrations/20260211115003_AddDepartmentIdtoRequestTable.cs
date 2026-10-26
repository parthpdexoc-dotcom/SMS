using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentIdtoRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                schema: "dbo",
                table: "Request",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestID",
                schema: "dbo",
                table: "BlockDate",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PostID",
                schema: "dbo",
                table: "BlockDate",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_DepartmentID",
                schema: "dbo",
                table: "Request",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Department_DepartmentID",
                schema: "dbo",
                table: "Request",
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
                name: "FK_Request_Department_DepartmentID",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_DepartmentID",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "Request");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestID",
                schema: "dbo",
                table: "BlockDate",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostID",
                schema: "dbo",
                table: "BlockDate",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
