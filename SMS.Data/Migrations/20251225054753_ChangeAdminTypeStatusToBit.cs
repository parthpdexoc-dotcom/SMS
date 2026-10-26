using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAdminTypeStatusToBit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             // 1️⃣ Force all existing rows to true
    migrationBuilder.Sql(@"
        UPDATE dbo.AdminType
        SET Status = 1
    ");

    // 2️⃣ Drop existing default constraint if any
    migrationBuilder.Sql(@"
        DECLARE @constraintName sysname;
        SELECT @constraintName = dc.name
        FROM sys.default_constraints dc
        INNER JOIN sys.columns c 
            ON dc.parent_object_id = c.object_id 
            AND dc.parent_column_id = c.column_id
        WHERE dc.parent_object_id = OBJECT_ID('dbo.AdminType')
          AND c.name = 'Status';

        IF @constraintName IS NOT NULL
            EXEC('ALTER TABLE dbo.AdminType DROP CONSTRAINT ' + @constraintName);
    ");

    // 3️⃣ Alter column (EF will add DEFAULT = 1 automatically)
    migrationBuilder.AlterColumn<bool>(
        name: "Status",
        table: "AdminType",
        type: "bit",
        nullable: false,
        defaultValue: true,
        oldClrType: typeof(string),
        oldType: "nvarchar(max)"
    );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.AlterColumn<string>(
        name: "Status",
        table: "AdminType",
        type: "nvarchar(max)",
        nullable: true,
        oldClrType: typeof(bool),
        oldType: "bit"
    );
        }
    }
}
