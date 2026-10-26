using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipMentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentType",
                schema: "dbo",
                columns: table => new
                {
                    EquipementTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipementTypeShort = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EquipementTypeLong = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsVehicle = table.Column<bool>(type: "bit", nullable: true),
                    EquipementTypeStatus = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentType", x => x.EquipementTypeID);
                    table.ForeignKey(
                        name: "FK_EquipmentType_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentType_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_EquipmentType_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                schema: "dbo",
                columns: table => new
                {
                    SkillID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillShort = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    SkillLong = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    SkillStatus = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserEditingID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.SkillID);
                    table.ForeignKey(
                        name: "FK_Skill_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_Employee_UserEditingID",
                        column: x => x.UserEditingID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentType_CreatedUserID",
                schema: "dbo",
                table: "EquipmentType",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentType_DepartmentID",
                schema: "dbo",
                table: "EquipmentType",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentType_EditedUserID",
                schema: "dbo",
                table: "EquipmentType",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_CreatedUserID",
                schema: "dbo",
                table: "Skill",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_DepartmentID",
                schema: "dbo",
                table: "Skill",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_UserEditingID",
                schema: "dbo",
                table: "Skill",
                column: "UserEditingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentType",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Skill",
                schema: "dbo");
        }
    }
}
