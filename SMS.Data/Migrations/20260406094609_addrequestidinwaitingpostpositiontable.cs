using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addrequestidinwaitingpostpositiontable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WaitingPostPosition_EmployeeID_PostID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostID",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestID",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPostPosition_EmployeeID_PostID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition",
                columns: new[] { "EmployeeID", "PostID", "DateOfWork", "PostPositionStatus" },
                unique: true,
                filter: "[PostID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPostPosition_EmployeeID_RequestID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition",
                columns: new[] { "EmployeeID", "RequestID", "DateOfWork", "PostPositionStatus" },
                unique: true,
                filter: "[RequestID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPostPosition_RequestID",
                schema: "dbo",
                table: "WaitingPostPosition",
                column: "RequestID");

            migrationBuilder.AddCheckConstraint(
                name: "CK_WaitingPostPosition_PostOrRequest",
                schema: "dbo",
                table: "WaitingPostPosition",
                sql: "[PostID] IS NOT NULL OR [RequestID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitingPostPosition_Request_RequestID",
                schema: "dbo",
                table: "WaitingPostPosition",
                column: "RequestID",
                principalSchema: "dbo",
                principalTable: "Request",
                principalColumn: "RequestID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaitingPostPosition_Request_RequestID",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropIndex(
                name: "IX_WaitingPostPosition_EmployeeID_PostID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropIndex(
                name: "IX_WaitingPostPosition_EmployeeID_RequestID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropIndex(
                name: "IX_WaitingPostPosition_RequestID",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WaitingPostPosition_PostOrRequest",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "RequestID",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostID",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPostPosition_EmployeeID_PostID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition",
                columns: new[] { "EmployeeID", "PostID", "DateOfWork", "PostPositionStatus" },
                unique: true);
        }
    }
}
