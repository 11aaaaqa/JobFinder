using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResponseMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterviewInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitedCompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmployeeResumeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeName = table.Column<string>(type: "text", nullable: false),
                    EmployeeSurname = table.Column<string>(type: "text", nullable: false),
                    EmployeeDateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    EmployeeWorkingExperience = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EmployeeCity = table.Column<string>(type: "text", nullable: true),
                    VacancyId = table.Column<Guid>(type: "uuid", nullable: false),
                    VacancyPosition = table.Column<string>(type: "text", nullable: false),
                    VacancySalaryFrom = table.Column<int>(type: "integer", nullable: true),
                    VacancySalaryTo = table.Column<int>(type: "integer", nullable: true),
                    VacancyWorkExperience = table.Column<string>(type: "text", nullable: true),
                    VacancyCity = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewInvitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VacancyResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VacancyCompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponseStatus = table.Column<string>(type: "text", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RespondedEmployeeResumeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeName = table.Column<string>(type: "text", nullable: false),
                    EmployeeSurname = table.Column<string>(type: "text", nullable: false),
                    EmployeeDateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    EmployeeWorkingExperience = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EmployeeCity = table.Column<string>(type: "text", nullable: true),
                    VacancyId = table.Column<Guid>(type: "uuid", nullable: false),
                    VacancyPosition = table.Column<string>(type: "text", nullable: false),
                    VacancySalaryFrom = table.Column<int>(type: "integer", nullable: true),
                    VacancySalaryTo = table.Column<int>(type: "integer", nullable: true),
                    VacancyWorkExperience = table.Column<string>(type: "text", nullable: true),
                    VacancyCity = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyResponses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewInvitations");

            migrationBuilder.DropTable(
                name: "VacancyResponses");
        }
    }
}
