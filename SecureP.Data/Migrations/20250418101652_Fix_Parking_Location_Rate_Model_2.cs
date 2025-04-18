using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Parking_Location_Rate_Model_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingLocationRates_ParkingLocations_ParkingLocationId",
                schema: "Identity",
                table: "ParkingLocationRates");

            migrationBuilder.DropForeignKey(
                name: "FK_ParkingLocationRates_ParkingRates_ParkingRateId",
                schema: "Identity",
                table: "ParkingLocationRates");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingLocationRates_ParkingLocations_ParkingLocationId",
                schema: "Identity",
                table: "ParkingLocationRates",
                column: "ParkingLocationId",
                principalSchema: "Identity",
                principalTable: "ParkingLocations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingLocationRates_ParkingRates_ParkingRateId",
                schema: "Identity",
                table: "ParkingLocationRates",
                column: "ParkingRateId",
                principalSchema: "Identity",
                principalTable: "ParkingRates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingLocationRates_ParkingLocations_ParkingLocationId",
                schema: "Identity",
                table: "ParkingLocationRates");

            migrationBuilder.DropForeignKey(
                name: "FK_ParkingLocationRates_ParkingRates_ParkingRateId",
                schema: "Identity",
                table: "ParkingLocationRates");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingLocationRates_ParkingLocations_ParkingLocationId",
                schema: "Identity",
                table: "ParkingLocationRates",
                column: "ParkingLocationId",
                principalSchema: "Identity",
                principalTable: "ParkingLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingLocationRates_ParkingRates_ParkingRateId",
                schema: "Identity",
                table: "ParkingLocationRates",
                column: "ParkingRateId",
                principalSchema: "Identity",
                principalTable: "ParkingRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
