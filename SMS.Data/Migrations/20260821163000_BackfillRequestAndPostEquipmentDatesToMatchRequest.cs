using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <summary>
    /// Backfills RequestEquipment and PostEquipment StartDate/EndDate to match the parent Request dates
    /// where they currently differ.
    /// </summary>
    public partial class BackfillRequestAndPostEquipmentDatesToMatchRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE re
SET
    re.StartDate = r.StartDate,
    re.EndDate   = r.EndDate
FROM
    dbo.[RequestEquipment] re
    INNER JOIN dbo.[Request] r ON r.RequestID = re.RequestID
WHERE
    re.StartDate <> r.StartDate
    OR re.EndDate <> r.EndDate;
");

            migrationBuilder.Sql(@"
UPDATE pe
SET
    pe.StartDate = r.StartDate,
    pe.EndDate   = r.EndDate
FROM
    dbo.[PostEquipment] pe
    INNER JOIN dbo.[Post]    p ON p.PostID    = pe.PostID
    INNER JOIN dbo.[Request] r ON r.RequestID = p.RequestID
WHERE
    pe.StartDate <> r.StartDate
    OR pe.EndDate <> r.EndDate;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversible — prior mismatched dates are not retained.
        }
    }
}
