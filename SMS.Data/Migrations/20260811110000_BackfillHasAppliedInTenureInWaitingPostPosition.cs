using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <summary>
    /// Backfills <c>WaitingPostPosition.HasAppliedInTenure</c> for historical rows using the same
    /// rule as <c>GetHasAppliedInTenureByDateAsync</c>:
    /// 1) current <c>RequestSchedule.ReleaseAs = TenureBased</c> covering <c>DateOfWork</c>;
    /// 2) schedule window covering <c>DateOfWork</c> that was TenureBased and later flipped
    /// (via sibling audit DB <c>{CurrentDb}_Audit.dbo.RequestSchedule_Audit</c>).
    /// </summary>
    public partial class BackfillHasAppliedInTenureInWaitingPostPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Case 1: current TenureBased schedule covers DateOfWork.
            migrationBuilder.Sql(@"
                UPDATE w
                SET w.HasAppliedInTenure = 1
                FROM dbo.WaitingPostPosition w
                INNER JOIN dbo.Post p
                    ON p.PostID = w.PostID
                WHERE w.HasAppliedInTenure = 0
                  AND EXISTS
                  (
                      SELECT 1
                      FROM dbo.RequestSchedule rs
                      WHERE rs.RequestId = COALESCE(
                                NULLIF(w.RequestID, '00000000-0000-0000-0000-000000000000'),
                                p.RequestID)
                        AND rs.ReleaseAs = 1 -- TenureBased
                        AND (rs.StartDate IS NULL OR w.DateOfWork >= rs.StartDate)
                        AND (rs.EndDate   IS NULL OR w.DateOfWork <= rs.EndDate)
                  );
            ");

            // Case 2: schedule covering DateOfWork was TenureBased and later flipped (sibling audit DB).
            // Resolves audit catalog as {CurrentDb}_Audit (same convention as other cross-DB migrations).
            migrationBuilder.Sql(@"
DECLARE @AuditDb sysname = DB_NAME() + N'_Audit';

IF DB_ID(@AuditDb) IS NULL
BEGIN
    DECLARE @missingAuditDbMessage nvarchar(400) =
        N'Audit database not found for HasAppliedInTenure backfill: ' + @AuditDb;
    THROW 50001, @missingAuditDbMessage, 1;
END;

DECLARE @sql nvarchar(max) = N'
UPDATE w
SET w.HasAppliedInTenure = 1
FROM dbo.WaitingPostPosition w
INNER JOIN dbo.Post p
    ON p.PostID = w.PostID
WHERE w.HasAppliedInTenure = 0
  AND EXISTS
  (
      SELECT 1
      FROM dbo.RequestSchedule rs
      INNER JOIN ' + QUOTENAME(@AuditDb) + N'.[dbo].[RequestSchedule_Audit] a
          ON a.EntityId = CONVERT(nvarchar(36), rs.RequestScheduleID)
      WHERE rs.RequestId = COALESCE(
                NULLIF(w.RequestID, ''00000000-0000-0000-0000-000000000000''),
                p.RequestID)
        AND (rs.StartDate IS NULL OR w.DateOfWork >= rs.StartDate)
        AND (rs.EndDate   IS NULL OR w.DateOfWork <= rs.EndDate)
        AND (
              a.ChangedFields LIKE N''%""Property"":""Release as""%''
           OR a.ChangedFields LIKE N''%""Property"":""ReleaseAs""%''
        )
        AND (
              a.ChangedFields LIKE N''%TenureBased%''
           OR a.ChangedFields LIKE N''%""Old"":1%''
           OR a.ChangedFields LIKE N''%""Old"":""1""%''
        )
  );
';

EXEC sys.sp_executesql @sql;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversed.
        }
    }
}
