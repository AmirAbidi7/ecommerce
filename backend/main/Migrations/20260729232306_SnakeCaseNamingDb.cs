using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace main.Migrations
{
    /// <inheritdoc />
    public partial class SnakeCaseNamingDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserProduct_Products_FavoriteProductsId",
                table: "AppUserProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserProduct_Users_AppUserId",
                table: "AppUserProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_UserId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Carts_CartId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Carts",
                table: "Carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppUserProduct",
                table: "AppUserProduct");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "products");

            migrationBuilder.RenameTable(
                name: "Carts",
                newName: "carts");

            migrationBuilder.RenameTable(
                name: "AppUserProduct",
                newName: "app_user_product");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "lastName",
                table: "users",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "firstName",
                table: "users",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "products",
                newName: "stock");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "products",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "products",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "products",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "imageUrl",
                table: "products",
                newName: "image_url");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "products",
                newName: "cart_id");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CartId",
                table: "products",
                newName: "ix_products_cart_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "carts",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "carts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "carts",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_UserId",
                table: "carts",
                newName: "ix_carts_user_id");

            migrationBuilder.RenameColumn(
                name: "FavoriteProductsId",
                table: "app_user_product",
                newName: "favorite_products_id");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "app_user_product",
                newName: "app_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_AppUserProduct_FavoriteProductsId",
                table: "app_user_product",
                newName: "ix_app_user_product_favorite_products_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_products",
                table: "products",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_carts",
                table: "carts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_app_user_product",
                table: "app_user_product",
                columns: new[] { "app_user_id", "favorite_products_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_app_user_product_products_favorite_products_id",
                table: "app_user_product",
                column: "favorite_products_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_app_user_product_users_app_user_id",
                table: "app_user_product",
                column: "app_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_carts_users_user_id",
                table: "carts",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_products_carts_cart_id",
                table: "products",
                column: "cart_id",
                principalTable: "carts",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_app_user_product_products_favorite_products_id",
                table: "app_user_product");

            migrationBuilder.DropForeignKey(
                name: "fk_app_user_product_users_app_user_id",
                table: "app_user_product");

            migrationBuilder.DropForeignKey(
                name: "fk_carts_users_user_id",
                table: "carts");

            migrationBuilder.DropForeignKey(
                name: "fk_products_carts_cart_id",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_products",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "pk_carts",
                table: "carts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_app_user_product",
                table: "app_user_product");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "products",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "carts",
                newName: "Carts");

            migrationBuilder.RenameTable(
                name: "app_user_product",
                newName: "AppUserProduct");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "Users",
                newName: "lastName");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "Users",
                newName: "firstName");

            migrationBuilder.RenameColumn(
                name: "stock",
                table: "Products",
                newName: "Stock");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Products",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "image_url",
                table: "Products",
                newName: "imageUrl");

            migrationBuilder.RenameColumn(
                name: "cart_id",
                table: "Products",
                newName: "CartId");

            migrationBuilder.RenameIndex(
                name: "ix_products_cart_id",
                table: "Products",
                newName: "IX_Products_CartId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Carts",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Carts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Carts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_carts_user_id",
                table: "Carts",
                newName: "IX_Carts_UserId");

            migrationBuilder.RenameColumn(
                name: "favorite_products_id",
                table: "AppUserProduct",
                newName: "FavoriteProductsId");

            migrationBuilder.RenameColumn(
                name: "app_user_id",
                table: "AppUserProduct",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "ix_app_user_product_favorite_products_id",
                table: "AppUserProduct",
                newName: "IX_AppUserProduct_FavoriteProductsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carts",
                table: "Carts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppUserProduct",
                table: "AppUserProduct",
                columns: new[] { "AppUserId", "FavoriteProductsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserProduct_Products_FavoriteProductsId",
                table: "AppUserProduct",
                column: "FavoriteProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserProduct_Users_AppUserId",
                table: "AppUserProduct",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Carts_CartId",
                table: "Products",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id");
        }
    }
}
