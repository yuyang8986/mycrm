using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCRM.Persistence.Migrations
{
    public partial class addindex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Stages",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Peoples",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Peoples",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Activities",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_EventStartDateTime",
                table: "Tasks",
                column: "EventStartDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_Name",
                table: "Stages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_DealAmount",
                table: "Pipelines",
                column: "DealAmount");

            migrationBuilder.CreateIndex(
                name: "IX_Peoples_FirstName",
                table: "Peoples",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Peoples_LastName",
                table: "Peoples",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventStartDateTime",
                table: "Events",
                column: "EventStartDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_EventStartDateTime",
                table: "Appointments",
                column: "EventStartDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Name",
                table: "Activities",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_EventStartDateTime",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Stages_Name",
                table: "Stages");

            migrationBuilder.DropIndex(
                name: "IX_Pipelines_DealAmount",
                table: "Pipelines");

            migrationBuilder.DropIndex(
                name: "IX_Peoples_FirstName",
                table: "Peoples");

            migrationBuilder.DropIndex(
                name: "IX_Peoples_LastName",
                table: "Peoples");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventStartDateTime",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Companies_Name",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_EventStartDateTime",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Activities_Name",
                table: "Activities");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Stages",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Peoples",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Peoples",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Activities",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
