using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    WorkingState = table.Column<string>(type: "text", nullable: false),
                    WorkingTime = table.Column<string>(type: "text", nullable: false),
                    Advantages = table.Column<string>(type: "text", nullable: false),
                    CanBeImproved = table.Column<string>(type: "text", nullable: false),
                    WorkingConditions = table.Column<int>(type: "integer", nullable: false),
                    Colleagues = table.Column<int>(type: "integer", nullable: false),
                    Management = table.Column<int>(type: "integer", nullable: false),
                    GrowthOpportunities = table.Column<int>(type: "integer", nullable: false),
                    RestConditions = table.Column<int>(type: "integer", nullable: false),
                    SalaryLevel = table.Column<int>(type: "integer", nullable: false),
                    GeneralEstimation = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");
        }
    }
}
