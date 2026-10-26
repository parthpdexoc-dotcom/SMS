using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatePostPositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "PostPosition",
               schema: "dbo",
               columns: table => new
               {
                   // 🔹 PRIMARY KEY
                   PostPositionID = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),

                   // 🔹 FOREIGN KEYS
                   DepartmentID = table.Column<Guid>(nullable: false),
                   PostID = table.Column<Guid>(nullable: false),
                   EmployeeID = table.Column<Guid>(nullable: false),

                   // 🔹 BUSINESS FIELDS
                   DateOfWork = table.Column<DateOnly>(nullable: true),
                   StartTime = table.Column<TimeOnly>(nullable: true),
                   LengthOfWork = table.Column<string>(type: "varchar(50)", nullable: true),
                   EquipmentRequired = table.Column<bool>(nullable: false, defaultValue: false),
                   CallSign = table.Column<string>(type: "varchar(50)", nullable: true),

                   // 🔹 DEPARTMENT DEFINED FIELDS
                   PostDepartmentDefinedFieldValue1 = table.Column<string>(type: "varchar(200)", nullable: true),
                   PostDepartmentDefinedFieldValue2 = table.Column<string>(type: "varchar(200)", nullable: true),
                   PostDepartmentDefinedFieldValue3 = table.Column<string>(type: "varchar(200)", nullable: true),
                   PostDepartmentDefinedFieldValue4 = table.Column<string>(type: "varchar(200)", nullable: true),
                   PostDepartmentDefinedFieldValue5 = table.Column<string>(type: "varchar(200)", nullable: true),
                   PostDepartmentDefinedFieldValue6 = table.Column<string>(type: "varchar(200)", nullable: true),
                   PostDepartmentDefinedFieldValue7 = table.Column<string>(type: "varchar(200)", nullable: true),
                   PostDepartmentDefinedFieldValue8 = table.Column<string>(type: "varchar(200)", nullable: true),
                   PostDepartmentDefinedFieldValue9 = table.Column<string>(type: "varchar(200)", nullable: true),
                   PostDepartmentDefinedFieldValue10 = table.Column<string>(type: "varchar(200)", nullable: true),

                   // 🔹 AUDIT FIELDS (BaseEntity)
                   CreatedDate = table.Column<DateTime>(nullable: true),
                   CreatedUserID = table.Column<Guid>(nullable: true),
                   CreatedBy = table.Column<string>(type: "varchar(255)", nullable: true),

                   EditedDate = table.Column<DateTime>(nullable: true),
                   EditedUserID = table.Column<Guid>(nullable: true),
                   EditedBy = table.Column<string>(type: "varchar(255)", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_PostPosition", x => x.PostPositionID);

                   // 🔹 FK → Department
                   table.ForeignKey(
                       name: "FK_PostPosition_Department",
                       column: x => x.DepartmentID,
                       principalSchema: "dbo",
                       principalTable: "Department",
                       principalColumn: "DepartmentID",
                       onDelete: ReferentialAction.Restrict);

                   // 🔹 FK → Post
                   table.ForeignKey(
                       name: "FK_PostPosition_Post",
                       column: x => x.PostID,
                       principalSchema: "dbo",
                       principalTable: "Post",
                       principalColumn: "PostID",
                       onDelete: ReferentialAction.Restrict);

                   // 🔹 FK → Employee
                   table.ForeignKey(
                       name: "FK_PostPosition_Employee",
                       column: x => x.EmployeeID,
                       principalSchema: "dbo",
                       principalTable: "Employee",
                       principalColumn: "EmployeeID",
                       onDelete: ReferentialAction.Restrict);

                   // 🔹 AUDIT → Employee
                   table.ForeignKey(
                       name: "FK_PostPosition_CreatedByEmployee",
                       column: x => x.CreatedUserID,
                       principalSchema: "dbo",
                       principalTable: "Employee",
                       principalColumn: "EmployeeID",
                       onDelete: ReferentialAction.Restrict);

                   table.ForeignKey(
                       name: "FK_PostPosition_EditedByEmployee",
                       column: x => x.EditedUserID,
                       principalSchema: "dbo",
                       principalTable: "Employee",
                       principalColumn: "EmployeeID",
                       onDelete: ReferentialAction.Restrict);
               });

            // 🔹 INDEXES (IMPORTANT FOR PERFORMANCE)
            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_DepartmentID",
                schema: "dbo",
                table: "PostPosition",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_PostID",
                schema: "dbo",
                table: "PostPosition",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_EmployeeID",
                schema: "dbo",
                table: "PostPosition",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_CreatedUserID",
                schema: "dbo",
                table: "PostPosition",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_EditedUserID",
                schema: "dbo",
                table: "PostPosition",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropTable(
                name: "PostPosition",
                schema: "dbo");
        }
    }
}
