using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostStartTimeAndPostEndTimeInClosedPostPositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "PostEndTime",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "PostStartTime",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.Sql(@"
                UPDATE cpp
                SET cpp.[PostStartTime] = p.[PostStartTime],
                    cpp.[PostEndTime] = p.[PostEndTime]
                FROM [dbo].[ClosedPostPosition] cpp
                INNER JOIN [dbo].[Post] p ON p.[PostID] = cpp.[PostID];
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostEndTime",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "PostStartTime",
                schema: "dbo",
                table: "ClosedPostPosition");
        }
    }
}
