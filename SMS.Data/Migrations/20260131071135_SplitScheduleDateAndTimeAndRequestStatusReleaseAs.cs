using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitScheduleDateAndTimeAndRequestStatusReleaseAs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new columns first so we can migrate data
            migrationBuilder.AddColumn<DateOnly>(
                name: "ScheduleDate",
                schema: "dbo",
                table: "Request",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduledTime",
                schema: "dbo",
                table: "Request",
                type: "time",
                nullable: true);

            // Migrate existing ScheduleDateAndTime data into ScheduleDate and ScheduledTime
            migrationBuilder.Sql(@"
                UPDATE dbo.[Request]
                SET ScheduleDate = CAST(ScheduleDateAndTime AS date),
                    ScheduledTime = CAST(ScheduleDateAndTime AS time)
                WHERE ScheduleDateAndTime IS NOT NULL;
            ");

            migrationBuilder.DropColumn(
                name: "ScheduleDateAndTime",
                schema: "dbo",
                table: "Request");

            migrationBuilder.RenameColumn(
                name: "ReleaseStatus",
                schema: "dbo",
                table: "Request",
                newName: "RequestStatus");

            migrationBuilder.AddColumn<int>(
                name: "ReleaseAs",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseAs",
                schema: "dbo",
                table: "Request");

            migrationBuilder.RenameColumn(
                name: "RequestStatus",
                schema: "dbo",
                table: "Request",
                newName: "ReleaseStatus");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleDateAndTime",
                schema: "dbo",
                table: "Request",
                type: "datetime",
                nullable: true);

            // Restore ScheduleDateAndTime from ScheduleDate and ScheduledTime on rollback
            migrationBuilder.Sql(@"
                UPDATE dbo.[Request]
                SET ScheduleDateAndTime = DATEADD(ms, DATEDIFF(ms, 0, ScheduledTime), CAST(ScheduleDate AS datetime))
                WHERE ScheduleDate IS NOT NULL AND ScheduledTime IS NOT NULL;
            ");

            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ScheduledTime",
                schema: "dbo",
                table: "Request");
        }
    }
}
