using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateVehicleApprovalRoutingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleApprovalRouting",
                schema: "dbo",
                columns: table => new
                {
                    VehicleApprovalRouteID = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false,
                        defaultValueSql: "newid()"),

                    ApproverID = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false),

                    VehicleApproverParentRouteId = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: true),

                    DepartmentEquipmentID = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false),

                    VehicleApproverIsParent = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false),

                    VehicleApproverHasFinalAuthority = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false),

                    CreatedDate = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    CreatedUserID = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: true),

                    CreatedBy = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: true),

                    EditedDate = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    EditedUserID = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: true),

                    EditedBy = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_VehicleApprovalRouting",
                        x => x.VehicleApprovalRouteID);

                    table.ForeignKey(
                        name: "FK_VehicleApprovalRouting_RoutingGroup_ApproverID",
                        column: x => x.ApproverID,
                        principalSchema: "dbo",
                        principalTable: "RoutingGroup",
                        principalColumn: "ApproverID",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_VehicleApprovalRouting_VehicleApprovalRouting_ParentRoute",
                        column: x => x.VehicleApproverParentRouteId,
                        principalSchema: "dbo",
                        principalTable: "VehicleApprovalRouting",
                        principalColumn: "VehicleApprovalRouteID",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_VehicleApprovalRouting_DepartmentEquipment_DepartmentEquipmentID",
                        column: x => x.DepartmentEquipmentID,
                        principalSchema: "dbo",
                        principalTable: "DepartmentEquipment",
                        principalColumn: "DepartmentEquipmentID",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_VehicleApprovalRouting_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_VehicleApprovalRouting_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleApprovalRouting_ApproverID",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                column: "ApproverID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleApprovalRouting_VehicleApproverParentRouteId",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                column: "VehicleApproverParentRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleApprovalRouting_DepartmentEquipmentID",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                column: "DepartmentEquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleApprovalRouting_CreatedUserID",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleApprovalRouting_EditedUserID",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "VehicleApprovalRouting",
               schema: "dbo");
        }
    }
}
