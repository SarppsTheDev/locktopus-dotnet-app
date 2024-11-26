using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace passwordvault_dataaccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIDForeignKeyToLoginItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "LoginItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LoginItems_UserId",
                table: "LoginItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoginItems_AspNetUsers_UserId",
                table: "LoginItems",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginItems_AspNetUsers_UserId",
                table: "LoginItems");

            migrationBuilder.DropIndex(
                name: "IX_LoginItems_UserId",
                table: "LoginItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LoginItems");
        }
    }
}
