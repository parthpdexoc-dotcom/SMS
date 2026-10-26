using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class BackfillPostEndTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PostLength is duration in minutes (same as PostDto / scheduling logic).
            // PostEndTime = PostStartTime + PostLength minutes.
            migrationBuilder.Sql(@"
                UPDATE [dbo].[Post]
                SET [PostEndTime] = CAST(DATEADD(MINUTE, [PostLength], CAST([PostStartTime] AS datetime)) AS time(7))
                WHERE [PostLength] > 0;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversed (previous PostEndTime values are not stored).
        }
    }
}
