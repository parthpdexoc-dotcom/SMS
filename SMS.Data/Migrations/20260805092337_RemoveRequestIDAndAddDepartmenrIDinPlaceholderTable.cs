using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRequestIDAndAddDepartmenrIDinPlaceholderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Placeholder_Request_RequestID",
                schema: "dbo",
                table: "Placeholder");

            //Rename RequestID -> DepartmentID
            migrationBuilder.RenameColumn(
               name: "RequestID",
               schema: "dbo",
               table: "Placeholder",
               newName: "DepartmentID");

            migrationBuilder.RenameIndex(
               name: "IX_Placeholder_RequestID",
               schema: "dbo",
               table: "Placeholder",
               newName: "IX_Placeholder_DepartmentID");

            // Convert stored RequestID values to DepartmentID (column still named RequestID).
            migrationBuilder.Sql(@"
                UPDATE ph
                SET ph.[DepartmentID] = r.[DepartmentID]
                FROM [dbo].[Placeholder] ph
                INNER JOIN [dbo].[Request] r 
                    ON r.[RequestID] = ph.[DepartmentID]
                WHERE r.[DepartmentID] IS NOT NULL;
            ");

            // Fallback for rows that could not resolve via Request (e.g. missing/invalid RequestID):
            // PostPosition → Post → Request.DepartmentID
            migrationBuilder.Sql(@"
                UPDATE ph
                SET ph.[DepartmentID] = src.[DepartmentID]
                FROM [dbo].[Placeholder] ph
                INNER JOIN (
                    SELECT pp.[PlaceholderID], MIN(r.[DepartmentID]) AS [DepartmentID]
                    FROM [dbo].[PostPosition] pp
                    INNER JOIN [dbo].[Post] p 
                        ON p.[PostID] = pp.[PostID]
                    INNER JOIN [dbo].[Request] r 
                        ON r.[RequestID] = p.[RequestID]
                    WHERE pp.[PlaceholderID] IS NOT NULL
                      AND r.[DepartmentID] IS NOT NULL
                    GROUP BY pp.[PlaceholderID]
                ) src 
                    ON src.[PlaceholderID] = ph.[PlaceholderID]
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM [dbo].[Department] d
                    WHERE d.[DepartmentID] = ph.[DepartmentID]
                );
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_Placeholder_Department_DepartmentID",
                schema: "dbo",
                table: "Placeholder",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Placeholder_Department_DepartmentID",
                schema: "dbo",
                table: "Placeholder");

            migrationBuilder.RenameColumn(
             name: "DepartmentID",
             schema: "dbo",
             table: "Placeholder",
             newName: "RequestID");

            migrationBuilder.RenameIndex(
                name: "IX_Placeholder_DepartmentID",
                schema: "dbo",
                table: "Placeholder",
                newName: "IX_Placeholder_RequestID");

            // Restore RequestID from linked PostPosition → Post (best-effort; original RequestID is not recoverable from DepartmentID alone).
            migrationBuilder.Sql(@"
                UPDATE ph
                SET ph.[RequestID] = src.[RequestID]
                FROM [dbo].[Placeholder] ph
                INNER JOIN (
                    SELECT pp.[PlaceholderID], MIN(p.[RequestID]) AS [RequestID]
                    FROM [dbo].[PostPosition] pp
                    INNER JOIN [dbo].[Post] p 
                        ON p.[PostID] = pp.[PostID]
                    WHERE pp.[PlaceholderID] IS NOT NULL
                      AND p.[RequestID] IS NOT NULL
                    GROUP BY pp.[PlaceholderID]
                ) src 
                    ON src.[PlaceholderID] = ph.[PlaceholderID];
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_Placeholder_Request_RequestID",
                schema: "dbo",
                table: "Placeholder",
                column: "RequestID",
                principalSchema: "dbo",
                principalTable: "Request",
                principalColumn: "RequestID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
