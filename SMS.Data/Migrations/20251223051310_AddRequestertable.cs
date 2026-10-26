using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requesters",
                schema: "dbo",
                columns: table => new
                {
                    RequesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentDefinedNumber = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true),
                    BusinessTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Address2 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    City = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    State = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Zip = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    Email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    PrimaryContact = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    SecondaryContact = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    SecondaryPhone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    InvoiceBillingName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    InvoiceContact = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    InvoiceAddress = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    InvoiceAddress2 = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    InvoiceCity = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    InvoiceState = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    InvoiceZip = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    InvoicePhone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    InvoicePhoneExt = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    InvoiceEmail = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    InvoiceFax = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RequiresLiquorLicense = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    LiquorLicense = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    BillingNotes = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Comments = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    SubmittedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    ApprovedForCivilians = table.Column<bool>(type: "bit", nullable: true),
                    DateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExemptFromExtract = table.Column<bool>(type: "bit", nullable: true),
                    IsChild = table.Column<bool>(type: "bit", nullable: true),
                    NotificationRecipients = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    ParentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requesters", x => x.RequesterID);
                    table.ForeignKey(
                        name: "FK_Requesters_BusinessType_BusinessTypeID",
                        column: x => x.BusinessTypeID,
                        principalSchema: "dbo",
                        principalTable: "BusinessType",
                        principalColumn: "BusinessTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requesters_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requesters_Requesters_ParentID",
                        column: x => x.ParentID,
                        principalSchema: "dbo",
                        principalTable: "Requesters",
                        principalColumn: "RequesterID",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateIndex(
                name: "IX_Requesters_BusinessTypeID",
                schema: "dbo",
                table: "Requesters",
                column: "BusinessTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Requesters_DepartmentID",
                schema: "dbo",
                table: "Requesters",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Requesters_ParentID",
                schema: "dbo",
                table: "Requesters",
                column: "ParentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requesters",
                schema: "dbo");
        }
    }
}
