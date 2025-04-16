using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_ConcurrencyStamp_To_ParkingRate_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                schema: "Identity",
                table: "ParkingRates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                schema: "Identity",
                table: "ParkingRates");
        }
    }
}
