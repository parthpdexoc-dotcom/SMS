using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockDatesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockDate",
                schema: "dbo",
                columns: table => new
                {
                    BlockDateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PostID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockDate", x => x.BlockDateId);
                    table.ForeignKey(
                        name: "FK_BlockDate_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlockDate_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlockDate_Post_PostID",
                        column: x => x.PostID,
                        principalSchema: "dbo",
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlockDate_Request_RequestID",
                        column: x => x.RequestID,
                        principalSchema: "dbo",
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockDate_CreatedUserID",
                schema: "dbo",
                table: "BlockDate",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_BlockDate_EditedUserID",
                schema: "dbo",
                table: "BlockDate",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_BlockDate_PostID",
                schema: "dbo",
                table: "BlockDate",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_BlockDate_RequestID",
                schema: "dbo",
                table: "BlockDate",
                column: "RequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockDate",
                schema: "dbo");
        }
    }
}
