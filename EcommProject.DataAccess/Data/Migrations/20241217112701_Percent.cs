using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommProject.DataAccess.Migrations
{
    public partial class Percent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<int>(
                name: "Percent",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfferExpirationTime",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Percent",
                table: "Products");
        }
    }
}
