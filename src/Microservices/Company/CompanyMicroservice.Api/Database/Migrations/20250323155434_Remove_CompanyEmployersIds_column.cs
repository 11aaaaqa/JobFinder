using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class Remove_CompanyEmployersIds_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyEmployersIds",
                table: "Companies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<Guid>>(
                name: "CompanyEmployersIds",
                table: "Companies",
                type: "uuid[]",
                nullable: false);
        }
    }
}
