using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentEquipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentEquipment",
                schema: "dbo",
                columns: table => new
                {
                    DepartmentEquipmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipmentTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChargedType = table.Column<int>(type: "int", nullable: true),
                    Charge = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    ChargeMileage = table.Column<bool>(type: "bit", nullable: true),
                    MileageCharge = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    IsReimburseable = table.Column<bool>(type: "bit", nullable: true),
                    ReimburseTo = table.Column<int>(type: "int", nullable: true),
                    MileageCondition = table.Column<int>(type: "int", nullable: true),
                    EquipmentStatus = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: true),
                    RequiresUtilizationRecorded = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentEquipment", x => x.DepartmentEquipmentID);
                    table.ForeignKey(
                        name: "FK_DepartmentEquipment_Admin_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Admin",
                        principalColumn: "AdminID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentEquipment_Admin_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Admin",
                        principalColumn: "AdminID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentEquipment_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentEquipment_EquipmentType_EquipmentTypeID",
                        column: x => x.EquipmentTypeID,
                        principalSchema: "dbo",
                        principalTable: "EquipmentType",
                        principalColumn: "EquipementTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentEquipment_CreatedUserID",
                schema: "dbo",
                table: "DepartmentEquipment",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentEquipment_DepartmentID",
                schema: "dbo",
                table: "DepartmentEquipment",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentEquipment_EditedUserID",
                schema: "dbo",
                table: "DepartmentEquipment",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentEquipment_EquipmentTypeID",
                schema: "dbo",
                table: "DepartmentEquipment",
                column: "EquipmentTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentEquipment",
                schema: "dbo");
        }
    }
}
