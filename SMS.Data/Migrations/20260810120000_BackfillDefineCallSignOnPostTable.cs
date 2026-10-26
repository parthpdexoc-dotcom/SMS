using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <summary>
    /// Sets DefineCallSign = true on Post rows linked to a Request that have
    /// a non-empty CallSign on PostCallSign or PostPosition.
    /// </summary>
    public partial class BackfillDefineCallSignOnPostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE p
                SET p.DefineCallSign = 1
                FROM dbo.Post p
                INNER JOIN dbo.Request r
                    ON r.RequestID = p.RequestID
                WHERE (
                      EXISTS (
                          SELECT 1
                          FROM dbo.PostCallSign pcs
                          WHERE pcs.PostID = p.PostID
                            AND pcs.CallSign IS NOT NULL
                            AND LTRIM(RTRIM(pcs.CallSign)) <> N''
                      )
                      OR EXISTS (
                          SELECT 1
                          FROM dbo.PostPosition pp
                          WHERE pp.PostID = p.PostID
                            AND pp.CallSign IS NOT NULL
                            AND LTRIM(RTRIM(pp.CallSign)) <> N''
                      )
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversed.
        }
    }
}
