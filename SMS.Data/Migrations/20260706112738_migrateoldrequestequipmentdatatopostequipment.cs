using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class migrateoldrequestequipmentdatatopostequipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO dbo.PostEquipment (
    PostEquipmentID,
    PostID,
    DepartmentEquipmentID,
    EquipmentPerPostPosition,
    EquipmentQuantity,
    IsVehicle,
    StartDate,
    EndDate,
    CreatedDate,
    CreatedUserID,
    CreatedBy,
    EditedDate,
    EditedUserID,
    EditedBy,
    IsDeleted
)
SELECT
    NEWID() AS PostEquipmentID,
    p.PostID,
    re.DepartmentEquipmentID,
    CASE WHEN re.EquipmentPerPost = 1 THEN 0 ELSE 1 END AS EquipmentPerPostPosition,
    re.EquipmentQuantity,
    re.IsVehicle,
    re.StartDate,
    re.EndDate,
    ISNULL(p.CreatedDate, ISNULL(re.CreatedDate, GETUTCDATE())) AS CreatedDate,
    p.CreatedUserID,
    p.CreatedBy,
    NULL AS EditedDate,
    NULL AS EditedUserID,
    NULL AS EditedBy,
    0 AS IsDeleted
FROM dbo.Post p
INNER JOIN dbo.Request r
    ON r.RequestID = p.RequestID
   AND ISNULL(r.IsDeleted, 0) = 0
INNER JOIN dbo.RequestEquipment re
    ON re.RequestID = p.RequestID
   AND ISNULL(re.IsDeleted, 0) = 0
WHERE ISNULL(p.IsDeleted, 0) = 0
  AND p.RequestID IS NOT NULL
  AND (
        (re.IsVehicle = 1 AND p.PostRequiresVehicle = 1)
     OR (re.IsVehicle = 0 AND p.PostRequiresEquipment = 1)
  )
  AND NOT EXISTS (
        SELECT 1
        FROM dbo.PostEquipment pe
        WHERE pe.PostID = p.PostID
          AND pe.DepartmentEquipmentID = re.DepartmentEquipmentID
          AND ISNULL(pe.IsDeleted, 0) = 0
  );
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill uses the post's CreatedBy; no automatic rollback.
        }
    }
}
