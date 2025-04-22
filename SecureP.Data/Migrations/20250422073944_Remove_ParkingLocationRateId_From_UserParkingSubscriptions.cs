using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Remove_ParkingLocationRateId_From_UserParkingSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserParkingSubscriptions_ParkingLocationRates_ParkingLocationRateId",
                schema: "Identity",
                table: "UserParkingSubscriptions");

            migrationBuilder.RenameColumn(
                name: "ParkingLocationRateId",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                newName: "ParkingLocationRate<string>Id");

            migrationBuilder.RenameIndex(
                name: "IX_UserParkingSubscriptions_ParkingLocationRateId",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                newName: "IX_UserParkingSubscriptions_ParkingLocationRate<string>Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserParkingSubscriptions_ParkingLocationRates_ParkingLocationRate<string>Id",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                column: "ParkingLocationRate<string>Id",
                principalSchema: "Identity",
                principalTable: "ParkingLocationRates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserParkingSubscriptions_ParkingLocationRates_ParkingLocationRate<string>Id",
                schema: "Identity",
                table: "UserParkingSubscriptions");

            migrationBuilder.RenameColumn(
                name: "ParkingLocationRate<string>Id",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                newName: "ParkingLocationRateId");

            migrationBuilder.RenameIndex(
                name: "IX_UserParkingSubscriptions_ParkingLocationRate<string>Id",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                newName: "IX_UserParkingSubscriptions_ParkingLocationRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserParkingSubscriptions_ParkingLocationRates_ParkingLocationRateId",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                column: "ParkingLocationRateId",
                principalSchema: "Identity",
                principalTable: "ParkingLocationRates",
                principalColumn: "Id");
        }
    }
}
