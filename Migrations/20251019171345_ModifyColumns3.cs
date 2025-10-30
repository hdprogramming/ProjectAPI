using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class ModifyColumns3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_projectStatusMsgs_StatusID",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "StatusID",
                table: "Projects",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_StatusID",
                table: "Projects",
                newName: "IX_Projects_StatusId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_projectStatusMsgs_StatusId",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Projects",
                newName: "StatusID");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_StatusId",
                table: "Projects",
                newName: "IX_Projects_StatusID");

            migrationBuilder.AlterColumn<int>(
                name: "StatusID",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_projectStatusMsgs_StatusID",
                table: "Projects",
                column: "StatusID",
                principalTable: "projectStatusMsgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
