using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Change_Delete_Behavior_Of_Parking_Location : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingLocationRates_ParkingLocations_ParkingLocationId",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingLocationRates_ParkingLocations_ParkingLocationId",
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
        }
    }
}
