using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class QueryInPostEquipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean invalid RequestID references before backfill
            migrationBuilder.Sql(@"
                UPDATE dbo.PostEquipment
                SET RequestID = NULL
                WHERE RequestID IS NOT NULL
                  AND RequestID NOT IN (SELECT RequestID FROM dbo.Request);
            ");

            migrationBuilder.Sql(@"
    UPDATE pe
    SET
        pe.RequestID = p.RequestID,
        pe.Equipment_Per_Post = re.Equipment_Per_Request,
        pe.Equipment_Quantity = re.Equipment_Quantity
    FROM dbo.PostEquipment pe
    INNER JOIN dbo.Post p
        ON pe.PostID = p.PostID
    INNER JOIN dbo.RequestEquipment re
        ON re.RequestID = p.RequestID
        AND re.DepartmentEquipmentID = pe.DepartmentEquipmentID
    INNER JOIN dbo.Request r
        ON r.RequestID = p.RequestID
    WHERE
        (
            pe.RequestID IS NULL
            OR pe.Equipment_Per_Post IS NULL
            OR pe.Equipment_Quantity IS NULL
        );
");

            migrationBuilder.Sql(@"
    INSERT INTO dbo.PostEquipment (
        PostEquipmentID,
        PostID,
        DepartmentEquipmentID,
        Equipment_Per_Post,
        Equipment_Quantity,
        RequestID
    )
    SELECT
        NEWID(),
        p.PostID,
        re.DepartmentEquipmentID,
        re.Equipment_Per_Request,
        re.Equipment_Quantity,
        p.RequestID
    FROM dbo.Post p
    INNER JOIN dbo.RequestEquipment re
        ON re.RequestID = p.RequestID
    INNER JOIN dbo.Request r
        ON r.RequestID = p.RequestID
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.PostEquipment pe
        WHERE pe.PostID = p.PostID
          AND pe.DepartmentEquipmentID = re.DepartmentEquipmentID
    );
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill migration; no automatic rollback.
        }
    }
}
