using System.ComponentModel.DataAnnotations;

namespace UserSyncApi.Models
{
    public class MetodePembayaran
    {
        [Key]
        public int Id { get; set; }

        public string NamaMetode { get; set; } = string.Empty;

        public string Kode { get; set; } = string.Empty; 
    }
}
