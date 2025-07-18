using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Savr.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Listings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Description = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: false),
                    Location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AverageRating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false, defaultValue: 0m),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<long>(type: "bigint", nullable: false),
                    PreviousPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PriceWithPromotion = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PriceDropPercentage = table.Column<double>(type: "double precision", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Listings_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReviews",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ListingId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReviews_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ListingId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Group",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "OwnerUserId", "Title" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 7, 18, 5, 54, 16, 668, DateTimeKind.Utc).AddTicks(7754), "Restaurants, cafes, and food delivery services.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Food & Drinks" },
                    { 2L, new DateTime(2025, 7, 18, 5, 54, 16, 668, DateTimeKind.Utc).AddTicks(7759), "Gyms, spas, salons, and wellness services.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Beauty & Fitness" },
                    { 3L, new DateTime(2025, 7, 18, 5, 54, 16, 668, DateTimeKind.Utc).AddTicks(7760), "Entertainment venues, attractions, and recreational activities.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Attraction & Leisure" },
                    { 4L, new DateTime(2025, 7, 18, 5, 54, 16, 668, DateTimeKind.Utc).AddTicks(7761), "Clothing stores, boutiques, and shopping centers.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Fashion & Retail" },
                    { 5L, new DateTime(2025, 7, 18, 5, 54, 16, 668, DateTimeKind.Utc).AddTicks(7762), "Essential services for daily needs like laundry or repairs.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Everyday Services" },
                    { 6L, new DateTime(2025, 7, 18, 5, 54, 16, 668, DateTimeKind.Utc).AddTicks(7763), "Hotels, travel agencies, and transportation services.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Hotel & Travel" },
                    { 7L, new DateTime(2025, 7, 18, 5, 54, 16, 668, DateTimeKind.Utc).AddTicks(7764), "Schools, tutors, and educational institutions.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Education" },
                    { 8L, new DateTime(2025, 7, 18, 5, 54, 16, 668, DateTimeKind.Utc).AddTicks(7766), "Hospitals, clinics, and healthcare providers.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Healthcare" },
                    { 9L, new DateTime(2025, 7, 18, 5, 54, 16, 668, DateTimeKind.Utc).AddTicks(7767), "Banks, insurance, and financial consulting.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Financial Services" },
                    { 10L, new DateTime(2025, 7, 18, 5, 54, 16, 668, DateTimeKind.Utc).AddTicks(7768), "Property listings, realtors, and housing services.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Real Estate" },
                    { 11L, new DateTime(2025, 7, 18, 5, 54, 16, 668, DateTimeKind.Utc).AddTicks(7769), "Professional consulting for businesses or individuals.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Consulting" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReviews_ListingId",
                table: "CustomerReviews",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_GroupId",
                table: "Listings",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "UniqueIndex_PDes_PName",
                table: "Listings",
                columns: new[] { "Description", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ListingId",
                table: "Tags",
                column: "ListingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReviews");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Listings");

            migrationBuilder.DropTable(
                name: "Group");
        }
    }
}
