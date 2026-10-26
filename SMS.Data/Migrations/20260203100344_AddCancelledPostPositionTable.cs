using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelledPostPositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CancelledPostPosition",
                schema: "dbo",
                columns: table => new
                {
                    CancelledPostPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentEquipmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkedHours = table.Column<int>(type: "int", nullable: true),
                    EquipmentUsedHours = table.Column<int>(type: "int", nullable: true),
                    Mileage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DivisionID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkedOnDuty = table.Column<int>(type: "int", nullable: true),
                    Seniority = table.Column<bool>(type: "bit", nullable: true),
                    ReimburseStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PayRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReimbursePayRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    QuestionaireRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    QuestionaireResponseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Compensation = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CancelledBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CancelledDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IPAddressCancelledBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: true),
                    RequiredHours = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelledPostPosition", x => x.CancelledPostPositionID);
                    table.ForeignKey(
                        name: "FK_CancelledPostPosition_DepartmentEquipment_DepartmentEquipmentID",
                        column: x => x.DepartmentEquipmentID,
                        principalSchema: "dbo",
                        principalTable: "DepartmentEquipment",
                        principalColumn: "DepartmentEquipmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelledPostPosition_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelledPostPosition_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelledPostPosition_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelledPostPosition_PostPosition_PostPositionID",
                        column: x => x.PostPositionID,
                        principalSchema: "dbo",
                        principalTable: "PostPosition",
                        principalColumn: "PostPositionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelledPostPosition_Post_PostID",
                        column: x => x.PostID,
                        principalSchema: "dbo",
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelledPostPosition_Response_QuestionaireResponseID",
                        column: x => x.QuestionaireResponseID,
                        principalSchema: "dbo",
                        principalTable: "Response",
                        principalColumn: "ResponseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CancelledPostPosition_CreatedUserID",
                schema: "dbo",
                table: "CancelledPostPosition",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_CancelledPostPosition_DepartmentEquipmentID",
                schema: "dbo",
                table: "CancelledPostPosition",
                column: "DepartmentEquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_CancelledPostPosition_EditedUserID",
                schema: "dbo",
                table: "CancelledPostPosition",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_CancelledPostPosition_EmployeeID",
                schema: "dbo",
                table: "CancelledPostPosition",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_CancelledPostPosition_PostID",
                schema: "dbo",
                table: "CancelledPostPosition",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_CancelledPostPosition_PostPositionID",
                schema: "dbo",
                table: "CancelledPostPosition",
                column: "PostPositionID");

            migrationBuilder.CreateIndex(
                name: "IX_CancelledPostPosition_QuestionaireResponseID",
                schema: "dbo",
                table: "CancelledPostPosition",
                column: "QuestionaireResponseID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CancelledPostPosition",
                schema: "dbo");
        }
    }
}
