using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserUpdateSeeding2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("615411e1-f506-4ccf-8a7c-223089ec024c"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "EMail", "PasswordHashed", "ProfileImageUrl", "UserName", "isRole" },
                values: new object[] { new Guid("b22698b8-42a2-4115-9631-1c2d1e2ac5f7"), "admin@projectapi.com", "AQAAAAIAAYagAAAAELkAZqnPHL2P6A9odPb4+6uW7wvkO7UcLfeWrydc+r2BDvI3x5s1DcNwp2tZXvXdyg==", "", "admin", "User" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b22698b8-42a2-4115-9631-1c2d1e2ac5f7"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "EMail", "PasswordHashed", "ProfileImageUrl", "UserName", "isRole" },
                values: new object[] { new Guid("615411e1-f506-4ccf-8a7c-223089ec024c"), "admin@projectapi.com", "AQAAAAIAAYagAAAAEPIa3nkiF1KzYQqr/6eLG53MgOZ7mvbAK3+XlS4PB8/3szW/ja9RL7LAhr2ro6683w==", "", "admin", "User" });
        }
    }
}
