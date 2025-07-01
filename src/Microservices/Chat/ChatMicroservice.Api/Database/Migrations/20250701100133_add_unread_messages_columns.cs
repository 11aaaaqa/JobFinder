using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_unread_messages_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeUnreadMessagesCount",
                table: "Chats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployerUnreadMessagesCount",
                table: "Chats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "EmployeeUnreadMessagesCount",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "EmployerUnreadMessagesCount",
                table: "Chats");
        }
    }
}
