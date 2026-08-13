using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace main.Migrations
{
    /// <inheritdoc />
    public partial class JoinTableCartProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_carts_cart_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "ix_products_cart_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "cart_id",
                table: "products");

            migrationBuilder.CreateTable(
                name: "cart_item",
                columns: table => new
                {
                    cart_id1 = table.Column<Guid>(type: "uuid", nullable: false),
                    products_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cart_item", x => new { x.cart_id1, x.products_id });
                    table.ForeignKey(
                        name: "fk_cart_item_carts_cart_id1",
                        column: x => x.cart_id1,
                        principalTable: "carts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cart_item_products_products_id",
                        column: x => x.products_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cart_item_products_id",
                table: "cart_item",
                column: "products_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cart_item");

            migrationBuilder.AddColumn<Guid>(
                name: "cart_id",
                table: "products",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_cart_id",
                table: "products",
                column: "cart_id");

            migrationBuilder.AddForeignKey(
                name: "fk_products_carts_cart_id",
                table: "products",
                column: "cart_id",
                principalTable: "carts",
                principalColumn: "id");
        }
    }
}
