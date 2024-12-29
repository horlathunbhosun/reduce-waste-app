using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewColuwnsForMagicBagStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8aee39d8-0e8a-4b66-8911-728f507cd20b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ac11b9c7-4377-4c4a-8f30-a895232130cf");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "be6acdbb-f1fc-401f-ba23-b55763e6667f");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MagicBags",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3af03efa-b090-40aa-8c8d-c70d2307d76e", null, "Partner", "PARTNER" },
                    { "88997028-7f1f-4d6b-8ca0-b2550866ead8", null, "User", "USER" },
                    { "8f476a4a-5bd3-4c61-a9ff-8b53f6ebbe02", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3af03efa-b090-40aa-8c8d-c70d2307d76e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88997028-7f1f-4d6b-8ca0-b2550866ead8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f476a4a-5bd3-4c61-a9ff-8b53f6ebbe02");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MagicBags");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8aee39d8-0e8a-4b66-8911-728f507cd20b", null, "Partner", "PARTNER" },
                    { "ac11b9c7-4377-4c4a-8f30-a895232130cf", null, "Admin", "ADMIN" },
                    { "be6acdbb-f1fc-401f-ba23-b55763e6667f", null, " ", "USER" }
                });
        }
    }
}
