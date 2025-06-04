using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResponseMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_vacancy_company_name_column_to_interview_invitation_and_vacancy_response_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VacancyCompanyName",
                table: "VacancyResponses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VacancyCompanyName",
                table: "InterviewInvitations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VacancyCompanyName",
                table: "VacancyResponses");

            migrationBuilder.DropColumn(
                name: "VacancyCompanyName",
                table: "InterviewInvitations");
        }
    }
}
