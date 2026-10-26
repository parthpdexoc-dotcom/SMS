using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipement",
                schema: "dbo",
                columns: table => new
                {
                    EquipementID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipementTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipementShort = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    EquipementLong = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    ChargedType = table.Column<int>(type: "int", nullable: true),
                    ChargeMileage = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    MileageCharge = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    IsReimburseable = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ReimburseTo = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    EquipementStatus = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipement", x => x.EquipementID);
                    table.ForeignKey(
                        name: "FK_Equipement_Admin_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Admin",
                        principalColumn: "AdminID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipement_Admin_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Admin",
                        principalColumn: "AdminID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipement_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipement_EquipmentType_EquipementTypeID",
                        column: x => x.EquipementTypeID,
                        principalSchema: "dbo",
                        principalTable: "EquipmentType",
                        principalColumn: "EquipementTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipement_CreatedUserID",
                schema: "dbo",
                table: "Equipement",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipement_DepartmentID",
                schema: "dbo",
                table: "Equipement",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipement_EditedUserID",
                schema: "dbo",
                table: "Equipement",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipement_EquipementTypeID",
                schema: "dbo",
                table: "Equipement",
                column: "EquipementTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipement",
                schema: "dbo");
        }
    }
}
