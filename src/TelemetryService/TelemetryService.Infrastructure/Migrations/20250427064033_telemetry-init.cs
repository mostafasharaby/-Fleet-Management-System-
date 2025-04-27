using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelemetryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class telemetryinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertThresholds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinValue = table.Column<double>(type: "float", nullable: false),
                    MaxValue = table.Column<double>(type: "float", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AlertMessage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertThresholds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Speed = table.Column<double>(type: "float", nullable: false),
                    FuelLevel = table.Column<double>(type: "float", nullable: false),
                    EngineTemperature = table.Column<double>(type: "float", nullable: false),
                    BatteryVoltage = table.Column<double>(type: "float", nullable: false),
                    EngineRpm = table.Column<int>(type: "int", nullable: false),
                    CheckEngineLightOn = table.Column<bool>(type: "bit", nullable: false),
                    OdometerReading = table.Column<double>(type: "float", nullable: false),
                    DiagnosticCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertThresholds_VehicleId",
                table: "AlertThresholds",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryData_Timestamp",
                table: "TelemetryData",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryData_VehicleId",
                table: "TelemetryData",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryData_VehicleId_Timestamp",
                table: "TelemetryData",
                columns: new[] { "VehicleId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertThresholds");

            migrationBuilder.DropTable(
                name: "TelemetryData");
        }
    }
}
