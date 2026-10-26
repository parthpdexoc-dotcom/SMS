using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostPositionVehicleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostPositionVehicle",
                schema: "dbo",
                columns: table => new
                {
                    PostPositionVehicleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentEquipmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargedType = table.Column<int>(type: "int", nullable: true),
                    Charge = table.Column<decimal>(type: "money", nullable: true),
                    ChargeMileage = table.Column<bool>(type: "bit", nullable: true),
                    MileageCharge = table.Column<decimal>(type: "money", nullable: true),
                    MileageCondition = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_PostPositionVehicle", x => x.PostPositionVehicleID);
                    table.ForeignKey(
                        name: "FK_PostPositionVehicle_DepartmentEquipment_DepartmentEquipmentID",
                        column: x => x.DepartmentEquipmentID,
                        principalSchema: "dbo",
                        principalTable: "DepartmentEquipment",
                        principalColumn: "DepartmentEquipmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostPositionVehicle_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostPositionVehicle_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostPositionVehicle_PostPosition_PostPositionID",
                        column: x => x.PostPositionID,
                        principalSchema: "dbo",
                        principalTable: "PostPosition",
                        principalColumn: "PostPositionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostPositionVehicle_CreatedUserID",
                schema: "dbo",
                table: "PostPositionVehicle",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PostPositionVehicle_DepartmentEquipmentID",
                schema: "dbo",
                table: "PostPositionVehicle",
                column: "DepartmentEquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PostPositionVehicle_EditedUserID",
                schema: "dbo",
                table: "PostPositionVehicle",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PostPositionVehicle_PostPositionID",
                schema: "dbo",
                table: "PostPositionVehicle",
                column: "PostPositionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostPositionVehicle",
                schema: "dbo");
        }
    }
}
