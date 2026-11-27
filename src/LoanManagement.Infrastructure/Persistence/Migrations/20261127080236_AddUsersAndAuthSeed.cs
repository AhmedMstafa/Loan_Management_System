using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LoanManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersAndAuthSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FullName", "IsActive", "IsDeleted", "OrganizationId", "PasswordHash", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { "usr_admin_01", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "system", "admin@bank.com", "System Admin", true, false, "org_default", "v1.100000.7ofzdEV3XgcM58cJqhvKrw==.nNfPleuFQ85eYAxJ5MYXQHtivS4L7yKbPXpHAkQyNPs=", "Admin", null },
                    { "usr_customer_01", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "system", "customer@bank.com", "Sample Customer", true, false, "org_default", "v1.100000.Q886Rs7thdYGS3AOKDG47A==.bZ5P+3NWhzYkcdAPAoT73cjnJRBpH8f7RyNn3GoffWM=", "Customer", null },
                    { "usr_officer_01", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "system", "officer@bank.com", "Loan Officer", true, false, "org_default", "v1.100000.MDWj9SbEjHU6tQB0DuZLGg==.duINfkxwhUCpFIvrzN1aqajQgDev0pIE7J3PZkYMAdc=", "LoanOfficer", null },
                    { "usr_ops_01", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "system", "ops@bank.com", "Back Office Ops", true, false, "org_default", "v1.100000.FefF3XKHgEB2WDp1gxMc1A==.naw8m2C279fjF9U5wsmhTTAtGMbNCclcQyaNiqQHGYE=", "BackOffice", null },
                    { "usr_underwriter_01", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "system", "underwriter@bank.com", "Underwriter", true, false, "org_default", "v1.100000.gGfggZQM6QqHijj6vf9glA==.I2hcIcH+zo25chFBSTMfEdGafdUsxevNfHNFtqBiXqY=", "Underwriter", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_OrganizationId_Role",
                table: "users",
                columns: new[] { "OrganizationId", "Role" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
