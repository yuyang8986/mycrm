using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCRM.Persistence.Migrations
{
    public partial class addstripesubid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionId",
                table: "Organizations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeSubscriptionId",
                table: "Organizations");
        }
    }
}
