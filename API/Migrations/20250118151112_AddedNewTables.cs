using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MagicBagId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    TransactionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Amount = table.Column<double>(type: "double(12,2)", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PickUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_MagicBags_MagicBagId",
                        column: x => x.MagicBagId,
                        principalTable: "MagicBags",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TransactionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Feedbacks_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1f516f62-b2d3-46e7-885f-cd9ce476be90", null, "Admin", "ADMIN" },
                    { "691c37a1-bcd2-458a-809a-17770fe4749c", null, "Partner", "PARTNER" },
                    { "824624b3-19b3-4516-b93d-b9893ddcd9a2", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_TransactionId",
                table: "Feedbacks",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId_TransactionId",
                table: "Feedbacks",
                columns: new[] { "UserId", "TransactionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_MagicBagId",
                table: "Transactions",
                column: "MagicBagId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_MagicBagId",
                table: "Transactions",
                columns: new[] { "UserId", "MagicBagId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1f516f62-b2d3-46e7-885f-cd9ce476be90");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "691c37a1-bcd2-458a-809a-17770fe4749c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "824624b3-19b3-4516-b93d-b9893ddcd9a2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "482d0a48-5364-408c-bcad-1051e69d4e46", null, "User", "USER" },
                    { "79d21691-5db6-4670-bcb7-e723d7265ccc", null, "Partner", "PARTNER" },
                    { "ee606a75-139a-478f-90c5-e1becdf2c292", null, "Admin", "ADMIN" }
                });
        }
    }
}
