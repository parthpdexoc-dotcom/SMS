using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <summary>
    /// For each PostPosition that has a CallSign but no PostCallSignID:
    /// 1. Insert a PostCallSign row (per distinct PostID + CallSign) when one does not already exist.
    /// 2. Set PostPosition.PostCallSignID to the matching PostCallSign row.
    /// </summary>
    public partial class BackfillPostCallSignFromPostPositionCallSign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: create missing PostCallSign rows from orphaned PostPosition.CallSign values.
            migrationBuilder.Sql(@"
                INSERT INTO dbo.PostCallSign
                (
                    PostCallSignID,
                    PostID,
                    CallSign,
                    IsDeleted,
                    DeletedAt,
                    DeletedUserID,
                    DeletedBy,
                    CreatedDate,
                    CreatedUserID,
                    CreatedBy,
                    EditedDate,
                    EditedUserID,
                    EditedBy
                )
                SELECT
                    NEWID(),
                    src.PostID,
                    src.CallSign,
                    0,
                    NULL,
                    NULL,
                    NULL,
                    COALESCE(src.CreatedDate, SYSUTCDATETIME()),
                    src.CreatedUserID,
                    src.CreatedBy,
                    NULL,
                    NULL,
                    NULL
                FROM
                (
                    SELECT
                        pp.PostID,
                        LTRIM(RTRIM(pp.CallSign)) AS CallSign,
                        MIN(pp.CreatedDate) AS CreatedDate,
                        MIN(pp.CreatedUserID) AS CreatedUserID,
                        MIN(pp.CreatedBy) AS CreatedBy
                    FROM dbo.PostPosition pp
                    INNER JOIN dbo.Post p
                        ON p.PostID = pp.PostID
                    WHERE pp.PostCallSignID IS NULL
                      AND pp.CallSign IS NOT NULL
                      AND LTRIM(RTRIM(pp.CallSign)) <> N''
                    GROUP BY pp.PostID, LTRIM(RTRIM(pp.CallSign))
                ) src
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM dbo.PostCallSign pcs
                    WHERE pcs.PostID = src.PostID
                      AND pcs.CallSign IS NOT NULL
                      AND LTRIM(RTRIM(pcs.CallSign)) = src.CallSign
                );
            ");

            // Step 2: link PostPosition.PostCallSignID to the matching PostCallSign row.
            migrationBuilder.Sql(@"
                ;WITH Matched AS
                (
                    SELECT
                        pp.PostPositionID,
                        pcs.PostCallSignID,
                        ROW_NUMBER() OVER (
                            PARTITION BY pp.PostPositionID
                            ORDER BY pcs.CreatedDate, pcs.PostCallSignID
                        ) AS rn
                    FROM dbo.PostPosition pp
                    INNER JOIN dbo.PostCallSign pcs
                        ON pcs.PostID = pp.PostID
                       AND pp.CallSign IS NOT NULL
                       AND LTRIM(RTRIM(pp.CallSign)) <> N''
                       AND pcs.CallSign IS NOT NULL
                       AND LTRIM(RTRIM(pcs.CallSign)) = LTRIM(RTRIM(pp.CallSign))
                    WHERE pp.PostCallSignID IS NULL
                )
                UPDATE pp
                SET pp.PostCallSignID = m.PostCallSignID
                FROM dbo.PostPosition pp
                INNER JOIN Matched m
                    ON m.PostPositionID = pp.PostPositionID
                   AND m.rn = 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversed.
        }
    }
}
