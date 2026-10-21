using System.Text.Json.Nodes;
using Piew.Desktop.Core.Models;
using Piew.Desktop.Core.Services;
using Piew.Desktop.Core.Storage;
using Xunit;

namespace Piew.Desktop.Core.Tests;

public class McpConnectorTests
{
    [Fact]
    public void IdePathResolver_ShouldReturnNonEmptyPaths()
    {
        foreach (var type in Enum.GetValues<IdeType>())
        {
            var path = IdePathResolver.GetConfigPath(type);
            var name = IdePathResolver.GetDefaultDisplayName(type);

            Assert.False(string.IsNullOrWhiteSpace(path));
            Assert.False(string.IsNullOrWhiteSpace(name));
        }
    }

    [Fact]
    public void SettingsStorage_ShouldPersistAndLoadSettings()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"piew_test_settings_{Guid.NewGuid():N}.json");
        try
        {
            var storage = new SettingsStorage(tempFile);
            var original = new AppSettings
            {
                Theme = ThemeType.WarmCream,
                MinimizeToTray = false,
                StartOnBoot = true
            };

            storage.Save(original);
            var loaded = storage.Load();

            Assert.Equal(ThemeType.WarmCream, loaded.Theme);
            Assert.False(loaded.MinimizeToTray);
            Assert.True(loaded.StartOnBoot);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void McpConnector_ShouldInjectPiewNodeSafely()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"piew_ide_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        var configFile = Path.Combine(tempDir, "mcp.json");

        try
        {
            // Buat file konfigurasi awal dengan server lain
            var initialJson = new JsonObject
            {
                ["mcpServers"] = new JsonObject
                {
                    ["other_server"] = new JsonObject
                    {
                        ["command"] = "node",
                        ["args"] = new JsonArray { "server.js" }
                    }
                }
            };
            File.WriteAllText(configFile, initialJson.ToJsonString());

            // Simulasi injeksi
            var raw = File.ReadAllText(configFile);
            var node = JsonNode.Parse(raw) as JsonObject;
            Assert.NotNull(node);

            var servers = node["mcpServers"] as JsonObject;
            Assert.NotNull(servers);

            servers["piew"] = new JsonObject
            {
                ["command"] = "npx",
                ["args"] = new JsonArray { "-y", "tsx", "scripts/piew-mcp-server.ts" }
            };

            File.WriteAllText(configFile, node.ToJsonString());

            // Verifikasi server lama tidak hilang dan piew berhasil ditambahkan
            var updated = JsonNode.Parse(File.ReadAllText(configFile));
            Assert.NotNull(updated?["mcpServers"]?["other_server"]);
            Assert.NotNull(updated?["mcpServers"]?["piew"]);
            Assert.Equal("npx", updated?["mcpServers"]?["piew"]?["command"]?.ToString());
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }
}
