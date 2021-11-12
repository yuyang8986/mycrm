using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCRM.Persistence.Migrations
{
    public partial class addindex2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Pipelines",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DealName",
                table: "Pipelines",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_ChangeStageDate",
                table: "Pipelines",
                column: "ChangeStageDate");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_CreatedDate",
                table: "Pipelines",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_DealName",
                table: "Pipelines",
                column: "DealName");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_Type",
                table: "Pipelines",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_UpdatedDate",
                table: "Pipelines",
                column: "UpdatedDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pipelines_ChangeStageDate",
                table: "Pipelines");

            migrationBuilder.DropIndex(
                name: "IX_Pipelines_CreatedDate",
                table: "Pipelines");

            migrationBuilder.DropIndex(
                name: "IX_Pipelines_DealName",
                table: "Pipelines");

            migrationBuilder.DropIndex(
                name: "IX_Pipelines_Type",
                table: "Pipelines");

            migrationBuilder.DropIndex(
                name: "IX_Pipelines_UpdatedDate",
                table: "Pipelines");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Pipelines",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DealName",
                table: "Pipelines",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
