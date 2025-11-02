using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace SMarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddNameAndNoteIntoOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductVectors",
                table: "ProductVectors");

            migrationBuilder.RenameTable(
                name: "ProductVectors",
                newName: "product_vectors");

            migrationBuilder.RenameColumn(
                name: "Embedding",
                table: "product_vectors",
                newName: "embedding");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "product_vectors",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<Vector>(
                name: "embedding",
                table: "product_vectors",
                type: "vector(768)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(1536)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_product_vectors",
                table: "product_vectors",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_product_vectors",
                table: "product_vectors");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "product_vectors",
                newName: "ProductVectors");

            migrationBuilder.RenameColumn(
                name: "embedding",
                table: "ProductVectors",
                newName: "Embedding");

            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "ProductVectors",
                type: "vector(1536)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(768)");

            migrationBuilder.AlterColumn<long>(
                name: "Price",
                table: "ProductVectors",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductVectors",
                table: "ProductVectors",
                column: "Id");
        }
    }
}
