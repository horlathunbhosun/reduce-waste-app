using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewColuwnsForMagicBag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "71a6326b-3762-49c5-973a-645960f4c8d6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87b71428-7764-4769-9028-642579b6d1fe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e426d605-9ae2-42e9-8609-8b89772b95c2");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MagicBags",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MagicBags",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MagicBags");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MagicBags");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "71a6326b-3762-49c5-973a-645960f4c8d6", null, "Admin", "ADMIN" },
                    { "87b71428-7764-4769-9028-642579b6d1fe", null, "User", "USER" },
                    { "e426d605-9ae2-42e9-8609-8b89772b95c2", null, "Partner", "PARTNER" }
                });
        }
    }
}
