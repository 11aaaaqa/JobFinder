using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_connection_between_resume_model_and_children : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Education_Resumes_ResumeId",
                table: "Education");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeExperience_Resumes_ResumeId",
                table: "EmployeeExperience");

            migrationBuilder.DropForeignKey(
                name: "FK_ForeignLanguage_Resumes_ResumeId",
                table: "ForeignLanguage");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "ForeignLanguage",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "EmployeeExperience",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Education",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Education_Resumes_ResumeId",
                table: "Education",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeExperience_Resumes_ResumeId",
                table: "EmployeeExperience",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForeignLanguage_Resumes_ResumeId",
                table: "ForeignLanguage",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Education_Resumes_ResumeId",
                table: "Education");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeExperience_Resumes_ResumeId",
                table: "EmployeeExperience");

            migrationBuilder.DropForeignKey(
                name: "FK_ForeignLanguage_Resumes_ResumeId",
                table: "ForeignLanguage");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "ForeignLanguage",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "EmployeeExperience",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Education",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Education_Resumes_ResumeId",
                table: "Education",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeExperience_Resumes_ResumeId",
                table: "EmployeeExperience",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForeignLanguage_Resumes_ResumeId",
                table: "ForeignLanguage",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id");
        }
    }
}
