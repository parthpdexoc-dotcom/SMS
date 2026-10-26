using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostEquipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostEquipment",
                schema: "dbo",
                columns: table => new
                {
                    PostEquipmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentEquipmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Equipment_Per_Post = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_PostEquipment", x => x.PostEquipmentID);
                    table.ForeignKey(
                        name: "FK_PostEquipment_DepartmentEquipment_DepartmentEquipmentID",
                        column: x => x.DepartmentEquipmentID,
                        principalSchema: "dbo",
                        principalTable: "DepartmentEquipment",
                        principalColumn: "DepartmentEquipmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostEquipment_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostEquipment_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostEquipment_Post_PostID",
                        column: x => x.PostID,
                        principalSchema: "dbo",
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostEquipment_CreatedUserID",
                schema: "dbo",
                table: "PostEquipment",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PostEquipment_DepartmentEquipmentID",
                schema: "dbo",
                table: "PostEquipment",
                column: "DepartmentEquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PostEquipment_EditedUserID",
                schema: "dbo",
                table: "PostEquipment",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PostEquipment_PostID",
                schema: "dbo",
                table: "PostEquipment",
                column: "PostID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostEquipment",
                schema: "dbo");
        }
    }
}
