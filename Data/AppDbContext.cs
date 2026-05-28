using Microsoft.EntityFrameworkCore;
using UserSyncApi.Models;

namespace UserSyncApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Mahasiswa> Mahasiswas => Set<Mahasiswa>();
        public DbSet<RiwayatPembayaran> RiwayatPembayarans => Set<RiwayatPembayaran>();
        public DbSet<MetodePembayaran> MetodePembayarans => Set<MetodePembayaran>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Mahasiswa>()
                .HasMany(m => m.RiwayatPembayaran)
                .WithOne(p => p.Mahasiswa)
                .HasForeignKey(p => p.MahasiswaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Mahasiswa>()
                .HasQueryFilter(m => !m.IsDeleted);

            modelBuilder.Entity<MetodePembayaran>().HasData(
                new MetodePembayaran { Id = 1, NamaMetode = "Transfer Bank", Kode = "TRF" },
                new MetodePembayaran { Id = 2, NamaMetode = "E-Wallet", Kode = "EWLT" },
                new MetodePembayaran { Id = 3, NamaMetode = "QRIS", Kode = "QRIS" },
                new MetodePembayaran { Id = 4, NamaMetode = "Tunai", Kode = "CASH" }
            );
        }
    }
}