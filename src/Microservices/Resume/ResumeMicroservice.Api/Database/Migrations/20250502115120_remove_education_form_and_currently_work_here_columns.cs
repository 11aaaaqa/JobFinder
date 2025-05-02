using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class remove_education_form_and_currently_work_here_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentlyWorkHere",
                table: "EmployeeExperience");

            migrationBuilder.DropColumn(
                name: "EducationForm",
                table: "Education");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CurrentlyWorkHere",
                table: "EmployeeExperience",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EducationForm",
                table: "Education",
                type: "text",
                nullable: true);
        }
    }
}
