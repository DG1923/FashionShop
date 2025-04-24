using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionShop.ProductService.Migrations
{
    /// <inheritdoc />
    public partial class udpateProductDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "ProductVariations");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Discounts");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "BasePrice");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalQuantityColor",
                table: "ProductColors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "BasePrice",
                table: "Products",
                newName: "Price");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ProductVariations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "TotalQuantityColor",
                table: "ProductColors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Discounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
