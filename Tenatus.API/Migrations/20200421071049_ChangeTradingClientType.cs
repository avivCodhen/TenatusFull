using Microsoft.EntityFrameworkCore.Migrations;

namespace Tenatus.API.Migrations
{
    public partial class ChangeTradingClientType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradingClientType",
                table: "TraderSettings");

            migrationBuilder.AddColumn<string>(
                name: "TradingClientType",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradingClientType",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "TradingClientType",
                table: "TraderSettings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
