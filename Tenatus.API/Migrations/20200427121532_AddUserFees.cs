using Microsoft.EntityFrameworkCore.Migrations;

namespace Tenatus.API.Migrations
{
    public partial class AddUserFees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Strategy_AspNetUsers_UserId",
                table: "Strategy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Strategy",
                table: "Strategy");

            migrationBuilder.RenameTable(
                name: "Strategy",
                newName: "Strategies");

            migrationBuilder.RenameIndex(
                name: "IX_Strategy_UserId",
                table: "Strategies",
                newName: "IX_Strategies_UserId");

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumFee",
                table: "AspNetUsers",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Strategies",
                table: "Strategies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Strategies_AspNetUsers_UserId",
                table: "Strategies",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Strategies_AspNetUsers_UserId",
                table: "Strategies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Strategies",
                table: "Strategies");

            migrationBuilder.DropColumn(
                name: "MinimumFee",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "Strategies",
                newName: "Strategy");

            migrationBuilder.RenameIndex(
                name: "IX_Strategies_UserId",
                table: "Strategy",
                newName: "IX_Strategy_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Strategy",
                table: "Strategy",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Strategy_AspNetUsers_UserId",
                table: "Strategy",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
