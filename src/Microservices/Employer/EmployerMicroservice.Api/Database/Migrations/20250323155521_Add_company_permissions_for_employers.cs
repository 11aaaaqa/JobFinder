using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployerMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class Add_company_permissions_for_employers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployersPermissions",
                columns: table => new
                {
                    EmployerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Permissions = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployersPermissions", x => x.EmployerId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployersPermissions");
        }
    }
}
