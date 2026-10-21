namespace Piew.Desktop.Core.Models;

public sealed class PromptItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "Engineering";
    public List<string> Tags { get; set; } = new();
    
    // 5 Pilar Arsitektur Prompt PIEW
    public string Role { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Task { get; set; } = string.Empty;
    public string NegativeConstraints { get; set; } = string.Empty;
    public string OutputFormat { get; set; } = string.Empty;

    public Dictionary<string, string> Variables { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsFavorite { get; set; }

    public int EstimatedTokens => CalculateTokens();

    private int CalculateTokens()
    {
        var totalChars = (Role?.Length ?? 0) +
                         (Context?.Length ?? 0) +
                         (Task?.Length ?? 0) +
                         (NegativeConstraints?.Length ?? 0) +
                         (OutputFormat?.Length ?? 0);
        return Math.Max(1, (int)Math.Ceiling(totalChars / 4.0));
    }
}
