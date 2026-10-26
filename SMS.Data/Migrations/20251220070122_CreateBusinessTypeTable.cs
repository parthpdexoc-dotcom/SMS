using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateBusinessTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessType",
                schema: "dbo",
                columns: table => new
                {
                    BusinessTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessTypeShort = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    BusinessTypeLong = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    BusinessTypeStatus = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessType", x => x.BusinessTypeID);
                    table.ForeignKey(
                        name: "FK_BusinessType_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessType_EmployeeAdmin_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "EmployeeAdmin",
                        principalColumn: "EmployeeAdminID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessType_EmployeeAdmin_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "EmployeeAdmin",
                        principalColumn: "EmployeeAdminID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessType_CreatedUserID",
                schema: "dbo",
                table: "BusinessType",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessType_DepartmentID",
                schema: "dbo",
                table: "BusinessType",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessType_EditedUserID",
                schema: "dbo",
                table: "BusinessType",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessType",
                schema: "dbo");
        }
    }
}
