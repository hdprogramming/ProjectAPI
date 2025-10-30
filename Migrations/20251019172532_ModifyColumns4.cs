using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class ModifyColumns4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_projectStatusMsgs_StatusId",
                table: "Projects");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_projectStatusMsgs_StatusId",
                table: "Projects",
                column: "StatusId",
                principalTable: "projectStatusMsgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_projectStatusMsgs_StatusId",
                table: "Projects");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "Projects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_projectStatusMsgs_StatusId",
                table: "Projects",
                column: "StatusId",
                principalTable: "projectStatusMsgs",
                principalColumn: "Id");
        }
    }
}
