using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixShareFileTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_SharedFiles_SharedFileId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<int>(
                name: "SharedFileId",
                table: "Feedbacks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_SharedFiles_SharedFileId",
                table: "Feedbacks",
                column: "SharedFileId",
                principalTable: "SharedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_SharedFiles_SharedFileId",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<int>(
                name: "SharedFileId",
                table: "Feedbacks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "Feedbacks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_SharedFiles_SharedFileId",
                table: "Feedbacks",
                column: "SharedFileId",
                principalTable: "SharedFiles",
                principalColumn: "Id");
        }
    }
}
