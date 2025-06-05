using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResponseMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_employee_desired_salary_to_interview_invitation_and_vacancy_response_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmployeeDesiredSalary",
                table: "VacancyResponses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeDesiredSalary",
                table: "InterviewInvitations",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeDesiredSalary",
                table: "VacancyResponses");

            migrationBuilder.DropColumn(
                name: "EmployeeDesiredSalary",
                table: "InterviewInvitations");
        }
    }
}
