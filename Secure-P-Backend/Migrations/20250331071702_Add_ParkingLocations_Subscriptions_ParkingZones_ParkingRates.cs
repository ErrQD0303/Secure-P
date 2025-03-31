using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Secure_P_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Add_ParkingLocations_Subscriptions_ParkingZones_ParkingRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParkingLocations",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AvailableSpaces = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParkingRates",
                schema: "Identity",
                columns: table => new
                {
                    ParkingLocationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HourlyRate = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    DailyRate = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    MonthlyRate = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingRates", x => x.ParkingLocationId);
                    table.ForeignKey(
                        name: "FK_ParkingRates_ParkingLocations_ParkingLocationId",
                        column: x => x.ParkingLocationId,
                        principalSchema: "Identity",
                        principalTable: "ParkingLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParkingZones",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ParkingLocationId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AvailableSpaces = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingZones_ParkingLocations_ParkingLocationId",
                        column: x => x.ParkingLocationId,
                        principalSchema: "Identity",
                        principalTable: "ParkingLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserParkingSubscriptions",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ParkingLocationId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ParkingZoneId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProductType = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubscriptionFee = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    ClampingFee = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    ChangeSignageFee = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(89)", maxLength: 89, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserParkingSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserParkingSubscriptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserParkingSubscriptions_ParkingLocations_ParkingLocationId",
                        column: x => x.ParkingLocationId,
                        principalSchema: "Identity",
                        principalTable: "ParkingLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserParkingSubscriptions_ParkingZones_ParkingZoneId",
                        column: x => x.ParkingZoneId,
                        principalSchema: "Identity",
                        principalTable: "ParkingZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParkingZones_ParkingLocationId",
                schema: "Identity",
                table: "ParkingZones",
                column: "ParkingLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserParkingSubscriptions_ParkingLocationId",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                column: "ParkingLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserParkingSubscriptions_ParkingZoneId",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                column: "ParkingZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_UserParkingSubscriptions_UserId",
                schema: "Identity",
                table: "UserParkingSubscriptions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParkingRates",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserParkingSubscriptions",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "ParkingZones",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "ParkingLocations",
                schema: "Identity");
        }
    }
}
