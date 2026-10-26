using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class BackfillClosedPostPositionNullStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ClosedPostPositionStatus.Closed = 1 (legacy rows may have NULL before Status was populated).
            migrationBuilder.Sql(@"
                UPDATE [dbo].[ClosedPostPosition]
                SET [Status] = 1
                WHERE [Status] IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversed (prior NULL vs 1 cannot be distinguished).
        }
    }
}
