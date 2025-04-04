using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Change_ClaimValue_Column_Type_In_AspNetRoleClaim_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClaimValue",
                schema: "Identity",
                table: "AspNetRoleClaims",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                schema: "Identity",
                table: "AspNetRoleClaims",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
