using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResponseMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_isClosed_column_to_interview_invitation_and_hangfire_database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "InterviewInvitations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "InterviewInvitations");
        }
    }
}
