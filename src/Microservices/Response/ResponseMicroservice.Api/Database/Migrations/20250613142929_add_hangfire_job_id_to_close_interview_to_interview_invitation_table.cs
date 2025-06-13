using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResponseMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_hangfire_job_id_to_close_interview_to_interview_invitation_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HangfireDelayedJobId",
                table: "InterviewInvitations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HangfireDelayedJobId",
                table: "InterviewInvitations");
        }
    }
}
