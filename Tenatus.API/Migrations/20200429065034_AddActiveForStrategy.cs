using Microsoft.EntityFrameworkCore.Migrations;

namespace Tenatus.API.Migrations
{
    public partial class AddActiveForStrategy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Strategies",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Strategies");
        }
    }
}
