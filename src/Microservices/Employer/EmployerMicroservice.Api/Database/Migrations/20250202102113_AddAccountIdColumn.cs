using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployerMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Employers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Employers");
        }
    }
}
