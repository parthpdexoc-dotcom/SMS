using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class BackfillClosedPostPositionWorkEndTimeFromHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // HoursWorked is stored as minutes (see reporting: HoursWorked / 60 for display hours).
            // WorkEndTime = WorkStartTime + HoursWorked minutes (e.g. 12:40 + 120 min => 14:40).
            migrationBuilder.Sql(@"
                UPDATE [dbo].[ClosedPostPosition]
                SET [WorkEndTime] = CAST(DATEADD(MINUTE, [HoursWorked], CAST([WorkStartTime] AS datetime)) AS time(7))
                WHERE [WorkStartTime] IS NOT NULL
                  AND [WorkEndTime] IS NULL
                  AND [HoursWorked] >= 0;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversed.
        }
    }
}
