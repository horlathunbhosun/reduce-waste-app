using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeColuwmFromGuidTOUuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "User",
                newName: "Uuid");

            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "Partner",
                newName: "Uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Uuid",
                table: "User",
                newName: "Guid");

            migrationBuilder.RenameColumn(
                name: "Uuid",
                table: "Partner",
                newName: "Guid");
        }
    }
}
