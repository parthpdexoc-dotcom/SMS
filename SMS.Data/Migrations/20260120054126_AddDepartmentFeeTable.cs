using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentFeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentFee",
                schema: "dbo",
                columns: table => new
                {
                    FeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeShort = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FeeLong = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FeeType = table.Column<int>(type: "int", nullable: false),
                    DefaultFee = table.Column<decimal>(type: "smallmoney", nullable: false),
                    AdditionalFee = table.Column<decimal>(type: "smallmoney", nullable: false),
                    ReimburseTo = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentFee", x => x.FeeId);
                    table.ForeignKey(
                        name: "FK_DepartmentFee_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentFee_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentFee_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentFee_CreatedUserID",
                schema: "dbo",
                table: "DepartmentFee",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentFee_DepartmentId",
                schema: "dbo",
                table: "DepartmentFee",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentFee_EditedUserID",
                schema: "dbo",
                table: "DepartmentFee",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentFee",
                schema: "dbo");
        }
    }
}
