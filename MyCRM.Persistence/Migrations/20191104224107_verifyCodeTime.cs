using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCRM.Persistence.Migrations
{
    public partial class verifyCodeTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VerifyExpiredDateTime",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifyExpiredDateTime",
                table: "AspNetUsers");
        }
    }
}
