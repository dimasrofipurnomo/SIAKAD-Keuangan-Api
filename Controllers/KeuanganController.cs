using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserSyncApi.Data;
using UserSyncApi.Models;
using UserSyncApi.Services;

namespace UserSyncApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KeuanganController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly SyncService _syncService;

        public KeuanganController(AppDbContext context, SyncService syncService)
        {
            _context = context;
            _syncService = syncService;
        }

        [HttpGet("/api/metode-pembayaran")]
        public async Task<IActionResult> GetMetodePembayaran()
        {
            var data = await _context.MetodePembayarans.OrderBy(m => m.Id).ToListAsync();
            return Ok(new
            {
                success = true,
                count = data.Count,
                data = data
            });
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {
            try
            {
                await _syncService.SyncMahasiswaAsync();
                return Ok(new 
                { 
                    success = true, 
                    message = "Sinkronisasi data mahasiswa berhasil dilakukan." 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = $"Gagal sinkronisasi: {ex.Message}" 
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Mahasiswas
                .Include(m => m.RiwayatPembayaran).ThenInclude(r => r.MetodePembayaran)
                .OrderBy(m => m.Nama)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = data.Count,
                data = data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var mahasiswa = await _context.Mahasiswas
                .Include(m => m.RiwayatPembayaran).ThenInclude(r => r.MetodePembayaran)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mahasiswa == null)
            {
                return NotFound(new 
                { 
                    success = false, 
                    message = "Data mahasiswa tidak ditemukan." 
                });
            }

            return Ok(new
            {
                success = true,
                data = mahasiswa
            });
        }

        [HttpPut("{id}/ukt")]
        public async Task<IActionResult> UpdateUkt(string id, [FromBody] UpdateUktRequest request)
        {
            if (request.NilaiUkt < 0)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = "Nilai UKT tidak boleh bernilai negatif." 
                });
            }

            var mahasiswa = await _context.Mahasiswas
                .Include(m => m.RiwayatPembayaran).ThenInclude(r => r.MetodePembayaran)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mahasiswa == null)
            {
                return NotFound(new 
                { 
                    success = false, 
                    message = "Data mahasiswa tidak ditemukan." 
                });
            }

            mahasiswa.NilaiUkt = request.NilaiUkt;
            _syncService.UpdateStatusTagihan(mahasiswa);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Nilai UKT berhasil diperbarui.",
                data = mahasiswa
            });
        }

        [HttpPost("{id}/bayar")]
        public async Task<IActionResult> BayarUkt(string id, [FromBody] PaymentRequest request)
        {
            if (request.JumlahBayar <= 0)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = "Jumlah pembayaran harus lebih besar dari 0." 
                });
            }

            var mahasiswa = await _context.Mahasiswas
                .Include(m => m.RiwayatPembayaran).ThenInclude(r => r.MetodePembayaran)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mahasiswa == null)
            {
                return NotFound(new 
                { 
                    success = false, 
                    message = "Data mahasiswa tidak ditemukan." 
                });
            }

            var metode = await _context.MetodePembayarans.FindAsync(request.MetodePembayaranId);
            if (metode == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "ID Metode pembayaran tidak valid."
                });
            }

            var pembayaran = new RiwayatPembayaran
            {
                MahasiswaId = id,
                JumlahBayar = request.JumlahBayar,
                TanggalBayar = DateTime.UtcNow,
                MetodePembayaranId = request.MetodePembayaranId,
                MetodePembayaran = metode
            };

            mahasiswa.RiwayatPembayaran.Add(pembayaran);
            _syncService.UpdateStatusTagihan(mahasiswa);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Pembayaran berhasil dicatat.",
                data = new
                {
                    mahasiswaId = mahasiswa.Id,
                    nama = mahasiswa.Nama,
                    nilaiUkt = mahasiswa.NilaiUkt,
                    statusTagihan = mahasiswa.StatusTagihan,
                    pembayaranTerakhir = new
                    {
                        pembayaran.Id,
                        pembayaran.MahasiswaId,
                        pembayaran.JumlahBayar,
                        pembayaran.TanggalBayar,
                        MetodePembayaran = new
                        {
                            metode.Id,
                            metode.NamaMetode,
                            metode.Kode
                        }
                    }
                }
            });
        }
    }
}
