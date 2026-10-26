using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateRequestApprovalRouting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestApprovalRouting",
                schema: "dbo",
                columns: table => new
                {
                    RequestApprovalRouteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    ApproverID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentEquipmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    RequestApproverHasFinalAuthority = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestApprovalRouting", x => x.RequestApprovalRouteID);
                    table.ForeignKey(
                        name: "FK_RequestApprovalRouting_DepartmentEquipment_DepartmentEquipmentID",
                        column: x => x.DepartmentEquipmentID,
                        principalSchema: "dbo",
                        principalTable: "DepartmentEquipment",
                        principalColumn: "DepartmentEquipmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestApprovalRouting_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestApprovalRouting_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestApprovalRouting_RoutingGroup_ApproverID",
                        column: x => x.ApproverID,
                        principalSchema: "dbo",
                        principalTable: "RoutingGroup",
                        principalColumn: "ApproverID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestApprovalRouting_ApproverID",
                schema: "dbo",
                table: "RequestApprovalRouting",
                column: "ApproverID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestApprovalRouting_CreatedUserID",
                schema: "dbo",
                table: "RequestApprovalRouting",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestApprovalRouting_DepartmentEquipmentID_Rank",
                schema: "dbo",
                table: "RequestApprovalRouting",
                columns: new[] { "DepartmentEquipmentID", "Rank" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestApprovalRouting_EditedUserID",
                schema: "dbo",
                table: "RequestApprovalRouting",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestApprovalRouting",
                schema: "dbo");
        }
    }
}
