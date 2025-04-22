using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Status_To_AppUserParkingSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Identity",
                table: "UserParkingSubscriptions");
        }
    }
}
