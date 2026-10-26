using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatePostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Post",
                schema: "dbo",
                columns: table => new
                {
                    // 🔹 PRIMARY KEY
                    PostID = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),

                    // 🔹 REQUIRED FK
                    DepartmentID = table.Column<Guid>(nullable: false),

                    // 🔹 OPTIONAL BUSINESS FKs
                    RequesterID = table.Column<Guid>(nullable: true),
                    RequestID = table.Column<Guid>(nullable: true),
                    PostParentID = table.Column<Guid>(nullable: true),

                    // 🔹 BUSINESS FIELDS
                    IsParent = table.Column<bool>(nullable: false, defaultValue: false),
                    NumberOfPosts = table.Column<int>(nullable: false),
                    RankEligible = table.Column<bool>(nullable: false, defaultValue: false),

                    PostTitle = table.Column<string>(type: "varchar(60)", nullable: false),
                    PostDescription = table.Column<string>(type: "varchar(60)", nullable: true),

                    PostDate = table.Column<DateOnly>(nullable: true),
                    PostStartTime = table.Column<TimeOnly>(nullable: true),

                    PostLength = table.Column<int>(nullable: true),
                    PostIsFlexible = table.Column<bool>(nullable: false, defaultValue: false),

                    PostPayRate = table.Column<decimal>(type: "money", nullable: true),
                    PostPremiumRate = table.Column<decimal>(type: "money", nullable: true),

                    PostRequiresVehicle = table.Column<bool>(nullable: false, defaultValue: false),
                    PostRequiresEquipment = table.Column<bool>(nullable: false, defaultValue: false),
                    IsSkillsRequired = table.Column<bool>(nullable: false, defaultValue: false),

                    PostNotes = table.Column<string>(type: "varchar(200)", nullable: true),
                    PostLocation = table.Column<string>(type: "varchar(200)", nullable: true),

                    IsPostRequiredDocument = table.Column<bool>(nullable: false, defaultValue: false),

                    // 🔹 DEPARTMENT-DEFINED VALUES
                    PostDepartmentDefinedValue1 = table.Column<string>(type: "varchar(200)", nullable: true),
                    PostDepartmentDefinedValue2 = table.Column<string>(type: "varchar(200)", nullable: true),
                    PostDepartmentDefinedValue3 = table.Column<string>(type: "varchar(200)", nullable: true),
                    PostDepartmentDefinedValue4 = table.Column<string>(type: "varchar(200)", nullable: true),
                    PostDepartmentDefinedValue5 = table.Column<string>(type: "varchar(200)", nullable: true),
                    PostDepartmentDefinedValue6 = table.Column<string>(type: "varchar(200)", nullable: true),
                    PostDepartmentDefinedValue7 = table.Column<string>(type: "varchar(200)", nullable: true),
                    PostDepartmentDefinedValue8 = table.Column<string>(type: "varchar(200)", nullable: true),
                    PostDepartmentDefinedValue9 = table.Column<string>(type: "varchar(200)", nullable: true),
                    PostDepartmentDefinedValue10 = table.Column<string>(type: "varchar(200)", nullable: true),

                    // 🔹 AUDIT FIELDS (FROM BaseEntity)
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserID = table.Column<Guid>(nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", nullable: true),

                    EditedDate = table.Column<DateTime>(nullable: true),
                    EditedUserID = table.Column<Guid>(nullable: true),
                    EditedBy = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostID);

                    // 🔹 Department
                    table.ForeignKey(
                        name: "FK_Post_Department",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);

                    // 🔹 Requester
                    table.ForeignKey(
                        name: "FK_Post_Requester",
                        column: x => x.RequesterID,
                        principalSchema: "dbo",
                        principalTable: "Requesters",
                        principalColumn: "RequesterID",
                        onDelete: ReferentialAction.Restrict);

                    // 🔹 Request
                    table.ForeignKey(
                        name: "FK_Post_Request",
                        column: x => x.RequestID,
                        principalSchema: "dbo",
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Restrict);

                    // 🔹 SELF-REFERENCING PARENT
                    table.ForeignKey(
                        name: "FK_Post_Parent",
                        column: x => x.PostParentID,
                        principalSchema: "dbo",
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Restrict);

                    // 🔹 AUDIT → EMPLOYEE
                    table.ForeignKey(
                        name: "FK_Post_CreatedByEmployee",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_Post_EditedByEmployee",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            // 🔹 INDEXES
            migrationBuilder.CreateIndex(
                name: "IX_Post_DepartmentID",
                schema: "dbo",
                table: "Post",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_RequesterID",
                schema: "dbo",
                table: "Post",
                column: "RequesterID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_RequestID",
                schema: "dbo",
                table: "Post",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostParentID",
                schema: "dbo",
                table: "Post",
                column: "PostParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_CreatedUserID",
                schema: "dbo",
                table: "Post",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_EditedUserID",
                schema: "dbo",
                table: "Post",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Post",
                schema: "dbo");
        }
    }
}
