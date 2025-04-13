using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VacancyMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class remove_worker_responsibilities_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkerResponsibilities",
                table: "Vacancies");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Vacancies",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Vacancies",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Vacancies",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Vacancies",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkerResponsibilities",
                table: "Vacancies",
                type: "text",
                nullable: true);
        }
    }
}
