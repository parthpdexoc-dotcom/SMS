using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationHistoryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationHistory",
                schema: "dbo",
                columns: table => new
                {
                    NotificationHistoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PostID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PostPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContextType = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FailureMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_NotificationHistory", x => x.NotificationHistoryID);
                });

            migrationBuilder.CreateTable(
                name: "NotificationHistoryAttachment",
                schema: "dbo",
                columns: table => new
                {
                    NotificationHistoryAttachmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationHistoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileUploadedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationHistoryAttachment", x => x.NotificationHistoryAttachmentID);
                    table.ForeignKey(
                        name: "FK_NotificationHistoryAttachment_NotificationHistory_NotificationHistoryID",
                        column: x => x.NotificationHistoryID,
                        principalSchema: "dbo",
                        principalTable: "NotificationHistory",
                        principalColumn: "NotificationHistoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationHistoryRecipient",
                schema: "dbo",
                columns: table => new
                {
                    NotificationHistoryRecipientID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationHistoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientType = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationHistoryRecipient", x => x.NotificationHistoryRecipientID);
                    table.ForeignKey(
                        name: "FK_NotificationHistoryRecipient_NotificationHistory_NotificationHistoryID",
                        column: x => x.NotificationHistoryID,
                        principalSchema: "dbo",
                        principalTable: "NotificationHistory",
                        principalColumn: "NotificationHistoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_DepartmentID",
                schema: "dbo",
                table: "NotificationHistory",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_PostID",
                schema: "dbo",
                table: "NotificationHistory",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_PostPositionID",
                schema: "dbo",
                table: "NotificationHistory",
                column: "PostPositionID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_RequesterID",
                schema: "dbo",
                table: "NotificationHistory",
                column: "RequesterID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_RequestID",
                schema: "dbo",
                table: "NotificationHistory",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistory_SentAt",
                schema: "dbo",
                table: "NotificationHistory",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistoryAttachment_NotificationHistoryID",
                schema: "dbo",
                table: "NotificationHistoryAttachment",
                column: "NotificationHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHistoryRecipient_NotificationHistoryID",
                schema: "dbo",
                table: "NotificationHistoryRecipient",
                column: "NotificationHistoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationHistoryAttachment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NotificationHistoryRecipient",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NotificationHistory",
                schema: "dbo");
        }
    }
}
