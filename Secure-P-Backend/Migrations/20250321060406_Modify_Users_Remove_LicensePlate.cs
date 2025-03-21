using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Secure_P_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Modify_Users_Remove_LicensePlate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicensePlateNumber",
                schema: "Identity",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LicensePlateNumber",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
