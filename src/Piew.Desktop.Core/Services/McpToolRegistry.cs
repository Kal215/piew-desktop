using Piew.Desktop.Core.Models;

namespace Piew.Desktop.Core.Services;

public static class McpToolRegistry
{
    public static List<McpToolModel> GetBuiltinTools()
    {
        return new List<McpToolModel>
        {
            new()
            {
                Id = "firecrawl-search",
                Name = "Web Scraping & Documentation Search",
                Description = "Mengambil dokumentasi teknis dan konten web terkini dalam format markdown bersih tanpa iklan.",
                Category = "Reconnaissance",
                IsEnabled = true,
                TransportType = "stdio",
                Capabilities = new() { "scrape_url", "crawl_domain", "extract_markdown" }
            },
            new()
            {
                Id = "codebase-inspector",
                Name = "Codebase Architecture & LOC Inspector",
                Description = "Memeriksa volume baris kode (LOC), bahasa pemrograman, dan batasan arsitektur proyek pengguna.",
                Category = "Engineering",
                IsEnabled = true,
                TransportType = "stdio",
                Capabilities = new() { "inspect_loc", "analyze_dependencies", "verify_boundaries" }
            },
            new()
            {
                Id = "git-diff-auditor",
                Name = "Physical Workspace Git Diff Auditor",
                Description = "Membaca perubahan berkas fisik yang baru saja dimodifikasi oleh AI untuk mendeteksi cacat sebelum commit.",
                Category = "Supervisor",
                IsEnabled = true,
                TransportType = "stdio",
                Capabilities = new() { "get_modified_files", "read_git_diff", "check_untracked" }
            },
            new()
            {
                Id = "terminal-runner",
                Name = "Safe Terminal Test & Build Runner",
                Description = "Mengeksekusi perintah kompilasi (tsc, build, test) di terminal komputer lokal secara terisolasi.",
                Category = "Supervisor",
                IsEnabled = true,
                TransportType = "stdio",
                Capabilities = new() { "run_typecheck", "run_unit_tests", "circuit_breaker" }
            },
            new()
            {
                Id = "database-explorer",
                Name = "SQLite & Prisma Database Explorer",
                Description = "Membaca skema Prisma, memvalidasi integritas relasi, dan memverifikasi data kanonis lokal tanpa merusak record.",
                Category = "Data",
                IsEnabled = true,
                TransportType = "stdio",
                Capabilities = new() { "read_schema", "query_records", "verify_canonical_data" }
            },
            new()
            {
                Id = "memory-vault",
                Name = "Long-Term Hybrid Memory Vault",
                Description = "Menyimpan preferensi pengguna, aturan arsitektur, dan memori permanen lintas sesi.",
                Category = "Memory",
                IsEnabled = true,
                TransportType = "stdio",
                Capabilities = new() { "store_fact", "recall_memory", "semantic_search" }
            },
            new()
            {
                Id = "telemetry-sentinel",
                Name = "Performance & Error Sentinel",
                Description = "Memantau latensi eksekusi prompt, penggunaan kuota token, dan kegagalan endpoint secara real-time.",
                Category = "Telemetry",
                IsEnabled = true,
                TransportType = "stdio",
                Capabilities = new() { "track_latency", "monitor_token_usage", "report_anomaly" }
            },
            new()
            {
                Id = "security-scanner",
                Name = "Zero-Secret Leakage Scanner",
                Description = "Memindai basis kode untuk mendeteksi kebocoran kunci API, token OAuth, dan kredensial sensitif.",
                Category = "Security",
                IsEnabled = true,
                TransportType = "stdio",
                Capabilities = new() { "scan_api_keys", "audit_owasp", "redact_secrets" }
            },
            new()
            {
                Id = "prompt-optimizer",
                Name = "Automated Meta-Prompt Enhancer",
                Description = "Mengubah instruksi mentah yang ambigu menjadi prompt 5 pilar terstruktur yang siap dipakai LLM.",
                Category = "Prompt Studio",
                IsEnabled = true,
                TransportType = "stdio",
                Capabilities = new() { "expand_constraints", "inject_few_shot", "optimize_structure" }
            },
            new()
            {
                Id = "spec-synthesizer",
                Name = "Zod & OpenAPI Schema Generator",
                Description = "Membuat skema validasi Zod TypeScript dan spesifikasi OpenAPI 3.0 secara otomatis dari contoh data.",
                Category = "Engineering",
                IsEnabled = true,
                TransportType = "stdio",
                Capabilities = new() { "generate_zod", "generate_openapi", "validate_payload" }
            }
        };
    }
}
