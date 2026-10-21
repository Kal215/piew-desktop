namespace Piew.Desktop.Core.Models;

public sealed class AppSettings
{
    public ThemeType Theme { get; set; } = ThemeType.PureSnow;
    public bool MinimizeToTray { get; set; } = true;
    public bool StartOnBoot { get; set; } = false;
    public bool AutoCheckUpdates { get; set; } = true;
    public string? CustomMcpScriptPath { get; set; }
    public string CloudApiUrl { get; set; } = "https://piew.rskl.tech";
}
