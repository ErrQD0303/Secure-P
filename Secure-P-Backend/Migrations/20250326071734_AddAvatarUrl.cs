using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Secure_P_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                schema: "Identity",
                table: "AspNetUsers");
        }
    }
}
