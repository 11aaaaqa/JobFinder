using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_currently_work_here_boolean_column_to_employee_experience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CurrentlyWorkHere",
                table: "EmployeeExperience",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentlyWorkHere",
                table: "EmployeeExperience");
        }
    }
}
