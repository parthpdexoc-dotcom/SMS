using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeNumberTypeInEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmployeeNumber",
                schema: "dbo",
                table: "Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql(@"
                UPDATE [dbo].[Employee]
                SET [EmployeeNumber] = [BadgeNumber];
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EmployeeNumber",
                schema: "dbo",
                table: "Employee",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.Sql(@"
                UPDATE [dbo].[Employee]
                SET [EmployeeNumber] = CASE
                    WHEN [EmployeeNumber] IS NULL OR LTRIM(RTRIM([EmployeeNumber])) = '' THEN 0
                    WHEN TRY_CAST([EmployeeNumber] AS int) IS NOT NULL THEN TRY_CAST([EmployeeNumber] AS int)
                    ELSE 0
                END;
            ");
        }
    }
}
