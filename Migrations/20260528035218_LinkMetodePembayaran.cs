using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserSyncApi.Migrations
{
    /// <inheritdoc />
    public partial class LinkMetodePembayaran : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetodePembayaran",
                table: "RiwayatPembayarans");

            migrationBuilder.AddColumn<int>(
                name: "MetodePembayaranId",
                table: "RiwayatPembayarans",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MetodePembayarans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NamaMetode = table.Column<string>(type: "text", nullable: false),
                    Kode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodePembayarans", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MetodePembayarans",
                columns: new[] { "Id", "Kode", "NamaMetode" },
                values: new object[,]
                {
                    { 1, "TRF", "Transfer Bank" },
                    { 2, "EWLT", "E-Wallet" },
                    { 3, "QRIS", "QRIS" },
                    { 4, "CASH", "Tunai" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RiwayatPembayarans_MetodePembayaranId",
                table: "RiwayatPembayarans",
                column: "MetodePembayaranId");

            migrationBuilder.AddForeignKey(
                name: "FK_RiwayatPembayarans_MetodePembayarans_MetodePembayaranId",
                table: "RiwayatPembayarans",
                column: "MetodePembayaranId",
                principalTable: "MetodePembayarans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RiwayatPembayarans_MetodePembayarans_MetodePembayaranId",
                table: "RiwayatPembayarans");

            migrationBuilder.DropTable(
                name: "MetodePembayarans");

            migrationBuilder.DropIndex(
                name: "IX_RiwayatPembayarans_MetodePembayaranId",
                table: "RiwayatPembayarans");

            migrationBuilder.DropColumn(
                name: "MetodePembayaranId",
                table: "RiwayatPembayarans");

            migrationBuilder.AddColumn<string>(
                name: "MetodePembayaran",
                table: "RiwayatPembayarans",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
