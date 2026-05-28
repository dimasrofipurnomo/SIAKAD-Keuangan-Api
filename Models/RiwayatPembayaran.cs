using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserSyncApi.Models
{
    public class RiwayatPembayaran
    {
        [Key]
        public int Id { get; set; }

        public string? MahasiswaId { get; set; }

        public decimal JumlahBayar { get; set; }

        public DateTime TanggalBayar { get; set; } = DateTime.UtcNow;

        public int? MetodePembayaranId { get; set; }

        public MetodePembayaran? MetodePembayaran { get; set; }

        [JsonIgnore]
        public Mahasiswa? Mahasiswa { get; set; }
    }
}
