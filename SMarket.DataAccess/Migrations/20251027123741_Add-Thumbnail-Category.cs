using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThumbnailId",
                table: "Categories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ThumbnailId",
                table: "Categories",
                column: "ThumbnailId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_SharedFiles_ThumbnailId",
                table: "Categories",
                column: "ThumbnailId",
                principalTable: "SharedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_SharedFiles_ThumbnailId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ThumbnailId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ThumbnailId",
                table: "Categories");
        }
    }
}
