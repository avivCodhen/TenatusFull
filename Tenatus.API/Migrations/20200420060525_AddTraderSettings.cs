using Microsoft.EntityFrameworkCore.Migrations;

namespace Tenatus.API.Migrations
{
    public partial class AddTraderSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyingValue",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SellingValue",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "TraderSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    BuyingValue = table.Column<decimal>(nullable: false),
                    SellingValue = table.Column<decimal>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_TraderSettings_UserId",
                table: "TraderSettings",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TraderSettings");

            migrationBuilder.AddColumn<decimal>(
                name: "BuyingValue",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SellingValue",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
