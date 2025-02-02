using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Employees");
        }
    }
}
