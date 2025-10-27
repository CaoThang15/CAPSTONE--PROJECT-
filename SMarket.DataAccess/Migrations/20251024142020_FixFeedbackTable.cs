using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixFeedbackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_FromUserId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_ToUserId",
                table: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "Feedbacks",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "FromUserId",
                table: "Feedbacks",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_ToUserId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_FromUserId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Products_ProductId",
                table: "Feedbacks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_UserId",
                table: "Feedbacks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Products_ProductId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_UserId",
                table: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Feedbacks",
                newName: "ToUserId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Feedbacks",
                newName: "FromUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_ProductId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_FromUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_FromUserId",
                table: "Feedbacks",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_ToUserId",
                table: "Feedbacks",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
