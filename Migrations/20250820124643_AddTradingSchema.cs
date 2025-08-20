using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIStockRadar.Migrations
{
    /// <inheritdoc />
    public partial class AddTradingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Securities",
                columns: table => new
                {
                    SecurityId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ticker = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Securities", x => x.SecurityId);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    TradeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    SecurityId = table.Column<int>(type: "INTEGER", nullable: false),
                    TradeDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    Side = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.TradeId);
                    table.CheckConstraint("CK_Trade_Price_NonNegative", "Price >= 0");
                    table.CheckConstraint("CK_Trade_Quantity_Positive", "Quantity > 0");
                    table.ForeignKey(
                        name: "FK_Trades_Securities_SecurityId",
                        column: x => x.SecurityId,
                        principalTable: "Securities",
                        principalColumn: "SecurityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trades_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserHoldings",
                columns: table => new
                {
                    HoldingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    SecurityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    AvgCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHoldings", x => x.HoldingId);
                    table.CheckConstraint("CK_UserHolding_Quantity_NonNegative", "Quantity >= 0");
                    table.ForeignKey(
                        name: "FK_UserHoldings_Securities_SecurityId",
                        column: x => x.SecurityId,
                        principalTable: "Securities",
                        principalColumn: "SecurityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserHoldings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Securities_Ticker",
                table: "Securities",
                column: "Ticker",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trades_SecurityId",
                table: "Trades",
                column: "SecurityId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_UserId_SecurityId_TradeDate",
                table: "Trades",
                columns: new[] { "UserId", "SecurityId", "TradeDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserHoldings_SecurityId",
                table: "UserHoldings",
                column: "SecurityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHoldings_UserId_SecurityId",
                table: "UserHoldings",
                columns: new[] { "UserId", "SecurityId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "UserHoldings");

            migrationBuilder.DropTable(
                name: "Securities");
        }
    }
}
