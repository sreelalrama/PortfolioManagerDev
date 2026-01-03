using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTradePro.StockData.API.Migrations
{
    /// <inheritdoc />
    public partial class FixesForUtcTimeDynamicValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Stocks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 14, 22, 3, 59, 392, DateTimeKind.Utc).AddTicks(5736));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Stocks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 14, 22, 3, 59, 392, DateTimeKind.Utc).AddTicks(4713));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                table: "StockPrices",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 14, 22, 3, 59, 396, DateTimeKind.Utc).AddTicks(3320));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                table: "MarketData",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 14, 22, 3, 59, 397, DateTimeKind.Utc).AddTicks(1282));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Stocks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 14, 22, 3, 59, 392, DateTimeKind.Utc).AddTicks(5736),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Stocks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 14, 22, 3, 59, 392, DateTimeKind.Utc).AddTicks(4713),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                table: "StockPrices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 14, 22, 3, 59, 396, DateTimeKind.Utc).AddTicks(3320),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                table: "MarketData",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 14, 22, 3, 59, 397, DateTimeKind.Utc).AddTicks(1282),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
