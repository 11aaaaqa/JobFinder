using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddJoiningRequestedEmployersTableAndEmployerIdsListInCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<Guid>>(
                name: "CompanyEmployersIds",
                table: "Companies",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "JoiningRequestedEmployers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployerId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployerName = table.Column<string>(type: "text", nullable: false),
                    EmployerSurname = table.Column<string>(type: "text", nullable: false),
                    JoiningRequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoiningRequestedEmployers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoiningRequestedEmployers");

            migrationBuilder.DropColumn(
                name: "CompanyEmployersIds",
                table: "Companies");
        }
    }
}
