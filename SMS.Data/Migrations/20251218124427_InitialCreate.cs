using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "EmployeeStatus",
                schema: "dbo",
                columns: table => new
                {
                    EmployeeStatusID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusShort = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StatusLong = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_EmployeeStatus", x => x.EmployeeStatusID);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesTypes",
                schema: "dbo",
                columns: table => new
                {
                    EmployeeTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeTypeShort = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmployeeTypeLong = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EmployeeTypeActive = table.Column<bool>(type: "bit", nullable: true),
                    RecordAddedDateTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddedByID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameOfUserAddingRecord = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserEditingID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserNameEditingRecord = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesTypes", x => x.EmployeeTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                schema: "dbo",
                columns: table => new
                {
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BadgeNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StatusID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    PrimaryEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SecondaryEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.EmployeeID);
                    table.ForeignKey(
                        name: "FK_Employee_EmployeeStatus_StatusID",
                        column: x => x.StatusID,
                        principalSchema: "dbo",
                        principalTable: "EmployeeStatus",
                        principalColumn: "EmployeeStatusID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_EmployeesTypes_EmployeeTypeID",
                        column: x => x.EmployeeTypeID,
                        principalSchema: "dbo",
                        principalTable: "EmployeesTypes",
                        principalColumn: "EmployeeTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                schema: "dbo",
                columns: table => new
                {
                    AdminID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.AdminID);
                    table.ForeignKey(
                        name: "FK_Admin_Admin_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Admin",
                        principalColumn: "AdminID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Admin_Admin_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Admin",
                        principalColumn: "AdminID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdminType",
                schema: "dbo",
                columns: table => new
                {
                    AdminTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminTypeShort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminTypeLong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminType", x => x.AdminTypeID);
                    table.ForeignKey(
                        name: "FK_AdminType_Admin_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Admin",
                        principalColumn: "AdminID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdminType_Admin_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Admin",
                        principalColumn: "AdminID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                schema: "dbo",
                columns: table => new
                {
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    City = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    State = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Zip = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    Domain = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DepartmentID);
                    table.ForeignKey(
                        name: "FK_Department_Admin_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Admin",
                        principalColumn: "AdminID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Department_Admin_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Admin",
                        principalColumn: "AdminID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAdmin",
                schema: "dbo",
                columns: table => new
                {
                    EmployeeAdminID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAdmin", x => x.EmployeeAdminID);
                    table.ForeignKey(
                        name: "FK_EmployeeAdmin_AdminType_AdminTypeID",
                        column: x => x.AdminTypeID,
                        principalSchema: "dbo",
                        principalTable: "AdminType",
                        principalColumn: "AdminTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAdmin_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAdmin_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAdmin_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_AdminTypeID",
                schema: "dbo",
                table: "Admin",
                column: "AdminTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Admin_CreatedUserID",
                schema: "dbo",
                table: "Admin",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Admin_EditedUserID",
                schema: "dbo",
                table: "Admin",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_AdminType_CreatedUserID",
                schema: "dbo",
                table: "AdminType",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_AdminType_EditedUserID",
                schema: "dbo",
                table: "AdminType",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Department_CreatedUserID",
                schema: "dbo",
                table: "Department",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Department_EditedUserID",
                schema: "dbo",
                table: "Department",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CreatedUserID",
                schema: "dbo",
                table: "Employee",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_EditedUserID",
                schema: "dbo",
                table: "Employee",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_EmployeeTypeID",
                schema: "dbo",
                table: "Employee",
                column: "EmployeeTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_StatusID",
                schema: "dbo",
                table: "Employee",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAdmin_AdminTypeID",
                schema: "dbo",
                table: "EmployeeAdmin",
                column: "AdminTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAdmin_CreatedUserID",
                schema: "dbo",
                table: "EmployeeAdmin",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAdmin_EditedUserID",
                schema: "dbo",
                table: "EmployeeAdmin",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAdmin_EmployeeID",
                schema: "dbo",
                table: "EmployeeAdmin",
                column: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Admin_AdminType_AdminTypeID",
                schema: "dbo",
                table: "Admin",
                column: "AdminTypeID",
                principalSchema: "dbo",
                principalTable: "AdminType",
                principalColumn: "AdminTypeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admin_AdminType_AdminTypeID",
                schema: "dbo",
                table: "Admin");

            migrationBuilder.DropTable(
                name: "Department",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EmployeeAdmin",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Employee",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EmployeeStatus",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EmployeesTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AdminType",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Admin",
                schema: "dbo");
        }
    }
}
