using Microsoft.EntityFrameworkCore.Migrations;
using SMS.Generic.Common.Helpers;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <summary>
    /// Ensures a bootstrap SuperUser row exists in dbo.Admin for new environments.
    /// Idempotent: skips when any non-deleted admin is already tied to AdminType &apos;SuperUser&apos;.
    /// Default login after insert: username <c>superuser</c>, password <c>SuperUser@123</c> (BCrypt-hashed).
    /// </summary>
    public partial class EnsureSuperUserAdminExists : Migration
    {
        private const string DefaultSuperUserPassword = "SuperUser@123";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var passwordHash = PasswordHelper.HashPassword(DefaultSuperUserPassword);

            migrationBuilder.Sql($@"
                DECLARE @now DATETIME2 = SYSUTCDATETIME();
                DECLARE @superTypeId UNIQUEIDENTIFIER;

                SELECT @superTypeId = [AdminTypeID]
                FROM [dbo].[AdminType]
                WHERE [AdminTypeShort] = N'SuperUser' AND [IsDeleted] = 0;

                IF @superTypeId IS NULL
                BEGIN
                    SET @superTypeId = NEWID();
                    INSERT INTO [dbo].[AdminType] (
                        [AdminTypeID], [AdminTypeShort], [AdminTypeLong], [Status],
                        [CreatedDate], [CreatedUserID], [CreatedBy], [EditedDate], [EditedUserID], [EditedBy],
                        [IsDeleted], [IsSuperUser], [IsManagePost], [IsChild], [IsSuperUserType])
                    VALUES (
                        @superTypeId, N'SuperUser', N'Stone Software Administrator', 1,
                        @now, NULL, NULL, @now, NULL, NULL,
                        0, 1, 0, 0, 0);
                END

                IF NOT EXISTS (
                    SELECT 1
                    FROM [dbo].[Admin] AS a
                    INNER JOIN [dbo].[AdminType] AS t ON a.[AdminTypeID] = t.[AdminTypeID]
                    WHERE t.[AdminTypeShort] = N'SuperUser'
                      AND a.[IsDeleted] = 0)
                BEGIN
                    DECLARE @adminId UNIQUEIDENTIFIER = NEWID();
                    INSERT INTO [dbo].[Admin] (
                        [AdminID], [AdminTypeID], [FirstName], [LastName], [UserName], [Password],
                        [Status], [IsDeleted],
                        [CreatedDate], [CreatedUserID], [CreatedBy], [EditedDate], [EditedUserID], [EditedBy],
                        [DeletedAt], [DeletedBy], [DeletedUserID])
                    VALUES (
                        @adminId, @superTypeId, N'Super', N'User', N'superuser', N'{passwordHash.Replace("'", "''")}',
                        1, 0,
                        @now, @adminId, N'System Migration', @now, @adminId, N'System Migration',
                        NULL, NULL, NULL);

                    UPDATE [dbo].[AdminType]
                    SET [CreatedUserID] = @adminId,
                        [CreatedBy] = N'Super User',
                        [EditedUserID] = @adminId,
                        [EditedBy] = N'Super User'
                    WHERE [AdminTypeID] = @superTypeId
                      AND [CreatedUserID] IS NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data-only migration: leave rows in place so Down does not drop production admins.
        }
    }
}
