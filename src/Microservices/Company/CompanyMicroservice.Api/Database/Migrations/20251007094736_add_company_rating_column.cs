using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_company_rating_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Companies",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Companies");
        }
    }
}
