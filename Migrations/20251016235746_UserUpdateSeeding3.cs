using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserUpdateSeeding3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b22698b8-42a2-4115-9631-1c2d1e2ac5f7"),
                columns: new[] { "PasswordHashed", "isRole" },
                values: new object[] { "AQAAAAIAAYagAAAAEDiRNCYoDBz1VQzr+rtj7cicug4dhAXqqYyxUBgdawRxt8dSOMpIGW+KTVm2m3YsUQ==", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b22698b8-42a2-4115-9631-1c2d1e2ac5f7"),
                columns: new[] { "PasswordHashed", "isRole" },
                values: new object[] { "AQAAAAIAAYagAAAAELkAZqnPHL2P6A9odPb4+6uW7wvkO7UcLfeWrydc+r2BDvI3x5s1DcNwp2tZXvXdyg==", "User" });
        }
    }
}
