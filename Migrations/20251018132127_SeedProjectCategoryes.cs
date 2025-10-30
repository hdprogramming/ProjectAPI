using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedProjectCategoryes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "projectCategories",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projectCategories", x => new { x.ProjectId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_projectCategories_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_projectCategories_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Kimya" },
                    { 2, "Fizik" },
                    { 3, "Elektronik" },
                    { 4, "Yazılım" },
                    { 5, "Katıhal Fiziği" },
                    { 6, "Elektrokimya" },
                    { 7, "Kuantum Fiziği" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_projectCategories_CategoryId",
                table: "projectCategories",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "projectCategories");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Projects");
        }
    }
}
