using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationDeptResponseAreaToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentResponseAreaID",
                schema: "dbo",
                table: "Request",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_DepartmentResponseAreaID",
                schema: "dbo",
                table: "Request",
                column: "DepartmentResponseAreaID");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_DepartmentResponseArea_DepartmentResponseAreaID",
                schema: "dbo",
                table: "Request",
                column: "DepartmentResponseAreaID",
                principalSchema: "dbo",
                principalTable: "DepartmentResponseArea",
                principalColumn: "DepartmentResponseAreaID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_DepartmentResponseArea_DepartmentResponseAreaID",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_DepartmentResponseAreaID",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "DepartmentResponseAreaID",
                schema: "dbo",
                table: "Request");
        }
    }
}
