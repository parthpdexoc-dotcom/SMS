using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequesterAuthorizedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LiquorLicenseNumber",
                schema: "dbo",
                table: "Request",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermitNumber",
                schema: "dbo",
                table: "Request",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReleaseDate",
                schema: "dbo",
                table: "Request",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReleaseStatus",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ReleaseTime",
                schema: "dbo",
                table: "Request",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeTypeIDs",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                schema: "dbo",
                table: "DepartmentEquipment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RequesterAuthorized",
                schema: "dbo",
                columns: table => new
                {
                    AuthorizedRequesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    RequesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    PrimaryPhone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    PPIsCell = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    SecondaryPhone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    SPIsCellPhone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    PrimaryEmail = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    SecondaryEmail = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequesterAuthorized", x => x.AuthorizedRequesterID);
                    table.ForeignKey(
                        name: "FK_RequesterAuthorized_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequesterAuthorized_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequesterAuthorized_Requester_RequesterID",
                        column: x => x.RequesterID,
                        principalSchema: "dbo",
                        principalTable: "Requester",
                        principalColumn: "RequesterID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequesterAuthorized_CreatedUserID",
                schema: "dbo",
                table: "RequesterAuthorized",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterAuthorized_EditedUserID",
                schema: "dbo",
                table: "RequesterAuthorized",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterAuthorized_RequesterID",
                schema: "dbo",
                table: "RequesterAuthorized",
                column: "RequesterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequesterAuthorized",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "LiquorLicenseNumber",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "PermitNumber",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ReleaseStatus",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ReleaseTime",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "EmployeeTypeIDs",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                schema: "dbo",
                table: "DepartmentEquipment");
        }
    }
}
