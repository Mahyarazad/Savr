using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Savr.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5a2b0e57-a185-4847-88d3-50dd34d2ac1a", null, "Merchant", "MERCHANT" },
                    { "9787f364-22ed-4abf-8944-bca26df0771f", null, "User", "USER" },
                    { "bc23dcb9-43ab-4d2d-b08a-8051cbe46ac6", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a2b0e57-a185-4847-88d3-50dd34d2ac1a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9787f364-22ed-4abf-8944-bca26df0771f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bc23dcb9-43ab-4d2d-b08a-8051cbe46ac6");
        }
    }
}
