using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VacancyMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class remove_profession_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profession",
                table: "Vacancies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Profession",
                table: "Vacancies",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
