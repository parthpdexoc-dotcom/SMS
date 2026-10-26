using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWaitingPostPositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WaitingPostPosition_EmployeeID",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "CallSign",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "EquipmentRequired",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "IsSplit",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "LengthOfWork",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "ParentPostPositionId",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "PostDepartmentDefinedFieldValue1",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "PostDepartmentDefinedFieldValue10",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "PostDepartmentDefinedFieldValue2",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "PostDepartmentDefinedFieldValue3",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "PostDepartmentDefinedFieldValue4",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "PostDepartmentDefinedFieldValue5",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "PostDepartmentDefinedFieldValue6",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "PostDepartmentDefinedFieldValue7",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "PostDepartmentDefinedFieldValue8",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "PostDepartmentDefinedFieldValue9",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "ReleaseAs",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "SplitPostPositionID",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "StartTime",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPostPosition_EmployeeID_PostID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition",
                columns: new[] { "EmployeeID", "PostID", "DateOfWork", "PostPositionStatus" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WaitingPostPosition_EmployeeID_PostID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.AddColumn<string>(
                name: "CallSign",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EquipmentRequired",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSplit",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LengthOfWork",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentPostPositionId",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostDepartmentDefinedFieldValue1",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostDepartmentDefinedFieldValue10",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostDepartmentDefinedFieldValue2",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostDepartmentDefinedFieldValue3",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostDepartmentDefinedFieldValue4",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostDepartmentDefinedFieldValue5",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostDepartmentDefinedFieldValue6",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostDepartmentDefinedFieldValue7",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostDepartmentDefinedFieldValue8",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostDepartmentDefinedFieldValue9",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReleaseAs",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SplitPostPositionID",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPostPosition_EmployeeID",
                schema: "dbo",
                table: "WaitingPostPosition",
                column: "EmployeeID");
        }
    }
}
