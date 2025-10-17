using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserUpdateSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "EMail", "PasswordHashed", "ProfileImageUrl", "UserName", "isRole" },
                values: new object[] { new Guid("615411e1-f506-4ccf-8a7c-223089ec024c"), "admin@projectapi.com", "AQAAAAIAAYagAAAAEPIa3nkiF1KzYQqr/6eLG53MgOZ7mvbAK3+XlS4PB8/3szW/ja9RL7LAhr2ro6683w==", "", "admin", "User" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("615411e1-f506-4ccf-8a7c-223089ec024c"));
        }
    }
}
