using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStartDateEndDateForRequestAndPostEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

        -- RequestEquipment
        UPDATE RE
        SET 
            RE.StartDate = R.StartDate,
            RE.EndDate = R.EndDate
        FROM RequestEquipment RE
        INNER JOIN Request R 
            ON R.RequestID = RE.RequestID
        WHERE 
            RE.StartDate = '0001-01-01'
            OR RE.EndDate = '0001-01-01';


        -- PostEquipment
        UPDATE PE
        SET 
            PE.StartDate = R.StartDate,
            PE.EndDate = R.EndDate
        FROM PostEquipment PE
        INNER JOIN Post P 
            ON P.PostID = PE.PostID
        INNER JOIN Request R 
            ON R.RequestID = P.RequestID
        WHERE 
            PE.StartDate = '0001-01-01'
            OR PE.EndDate = '0001-01-01';

        -- Sync EquipmentPerPostPosition
        UPDATE PE
        SET 
            PE.EquipmentPerPostPosition =
                CASE 
                    WHEN RE.EquipmentPerPost = 1 THEN 0
                    ELSE 1
                END
        FROM PostEquipment PE
        INNER JOIN Post P 
            ON P.PostID = PE.PostID
        INNER JOIN RequestEquipment RE
            ON RE.RequestID = P.RequestID;

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
