using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicalApprovalDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleApprovalDetail",
                schema: "dbo",
                columns: table => new
                {
                    VehicleApprovalDetailID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteToID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleApproverComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Approved = table.Column<bool>(type: "bit", nullable: false),
                    FinalApproval = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleApprovalDetail", x => x.VehicleApprovalDetailID);
                    table.ForeignKey(
                        name: "FK_VehicleApprovalDetail_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleApprovalDetail_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleApprovalDetail_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleApprovalDetail_Request_RequestID",
                        column: x => x.RequestID,
                        principalSchema: "dbo",
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleApprovalDetail_CreatedUserID",
                schema: "dbo",
                table: "VehicleApprovalDetail",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleApprovalDetail_DepartmentID",
                schema: "dbo",
                table: "VehicleApprovalDetail",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleApprovalDetail_EditedUserID",
                schema: "dbo",
                table: "VehicleApprovalDetail",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleApprovalDetail_RequestID",
                schema: "dbo",
                table: "VehicleApprovalDetail",
                column: "RequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleApprovalDetail",
                schema: "dbo");
        }
    }
}
