using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCRM.Persistence.Migrations
{
    public partial class userVerifyCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerifyCode",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifyCode",
                table: "AspNetUsers");
        }
    }
}
