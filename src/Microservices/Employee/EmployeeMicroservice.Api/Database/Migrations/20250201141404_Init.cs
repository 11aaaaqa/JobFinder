using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Education");

            migrationBuilder.DropTable(
                name: "EmployeeExperience");

            migrationBuilder.DropTable(
                name: "ForeignLanguage");

            migrationBuilder.DropColumn(
                name: "AboutMe",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DesiredSalaryFrom",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DesiredSalaryTo",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ReadyToMove",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "Post",
                table: "Employees",
                newName: "Patronymic");

            migrationBuilder.RenameColumn(
                name: "OccupationType",
                table: "Employees",
                newName: "Gender");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Employees",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "Patronymic",
                table: "Employees",
                newName: "Post");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Employees",
                newName: "OccupationType");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Employees",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AboutMe",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DesiredSalaryFrom",
                table: "Employees",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DesiredSalaryTo",
                table: "Employees",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReadyToMove",
                table: "Employees",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Education",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationForm = table.Column<string>(type: "text", nullable: true),
                    EducationType = table.Column<string>(type: "text", nullable: false),
                    EducationalInstitution = table.Column<string>(type: "text", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Specialization = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Education", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Education_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeExperience",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    CompanyPost = table.Column<string>(type: "text", nullable: false),
                    CurrentlyWorkHere = table.Column<bool>(type: "boolean", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Responsibilities = table.Column<string>(type: "text", nullable: true),
                    WorkingFromMonth = table.Column<string>(type: "text", nullable: false),
                    WorkingFromYear = table.Column<string>(type: "text", nullable: false),
                    WorkingUntilMonth = table.Column<string>(type: "text", nullable: false),
                    WorkingUntilYear = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeExperience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeExperience_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ForeignLanguage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    LanguageName = table.Column<string>(type: "text", nullable: false),
                    LanguageProficiencyLevel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignLanguage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForeignLanguage_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Education_EmployeeId",
                table: "Education",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeExperience_EmployeeId",
                table: "EmployeeExperience",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignLanguage_EmployeeId",
                table: "ForeignLanguage",
                column: "EmployeeId");
        }
    }
}
