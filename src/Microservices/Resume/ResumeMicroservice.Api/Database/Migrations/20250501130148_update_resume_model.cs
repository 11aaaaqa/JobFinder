using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class update_resume_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkingFromMonth",
                table: "EmployeeExperience");

            migrationBuilder.DropColumn(
                name: "WorkingFromYear",
                table: "EmployeeExperience");

            migrationBuilder.RenameColumn(
                name: "WorkingUntilYear",
                table: "EmployeeExperience",
                newName: "WorkingUntil");

            migrationBuilder.RenameColumn(
                name: "WorkingUntilMonth",
                table: "EmployeeExperience",
                newName: "WorkingFrom");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Resumes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "OccupationTypes",
                table: "Resumes",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "WorkTypes",
                table: "Resumes",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkingExperience",
                table: "Resumes",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkingDuration",
                table: "EmployeeExperience",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "OccupationTypes",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "WorkTypes",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "WorkingExperience",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "WorkingDuration",
                table: "EmployeeExperience");

            migrationBuilder.RenameColumn(
                name: "WorkingUntil",
                table: "EmployeeExperience",
                newName: "WorkingUntilYear");

            migrationBuilder.RenameColumn(
                name: "WorkingFrom",
                table: "EmployeeExperience",
                newName: "WorkingUntilMonth");

            migrationBuilder.AddColumn<string>(
                name: "WorkingFromMonth",
                table: "EmployeeExperience",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkingFromYear",
                table: "EmployeeExperience",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
