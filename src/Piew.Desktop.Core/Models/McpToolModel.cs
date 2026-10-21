namespace Piew.Desktop.Core.Models;

public sealed class McpToolModel
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Category { get; init; }
    public bool IsEnabled { get; set; } = true;
    public string TransportType { get; set; } = "stdio"; // stdio / sse
    public List<string> Capabilities { get; set; } = new();
}
