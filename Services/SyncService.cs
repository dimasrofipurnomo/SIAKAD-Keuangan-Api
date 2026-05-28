using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UserSyncApi.Models;
using UserSyncApi.Data;

namespace UserSyncApi.Services
{
    public class SyncService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly ILogger<SyncService> _logger;

        public SyncService(HttpClient httpClient, AppDbContext context, ILogger<SyncService> logger)
        {
            _httpClient = httpClient;
            _context = context;
            _logger = logger;
        }

        public async Task SyncMahasiswaAsync()
        {
            _logger.LogInformation("Starting synchronization with Mahasiswa API...");
            
            var response = await _httpClient.GetAsync("https://mahasiswa-api-psi.vercel.app/api/mahasiswa");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<MahasiswaApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse == null || apiResponse.Data == null)
            {
                _logger.LogWarning("Failed to deserialize Mahasiswa API response or data is null.");
                return;
            }

            var externalIds = apiResponse.Data.Select(x => x.Id).ToList();

            var deletedStudents = await _context.Mahasiswas
                .Where(m => !externalIds.Contains(m.Id))
                .ToListAsync();

            if (deletedStudents.Any())
            {
                foreach (var student in deletedStudents)
                {
                    student.IsDeleted = true;
                    student.DeletedAt = DateTime.UtcNow;
                }
                _logger.LogInformation($"Soft-deleted {deletedStudents.Count} local student records not present in the external API.");
            }

            foreach (var item in apiResponse.Data)
            {
                var existing = await _context.Mahasiswas
                    .IgnoreQueryFilters()
                    .Include(m => m.RiwayatPembayaran)
                    .FirstOrDefaultAsync(m => m.Id == item.Id);

                if (existing == null)
                {
                    var newMahasiswa = new Mahasiswa
                    {
                        Id = item.Id,
                        Nama = item.Nama,
                        ProgramStudi = item.ProgramStudi,
                        MataKuliah = item.MataKuliah,
                        StatusAkademik = item.StatusAkademik,
                        NilaiUkt = GetDefaultUktForProdi(item.ProgramStudi),
                        StatusTagihan = "Belum Lunas",
                        SyncedAt = DateTime.UtcNow,
                        IsDeleted = false,
                        DeletedAt = null
                    };
                    _context.Mahasiswas.Add(newMahasiswa);
                    _logger.LogInformation($"Adding new mahasiswa: {item.Nama} ({item.Id})");
                }
                else
                {
                    if (existing.IsDeleted)
                    {
                        existing.IsDeleted = false;
                        existing.DeletedAt = null;
                        _logger.LogInformation($"Restoring soft-deleted mahasiswa: {item.Nama} ({item.Id})");
                    }

                    existing.Nama = item.Nama;
                    existing.ProgramStudi = item.ProgramStudi;
                    existing.MataKuliah = item.MataKuliah;
                    existing.StatusAkademik = item.StatusAkademik;
                    existing.SyncedAt = DateTime.UtcNow;

                    UpdateStatusTagihan(existing);
                    _logger.LogInformation($"Updating existing mahasiswa: {item.Nama} ({item.Id})");
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Synchronization completed successfully.");
        }

        private decimal GetDefaultUktForProdi(string prodi)
        {
            return prodi.ToLower() switch
            {
                "teknik informatika" => 6000000m,
                "sistem informasi" => 5500000m,
                "teknologi informasi" => 5800000m,
                _ => 5000000m
            };
        }

        public void UpdateStatusTagihan(Mahasiswa mahasiswa)
        {
            var totalTerbayar = mahasiswa.RiwayatPembayaran.Sum(p => p.JumlahBayar);
            if (totalTerbayar >= mahasiswa.NilaiUkt)
            {
                mahasiswa.StatusTagihan = "Lunas";
            }
            else if (totalTerbayar > 0)
            {
                mahasiswa.StatusTagihan = $"Belum Lunas (Kurang {(mahasiswa.NilaiUkt - totalTerbayar):N0})";
            }
            else
            {
                mahasiswa.StatusTagihan = "Belum Lunas";
            }
        }
    }
}