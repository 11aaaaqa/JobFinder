using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VacancyMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_vacancy_creation_date_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "CreatedAt",
                table: "Vacancies",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Vacancies");
        }
    }
}
