using Microsoft.EntityFrameworkCore.Migrations;

namespace Tenatus.API.Migrations
{
    public partial class AddUserAccountName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "AspNetUsers");
        }
    }
}
