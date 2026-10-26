using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableDepartmentCronManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentCronJobManagement",
                schema: "dbo",
                columns: table => new
                {
                    DepartmentCronJobManagementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Job = table.Column<int>(type: "int", nullable: false),
                    CronExpression = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentCronJobManagement", x => x.DepartmentCronJobManagementId);
                    table.ForeignKey(
                        name: "FK_DepartmentCronJobManagement_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentCronJobManagement_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentCronJobManagement_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentCronJobManagement_CreatedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentCronJobManagement_DepartmentID",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentCronJobManagement_DepartmentID_Job",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                columns: new[] { "DepartmentID", "Job" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentCronJobManagement_EditedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentCronJobManagement",
                schema: "dbo");
        }
    }
}
