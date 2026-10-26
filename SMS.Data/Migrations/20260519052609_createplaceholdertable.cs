using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class createplaceholdertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlaceholderID",
                schema: "dbo",
                table: "PostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Placeholder",
                schema: "dbo",
                columns: table => new
                {
                    PlaceholderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_Placeholder", x => x.PlaceholderID);
                    table.ForeignKey(
                        name: "FK_Placeholder_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Placeholder_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_PlaceholderID",
                schema: "dbo",
                table: "PostPosition",
                column: "PlaceholderID");

            migrationBuilder.CreateIndex(
                name: "IX_Placeholder_CreatedUserID",
                schema: "dbo",
                table: "Placeholder",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Placeholder_EditedUserID",
                schema: "dbo",
                table: "Placeholder",
                column: "EditedUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_PostPosition_Placeholder_PlaceholderID",
                schema: "dbo",
                table: "PostPosition",
                column: "PlaceholderID",
                principalSchema: "dbo",
                principalTable: "Placeholder",
                principalColumn: "PlaceholderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
    DECLARE @PlaceholderMap TABLE
    (
        PlaceholderID UNIQUEIDENTIFIER,
        PlaceholderName NVARCHAR(200)
    );

    MERGE dbo.Placeholder AS target
    USING
    (
        SELECT DISTINCT Placeholder
        FROM dbo.PostPosition
        WHERE IsPlaceholder = 1
          AND Placeholder IS NOT NULL
          AND LTRIM(RTRIM(Placeholder)) <> ''
    ) AS src
    ON 1 = 0

    WHEN NOT MATCHED THEN
        INSERT
        (
            PlaceholderID,
            FirstName,
            LastName,
            IsDeleted
        )
        VALUES
        (
            NEWID(),

            LTRIM(RTRIM(
                CASE
                    WHEN CHARINDEX(',', src.Placeholder) > 0
                    THEN SUBSTRING(
                        src.Placeholder,
                        CHARINDEX(',', src.Placeholder) + 1,
                        LEN(src.Placeholder)
                    )
                    ELSE src.Placeholder
                END
            )),

            LTRIM(RTRIM(
                CASE
                    WHEN CHARINDEX(',', src.Placeholder) > 0
                    THEN LEFT(src.Placeholder, CHARINDEX(',', src.Placeholder) - 1)
                    ELSE ''
                END
            )),

            0
        )

    OUTPUT inserted.PlaceholderID, src.Placeholder
    INTO @PlaceholderMap(PlaceholderID, PlaceholderName);

    UPDATE pp
    SET pp.PlaceholderID = map.PlaceholderID
    FROM dbo.PostPosition pp
    INNER JOIN @PlaceholderMap map
        ON pp.Placeholder = map.PlaceholderName
    WHERE pp.IsPlaceholder = 1;
");
            migrationBuilder.DropColumn(
               name: "Placeholder",
               schema: "dbo",
               table: "PostPosition");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostPosition_Placeholder_PlaceholderID",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropTable(
                name: "Placeholder",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_PostPosition_PlaceholderID",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "PlaceholderID",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.AddColumn<string>(
                name: "Placeholder",
                schema: "dbo",
                table: "PostPosition",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
