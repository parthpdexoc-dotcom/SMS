using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeePostLocationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeePostLocation",
                schema: "dbo",
                columns: table => new
                {
                    EmployeePostLocationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    PostPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePostLocation", x => x.EmployeePostLocationID);
                    table.ForeignKey(
                        name: "FK_EmployeePostLocation_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeePostLocation_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeePostLocation_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeePostLocation_PostPosition_PostPositionID",
                        column: x => x.PostPositionID,
                        principalSchema: "dbo",
                        principalTable: "PostPosition",
                        principalColumn: "PostPositionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePostLocation_CreatedUserID",
                schema: "dbo",
                table: "EmployeePostLocation",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePostLocation_EditedUserID",
                schema: "dbo",
                table: "EmployeePostLocation",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePostLocation_EmployeeID",
                schema: "dbo",
                table: "EmployeePostLocation",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePostLocation_PostPositionID_CapturedAt",
                schema: "dbo",
                table: "EmployeePostLocation",
                columns: new[] { "PostPositionID", "CapturedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeePostLocation",
                schema: "dbo");
        }
    }
}
