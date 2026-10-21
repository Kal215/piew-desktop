namespace Piew.Desktop.Core.Models;

public sealed class IdeTargetInfo
{
    public required IdeType Type { get; init; }
    public required string DisplayName { get; init; }
    public required string ConfigPath { get; init; }
    public required string Description { get; init; }
    public bool IsInstalled { get; set; }
    public bool IsPiewConnected { get; set; }
    public string? LastError { get; set; }
}
