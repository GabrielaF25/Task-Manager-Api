using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Manager_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserTypeToUser2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserType",
                table: "Users",
                newName: "UserRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserRole",
                table: "Users",
                newName: "UserType");
        }
    }
}
