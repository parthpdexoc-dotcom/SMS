using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedRequestdepartmentfeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnableFees",
                schema: "dbo",
                table: "Request",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EffectiveEndDate",
                schema: "dbo",
                table: "DepartmentFee",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EffectiveStartDate",
                schema: "dbo",
                table: "DepartmentFee",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                schema: "dbo",
                table: "DepartmentFee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RequestDepartmentFee",
                schema: "dbo",
                columns: table => new
                {
                    RequestDepartmentFeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    RequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentFeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestDepartmentFee", x => x.RequestDepartmentFeeID);
                    table.ForeignKey(
                        name: "FK_RequestDepartmentFee_DepartmentFee_DepartmentFeeID",
                        column: x => x.DepartmentFeeID,
                        principalSchema: "dbo",
                        principalTable: "DepartmentFee",
                        principalColumn: "FeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestDepartmentFee_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestDepartmentFee_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestDepartmentFee_Request_RequestID",
                        column: x => x.RequestID,
                        principalSchema: "dbo",
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestDepartmentFee_CreatedUserID",
                schema: "dbo",
                table: "RequestDepartmentFee",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestDepartmentFee_DepartmentFeeID",
                schema: "dbo",
                table: "RequestDepartmentFee",
                column: "DepartmentFeeID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestDepartmentFee_EditedUserID",
                schema: "dbo",
                table: "RequestDepartmentFee",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestDepartmentFee_RequestID",
                schema: "dbo",
                table: "RequestDepartmentFee",
                column: "RequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestDepartmentFee",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "EnableFees",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "EffectiveEndDate",
                schema: "dbo",
                table: "DepartmentFee");

            migrationBuilder.DropColumn(
                name: "EffectiveStartDate",
                schema: "dbo",
                table: "DepartmentFee");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                schema: "dbo",
                table: "DepartmentFee");
        }
    }
}
