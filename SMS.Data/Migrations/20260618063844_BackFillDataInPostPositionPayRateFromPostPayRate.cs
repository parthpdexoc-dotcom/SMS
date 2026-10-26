using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class BackFillDataInPostPositionPayRateFromPostPayRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE pp
                SET pp.[PostPositionPayRate] = p.[PostPayRate]
                FROM [dbo].[PostPosition] pp
                INNER JOIN [dbo].[Post] p ON p.[PostID] = pp.[PostID]
                WHERE pp.[PostPositionPayRate] IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversed.
        }
    }
}
