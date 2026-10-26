using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations.Audit
{
    /// <inheritdoc />
    public partial class AddEmployeeColumnIn_AuditTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "Requester_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "Request_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "PostPosition_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "Post_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "Employee_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "ClosedPostPositionEquipment_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "ClosedPostPosition_Audit",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "Requester_Audit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "Request_Audit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "PostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "Post_Audit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "Employee_Audit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "ClosedPostPositionEquipment_Audit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "ClosedPostPosition_Audit");
        }
    }
}
