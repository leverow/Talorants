using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talorants.Blog.Mvc.Data.Migrations
{
    public partial class Roles_Add : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Roles",
                table: "AspNetUsers",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "AspNetUsers");
        }
    }
}
