using System.Text;
using System.Text.RegularExpressions;
using Piew.Desktop.Core.Models;

namespace Piew.Desktop.Core.Services;

public sealed class CompiledPromptResult
{
    public required string FullPromptText { get; init; }
    public int EstimatedTokens { get; init; }
    public string EstimatedCost { get; init; } = "$0.00";
    public bool HasRole { get; init; }
    public bool HasContext { get; init; }
    public bool HasTask { get; init; }
    public bool HasNegativeConstraints { get; init; }
    public bool HasOutputFormat { get; init; }
    public int QualityScore { get; init; } // 0 - 100
}

public static class PromptCompilerService
{
    public static CompiledPromptResult Compile(PromptItem item, Dictionary<string, string>? runtimeVariables = null)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(item.Role))
        {
            sb.AppendLine("### ROLE & IDENTITAS");
            sb.AppendLine(item.Role.Trim());
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(item.Context))
        {
            sb.AppendLine("### KONTEKS ARSITEKTUR & LATAR BELAKANG");
            sb.AppendLine(item.Context.Trim());
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(item.Task))
        {
            sb.AppendLine("### TUJUAN & TUGAS UTAMA");
            sb.AppendLine(item.Task.Trim());
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(item.NegativeConstraints))
        {
            sb.AppendLine("### BATASAN MUTLAK (NEGATIVE CONSTRAINTS)");
            sb.AppendLine(item.NegativeConstraints.Trim());
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(item.OutputFormat))
        {
            sb.AppendLine("### FORMAT KELUARAN YANG DIWAJIBKAN");
            sb.AppendLine(item.OutputFormat.Trim());
            sb.AppendLine();
        }

        var fullText = sb.ToString().TrimEnd();

        // Substitusi variabel dinamis: {{nama_variabel}}
        var vars = new Dictionary<string, string>(item.Variables, StringComparer.OrdinalIgnoreCase);
        if (runtimeVariables != null)
        {
            foreach (var kvp in runtimeVariables)
            {
                vars[kvp.Key] = kvp.Value;
            }
        }

        foreach (var kvp in vars)
        {
            var pattern = @"\{\{\s*" + Regex.Escape(kvp.Key) + @"\s*\}\}";
            fullText = Regex.Replace(fullText, pattern, kvp.Value ?? string.Empty, RegexOptions.IgnoreCase);
        }

        var tokens = Math.Max(1, (int)Math.Ceiling(fullText.Length / 4.0));
        var cost = (tokens * 0.0000025).ToString("$0.0000");

        var hasRole = !string.IsNullOrWhiteSpace(item.Role);
        var hasContext = !string.IsNullOrWhiteSpace(item.Context);
        var hasTask = !string.IsNullOrWhiteSpace(item.Task);
        var hasConstraints = !string.IsNullOrWhiteSpace(item.NegativeConstraints);
        var hasFormat = !string.IsNullOrWhiteSpace(item.OutputFormat);

        var score = 0;
        if (hasRole) score += 20;
        if (hasContext) score += 20;
        if (hasTask) score += 20;
        if (hasConstraints) score += 20;
        if (hasFormat) score += 20;

        return new CompiledPromptResult
        {
            FullPromptText = fullText,
            EstimatedTokens = tokens,
            EstimatedCost = cost,
            HasRole = hasRole,
            HasContext = hasContext,
            HasTask = hasTask,
            HasNegativeConstraints = hasConstraints,
            HasOutputFormat = hasFormat,
            QualityScore = score
        };
    }
}
