# SIAKAD Keuangan API

SIAKAD Keuangan API adalah sebuah RESTful Web API berbasis **ASP.NET Core (.NET 8)** yang dirancang untuk mengelola data tagihan dan riwayat pembayaran Uang Kuliah Tunggal (UKT) mahasiswa. Sistem ini memiliki kemampuan sinkronisasi otomatis data mahasiswa dari API eksternal serta mendukung pencatatan pembayaran menggunakan berbagai metode bayar.

---

## Fitur Utama

1. **Sinkronisasi Data Mahasiswa Otomatis (`Sync Service`)**:
   - Menghubungkan ke API eksternal (`https://mahasiswa-api-psi.vercel.app/api/mahasiswa`).
   - Menyimpan data mahasiswa baru secara otomatis ke database lokal.
   - Melakukan *soft-delete* (`IsDeleted = true`) pada mahasiswa lokal jika data mereka sudah tidak ditemukan di API eksternal.
   - Memulihkan data mahasiswa secara otomatis jika data mereka kembali aktif di API eksternal.
2. **Manajemen Tarif UKT Default**:
   - Memberikan nominal UKT default secara otomatis berdasarkan Program Studi saat pertama kali disinkronisasi:
     - **Teknik Informatika**: Rp 6.000.000
     - **Sistem Informasi**: Rp 5.500.000
     - **Teknologi Informasi**: Rp 5.800.000
     - **Program Studi Lain**: Rp 5.000.000
   - Mendukung pembaruan nominal UKT secara manual per mahasiswa melalui endpoint API.
3. **Pencatatan & Riwayat Pembayaran**:
   - Mencatat setiap transaksi pembayaran UKT mahasiswa.
   - Mendukung berbagai metode pembayaran (seperti Transfer Bank, E-Wallet, QRIS, dan Tunai).
   - Menghitung sisa tagihan secara real-time dan memperbarui status tagihan (`Lunas`, `Belum Lunas`, atau `Belum Lunas (Kurang [Sisa Tagihan])`).

---

## Teknologi yang Digunakan

* **Runtime & Framework**: .NET 8 (ASP.NET Core Web API)
* **Database Provider**: PostgreSQL (via Npgsql EF Core Provider)
* **ORM**: Entity Framework Core (EF Core)
* **Dokumentasi API**: Swagger / OpenAPI (Swashbuckle)
* **HTTP Client**: `HttpClient` (terintegrasi dengan `SyncService`)

---

## Struktur Proyek

```text
SIAKAD-Keuangan-Api/
├── Controllers/
│   └── KeuanganController.cs         # Endpoint API untuk Keuangan & Sinkronisasi
├── Data/
│   └── AppDbContext.cs               # Context Database & Konfigurasi Seeding Data
├── Models/
│   ├── DTOs.cs                       # Data Transfer Objects untuk Request/Response
│   ├── Mahasiswa.cs                  # Model Entity Mahasiswa
│   ├── MetodePembayaran.cs           # Model Entity Metode Pembayaran
│   └── RiwayatPembayaran.cs          # Model Entity Riwayat Pembayaran
├── Program.cs                        # Entry Point Aplikasi & Registrasi Services
├── appsettings.json                  # Konfigurasi Database Connection String
└── UserSyncApi.csproj                # File Definisi Dependensi & Target Project
```

---

## Relasi Database

Aplikasi menggunakan database PostgreSQL dengan relasi sebagai berikut:
1. **Mahasiswa (1) ── (N) RiwayatPembayaran**: Satu mahasiswa dapat memiliki banyak riwayat transaksi pembayaran. Penghapusan mahasiswa secara fisik (*hard-delete*) dikonfigurasi dengan *Cascade Delete*.
2. **MetodePembayaran (1) ── (N) RiwayatPembayaran**: Setiap riwayat pembayaran dikaitkan dengan satu metode pembayaran tertentu.

Metode pembayaran berikut akan ter-*seed* secara otomatis saat inisialisasi database:
* `1` - Transfer Bank (TRF)
* `2` - E-Wallet (EWLT)
* `3` - QRIS (QRIS)
* `4` - Tunai (CASH)

---

## Cara Menjalankan Project

### 1. Prasyarat
Pastikan Anda sudah menginstal:
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [PostgreSQL](https://www.postgresql.org/download/)

### 2. Kloning Repositori
```bash
git clone https://github.com/dimasrofipurnomo/SIAKAD-Keuangan-Api.git
cd SIAKAD-Keuangan-Api/SIAKAD-Keuangan-Api
```

### 3. Konfigurasi Connection String
Sesuaikan konfigurasi koneksi database PostgreSQL Anda pada berkas [appsettings.json](file:///d:/KULIAH/SEMESTER%204/PAA/SIAKAD-Keuangan-Api/SIAKAD-Keuangan-Api/appsettings.json):
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=usersyncdb;Username=postgres;Password=YOUR_PASSWORD"
}
```

### Jalankan Migrasi Database
Buka terminal pada direktori proyek dan jalankan perintah EF Core untuk membuat schema database:
```bash
dotnet ef database update
```
*(Catatan: Pastikan Anda telah menginstal tool EF Core secara global dengan `dotnet tool install -g dotnet-ef`)*

### 5. Jalankan Aplikasi
Jalankan aplikasi dengan perintah:
```bash
dotnet run
```
Aplikasi akan berjalan secara default di `http://localhost:5000` atau `https://localhost:5001`. Anda dapat mengakses dokumentasi Swagger UI di browser melalui alamat:
`https://localhost:5001/swagger/index.html` (atau port HTTP/HTTPS yang tertera di terminal saat dijalankan).