using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SMarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixPropertyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Categories_CategoryId",
                table: "Properties");

            migrationBuilder.DropTable(
                name: "ProductProperties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_CategoryId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Properties");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Properties",
                type: "integer",
                nullable: false,
                defaultValue: 6);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Properties",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ProductId",
                table: "Properties",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Products_ProductId",
                table: "Properties",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Products_ProductId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_ProductId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Properties");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Properties",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    PropertyId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductProperties_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductProperties_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_CategoryId",
                table: "Properties",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProperties_ProductId",
                table: "ProductProperties",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProperties_PropertyId",
                table: "ProductProperties",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Categories_CategoryId",
                table: "Properties",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
