using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talorants.Blog.Mvc.Data.Migrations
{
    public partial class TelegramId_Create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TelegramUserId",
                table: "AspNetUsers",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramUserId",
                table: "AspNetUsers");
        }
    }
}
