using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Remove_ParkingLocationRateId_From_UserParkingSubscriptions_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserParkingSubscriptions_ParkingLocationRates_ParkingLocationRate<string>Id",
                schema: "Identity",
                table: "UserParkingSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserParkingSubscriptions_ParkingLocationRate<string>Id",
                schema: "Identity",
                table: "UserParkingSubscriptions");

            migrationBuilder.DropColumn(
                name: "ParkingLocationRate<string>Id",
                schema: "Identity",
                table: "UserParkingSubscriptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParkingLocationRate<string>Id",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserParkingSubscriptions_ParkingLocationRate<string>Id",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                column: "ParkingLocationRate<string>Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserParkingSubscriptions_ParkingLocationRates_ParkingLocationRate<string>Id",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                column: "ParkingLocationRate<string>Id",
                principalSchema: "Identity",
                principalTable: "ParkingLocationRates",
                principalColumn: "Id");
        }
    }
}
