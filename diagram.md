```mermaid
erDiagram
    MAHASISWA ||--o{ RIWAYAT_PEMBAYARAN : memiliki

    MAHASISWA {
        string Id PK
        string Nama
        string ProgramStudi
        string MataKuliah
        string StatusAkademik
        decimal NilaiUkt
        string StatusTagihan
        DateTime SyncedAt
        bool IsDeleted
        DateTime DeletedAt
    }

    RIWAYAT_PEMBAYARAN {
        int Id PK
        string MahasiswaId FK
        decimal JumlahBayar
        DateTime TanggalBayar
        string MetodePembayaran
    }
```