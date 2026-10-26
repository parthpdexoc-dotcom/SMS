using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostEndTimeInPostTableAndEndTimeInPostPositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                schema: "dbo",
                table: "PostPosition",
                type: "time",
                nullable: true);

            // Backfill EndTime from StartTime + LengthOfWork (minutes), same as PostPositionDto.GetPostPositionEndDateTime().
            migrationBuilder.Sql(@"
                UPDATE [dbo].[PostPosition]
                SET [EndTime] = CAST(DATEADD(MINUTE, [LengthOfWork], CAST([StartTime] AS datetime)) AS time(7))
                WHERE [EndTime] IS NULL
                  AND [StartTime] IS NOT NULL;
            ");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "PostEndTime",
                schema: "dbo",
                table: "Post",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "PostEndTime",
                schema: "dbo",
                table: "Post");
        }
    }
}
