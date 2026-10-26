using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <summary>
    /// Backfills PostCallSign rows from distinct non-null PostPosition.CallSign values only
    /// (sequential slot order), then links PostPosition.PostCallSignID by PostID + CallSign.
    /// </summary>
    public partial class BackfillPostCallSignFromPostPositionNotNullOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert one PostCallSign row per distinct non-null CallSign, in sequential slot order.
            migrationBuilder.Sql(@"
                ;WITH SlotNumbers AS
                (
                    SELECT 1 AS SlotNumber
                    UNION ALL
                    SELECT SlotNumber + 1
                    FROM SlotNumbers
                    WHERE SlotNumber < 200
                ),
                DistinctCallSigns AS
                (
                    SELECT
                        pp.PostID,
                        LTRIM(RTRIM(pp.CallSign)) AS CallSign,
                        ROW_NUMBER() OVER (
                            PARTITION BY pp.PostID
                            ORDER BY MIN(pp.CreatedDate), LTRIM(RTRIM(pp.CallSign))
                        ) AS SlotNumber
                    FROM dbo.PostPosition pp
                    INNER JOIN dbo.Post p ON p.PostID = pp.PostID AND p.IsDeleted = 0
                    WHERE pp.IsDeleted = 0
                      AND pp.CallSign IS NOT NULL
                      AND LTRIM(RTRIM(pp.CallSign)) <> N''
                    GROUP BY pp.PostID, LTRIM(RTRIM(pp.CallSign))
                ),
                PostSlots AS
                (
                    SELECT
                        p.PostID,
                        sn.SlotNumber,
                        p.CreatedDate,
                        p.CreatedUserID,
                        p.CreatedBy
                    FROM dbo.Post p
                    INNER JOIN SlotNumbers sn ON sn.SlotNumber <= p.TotalPostPositions
                    WHERE p.IsDeleted = 0
                      AND p.TotalPostPositions > 0
                      AND NOT EXISTS (
                          SELECT 1
                          FROM dbo.PostCallSign pcs
                          WHERE pcs.PostID = p.PostID
                            AND pcs.IsDeleted = 0
                      )
                )
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
                    ps.PostID,
                    dcs.CallSign,
                    0,
                    NULL,
                    NULL,
                    NULL,
                    COALESCE(ps.CreatedDate, SYSUTCDATETIME()),
                    ps.CreatedUserID,
                    ps.CreatedBy,
                    NULL,
                    NULL,
                    NULL
                FROM PostSlots ps
                INNER JOIN DistinctCallSigns dcs
                    ON dcs.PostID = ps.PostID
                   AND dcs.SlotNumber = ps.SlotNumber
                WHERE dcs.CallSign IS NOT NULL
                  AND LTRIM(RTRIM(dcs.CallSign)) <> N''
                OPTION (MAXRECURSION 200);
            ");

            // Link PostPosition.PostCallSignID from PostCallSign matched by PostID + CallSign.
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
                       AND pcs.IsDeleted = 0
                       AND pp.CallSign IS NOT NULL
                       AND LTRIM(RTRIM(pp.CallSign)) <> N''
                       AND pcs.CallSign IS NOT NULL
                       AND LTRIM(RTRIM(pcs.CallSign)) = LTRIM(RTRIM(pp.CallSign))
                    WHERE pp.PostCallSignID IS NULL
                      AND pp.IsDeleted = 0
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
