using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestEquipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestEquipment",
                schema: "dbo",
                columns: table => new
                {
                    RequestEquipmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentEquipmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Equipment_Per_Request = table.Column<bool>(type: "bit", nullable: false),
                    Equipment_Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestEquipment", x => x.RequestEquipmentID);
                    table.ForeignKey(
                        name: "FK_RequestEquipment_DepartmentEquipment_DepartmentEquipmentID",
                        column: x => x.DepartmentEquipmentID,
                        principalSchema: "dbo",
                        principalTable: "DepartmentEquipment",
                        principalColumn: "DepartmentEquipmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestEquipment_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestEquipment_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestEquipment_Request_RequestID",
                        column: x => x.RequestID,
                        principalSchema: "dbo",
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestEquipment_CreatedUserID",
                schema: "dbo",
                table: "RequestEquipment",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestEquipment_DepartmentEquipmentID",
                schema: "dbo",
                table: "RequestEquipment",
                column: "DepartmentEquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestEquipment_EditedUserID",
                schema: "dbo",
                table: "RequestEquipment",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestEquipment_RequestID",
                schema: "dbo",
                table: "RequestEquipment",
                column: "RequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestEquipment",
                schema: "dbo");
        }
    }
}
