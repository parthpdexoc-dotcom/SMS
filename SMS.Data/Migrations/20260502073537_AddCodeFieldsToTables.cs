using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeFieldsToTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "Seq_EmployeeCode",
                schema: "dbo");

            migrationBuilder.CreateSequence<int>(
                name: "Seq_PostCode",
                schema: "dbo");

            migrationBuilder.CreateSequence<int>(
                name: "Seq_PostpositionCode",
                schema: "dbo");

            migrationBuilder.CreateSequence<int>(
                name: "Seq_RequestCode",
                schema: "dbo");

            migrationBuilder.CreateSequence<int>(
                name: "Seq_RequesterCode",
                schema: "dbo");

            migrationBuilder.AddColumn<string>(
                name: "RequesterCode",
                schema: "dbo",
                table: "Requester",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValueSql: "FORMAT(NEXT VALUE FOR [dbo].[Seq_RequesterCode], '000000')");

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                schema: "dbo",
                table: "Request",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValueSql: "FORMAT(NEXT VALUE FOR [dbo].[Seq_RequestCode], '000000')");

            migrationBuilder.AddColumn<string>(
                name: "PostpositionCode",
                schema: "dbo",
                table: "PostPosition",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValueSql: "FORMAT(NEXT VALUE FOR [dbo].[Seq_PostpositionCode], '000000')");

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValueSql: "FORMAT(NEXT VALUE FOR [dbo].[Seq_PostCode], '000000')");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                schema: "dbo",
                table: "Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValueSql: "FORMAT(NEXT VALUE FOR [dbo].[Seq_EmployeeCode], '000000')");

            // Backfill existing rows with zero-padded codes starting at 000001 (stable order by PK), then align sequences for new inserts.
            migrationBuilder.Sql(@"
            ;WITH cte AS (SELECT RequesterID, FORMAT(ROW_NUMBER() OVER (ORDER BY RequesterID), '000000') AS Code FROM dbo.Requester)
            UPDATE r SET r.RequesterCode = cte.Code FROM dbo.Requester r INNER JOIN cte ON r.RequesterID = cte.RequesterID;

            DECLARE @rRequester INT = (SELECT COALESCE(MAX(TRY_CAST(RequesterCode AS INT)), 0) + 1 FROM dbo.Requester);
            DECLARE @sqlSeqRequester nvarchar(400) = CONCAT(N'ALTER SEQUENCE dbo.Seq_RequesterCode RESTART WITH ', @rRequester);
            EXEC(@sqlSeqRequester);
            ");

            migrationBuilder.Sql(@"
            ;WITH cte AS (SELECT RequestID, FORMAT(ROW_NUMBER() OVER (ORDER BY RequestID), '000000') AS Code FROM dbo.Request)
            UPDATE q SET q.RequestCode = cte.Code FROM dbo.Request q INNER JOIN cte ON q.RequestID = cte.RequestID;

            DECLARE @rRequest INT = (SELECT COALESCE(MAX(TRY_CAST(RequestCode AS INT)), 0) + 1 FROM dbo.Request);
            DECLARE @sqlSeqRequest nvarchar(400) = CONCAT(N'ALTER SEQUENCE dbo.Seq_RequestCode RESTART WITH ', @rRequest);
            EXEC(@sqlSeqRequest);
            ");

            migrationBuilder.Sql(@"
            ;WITH cte AS (SELECT PostPositionID, FORMAT(ROW_NUMBER() OVER (ORDER BY PostPositionID), '000000') AS Code FROM dbo.PostPosition)
            UPDATE p SET p.PostpositionCode = cte.Code FROM dbo.PostPosition p INNER JOIN cte ON p.PostPositionID = cte.PostPositionID;

            DECLARE @rPostPosition INT = (SELECT COALESCE(MAX(TRY_CAST(PostpositionCode AS INT)), 0) + 1 FROM dbo.PostPosition);
            DECLARE @sqlSeqPostPosition nvarchar(400) = CONCAT(N'ALTER SEQUENCE dbo.Seq_PostpositionCode RESTART WITH ', @rPostPosition);
            EXEC(@sqlSeqPostPosition);
            ");

            migrationBuilder.Sql(@"
            ;WITH cte AS (SELECT PostID, FORMAT(ROW_NUMBER() OVER (ORDER BY PostID), '000000') AS Code FROM dbo.Post)
            UPDATE p SET p.PostCode = cte.Code FROM dbo.Post p INNER JOIN cte ON p.PostID = cte.PostID;

            DECLARE @rPost INT = (SELECT COALESCE(MAX(TRY_CAST(PostCode AS INT)), 0) + 1 FROM dbo.Post);
            DECLARE @sqlSeqPost nvarchar(400) = CONCAT(N'ALTER SEQUENCE dbo.Seq_PostCode RESTART WITH ', @rPost);
            EXEC(@sqlSeqPost);
            ");

            migrationBuilder.Sql(@"
            ;WITH cte AS (SELECT EmployeeID, FORMAT(ROW_NUMBER() OVER (ORDER BY EmployeeID), '000000') AS Code FROM dbo.Employee)
            UPDATE e SET e.EmployeeCode = cte.Code FROM dbo.Employee e INNER JOIN cte ON e.EmployeeID = cte.EmployeeID;

            DECLARE @rEmployee INT = (SELECT COALESCE(MAX(TRY_CAST(EmployeeCode AS INT)), 0) + 1 FROM dbo.Employee);
            DECLARE @sqlSeqEmployee nvarchar(400) = CONCAT(N'ALTER SEQUENCE dbo.Seq_EmployeeCode RESTART WITH ', @rEmployee);
            EXEC(@sqlSeqEmployee);
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Requester_RequesterCode",
                schema: "dbo",
                table: "Requester",
                column: "RequesterCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestCode",
                schema: "dbo",
                table: "Request",
                column: "RequestCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_PostpositionCode",
                schema: "dbo",
                table: "PostPosition",
                column: "PostpositionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostCode",
                schema: "dbo",
                table: "Post",
                column: "PostCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_EmployeeCode",
                schema: "dbo",
                table: "Employee",
                column: "EmployeeCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requester_RequesterCode",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropIndex(
                name: "IX_Request_RequestCode",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_PostPosition_PostpositionCode",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropIndex(
                name: "IX_Post_PostCode",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Employee_EmployeeCode",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "RequesterCode",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "PostpositionCode",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "PostCode",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropSequence(
                name: "Seq_EmployeeCode",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "Seq_PostCode",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "Seq_PostpositionCode",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "Seq_RequestCode",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "Seq_RequesterCode",
                schema: "dbo");
        }
    }
}
