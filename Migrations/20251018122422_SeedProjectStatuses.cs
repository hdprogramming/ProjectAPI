using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedProjectStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "projectStatusMsgs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projectStatusMsgs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "projectStatusMsgs",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Başlangıç Aşamasında" },
                    { 2, "Devam Ediyor" },
                    { 3, "Test Aşamasında" },
                    { 4, "Tamamlandı" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_StatusID",
                table: "Projects",
                column: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_projectStatusMsgs_StatusID",
                table: "Projects",
                column: "StatusID",
                principalTable: "projectStatusMsgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_projectStatusMsgs_StatusID",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "projectStatusMsgs");

            migrationBuilder.DropIndex(
                name: "IX_Projects_StatusID",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
