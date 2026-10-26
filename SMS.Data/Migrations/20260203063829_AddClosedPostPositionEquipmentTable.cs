using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClosedPostPositionEquipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClosedPostPositionEquipment",
                schema: "dbo",
                columns: table => new
                {
                    ClosedPostPositionEquipmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClosedPostPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentEquipmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkStartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    HoursWorked = table.Column<int>(type: "int", nullable: false),
                    IsWorked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NoWorkReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsUnPaidBreakTime = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BreakTime = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UsedEquipmentQuantity = table.Column<int>(type: "int", nullable: true),
                    EquipmentUsedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaidHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EquipmentUsedIsVehicle = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    VehicleOwnedBy = table.Column<int>(type: "int", nullable: true),
                    VehNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartMileage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EndMileage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalMileage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosedPostPositionEquipment", x => x.ClosedPostPositionEquipmentID);
                    table.ForeignKey(
                        name: "FK_ClosedPostPositionEquipment_ClosedPostPosition_ClosedPostPositionID",
                        column: x => x.ClosedPostPositionID,
                        principalSchema: "dbo",
                        principalTable: "ClosedPostPosition",
                        principalColumn: "ClosedPostPositionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClosedPostPositionEquipment_DepartmentEquipment_DepartmentEquipmentID",
                        column: x => x.DepartmentEquipmentID,
                        principalSchema: "dbo",
                        principalTable: "DepartmentEquipment",
                        principalColumn: "DepartmentEquipmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClosedPostPositionEquipment_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClosedPostPositionEquipment_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClosedPostPositionEquipment_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClosedPostPositionEquipment_Post_PostID",
                        column: x => x.PostID,
                        principalSchema: "dbo",
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPositionEquipment_ClosedPostPositionID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                column: "ClosedPostPositionID");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPositionEquipment_CreatedUserID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPositionEquipment_DepartmentEquipmentID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                column: "DepartmentEquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPositionEquipment_EditedUserID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPositionEquipment_EmployeeID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPositionEquipment_PostID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                column: "PostID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClosedPostPositionEquipment",
                schema: "dbo");
        }
    }
}
