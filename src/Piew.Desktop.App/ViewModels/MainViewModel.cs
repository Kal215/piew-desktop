using System.Collections.ObjectModel;
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
        set => SetProperty(ref _isConnected, value);
    }

    public string? StatusDescription
    {
        get => _statusDescription;
        set => SetProperty(ref _statusDescription, value);
    }
}

public sealed class MainViewModel : ViewModelBase
{
    private readonly McpConnectorService _connectorService = new();
    private readonly SettingsStorage _settingsStorage = new();
    private readonly TerminalRunner _terminalRunner = new();

    private string _currentTab = "Connect";
    private ThemeType _currentTheme = ThemeType.PureSnow;
    private string _statusBanner = "Siap menghubungkan AI Agent Anda.";
    private bool _isBusy;
    private string _auditStatusText = "Siaga (Idle)";
    private string _auditSummary = "Belum ada verifikasi yang dijalankan.";

    public MainViewModel()
    {
        var settings = _settingsStorage.Load();
        _currentTheme = settings.Theme;

        IdeList = new ObservableCollection<IdeItemViewModel>();
        ChangedFiles = new ObservableCollection<string>();

        ConnectAllCommand = new RelayCommand(ExecuteConnectAll);
        ToggleIdeCommand = new RelayCommand(p => ExecuteToggleIde(p as IdeItemViewModel));
        RunAuditCommand = new RelayCommand(ExecuteRunAudit);
        ChangeThemeCommand = new RelayCommand(p => ExecuteChangeTheme(p));
        SelectTabCommand = new RelayCommand(p => CurrentTab = p?.ToString() ?? "Connect");

        RefreshIdeList();
    }

    public ObservableCollection<IdeItemViewModel> IdeList { get; }
    public ObservableCollection<string> ChangedFiles { get; }

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

    public ICommand ConnectAllCommand { get; }
    public ICommand ToggleIdeCommand { get; }
    public ICommand RunAuditCommand { get; }
    public ICommand ChangeThemeCommand { get; }
    public ICommand SelectTabCommand { get; }

    public event Action<ThemeType>? ThemeChangeRequested;

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
        StatusBanner = "Menyambungkan seluruh IDE...";

        // Path script MCP Server PIEW (dapat diarahkan ke file lokal atau npx tsx)
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

    public void ExecuteChangeTheme(object? parameter)
    {
        if (parameter is ThemeType theme)
        {
            CurrentTheme = theme;
            SaveThemeSettings(theme);
            ThemeChangeRequested?.Invoke(theme);
        }
        else if (parameter is string themeStr && Enum.TryParse<ThemeType>(themeStr, out var parsedTheme))
        {
            CurrentTheme = parsedTheme;
            SaveThemeSettings(parsedTheme);
            ThemeChangeRequested?.Invoke(parsedTheme);
        }
    }

    private void SaveThemeSettings(ThemeType theme)
    {
        var settings = _settingsStorage.Load();
        settings.Theme = theme;
        _settingsStorage.Save(settings);
    }
}
