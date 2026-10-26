using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Request",
                schema: "dbo",
                columns: table => new
                {
                    RequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    RequesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsTenureBasedRequest = table.Column<bool>(type: "bit", nullable: true),
                    RequestHasBeenUpdated = table.Column<bool>(type: "bit", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CellPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AlternateCellPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrimaryEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SecondaryEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsPostContact = table.Column<bool>(type: "bit", nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostLocationsAreStationary = table.Column<bool>(type: "bit", nullable: true),
                    RequiresPermit = table.Column<bool>(type: "bit", nullable: true),
                    IsSpecialEvent = table.Column<bool>(type: "bit", nullable: true),
                    SpecialEventApproved = table.Column<bool>(type: "bit", nullable: true),
                    RequiresLiquorLicense = table.Column<bool>(type: "bit", nullable: true),
                    RequiresVehicle = table.Column<bool>(type: "bit", nullable: true),
                    RequiresVehicleAtEveryPost = table.Column<bool>(type: "bit", nullable: true),
                    NumberOfVehicles = table.Column<int>(type: "int", nullable: true),
                    VehicleApproved = table.Column<bool>(type: "bit", nullable: true),
                    RequiresEquipment = table.Column<bool>(type: "bit", nullable: true),
                    RequiresEquipmentAtEachPost = table.Column<bool>(type: "bit", nullable: true),
                    NumberOfEquipment = table.Column<int>(type: "int", nullable: true),
                    IsSkillRequired = table.Column<bool>(type: "bit", nullable: true),
                    IsChild = table.Column<bool>(type: "bit", nullable: true),
                    DepartmentDefinedFieldValue1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentDefinedFieldValue2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentDefinedFieldValue3 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentDefinedFieldValue4 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentDefinedFieldValue5 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentDefinedFieldValue6 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentDefinedFieldValue7 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentDefinedFieldValue8 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentDefinedFieldValue9 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentDefinedFieldValue10 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_Request_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_Request_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_Request_Request_RelatedRequestID",
                        column: x => x.RelatedRequestID,
                        principalSchema: "dbo",
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_Requesters_RequesterID",
                        column: x => x.RequesterID,
                        principalSchema: "dbo",
                        principalTable: "Requesters",
                        principalColumn: "RequesterID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_CreatedUserID",
                schema: "dbo",
                table: "Request",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Request_EditedUserID",
                schema: "dbo",
                table: "Request",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RelatedRequestID",
                schema: "dbo",
                table: "Request",
                column: "RelatedRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequesterID",
                schema: "dbo",
                table: "Request",
                column: "RequesterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request",
                schema: "dbo");
        }
    }
}
