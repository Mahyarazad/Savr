using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Savr.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Groups_GroupId",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_GroupId",
                table: "Listings");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Listings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Listings",
                type: "character varying(1500)",
                maxLength: 1500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<int>(
                name: "GroupId1",
                table: "Listings",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Groups",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "OwnerUserId", "Title" },
                values: new object[,]
                {
                    { -11, new DateTime(2025, 7, 17, 13, 18, 49, 966, DateTimeKind.Utc).AddTicks(6847), "Professional consulting for businesses or individuals.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Consulting" },
                    { -10, new DateTime(2025, 7, 17, 13, 18, 49, 966, DateTimeKind.Utc).AddTicks(6846), "Property listings, realtors, and housing services.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Real Estate" },
                    { -9, new DateTime(2025, 7, 17, 13, 18, 49, 966, DateTimeKind.Utc).AddTicks(6845), "Banks, insurance, and financial consulting.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Financial Services" },
                    { -8, new DateTime(2025, 7, 17, 13, 18, 49, 966, DateTimeKind.Utc).AddTicks(6844), "Hospitals, clinics, and healthcare providers.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Healthcare" },
                    { -7, new DateTime(2025, 7, 17, 13, 18, 49, 966, DateTimeKind.Utc).AddTicks(6843), "Schools, tutors, and educational institutions.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Education" },
                    { -6, new DateTime(2025, 7, 17, 13, 18, 49, 966, DateTimeKind.Utc).AddTicks(6842), "Hotels, travel agencies, and transportation services.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Hotel & Travel" },
                    { -5, new DateTime(2025, 7, 17, 13, 18, 49, 966, DateTimeKind.Utc).AddTicks(6841), "Essential services for daily needs like laundry or repairs.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Everyday Services" },
                    { -4, new DateTime(2025, 7, 17, 13, 18, 49, 966, DateTimeKind.Utc).AddTicks(6840), "Clothing stores, boutiques, and shopping centers.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Fashion & Retail" },
                    { -3, new DateTime(2025, 7, 17, 13, 18, 49, 966, DateTimeKind.Utc).AddTicks(6839), "Entertainment venues, attractions, and recreational activities.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Attraction & Leisure" },
                    { -2, new DateTime(2025, 7, 17, 13, 18, 49, 966, DateTimeKind.Utc).AddTicks(6838), "Gyms, spas, salons, and wellness services.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Beauty & Fitness" },
                    { -1, new DateTime(2025, 7, 17, 13, 18, 49, 966, DateTimeKind.Utc).AddTicks(6836), "Restaurants, cafes, and food delivery services.", true, new Guid("3d32337a-7372-4261-98b9-8352c83d8751"), "Food & Drinks" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Listings_GroupId1",
                table: "Listings",
                column: "GroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Groups_GroupId1",
                table: "Listings",
                column: "GroupId1",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Groups_GroupId1",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_GroupId1",
                table: "Listings");

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: -11);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: -10);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: -9);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: -8);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DropColumn(
                name: "GroupId1",
                table: "Listings");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Listings",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Listings",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1500)",
                oldMaxLength: 1500);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Groups",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Listings_GroupId",
                table: "Listings",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Groups_GroupId",
                table: "Listings",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
