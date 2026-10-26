using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class createtableTenureWaitingPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenureWaitingPosition",
                schema: "dbo",
                columns: table => new
                {
                    TenureWaitingPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    PostID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfWork = table.Column<DateOnly>(type: "date", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    LengthOfWork = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    EquipmentRequired = table.Column<bool>(type: "bit", nullable: false),
                    CallSign = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostPositionStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ParentPostPositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsSplit = table.Column<bool>(type: "bit", nullable: false),
                    SplitPostPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PostDepartmentDefinedFieldValue1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostDepartmentDefinedFieldValue2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostDepartmentDefinedFieldValue3 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostDepartmentDefinedFieldValue4 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostDepartmentDefinedFieldValue5 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostDepartmentDefinedFieldValue6 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostDepartmentDefinedFieldValue7 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostDepartmentDefinedFieldValue8 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostDepartmentDefinedFieldValue9 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostDepartmentDefinedFieldValue10 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenureWaitingPosition", x => x.TenureWaitingPositionID);
                    table.ForeignKey(
                        name: "FK_TenureWaitingPosition_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenureWaitingPosition_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenureWaitingPosition_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenureWaitingPosition_Post_PostID",
                        column: x => x.PostID,
                        principalSchema: "dbo",
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenureWaitingPosition_CreatedUserID",
                schema: "dbo",
                table: "TenureWaitingPosition",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TenureWaitingPosition_EditedUserID",
                schema: "dbo",
                table: "TenureWaitingPosition",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TenureWaitingPosition_EmployeeID",
                schema: "dbo",
                table: "TenureWaitingPosition",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_TenureWaitingPosition_PostID",
                schema: "dbo",
                table: "TenureWaitingPosition",
                column: "PostID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenureWaitingPosition",
                schema: "dbo");
        }
    }
}
