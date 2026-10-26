using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddArchivedReportPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. PostPosition: Covering Non-Clustered Index for Status, DateOfWork, and Placeholder filtering
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PostPosition_ArchivedReport_Perf' AND object_id = OBJECT_ID('dbo.PostPosition'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_PostPosition_ArchivedReport_Perf]
                    ON [dbo].[PostPosition] (
                        [PostPositionStatus],
                        [DateOfWork],
                        [IsPlaceholder]
                    )
                    INCLUDE (
                        [PostPositionID],
                        [PostID],
                        [EmployeeID],
                        [PlaceholderID],
                        [PostCallSignID],
                        [LengthOfWork],
                        [StartTime],
                        [CallSign],
                        [EquipmentRequired],
                        [IsDeleted]
                    );
                END
            ");

            // 2. ClosedPostPosition: Fast position-to-close resolution ordered by CloseDateTime DESC
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ClosedPostPosition_PostPositionID_CloseDate' AND object_id = OBJECT_ID('dbo.ClosedPostPosition'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_ClosedPostPosition_PostPositionID_CloseDate]
                    ON [dbo].[ClosedPostPosition] (
                        [PostPositionID],
                        [CloseDateTime] DESC
                    )
                    INCLUDE (
                        [ClosedPostPositionID],
                        [HoursWorked],
                        [Comment],
                        [CloseBy],
                        [WorkStartDate],
                        [WorkStartTime],
                        [WorkedOnDuty],
                        [Status]
                    );
                END
            ");

            // 3. InvoiceDetails: Fast index seek for correlated subquery (ClosePostPositionID + IsDeleted)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_InvoiceDetails_ClosePostPositionID_IsDeleted' AND object_id = OBJECT_ID('dbo.InvoiceDetails'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_InvoiceDetails_ClosePostPositionID_IsDeleted]
                    ON [dbo].[InvoiceDetails] (
                        [ClosePostPositionID],
                        [IsDeleted]
                    );
                END
            ");

            // 4. Invoices: Fast index seek for correlated subquery (RequestId + IsDeleted)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Invoices_RequestId_IsDeleted' AND object_id = OBJECT_ID('dbo.Invoices'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_Invoices_RequestId_IsDeleted]
                    ON [dbo].[Invoices] (
                        [RequestId],
                        [IsDeleted]
                    );
                END
            ");

            // 5. ClosedPostPositionEquipment: Fast vehicle lookup for paged closed positions
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ClosedPostPositionEquipment_CppId_Vehicle' AND object_id = OBJECT_ID('dbo.ClosedPostPositionEquipment'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_ClosedPostPositionEquipment_CppId_Vehicle]
                    ON [dbo].[ClosedPostPositionEquipment] (
                        [ClosedPostPositionID]
                    )
                    INCLUDE (
                        [DepartmentEquipmentID],
                        [VehNumber],
                        [VehicleUsedHours]
                    );
                END
            ");

            // 6. Employee: Fast department-scoped lookup with covered demographic columns
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Employee_DepartmentID_ArchivedReport' AND object_id = OBJECT_ID('dbo.Employee'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_Employee_DepartmentID_ArchivedReport]
                    ON [dbo].[Employee] (
                        [DepartmentID],
                        [IsDeleted]
                    )
                    INCLUDE (
                        [EmployeeID],
                        [EmployeeCode],
                        [FirstName],
                        [LastName],
                        [EmployeeNumber],
                        [OffDutyCallBackPhoneNumber],
                        [HourlySalary],
                        [EmployeeTypeID]
                    );
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PostPosition_ArchivedReport_Perf' AND object_id = OBJECT_ID('dbo.PostPosition'))
                BEGIN
                    DROP INDEX [IX_PostPosition_ArchivedReport_Perf] ON [dbo].[PostPosition];
                END

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ClosedPostPosition_PostPositionID_CloseDate' AND object_id = OBJECT_ID('dbo.ClosedPostPosition'))
                BEGIN
                    DROP INDEX [IX_ClosedPostPosition_PostPositionID_CloseDate] ON [dbo].[ClosedPostPosition];
                END

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_InvoiceDetails_ClosePostPositionID_IsDeleted' AND object_id = OBJECT_ID('dbo.InvoiceDetails'))
                BEGIN
                    DROP INDEX [IX_InvoiceDetails_ClosePostPositionID_IsDeleted] ON [dbo].[InvoiceDetails];
                END

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Invoices_RequestId_IsDeleted' AND object_id = OBJECT_ID('dbo.Invoices'))
                BEGIN
                    DROP INDEX [IX_Invoices_RequestId_IsDeleted] ON [dbo].[Invoices];
                END

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ClosedPostPositionEquipment_CppId_Vehicle' AND object_id = OBJECT_ID('dbo.ClosedPostPositionEquipment'))
                BEGIN
                    DROP INDEX [IX_ClosedPostPositionEquipment_CppId_Vehicle] ON [dbo].[ClosedPostPositionEquipment];
                END

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Employee_DepartmentID_ArchivedReport' AND object_id = OBJECT_ID('dbo.Employee'))
                BEGIN
                    DROP INDEX [IX_Employee_DepartmentID_ArchivedReport] ON [dbo].[Employee];
                END
            ");
        }
    }
}
