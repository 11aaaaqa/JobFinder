using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class Remove_PhoneNumber_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_PhoneNumber",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Employees_PhoneNumber",
                table: "Employees",
                column: "PhoneNumber",
                unique: true);
        }
    }
}
