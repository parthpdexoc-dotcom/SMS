using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetIsPasswordResetTrueForDepartmentEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE [dbo].[Employee]
                SET [isPasswordReset] = 1
                WHERE [DepartmentID] = '992fab13-e61b-4afd-95d1-7dd53b928038';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE [dbo].[Employee]
                SET [isPasswordReset] = 0
                WHERE [DepartmentID] = '992fab13-e61b-4afd-95d1-7dd53b928038';
            ");
        }
    }
}
