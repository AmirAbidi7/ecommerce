using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace main.Migrations
{
    /// <inheritdoc />
    public partial class JoinTableFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cart_item_carts_cart_id1",
                table: "cart_item");

            migrationBuilder.DropForeignKey(
                name: "fk_cart_item_products_products_id",
                table: "cart_item");

            migrationBuilder.RenameColumn(
                name: "products_id",
                table: "cart_item",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "cart_id1",
                table: "cart_item",
                newName: "cart_id");

            migrationBuilder.RenameIndex(
                name: "ix_cart_item_products_id",
                table: "cart_item",
                newName: "ix_cart_item_product_id");

            migrationBuilder.AddColumn<int>(
                name: "product_amount",
                table: "cart_item",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "fk_cart_item_carts_cart_id",
                table: "cart_item",
                column: "cart_id",
                principalTable: "carts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_cart_item_products_product_id",
                table: "cart_item",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cart_item_carts_cart_id",
                table: "cart_item");

            migrationBuilder.DropForeignKey(
                name: "fk_cart_item_products_product_id",
                table: "cart_item");

            migrationBuilder.DropColumn(
                name: "product_amount",
                table: "cart_item");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "cart_item",
                newName: "products_id");

            migrationBuilder.RenameColumn(
                name: "cart_id",
                table: "cart_item",
                newName: "cart_id1");

            migrationBuilder.RenameIndex(
                name: "ix_cart_item_product_id",
                table: "cart_item",
                newName: "ix_cart_item_products_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cart_item_carts_cart_id1",
                table: "cart_item",
                column: "cart_id1",
                principalTable: "carts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_cart_item_products_products_id",
                table: "cart_item",
                column: "products_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
