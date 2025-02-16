using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployerMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeColleaguesCountColumnString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CompanyColleaguesCount",
                table: "Companies",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "CompanyColleaguesCount",
                table: "Companies",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
