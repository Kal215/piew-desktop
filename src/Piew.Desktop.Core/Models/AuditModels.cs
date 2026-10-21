namespace Piew.Desktop.Core.Models;

public enum AuditStatus
{
    Idle,
    Inspecting,
    Passed,
    Warning,
    Failed
}

public sealed class AuditEntry
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public required string Phase { get; init; }
    public required string Message { get; init; }
    public AuditStatus Status { get; init; }
    public string? DiffSummary { get; init; }
    public string? DiagnosticDetails { get; init; }
}

public sealed class VerificationReport
{
    public required string ProjectRoot { get; init; }
    public bool BuildPassed { get; set; }
    public bool TestsPassed { get; set; }
    public int ModifiedFilesCount { get; set; }
    public List<string> ChangedFiles { get; set; } = new();
    public string RawOutput { get; set; } = string.Empty;
    public DateTime CheckedAt { get; init; } = DateTime.UtcNow;
}
