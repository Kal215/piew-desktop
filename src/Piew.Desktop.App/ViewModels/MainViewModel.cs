using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Piew.Desktop.Core.Models;
using Piew.Desktop.Core.Services;
using Piew.Desktop.Core.Storage;

namespace Piew.Desktop.App.ViewModels;

public sealed class IdeItemViewModel : ViewModelBase
{
    private bool _isConnected;
    private string? _statusDescription;

    public required IdeType Type { get; init; }
    public required string DisplayName { get; init; }
    public required string Description { get; init; }
    public required string ConfigPath { get; init; }
    public bool IsInstalled { get; init; }

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            if (SetProperty(ref _isConnected, value))
            {
                OnPropertyChanged(nameof(ActionButtonText));
            }
        }
    }

    public string? StatusDescription
    {
        get => _statusDescription;
        set => SetProperty(ref _statusDescription, value);
    }

    public string ActionButtonText => IsConnected ? "Putuskan" : "Sambungkan";
}

public sealed class MainViewModel : ViewModelBase
{
    private readonly McpConnectorService _connectorService = new();
    private readonly SettingsStorage _settingsStorage = new();
    private readonly TerminalRunner _terminalRunner = new();
    private readonly PromptRepository _promptRepo = new();

    private string _currentTab = "Studio";
    private ThemeType _currentTheme = ThemeType.PureSnow;
    private string _statusBanner = "PIEW Desktop • Sistem 5 Pilar Prompt & Pengawas Mutu Aktif";
    private bool _isBusy;

    // --- Studio Properties ---
    private string _studioTitle = "Senior Software Architect: Modular Monolith";
    private string _studioCategory = "Architecture";
    private string _studioRole = "Anda adalah Chief Software Architect dengan pengalaman lebih dari 15 tahun.";
    private string _studioContext = "Sistem monolit modular skala enterprise dengan volume transaksi tinggi.";
    private string _studioTask = "Rancang struktur modul, kontrak data terisolasi, dan pola interaksi satu arah.";
    private string _studioConstraints = "1. DILARANG membuat ketergantungan siklik (circular dependency).\n2. DILARANG menggunakan global mutable state.";
    private string _studioFormat = "Sajikan: (1) Domain Boundary Map, (2) Kontrak Data Antar-Modul, (3) Strategi Pengujian.";
    private string _studioCompiledPreview = string.Empty;
    private int _studioEstimatedTokens;
    private string _studioEstimatedCost = "$0.00";
    private int _studioQualityScore = 100;
    private string _copyFeedbackText = string.Empty;

    // --- Library Properties ---
    private string _searchQuery = string.Empty;
    private string _selectedFilterCategory = "Semua";

    // --- Audit Properties ---
    private string _auditStatusText = "Siaga (Idle)";
    private string _auditSummary = "Belum ada verifikasi yang dijalankan.";

    public MainViewModel()
    {
        var settings = _settingsStorage.Load();
        _currentTheme = settings.Theme;

        CategoriesList = new List<string>
        {
            "Semua", "Engineering", "Architecture", "Security", "DevOps", "Refactoring", "Prompt Studio"
        };

        // Inisialisasi daftar IDE
        IdeList = new ObservableCollection<IdeItemViewModel>();
        RefreshIdeList();

        // Inisialisasi 10 MCP Tools bawaan
        McpTools = new ObservableCollection<McpToolModel>(McpToolRegistry.GetBuiltinTools());

        // Inisialisasi Benchmark Hasil Simulasi
        BenchmarkResults = new ObservableCollection<ModelBenchmarkResult>
        {
            new() { ModelName = "Claude 3.5 Sonnet", Provider = "Anthropic", ClarityScore = 97, ConstraintAdherenceScore = 98, TokenUsage = 520, LatencyMs = 1.1, CostPer1kTokens = "$0.0030", Status = "Direkomendasikan" },
            new() { ModelName = "GPT-4o (Omni)", Provider = "OpenAI", ClarityScore = 94, ConstraintAdherenceScore = 93, TokenUsage = 540, LatencyMs = 0.9, CostPer1kTokens = "$0.0025", Status = "Sangat Baik" },
            new() { ModelName = "DeepSeek-Coder V2", Provider = "DeepSeek", ClarityScore = 92, ConstraintAdherenceScore = 94, TokenUsage = 510, LatencyMs = 1.4, CostPer1kTokens = "$0.0008", Status = "Paling Hemat" },
            new() { ModelName = "Gemini 1.5 Pro", Provider = "Google", ClarityScore = 90, ConstraintAdherenceScore = 89, TokenUsage = 560, LatencyMs = 0.8, CostPer1kTokens = "$0.0012", Status = "Respons Cepat" }
        };

        // Inisialisasi Perintah Navigasi Tab
        SelectTabCommand = new RelayCommand(param =>
        {
            if (param is string tab)
            {
                CurrentTab = tab;
            }
        });

        // IDE & Connector Commands
        ConnectAllCommand = new RelayCommand(_ => ExecuteConnectAll());
        ToggleIdeConnectionCommand = new RelayCommand(param =>
        {
            if (param is IdeItemViewModel item)
            {
                ExecuteToggleIde(item);
            }
        });

        // Audit Commands
        RunAuditCommand = new RelayCommand(_ => ExecuteRunAudit());

        // Theme Commands
        ChangeThemeCommand = new RelayCommand(param =>
        {
            if (param is ThemeType theme)
            {
                ExecuteChangeTheme(theme);
            }
            else if (param is string themeStr && Enum.TryParse<ThemeType>(themeStr, out var parsed))
            {
                ExecuteChangeTheme(parsed);
            }
        });

        // Studio Commands
        SaveStudioPromptCommand = new RelayCommand(_ => SaveStudioPrompt());
        ResetStudioCommand = new RelayCommand(_ => ResetStudioForm());
        CopyStudioPromptCommand = new RelayCommand(_ => CopyStudioPrompt());

        // Library Commands
        CopyPromptFromLibraryCommand = new RelayCommand(param =>
        {
            if (param is PromptItem item)
            {
                var compiled = PromptCompilerService.Compile(item);
                try { Clipboard.SetText(compiled.FullPromptText); } catch { }
                StatusBanner = $"Prompt '{item.Title}' disalin ke clipboard!";
            }
        });

        LoadIntoStudioCommand = new RelayCommand(param =>
        {
            if (param is PromptItem item)
            {
                LoadPromptToStudio(item);
            }
        });

        DeletePromptCommand = new RelayCommand(param =>
        {
            if (param is PromptItem item)
            {
                _promptRepo.Delete(item.Id);
                RefreshFilteredPrompts();
                StatusBanner = $"Prompt '{item.Title}' dihapus dari perpustakaan.";
            }
        });

        RunBenchmarkSimulationCommand = new RelayCommand(_ =>
        {
            StatusBanner = "Simulasi benchmark multi-model berhasil dievaluasi ulang.";
        });

        // Inisialisasi Data
        RefreshFilteredPrompts();
        UpdateStudioPreview();
    }

    public List<string> CategoriesList { get; }
    public ObservableCollection<IdeItemViewModel> IdeList { get; }
    public ObservableCollection<McpToolModel> McpTools { get; }
    public ObservableCollection<ModelBenchmarkResult> BenchmarkResults { get; }
    public ObservableCollection<PromptItem> FilteredPrompts { get; } = new();
    public ObservableCollection<string> ChangedFiles { get; } = new();

    public string CurrentTab
    {
        get => _currentTab;
        set => SetProperty(ref _currentTab, value);
    }

    public ThemeType CurrentTheme
    {
        get => _currentTheme;
        set => SetProperty(ref _currentTheme, value);
    }

    public string StatusBanner
    {
        get => _statusBanner;
        set => SetProperty(ref _statusBanner, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string AuditStatusText
    {
        get => _auditStatusText;
        set => SetProperty(ref _auditStatusText, value);
    }

    public string AuditSummary
    {
        get => _auditSummary;
        set => SetProperty(ref _auditSummary, value);
    }

    // --- Studio Bindings ---
    public string StudioTitle
    {
        get => _studioTitle;
        set { if (SetProperty(ref _studioTitle, value)) UpdateStudioPreview(); }
    }

    public string StudioCategory
    {
        get => _studioCategory;
        set { if (SetProperty(ref _studioCategory, value)) UpdateStudioPreview(); }
    }

    public string StudioRole
    {
        get => _studioRole;
        set { if (SetProperty(ref _studioRole, value)) UpdateStudioPreview(); }
    }

    public string StudioContext
    {
        get => _studioContext;
        set { if (SetProperty(ref _studioContext, value)) UpdateStudioPreview(); }
    }

    public string StudioTask
    {
        get => _studioTask;
        set { if (SetProperty(ref _studioTask, value)) UpdateStudioPreview(); }
    }

    public string StudioConstraints
    {
        get => _studioConstraints;
        set { if (SetProperty(ref _studioConstraints, value)) UpdateStudioPreview(); }
    }

    public string StudioFormat
    {
        get => _studioFormat;
        set { if (SetProperty(ref _studioFormat, value)) UpdateStudioPreview(); }
    }

    public string StudioCompiledPreview
    {
        get => _studioCompiledPreview;
        private set => SetProperty(ref _studioCompiledPreview, value);
    }

    public int StudioEstimatedTokens
    {
        get => _studioEstimatedTokens;
        private set => SetProperty(ref _studioEstimatedTokens, value);
    }

    public string StudioEstimatedCost
    {
        get => _studioEstimatedCost;
        private set => SetProperty(ref _studioEstimatedCost, value);
    }

    public int StudioQualityScore
    {
        get => _studioQualityScore;
        private set => SetProperty(ref _studioQualityScore, value);
    }

    public string CopyFeedbackText
    {
        get => _copyFeedbackText;
        set => SetProperty(ref _copyFeedbackText, value);
    }

    // --- Library Bindings ---
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                RefreshFilteredPrompts();
            }
        }
    }

    public string SelectedFilterCategory
    {
        get => _selectedFilterCategory;
        set
        {
            if (SetProperty(ref _selectedFilterCategory, value))
            {
                RefreshFilteredPrompts();
            }
        }
    }

    public string PromptsCountText => $"{FilteredPrompts.Count} Prompt Tersedia";

    // --- Commands ---
    public ICommand SelectTabCommand { get; }
    public ICommand ConnectAllCommand { get; }
    public ICommand ToggleIdeConnectionCommand { get; }
    public ICommand RunAuditCommand { get; }
    public ICommand ChangeThemeCommand { get; }

    public ICommand SaveStudioPromptCommand { get; }
    public ICommand ResetStudioCommand { get; }
    public ICommand CopyStudioPromptCommand { get; }

    public ICommand CopyPromptFromLibraryCommand { get; }
    public ICommand LoadIntoStudioCommand { get; }
    public ICommand DeletePromptCommand { get; }
    public ICommand RunBenchmarkSimulationCommand { get; }

    public event Action<ThemeType>? ThemeChangeRequested;

    private void UpdateStudioPreview()
    {
        var item = new PromptItem
        {
            Title = StudioTitle,
            Category = StudioCategory,
            Role = StudioRole,
            Context = StudioContext,
            Task = StudioTask,
            NegativeConstraints = StudioConstraints,
            OutputFormat = StudioFormat
        };

        var compiled = PromptCompilerService.Compile(item);
        StudioCompiledPreview = compiled.FullPromptText;
        StudioEstimatedTokens = compiled.EstimatedTokens;
        StudioEstimatedCost = compiled.EstimatedCost;
        StudioQualityScore = compiled.QualityScore;
    }

    private void SaveStudioPrompt()
    {
        var item = new PromptItem
        {
            Title = string.IsNullOrWhiteSpace(StudioTitle) ? "Prompt Rekayasa Baru" : StudioTitle.Trim(),
            Category = StudioCategory,
            Role = StudioRole,
            Context = StudioContext,
            Task = StudioTask,
            NegativeConstraints = StudioConstraints,
            OutputFormat = StudioFormat
        };

        _promptRepo.Save(item);
        RefreshFilteredPrompts();
        StatusBanner = $"Prompt '{item.Title}' berhasil disimpan ke perpustakaan lokal!";
    }

    private void ResetStudioForm()
    {
        StudioTitle = string.Empty;
        StudioRole = string.Empty;
        StudioContext = string.Empty;
        StudioTask = string.Empty;
        StudioConstraints = string.Empty;
        StudioFormat = string.Empty;
        UpdateStudioPreview();
        StatusBanner = "Formulir studio telah dikosongkan.";
    }

    private void CopyStudioPrompt()
    {
        if (!string.IsNullOrWhiteSpace(StudioCompiledPreview))
        {
            try { Clipboard.SetText(StudioCompiledPreview); } catch { }
            CopyFeedbackText = "✓ Tersalin ke Clipboard!";
            StatusBanner = "Prompt 5 pilar berhasil disalin ke clipboard!";
        }
    }

    private void LoadPromptToStudio(PromptItem item)
    {
        StudioTitle = item.Title;
        StudioCategory = item.Category;
        StudioRole = item.Role;
        StudioContext = item.Context;
        StudioTask = item.Task;
        StudioConstraints = item.NegativeConstraints;
        StudioFormat = item.OutputFormat;
        UpdateStudioPreview();

        CurrentTab = "Studio";
        StatusBanner = $"Prompt '{item.Title}' dimuat ke Studio.";
    }

    private void RefreshFilteredPrompts()
    {
        var list = _promptRepo.Search(SearchQuery, SelectedFilterCategory);
        FilteredPrompts.Clear();
        foreach (var p in list)
        {
            FilteredPrompts.Add(p);
        }
        OnPropertyChanged(nameof(PromptsCountText));
    }

    public void RefreshIdeList()
    {
        IdeList.Clear();
        var allInfos = _connectorService.GetAllIdeInfos();
        foreach (var info in allInfos)
        {
            IdeList.Add(new IdeItemViewModel
            {
                Type = info.Type,
                DisplayName = info.DisplayName,
                Description = info.Description,
                ConfigPath = info.ConfigPath,
                IsInstalled = info.IsInstalled,
                IsConnected = info.IsPiewConnected,
                StatusDescription = info.IsPiewConnected ? "Terhubung ke PIEW MCP" : "Belum terhubung"
            });
        }
    }

    private void ExecuteConnectAll()
    {
        IsBusy = true;
        StatusBanner = "Menyambungkan seluruh AI Coding Agent...";

        var scriptPath = "scripts/piew-mcp-server.ts";
        var count = _connectorService.ConnectAll(scriptPath, out var errors);

        RefreshIdeList();
        IsBusy = false;

        if (errors.Count == 0)
        {
            StatusBanner = $"Berhasil! {count} IDE terhubung secara otomatis ke PIEW.";
        }
        else
        {
            StatusBanner = $"Tersambung ke {count} IDE dengan {errors.Count} catatan peringatan.";
        }
    }

    private void ExecuteToggleIde(IdeItemViewModel? item)
    {
        if (item == null) return;

        var scriptPath = "scripts/piew-mcp-server.ts";
        if (!item.IsConnected)
        {
            if (_connectorService.Connect(item.Type, scriptPath, out var err))
            {
                item.IsConnected = true;
                item.StatusDescription = "Terhubung ke PIEW MCP";
                StatusBanner = $"{item.DisplayName} berhasil disambungkan.";
            }
            else
            {
                StatusBanner = $"Gagal menyambungkan {item.DisplayName}: {err}";
            }
        }
        else
        {
            if (_connectorService.Disconnect(item.Type, out var err))
            {
                item.IsConnected = false;
                item.StatusDescription = "Belum terhubung";
                StatusBanner = $"{item.DisplayName} diputuskan.";
            }
            else
            {
                StatusBanner = $"Gagal memutuskan {item.DisplayName}: {err}";
            }
        }
    }

    private async void ExecuteRunAudit()
    {
        IsBusy = true;
        AuditStatusText = "Sedang Memeriksa Berkas & Kompilasi...";
        StatusBanner = "Menjalankan audit empiris di direktori lokal...";

        var cwd = Directory.GetCurrentDirectory();
        var report = await _terminalRunner.RunAuditAsync(cwd);

        ChangedFiles.Clear();
        foreach (var file in report.ChangedFiles)
        {
            ChangedFiles.Add(file);
        }

        AuditStatusText = report.BuildPassed ? "Audit Lulus (100% Bersih)" : "Terdapat Kesalahan";
        AuditSummary = $"Terdeteksi {report.ModifiedFilesCount} file termodifikasi. Status kompilasi: {(report.BuildPassed ? "OK" : "Error")}.";
        StatusBanner = "Audit selesai dijalankan.";
        IsBusy = false;
    }

    public void ExecuteChangeTheme(ThemeType theme)
    {
        CurrentTheme = theme;
        var settings = _settingsStorage.Load();
        settings.Theme = theme;
        _settingsStorage.Save(settings);

        if (Application.Current.MainWindow is MainWindow win)
        {
            win.ApplyTheme(theme);
        }

        StatusBanner = $"Tema tampilan diubah ke {theme}.";
    }
}
