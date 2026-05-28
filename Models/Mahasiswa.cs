using System.ComponentModel.DataAnnotations;

namespace UserSyncApi.Models
{
    public class Mahasiswa
    {
        [Key]
        public string Id { get; set; } = string.Empty; 

        public string Nama { get; set; } = string.Empty;

        public string ProgramStudi { get; set; } = string.Empty;

        public List<string> MataKuliah { get; set; } = new();

        public string StatusAkademik { get; set; } = string.Empty;

        public decimal NilaiUkt { get; set; }

        public string StatusTagihan { get; set; } = "Belum Lunas"; 

        public DateTime SyncedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public List<RiwayatPembayaran> RiwayatPembayaran { get; set; } = new();
    }
}
