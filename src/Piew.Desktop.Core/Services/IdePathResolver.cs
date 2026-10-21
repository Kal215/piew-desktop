using System.Runtime.InteropServices;
using Piew.Desktop.Core.Models;

namespace Piew.Desktop.Core.Services;

public static class IdePathResolver
{
    public static string GetConfigPath(IdeType type)
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        return type switch
        {
            IdeType.Cursor => Path.Combine(home, ".cursor", "mcp.json"),

            IdeType.ClaudeDesktop => isWindows
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Claude", "claude_desktop_config.json")
                : isMac
                    ? Path.Combine(home, "Library", "Application Support", "Claude", "claude_desktop_config.json")
                    : Path.Combine(home, ".config", "Claude", "claude_desktop_config.json"),

            IdeType.Windsurf => Path.Combine(home, ".codeium", "windsurf", "mcp_config.json"),

            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Tipe IDE tidak dikenali.")
        };
    }

    public static string GetDefaultDisplayName(IdeType type) => type switch
    {
        IdeType.Cursor => "Cursor",
        IdeType.ClaudeDesktop => "Claude Desktop",
        IdeType.Windsurf => "Windsurf",
        _ => type.ToString()
    };

    public static string GetDescription(IdeType type) => type switch
    {
        IdeType.Cursor => "Editor AI berbasis VS Code dengan dukungan MCP penuh.",
        IdeType.ClaudeDesktop => "Aplikasi desktop resmi Anthropic Claude.",
        IdeType.Windsurf => "IDE agen cerdas dari Codeium dengan alur kerja Cascade.",
        _ => string.Empty
    };
}
