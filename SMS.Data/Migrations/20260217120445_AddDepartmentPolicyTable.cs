using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentPolicyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentPolicy",
                schema: "dbo",
                columns: table => new
                {
                    DepartmentPolicyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostsTakenPerDay = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    HoursPerDayWorked = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    HoursPerWeekWorked = table.Column<int>(type: "int", nullable: false, defaultValue: 20),
                    HoursPerMonthWorked = table.Column<int>(type: "int", nullable: false, defaultValue: 60),
                    DeptOpenPostsTakenPerDay = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    TenurePostsAllowedPerWeek = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    TenurePostsAllowedPerDay = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    DeptOpenPostsTakenPerWeek = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    Previewable = table.Column<short>(type: "smallint", nullable: true),
                    PreviewableHours = table.Column<short>(type: "smallint", nullable: true),
                    Tenure24HourLimit = table.Column<short>(type: "smallint", nullable: true),
                    Tenure24HourLimitPosts = table.Column<short>(type: "smallint", nullable: true),
                    MinHoursPaid = table.Column<int>(type: "int", nullable: false),
                    UsesTenureJobsForWeekRule = table.Column<int>(type: "int", nullable: true),
                    Position24HourQuota = table.Column<short>(type: "smallint", nullable: true),
                    PositionExempt24HourQuota = table.Column<short>(type: "smallint", nullable: true),
                    Managed24HourQuota = table.Column<short>(type: "smallint", nullable: true),
                    PostToSupervisor = table.Column<int>(type: "int", nullable: true),
                    OverlapJobConflictConfirmation = table.Column<bool>(type: "bit", nullable: false),
                    SupervisorsCanViewLowerEmpTypeJobs = table.Column<bool>(type: "bit", nullable: false),
                    SupervisorsCanViewLowerEmpTypeJobsCount = table.Column<int>(type: "int", nullable: false),
                    NumberOfEmployeesBeforeSupervisorRequired = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentPolicy", x => x.DepartmentPolicyID);
                    table.ForeignKey(
                        name: "FK_DepartmentPolicy_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentPolicy_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentPolicy_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPolicy_CreatedUserID",
                schema: "dbo",
                table: "DepartmentPolicy",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPolicy_DepartmentID",
                schema: "dbo",
                table: "DepartmentPolicy",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPolicy_EditedUserID",
                schema: "dbo",
                table: "DepartmentPolicy",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentPolicy",
                schema: "dbo");
        }
    }
}
