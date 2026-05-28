using System.Text.Json.Serialization;

namespace UserSyncApi.Models
{
    public class MahasiswaDto
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("nama")]
        public string Nama { get; set; } = string.Empty;

        [JsonPropertyName("programStudi")]
        public string ProgramStudi { get; set; } = string.Empty;

        [JsonPropertyName("mataKuliah")]
        public List<string> MataKuliah { get; set; } = new();

        [JsonPropertyName("statusAkademik")]
        public string StatusAkademik { get; set; } = string.Empty;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class MahasiswaApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("data")]
        public List<MahasiswaDto> Data { get; set; } = new();
    }

    public class PaymentRequest
    {
        public decimal JumlahBayar { get; set; }
        public int MetodePembayaranId { get; set; }
    }

    public class UpdateUktRequest
    {
        public decimal NilaiUkt { get; set; }
    }
}
