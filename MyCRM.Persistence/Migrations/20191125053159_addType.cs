using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCRM.Persistence.Migrations
{
    public partial class addType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "EstimatedCost",
                table: "Pipelines",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<double>(
                name: "Margin",
                table: "Pipelines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Pipelines",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Margin",
                table: "Pipelines");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Pipelines");

            migrationBuilder.AlterColumn<double>(
                name: "EstimatedCost",
                table: "Pipelines",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
