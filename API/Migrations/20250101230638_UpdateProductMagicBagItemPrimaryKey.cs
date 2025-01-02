using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductMagicBagItemPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductMagicBagItems_MagicBags_MagicBagId",
                table: "ProductMagicBagItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductMagicBagItems_Products_ProductId",
                table: "ProductMagicBagItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductMagicBagItems",
                table: "ProductMagicBagItems");

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

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "ProductMagicBagItems",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "MagicBagId",
                table: "ProductMagicBagItems",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductMagicBagItems",
                table: "ProductMagicBagItems",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "482d0a48-5364-408c-bcad-1051e69d4e46", null, "User", "USER" },
                    { "79d21691-5db6-4670-bcb7-e723d7265ccc", null, "Partner", "PARTNER" },
                    { "ee606a75-139a-478f-90c5-e1becdf2c292", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMagicBagItems_MagicBagId_ProductId",
                table: "ProductMagicBagItems",
                columns: new[] { "MagicBagId", "ProductId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMagicBagItems_MagicBags_MagicBagId",
                table: "ProductMagicBagItems",
                column: "MagicBagId",
                principalTable: "MagicBags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMagicBagItems_Products_ProductId",
                table: "ProductMagicBagItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductMagicBagItems_MagicBags_MagicBagId",
                table: "ProductMagicBagItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductMagicBagItems_Products_ProductId",
                table: "ProductMagicBagItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductMagicBagItems",
                table: "ProductMagicBagItems");

            migrationBuilder.DropIndex(
                name: "IX_ProductMagicBagItems_MagicBagId_ProductId",
                table: "ProductMagicBagItems");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "482d0a48-5364-408c-bcad-1051e69d4e46");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "79d21691-5db6-4670-bcb7-e723d7265ccc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ee606a75-139a-478f-90c5-e1becdf2c292");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "ProductMagicBagItems",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "MagicBagId",
                table: "ProductMagicBagItems",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductMagicBagItems",
                table: "ProductMagicBagItems",
                columns: new[] { "MagicBagId", "ProductId" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3af03efa-b090-40aa-8c8d-c70d2307d76e", null, "Partner", "PARTNER" },
                    { "88997028-7f1f-4d6b-8ca0-b2550866ead8", null, "User", "USER" },
                    { "8f476a4a-5bd3-4c61-a9ff-8b53f6ebbe02", null, "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMagicBagItems_MagicBags_MagicBagId",
                table: "ProductMagicBagItems",
                column: "MagicBagId",
                principalTable: "MagicBags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMagicBagItems_Products_ProductId",
                table: "ProductMagicBagItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
