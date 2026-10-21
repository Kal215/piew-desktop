using Piew.Desktop.Core.Models;

namespace Piew.Desktop.Core.Services;

public static class PromptTemplateCatalog
{
    public static List<PromptItem> GetCanonicalTemplates()
    {
        return new List<PromptItem>
        {
            new()
            {
                Id = "template-architect",
                Title = "Senior Software Architect: Modular Monolith",
                Description = "Standar rancang bangun arsitektur modular bersih dengan pemisahan domain yang ketat.",
                Category = "Architecture",
                Tags = new() { "Architecture", "DDD", "Modular", "TypeScript" },
                Role = "Anda adalah Chief Software Architect dengan pengalaman lebih dari 15 tahun dalam merancang arsitektur monolit modular skala enterprise.",
                Context = "Sistem sedang dikembangkan untuk menangani volume transaksi tinggi dengan aturan domain kompleks tanpa mikroservis terdistribusi yang membebani.",
                Task = "Rancang struktur modul, kontrak data terisolasi, dan pola interaksi antar-layanan yang menjamin ketergantungan bersifat satu arah (unidirectional).",
                NegativeConstraints = "1. DILARANG membuat ketergantungan siklik (circular dependency).\n2. DILARANG menggunakan global mutable state.\n3. DILARANG membocorkan detail implementasi internal modul ke modul lain.",
                OutputFormat = "Sajikan dalam bentuk dokumen spesifikasi arsitektur: (1) Domain Boundary Map, (2) Kontrak Data Antar-Modul, (3) Strategi Pengujian Isolasi.",
                IsFavorite = true
            },
            new()
            {
                Id = "template-supervisor",
                Title = "Empirical Code Supervisor & Quality Gatekeeper",
                Description = "Pengawas otonom untuk memeriksa modifikasi file, git diff, dan verifikasi kompilasi lokal.",
                Category = "Engineering",
                Tags = new() { "Supervisor", "Audit", "Git", "Quality" },
                Role = "Anda adalah Pengawas Kualitas Kode Empiris (Empirical Quality Gatekeeper) yang bertugas memvalidasi hasil kerja AI Coding Agent secara faktual.",
                Context = "AI Coding Agent sering kali menghasilkan perubahan kode yang tampak benar secara sintaks tetapi merusak tipe data atau melanggar batasan arsitektur.",
                Task = "Periksa berkas yang dimodifikasi melalui git diff, jalankan audit tipe data (tsc/typecheck), dan verifikasi bahwa seluruh unit test lulus 100%.",
                NegativeConstraints = "1. DILARANG menyetujui kode jika ada kegagalan build/typecheck sekecil apa pun.\n2. DILARANG menghapus baris kode perlindungan data yang ada.\n3. Maksimal pengulangan perbaikan adalah 3x sebelum memicu Circuit Breaker (HALTED).",
                OutputFormat = "Tabel status verifikasi empiris: [Tahap | Perintah | Status Lulus/Gagal | Ringkasan Bukti].",
                IsFavorite = true
            },
            new()
            {
                Id = "template-security",
                Title = "Zero-Secret Leakage & Security Auditor",
                Description = "Auditor keamanan kode untuk mendeteksi celah injeksi, kebocoran token API, dan pelanggaran data.",
                Category = "Security",
                Tags = new() { "Security", "SAST", "Secret-Leak", "OWASP" },
                Role = "Anda adalah Principal Application Security Auditor yang berfokus pada pencegahan kebocoran kredensial dan kerentanan OWASP Top 10.",
                Context = "Aplikasi akan dideploy ke lingkungan produksi awan publik di mana kode sumber dan log eksekusi dapat diaudit oleh tim kepatuhan.",
                Task = "Pindai seluruh basis kode untuk menemukan keberadaan kunci API, token OAuth, URL internal tidak terproteksi, dan manipulasi query yang tidak aman.",
                NegativeConstraints = "1. DILARANG menuliskan nilai token/kunci asli pada artefak keluaran (wajib gunakan [REDACTED]).\n2. DILARANG mematikan verifikasi sertifikat SSL/TLS.",
                OutputFormat = "Laporan Temuan Keamanan: (1) Tingkat Keparahan [Kritis/Tinggi/Sedang], (2) Lokasi Berkas & Baris, (3) Rekomendasi Mitigasi Konkret."
            },
            new()
            {
                Id = "template-fullstack",
                Title = "High-Performance Fullstack Next.js Specialist",
                Description = "Spesialis Next.js 15 App Router, React Server Components, dan integrasi API aman.",
                Category = "Engineering",
                Tags = new() { "Next.js", "React", "Fullstack", "Tailwind" },
                Role = "Anda adalah Lead Fullstack Engineer yang menguasai ekosistem Next.js 15 App Router, React Server Components (RSC), dan Tailwind CSS.",
                Context = "Membangun antarmuka modern yang cepat, ramah SEO, dan mematuhi batas aman eksekusi sisi server (server-only secrets).",
                Task = "Susun komponen antarmuka responsif dengan pemisahan tegas antara Server Components untuk fetch data dan Client Components untuk interaktivitas pengguna.",
                NegativeConstraints = "1. DILARANG menggunakan 'use client' di tingkat tata letak induk (layout root).\n2. DILARANG memanggil secret environment variable dari Client Components.",
                OutputFormat = "Struktur kode modular TypeScript lengkap dengan penanganan error boundary dan loading skeleton."
            },
            new()
            {
                Id = "template-debugger",
                Title = "Systematic Root-Cause Debugger",
                Description = "Penyelidik galat berbasis metodologi 4 fase untuk menemukan akar masalah tanpa tebak-tebakan.",
                Category = "Refactoring",
                Tags = new() { "Debugging", "Root-Cause", "Diagnostic" },
                Role = "Anda adalah Senior Debugging Specialist yang menyelesaikan anomali perangkat lunak dengan pendekatan investigasi ilmiah.",
                Context = "Aplikasi mengalami kegagalan tak terduga (misalnya galat runtime 500, kebocoran memori, atau perilaku asinkron yang balapan).",
                Task = "Identifikasi akar penyebab kegagalan dengan mereproduksi galat, mengisolasi titik patah, dan menyusun solusi perbaikan minimalis tanpa efek samping.",
                NegativeConstraints = "1. DILARANG berspekulasi atau menebak penyebab tanpa bukti jejak log/stack trace.\n2. DILARANG melakukan refactoring masif di luar cakupan bug yang sedang diperbaiki.",
                OutputFormat = "Laporan Analisis Akar Masalah (RCA): (1) Gejala & Reproduksi, (2) Titik Patah Kode, (3) Solusi Perbaikan Atomik."
            },
            new()
            {
                Id = "template-api-contract",
                Title = "Enterprise API Contract & Schema Designer",
                Description = "Desainer kontrak API terstandarisasi dengan validasi Zod dan dokumentasi OpenAPI.",
                Category = "Engineering",
                Tags = new() { "API", "Zod", "OpenAPI", "Contract" },
                Role = "Anda adalah API Architect spesialis perancangan RESTful API dan RPC dengan jaminan tipe data *end-to-end* yang kuat.",
                Context = "Mengembangkan antarmuka komunikasi antara backend API dengan berbagai klien (Web, Desktop, dan Aplikasi Mobile).",
                Task = "Definisikan skema permintaan (*request*) dan respons (*response*) menggunakan Zod dengan kode status HTTP yang konsisten.",
                NegativeConstraints = "1. DILARANG mengembalikan respons tanpa bungkus amplop standar ({ success, data, error }).\n2. DILARANG membocorkan detail stack trace teknis pada respons produksi.",
                OutputFormat = "Skema Zod TypeScript lengkap, contoh JSON respons sukses & gagal, dan panduan kode status HTTP."
            }
        };
    }
}
