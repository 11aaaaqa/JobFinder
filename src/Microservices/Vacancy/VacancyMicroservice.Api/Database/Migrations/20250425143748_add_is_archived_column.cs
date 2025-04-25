using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VacancyMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_is_archived_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Vacancies",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Vacancies");
        }
    }
}
