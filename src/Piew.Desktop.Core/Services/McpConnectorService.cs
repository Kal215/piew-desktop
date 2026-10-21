using System.Text.Json;
using System.Text.Json.Nodes;
using Piew.Desktop.Core.Models;

namespace Piew.Desktop.Core.Services;

public sealed class McpConnectorService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public IdeTargetInfo GetIdeInfo(IdeType type)
    {
        var configPath = IdePathResolver.GetConfigPath(type);
        var dir = Path.GetDirectoryName(configPath);
        var isInstalled = !string.IsNullOrEmpty(dir) && Directory.Exists(dir);
        var isConnected = false;
        string? error = null;

        if (File.Exists(configPath))
        {
            try
            {
                var content = File.ReadAllText(configPath);
                var node = JsonNode.Parse(content);
                if (node?["mcpServers"]?["piew"] != null)
                {
                    isConnected = true;
                }
            }
            catch (Exception ex)
            {
                error = $"Gagal membaca konfigurasi: {ex.Message}";
            }
        }

        return new IdeTargetInfo
        {
            Type = type,
            DisplayName = IdePathResolver.GetDefaultDisplayName(type),
            ConfigPath = configPath,
            Description = IdePathResolver.GetDescription(type),
            IsInstalled = isInstalled,
            IsPiewConnected = isConnected,
            LastError = error
        };
    }

    public IReadOnlyList<IdeTargetInfo> GetAllIdeInfos()
    {
        return Enum.GetValues<IdeType>()
            .Select(GetIdeInfo)
            .ToList();
    }

    public bool Connect(IdeType type, string scriptPath, out string? errorMessage)
    {
        errorMessage = null;
        var configPath = IdePathResolver.GetConfigPath(type);

        try
        {
            var dir = Path.GetDirectoryName(configPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            JsonObject root;
            if (File.Exists(configPath))
            {
                // Buat salinan cadangan (backup) sebelum menulis
                var backupPath = configPath + ".bak";
                File.Copy(configPath, backupPath, overwrite: true);

                var raw = File.ReadAllText(configPath);
                var parsed = JsonNode.Parse(raw);
                root = parsed as JsonObject ?? new JsonObject();
            }
            else
            {
                root = new JsonObject();
            }

            if (root["mcpServers"] is not JsonObject servers)
            {
                servers = new JsonObject();
                root["mcpServers"] = servers;
            }

            var piewNode = new JsonObject
            {
                ["command"] = "npx",
                ["args"] = new JsonArray
                {
                    "-y",
                    "tsx",
                    scriptPath
                }
            };

            servers["piew"] = piewNode;

            File.WriteAllText(configPath, root.ToJsonString(JsonOptions));
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }

    public bool Disconnect(IdeType type, out string? errorMessage)
    {
        errorMessage = null;
        var configPath = IdePathResolver.GetConfigPath(type);

        if (!File.Exists(configPath))
        {
            return true;
        }

        try
        {
            var raw = File.ReadAllText(configPath);
            var node = JsonNode.Parse(raw);
            if (node is JsonObject root && root["mcpServers"] is JsonObject servers)
            {
                servers.Remove("piew");
                File.WriteAllText(configPath, root.ToJsonString(JsonOptions));
            }
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }

    public int ConnectAll(string scriptPath, out List<string> errors)
    {
        errors = new List<string>();
        var successCount = 0;

        foreach (var type in Enum.GetValues<IdeType>())
        {
            if (Connect(type, scriptPath, out var err))
            {
                successCount++;
            }
            else if (!string.IsNullOrEmpty(err))
            {
                errors.Add($"[{IdePathResolver.GetDefaultDisplayName(type)}]: {err}");
            }
        }

        return successCount;
    }
}
