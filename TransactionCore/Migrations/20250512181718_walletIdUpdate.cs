using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionCore.Migrations
{
    /// <inheritdoc />
    public partial class walletIdUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReferralPayments");

            migrationBuilder.AddColumn<decimal>(
                name: "PayedFee",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TxHash",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "Networks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "Cryptos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayedFee",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TxHash",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "Networks");

            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "Cryptos");

            migrationBuilder.CreateTable(
                name: "ReferralPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferrerUserInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Commission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferralPayments_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReferralPayments_UserInfos_ReferrerUserInfoId",
                        column: x => x.ReferrerUserInfoId,
                        principalTable: "UserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReferralPayments_PaymentId",
                table: "ReferralPayments",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralPayments_ReferrerUserInfoId",
                table: "ReferralPayments",
                column: "ReferrerUserInfoId");
        }
    }
}
