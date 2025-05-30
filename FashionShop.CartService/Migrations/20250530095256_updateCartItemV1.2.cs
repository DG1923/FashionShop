using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionShop.CartService.Migrations
{
    /// <inheritdoc />
    public partial class updateCartItemV12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "CartItems",
                newName: "BasePrice");

            migrationBuilder.AlterColumn<Guid>(
                name: "CartId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorName",
                table: "CartItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "CartItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductColorId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductVariationId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "CartItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ColorName",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DiscountId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ProductColorId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ProductVariationId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "BasePrice",
                table: "CartItems",
                newName: "Price");

            migrationBuilder.AlterColumn<Guid>(
                name: "CartId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id");
        }
    }
}
