using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangePassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "PasswordHashed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordHashed",
                table: "Users",
                newName: "Password");
        }
    }
}
