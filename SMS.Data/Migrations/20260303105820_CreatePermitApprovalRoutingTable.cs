using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatePermitApprovalRoutingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PermitApprovalRouting",
                schema: "dbo",
                columns: table => new
                {
                    PermitApprovalRoutingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    RequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoutingGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermitApprovalRouting", x => x.PermitApprovalRoutingID);
                    table.ForeignKey(
                        name: "FK_PermitApprovalRouting_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermitApprovalRouting_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermitApprovalRouting_Request_RequestID",
                        column: x => x.RequestID,
                        principalSchema: "dbo",
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermitApprovalRouting_RoutingGroup_RoutingGroupId",
                        column: x => x.RoutingGroupId,
                        principalSchema: "dbo",
                        principalTable: "RoutingGroup",
                        principalColumn: "RoutingGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermitApprovalRouting_RoutingGroupId",
                schema: "dbo",
                table: "PermitApprovalRouting",
                column: "RoutingGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PermitApprovalRouting_CreatedUserID",
                schema: "dbo",
                table: "PermitApprovalRouting",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PermitApprovalRouting_EditedUserID",
                schema: "dbo",
                table: "PermitApprovalRouting",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PermitApprovalRouting_RequestID",
                schema: "dbo",
                table: "PermitApprovalRouting",
                column: "RequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermitApprovalRouting",
                schema: "dbo");
        }
    }
}
