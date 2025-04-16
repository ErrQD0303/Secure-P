using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_AppUserRole_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                schema: "Identity",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                schema: "Identity",
                table: "AspNetUserRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                schema: "Identity",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                schema: "Identity",
                table: "AspNetUserRoles",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                schema: "Identity",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                schema: "Identity",
                table: "AspNetUserRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                schema: "Identity",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                schema: "Identity",
                table: "AspNetUserRoles",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
