using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tenatus.API.Migrations
{
    public partial class AddStockData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "TraderSettings");

            migrationBuilder.AddColumn<bool>(
                name: "Filled",
                table: "UserOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Stock",
                table: "UserOrders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StocksData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Stock = table.Column<string>(nullable: true),
                    Open = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    High = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Low = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Close = table.Column<decimal>(type: "decimal(18,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StocksData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Strategy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    Stock = table.Column<string>(nullable: true),
                    UserOrderType = table.Column<int>(nullable: false),
                    Budget = table.Column<decimal>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Percent = table.Column<decimal>(nullable: true),
                    Minimum = table.Column<decimal>(nullable: true),
                    Maximum = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strategy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Strategy_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Strategy_UserId",
                table: "Strategy",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StocksData");

            migrationBuilder.DropTable(
                name: "Strategy");

            migrationBuilder.DropColumn(
                name: "Filled",
                table: "UserOrders");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "UserOrders");

            migrationBuilder.CreateTable(
                name: "TraderSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuyingValue = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    SellingValue = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraderSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraderSettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TradeSettingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stock_TraderSettings_TradeSettingId",
                        column: x => x.TradeSettingId,
                        principalTable: "TraderSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stock_TradeSettingId",
                table: "Stock",
                column: "TradeSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_TraderSettings_UserId",
                table: "TraderSettings",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }
    }
}
