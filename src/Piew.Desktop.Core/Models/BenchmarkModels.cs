namespace Piew.Desktop.Core.Models;

public sealed class BenchmarkScenario
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string PromptPreview { get; set; } = string.Empty;
    public List<ModelBenchmarkResult> Results { get; set; } = new();
}

public sealed class ModelBenchmarkResult
{
    public string ModelName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public double ClarityScore { get; set; } // 0 - 100
    public double ConstraintAdherenceScore { get; set; } // 0 - 100
    public int TokenUsage { get; set; }
    public double LatencyMs { get; set; }
    public string CostPer1kTokens { get; set; } = "$0.00";
    public string Status { get; set; } = "Passed";
}
