# PIEW Desktop Bridge (.NET 8 Native)

Aplikasi desktop native C# .NET 8 mandiri bergaya antarmuka minimalis modern ala **macOS**, dirancang untuk menghubungkan AI Coding Agent (Cursor, Claude Desktop, Windsurf) secara fisik ke protokol MCP PIEW dan memantau pengawasan mutu kode (*Empirical Quality Gatekeeper*) secara lokal.

---

## 🎨 Tema Tampilan Antarmuka (Dynamic Theme Engine)

Aplikasi mengadopsi bahasa desain *floating frameless window* dengan ornamen *traffic lights* khas macOS, sudut membulat (*CornerRadius* 12-14px), efek bayangan halus, dan 3 palet warna dinamis:

1. **Pure Snow (Default)**: Putih gading bersih (`#FAFAFC` / `#FFFFFF`) dengan teks arang gelap (`#1D1D1F`) dan aksen biru laut lembut (`#007AFF`).
2. **Warm Cream / Linen**: Krem hangat (`#F7F5F0`) dengan panel putih susu, batas pasir tipis (`#E5DFD3`), dan aksen karamel lembut (`#D97706`).
3. **Dark Obsidian**: Hitam pekat matte (`#121214`) dengan kartu abu-abu gelap (`#1C1C1F`), teks porselen (`#F4F4F6`), dan aksen *sky blue* terang (`#0A84FF`).

Tema dapat diganti secara langsung saat aplikasi berjalan (*runtime*) melalui tab **Pengaturan**.

---

## 📁 Struktur Solusi Proyek

```
piew-desktop/
├── Piew.Desktop.sln
├── src/
│   ├── Piew.Desktop.Core/         # Class Library .NET 8 (Logika Bisnis & Layanan)
│   │   ├── Models/               # Model data IDE, Tema, Audit, dan Pengaturan
│   │   ├── Services/             # IdePathResolver, McpConnectorService, TerminalRunner
│   │   └── Storage/              # SettingsStorage (penyimpanan JSON lokal)
│   └── Piew.Desktop.App/          # Aplikasi WPF Native Windows
│       ├── Assets/               # Ikon dan aset grafis
│       ├── Themes/               # Resource Dictionary XAML (PureSnow, WarmCream, DarkObsidian)
│       ├── ViewModels/           # MVVM (MainViewModel, ViewModelBase, RelayCommand)
│       ├── Views/                # UserControl (ConnectView, AuditView, SettingsView)
│       ├── MainWindow.xaml       # Jendela kustom macOS tanpa bingkai
│       └── App.xaml              # Siklus hidup aplikasi & single-instance mutex
└── tests/
    └── Piew.Desktop.Core.Tests/  # Rangkaian pengujian unit xUnit
```

---

## ⚡ Fitur Utama

1. **Sambungan Satu-Klik IDE (*1-Click IDE Connector*)**:
   - Mendeteksi file konfigurasi Cursor (`~/.cursor/mcp.json`), Claude Desktop (`claude_desktop_config.json`), dan Windsurf (`mcp_config.json`).
   - Melakukan injeksi atau pemutusan konfigurasi server MCP PIEW secara aman dengan pencadangan otomatis (`.bak`).
2. **Pengawas Mutu Empiris (*Empirical Quality Gatekeeper*)**:
   - Membaca perubahan kode lokal secara langsung via `git diff` dan `git status`.
   - Menjalankan uji kompilasi dan tes terminal lokal tanpa batasan sandbox browser.
3. **System Tray & Single Instance**:
   - Aplikasi berjalan ringan di area notifikasi Windows (*System Tray*) dan mencegah pembukaan aplikasi ganda.

---

## 🛠️ Panduan Build & Eksekusi di Windows

Jalankan perintah berikut di PowerShell atau Command Prompt pada komputer Windows Anda:

```bash
# 1. Jalankan pengujian unit
dotnet test

# 2. Jalankan aplikasi langsung
dotnet run --project src/Piew.Desktop.App

# 3. Bangun berkas .exe tunggal mandiri (Self-Contained)
dotnet publish src/Piew.Desktop.App/Piew.Desktop.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```
Berkas `publish/PiewDesktop.exe` siap langsung dijalankan tanpa perlu menginstal dependensi tambahan di komputer pengguna.
