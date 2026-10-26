using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations.Audit
{
    /// <inheritdoc />
    public partial class UpdatePostWorkflowAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "PostWorkflow_Audit");

            migrationBuilder.RenameColumn(
                name: "OccurredOn",
                schema: "dbo",
                table: "PostWorkflow_Audit",
                newName: "RecordedDate");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                schema: "dbo",
                table: "PostWorkflow_Audit",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostTitle",
                schema: "dbo",
                table: "PostWorkflow_Audit",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestTitle",
                schema: "dbo",
                table: "PostWorkflow_Audit",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeName",
                schema: "dbo",
                table: "PostWorkflow_Audit");

            migrationBuilder.DropColumn(
                name: "PostTitle",
                schema: "dbo",
                table: "PostWorkflow_Audit");

            migrationBuilder.DropColumn(
                name: "RequestTitle",
                schema: "dbo",
                table: "PostWorkflow_Audit");

            migrationBuilder.RenameColumn(
                name: "RecordedDate",
                schema: "dbo",
                table: "PostWorkflow_Audit",
                newName: "OccurredOn");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "dbo",
                table: "PostWorkflow_Audit",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}
