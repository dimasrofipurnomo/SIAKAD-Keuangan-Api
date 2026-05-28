using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserSyncApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToMahasiswa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MahasiswaId",
                table: "RiwayatPembayarans",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Mahasiswas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Mahasiswas",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Mahasiswas");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Mahasiswas");

            migrationBuilder.AlterColumn<string>(
                name: "MahasiswaId",
                table: "RiwayatPembayarans",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
