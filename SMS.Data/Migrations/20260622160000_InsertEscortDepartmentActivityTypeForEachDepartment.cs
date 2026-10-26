using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class InsertEscortDepartmentActivityTypeForEachDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO dbo.DepartmentActivityType (
                    DepartmentActivityTypeId,
                    DepartmentID,
                    ActivityTypeShort,
                    ActivityTypeLong,
                    ActivityTypeStatus,
                    CreatedBy,
                    CreatedUserID,
                    CreatedDate,
                    IsDeleted)
                SELECT
                    NEWID(),
                    d.DepartmentID,
                    'Escort',
                    'Escort',
                    1,
                    'MASTER',
                    e.EmployeeID,
                    GETUTCDATE(),
                    0
                FROM dbo.Department d
                INNER JOIN dbo.EmployeeType et
                    ON et.DepartmentID = d.DepartmentID
                   AND et.IsSystemUser = 1
                   AND et.EmployeeTypeShort = 'MasterType'
                INNER JOIN dbo.Employee e
                    ON e.DepartmentID = d.DepartmentID
                   AND e.EmployeeTypeID = et.EmployeeTypeID
                   AND e.IsDeleted = 0
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM dbo.DepartmentActivityType dat
                    WHERE dat.DepartmentID = d.DepartmentID
                      AND dat.IsDeleted = 0
                      AND LOWER(LTRIM(RTRIM(dat.ActivityTypeShort))) = 'escort');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data seed is not reversed.
        }
    }
}
