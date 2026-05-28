using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UserSyncApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mahasiswas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nama = table.Column<string>(type: "text", nullable: false),
                    ProgramStudi = table.Column<string>(type: "text", nullable: false),
                    MataKuliah = table.Column<List<string>>(type: "text[]", nullable: false),
                    StatusAkademik = table.Column<string>(type: "text", nullable: false),
                    NilaiUkt = table.Column<decimal>(type: "numeric", nullable: false),
                    StatusTagihan = table.Column<string>(type: "text", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mahasiswas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiwayatPembayarans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MahasiswaId = table.Column<string>(type: "text", nullable: false),
                    JumlahBayar = table.Column<decimal>(type: "numeric", nullable: false),
                    TanggalBayar = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MetodePembayaran = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiwayatPembayarans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiwayatPembayarans_Mahasiswas_MahasiswaId",
                        column: x => x.MahasiswaId,
                        principalTable: "Mahasiswas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RiwayatPembayarans_MahasiswaId",
                table: "RiwayatPembayarans",
                column: "MahasiswaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RiwayatPembayarans");

            migrationBuilder.DropTable(
                name: "Mahasiswas");
        }
    }
}
