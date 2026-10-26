using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeStatusForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "Admin",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Clean up invalid DepartmentID values before adding foreign key
            migrationBuilder.Sql(@"
                UPDATE [dbo].[EmployeeStatus]
                SET [DepartmentID] = (SELECT TOP 1 [DepartmentID] FROM [dbo].[Department] ORDER BY [DepartmentID])
                WHERE [DepartmentID] NOT IN (SELECT [DepartmentID] FROM [dbo].[Department])
                AND EXISTS (SELECT 1 FROM [dbo].[Department]);
            ");

            // Clean up invalid CreatedUserID values before adding foreign key
            migrationBuilder.Sql(@"
                UPDATE [dbo].[EmployeeStatus]
                SET [CreatedUserID] = NULL
                WHERE [CreatedUserID] IS NOT NULL
                AND [CreatedUserID] NOT IN (SELECT [EmployeeID] FROM [dbo].[Employee]);
            ");

            // Clean up invalid EditedUserID values before adding foreign key
            migrationBuilder.Sql(@"
                UPDATE [dbo].[EmployeeStatus]
                SET [EditedUserID] = NULL
                WHERE [EditedUserID] IS NOT NULL
                AND [EditedUserID] NOT IN (SELECT [EmployeeID] FROM [dbo].[Employee]);
            ");

            // Drop indexes if they exist (in case of partial migration)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EmployeeStatus_DepartmentID' AND object_id = OBJECT_ID('dbo.EmployeeStatus'))
                    DROP INDEX [IX_EmployeeStatus_DepartmentID] ON [dbo].[EmployeeStatus];
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EmployeeStatus_CreatedUserID' AND object_id = OBJECT_ID('dbo.EmployeeStatus'))
                    DROP INDEX [IX_EmployeeStatus_CreatedUserID] ON [dbo].[EmployeeStatus];
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EmployeeStatus_EditedUserID' AND object_id = OBJECT_ID('dbo.EmployeeStatus'))
                    DROP INDEX [IX_EmployeeStatus_EditedUserID] ON [dbo].[EmployeeStatus];
            ");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeStatus_DepartmentID",
                schema: "dbo",
                table: "EmployeeStatus",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeStatus_CreatedUserID",
                schema: "dbo",
                table: "EmployeeStatus",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeStatus_EditedUserID",
                schema: "dbo",
                table: "EmployeeStatus",
                column: "EditedUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeStatus_Department_DepartmentID",
                schema: "dbo",
                table: "EmployeeStatus",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeStatus_Employee_CreatedUserID",
                schema: "dbo",
                table: "EmployeeStatus",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeStatus_Employee_EditedUserID",
                schema: "dbo",
                table: "EmployeeStatus",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeStatus_Department_DepartmentID",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeStatus_Employee_CreatedUserID",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeStatus_Employee_EditedUserID",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeStatus_DepartmentID",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeStatus_CreatedUserID",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeStatus_EditedUserID",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "Admin",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);
        }
    }
}
