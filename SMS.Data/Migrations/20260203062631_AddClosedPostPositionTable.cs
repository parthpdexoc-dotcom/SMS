using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClosedPostPositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClosedPostPosition",
                schema: "dbo",
                columns: table => new
                {
                    ClosedPostPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    WorkStartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    HoursWorked = table.Column<int>(type: "int", nullable: false),
                    WorkedOnDuty = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestedStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RequestedStartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    CloseBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CloseDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsMinimumDue = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MinimumHoursDue = table.Column<int>(type: "int", nullable: true),
                    Seniority = table.Column<bool>(type: "bit", nullable: true),
                    ReimburseStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PayRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReimbursePayRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Split = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuestionaireRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    QuestionaireResponseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosedPostPosition", x => x.ClosedPostPositionID);
                    table.ForeignKey(
                        name: "FK_ClosedPostPosition_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClosedPostPosition_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClosedPostPosition_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClosedPostPosition_PostPosition_PostPositionID",
                        column: x => x.PostPositionID,
                        principalSchema: "dbo",
                        principalTable: "PostPosition",
                        principalColumn: "PostPositionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClosedPostPosition_Post_PostID",
                        column: x => x.PostID,
                        principalSchema: "dbo",
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClosedPostPosition_Response_QuestionaireResponseID",
                        column: x => x.QuestionaireResponseID,
                        principalSchema: "dbo",
                        principalTable: "Response",
                        principalColumn: "ResponseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPosition_CreatedUserID",
                schema: "dbo",
                table: "ClosedPostPosition",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPosition_EditedUserID",
                schema: "dbo",
                table: "ClosedPostPosition",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPosition_EmployeeID",
                schema: "dbo",
                table: "ClosedPostPosition",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPosition_PostID",
                schema: "dbo",
                table: "ClosedPostPosition",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPosition_PostPositionID",
                schema: "dbo",
                table: "ClosedPostPosition",
                column: "PostPositionID");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPosition_QuestionaireResponseID",
                schema: "dbo",
                table: "ClosedPostPosition",
                column: "QuestionaireResponseID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClosedPostPosition",
                schema: "dbo");
        }
    }
}
