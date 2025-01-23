using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommProject.DataAccess.Migrations
{
    public partial class AddColumnsToShoppingCartModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRemoveFromCart",
                table: "ShoppingCarts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "ShoppingCarts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemoveFromCart",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "ShoppingCarts");
        }
    }
}
