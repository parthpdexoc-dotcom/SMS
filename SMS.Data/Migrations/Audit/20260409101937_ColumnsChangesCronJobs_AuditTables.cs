using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations.Audit
{
    /// <inheritdoc />
    public partial class ColumnsChangesCronJobs_AuditTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedUtc",
                schema: "dbo",
                table: "WaitingPostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "FailureSummary",
                schema: "dbo",
                table: "WaitingPostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "JobName",
                schema: "dbo",
                table: "WaitingPostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "ScheduledSlotUtc",
                schema: "dbo",
                table: "WaitingPostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "Succeeded",
                schema: "dbo",
                table: "WaitingPostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "CompletedUtc",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit");

            migrationBuilder.DropColumn(
                name: "FailureSummary",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit");

            migrationBuilder.DropColumn(
                name: "JobName",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit");

            migrationBuilder.DropColumn(
                name: "ScheduledSlotUtc",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit");

            migrationBuilder.DropColumn(
                name: "Succeeded",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit");

            migrationBuilder.DropColumn(
                name: "CompletedUtc",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit");

            migrationBuilder.DropColumn(
                name: "FailureSummary",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit");

            migrationBuilder.DropColumn(
                name: "JobName",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit");

            migrationBuilder.DropColumn(
                name: "ScheduledSlotUtc",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit");

            migrationBuilder.DropColumn(
                name: "Succeeded",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit");

            migrationBuilder.DropColumn(
                name: "CompletedUtc",
                schema: "dbo",
                table: "Ptaeo_Audit");

            migrationBuilder.DropColumn(
                name: "FailureSummary",
                schema: "dbo",
                table: "Ptaeo_Audit");

            migrationBuilder.DropColumn(
                name: "JobName",
                schema: "dbo",
                table: "Ptaeo_Audit");

            migrationBuilder.DropColumn(
                name: "ScheduledSlotUtc",
                schema: "dbo",
                table: "Ptaeo_Audit");

            migrationBuilder.DropColumn(
                name: "Succeeded",
                schema: "dbo",
                table: "Ptaeo_Audit");

            migrationBuilder.DropColumn(
                name: "CompletedUtc",
                schema: "dbo",
                table: "EmployeeJob_Audit");

            migrationBuilder.DropColumn(
                name: "FailureSummary",
                schema: "dbo",
                table: "EmployeeJob_Audit");

            migrationBuilder.DropColumn(
                name: "JobName",
                schema: "dbo",
                table: "EmployeeJob_Audit");

            migrationBuilder.DropColumn(
                name: "ScheduledSlotUtc",
                schema: "dbo",
                table: "EmployeeJob_Audit");

            migrationBuilder.DropColumn(
                name: "Succeeded",
                schema: "dbo",
                table: "EmployeeJob_Audit");

            migrationBuilder.RenameColumn(
                name: "StartedUtc",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                newName: "ChangedOn");

            migrationBuilder.RenameColumn(
                name: "CronExpression",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "StartedUtc",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                newName: "ChangedOn");

            migrationBuilder.RenameColumn(
                name: "CronExpression",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "StartedUtc",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                newName: "ChangedOn");

            migrationBuilder.RenameColumn(
                name: "CronExpression",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "StartedUtc",
                schema: "dbo",
                table: "Ptaeo_Audit",
                newName: "ChangedOn");

            migrationBuilder.RenameColumn(
                name: "CronExpression",
                schema: "dbo",
                table: "Ptaeo_Audit",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "StartedUtc",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                newName: "ChangedOn");

            migrationBuilder.RenameColumn(
                name: "CronExpression",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "Action",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ChangedFields",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Action",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ChangedFields",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Action",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ChangedFields",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Action",
                schema: "dbo",
                table: "Ptaeo_Audit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ChangedFields",
                schema: "dbo",
                table: "Ptaeo_Audit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                schema: "dbo",
                table: "Ptaeo_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "dbo",
                table: "Ptaeo_Audit",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "Ptaeo_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                schema: "dbo",
                table: "Ptaeo_Audit",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Action",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ChangedFields",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                schema: "dbo",
                table: "WaitingPostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "ChangedFields",
                schema: "dbo",
                table: "WaitingPostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "WaitingPostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "dbo",
                table: "WaitingPostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "WaitingPostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "EntityId",
                schema: "dbo",
                table: "WaitingPostPosition_Audit");

            migrationBuilder.DropColumn(
                name: "Action",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit");

            migrationBuilder.DropColumn(
                name: "ChangedFields",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit");

            migrationBuilder.DropColumn(
                name: "EntityId",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit");

            migrationBuilder.DropColumn(
                name: "Action",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit");

            migrationBuilder.DropColumn(
                name: "ChangedFields",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit");

            migrationBuilder.DropColumn(
                name: "EntityId",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit");

            migrationBuilder.DropColumn(
                name: "Action",
                schema: "dbo",
                table: "Ptaeo_Audit");

            migrationBuilder.DropColumn(
                name: "ChangedFields",
                schema: "dbo",
                table: "Ptaeo_Audit");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "Ptaeo_Audit");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "dbo",
                table: "Ptaeo_Audit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "Ptaeo_Audit");

            migrationBuilder.DropColumn(
                name: "EntityId",
                schema: "dbo",
                table: "Ptaeo_Audit");

            migrationBuilder.DropColumn(
                name: "Action",
                schema: "dbo",
                table: "EmployeeJob_Audit");

            migrationBuilder.DropColumn(
                name: "ChangedFields",
                schema: "dbo",
                table: "EmployeeJob_Audit");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "EmployeeJob_Audit");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "dbo",
                table: "EmployeeJob_Audit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "dbo",
                table: "EmployeeJob_Audit");

            migrationBuilder.DropColumn(
                name: "EntityId",
                schema: "dbo",
                table: "EmployeeJob_Audit");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                newName: "CronExpression");

            migrationBuilder.RenameColumn(
                name: "ChangedOn",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                newName: "StartedUtc");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                newName: "CronExpression");

            migrationBuilder.RenameColumn(
                name: "ChangedOn",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                newName: "StartedUtc");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                newName: "CronExpression");

            migrationBuilder.RenameColumn(
                name: "ChangedOn",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                newName: "StartedUtc");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "dbo",
                table: "Ptaeo_Audit",
                newName: "CronExpression");

            migrationBuilder.RenameColumn(
                name: "ChangedOn",
                schema: "dbo",
                table: "Ptaeo_Audit",
                newName: "StartedUtc");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                newName: "CronExpression");

            migrationBuilder.RenameColumn(
                name: "ChangedOn",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                newName: "StartedUtc");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedUtc",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FailureSummary",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobName",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledSlotUtc",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Succeeded",
                schema: "dbo",
                table: "WaitingPostPosition_Audit",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedUtc",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FailureSummary",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobName",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledSlotUtc",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Succeeded",
                schema: "dbo",
                table: "UpdateReleasedOnToReleased_Audit",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedUtc",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FailureSummary",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobName",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledSlotUtc",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Succeeded",
                schema: "dbo",
                table: "UpdatePreferredToManaged_Audit",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedUtc",
                schema: "dbo",
                table: "Ptaeo_Audit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FailureSummary",
                schema: "dbo",
                table: "Ptaeo_Audit",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobName",
                schema: "dbo",
                table: "Ptaeo_Audit",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledSlotUtc",
                schema: "dbo",
                table: "Ptaeo_Audit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Succeeded",
                schema: "dbo",
                table: "Ptaeo_Audit",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedUtc",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FailureSummary",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobName",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledSlotUtc",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Succeeded",
                schema: "dbo",
                table: "EmployeeJob_Audit",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
