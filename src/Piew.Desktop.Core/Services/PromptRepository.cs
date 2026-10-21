using System.Text.Json;
using Piew.Desktop.Core.Models;

namespace Piew.Desktop.Core.Services;

public sealed class PromptRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly string _storageFilePath;
    private readonly List<PromptItem> _cache = new();
    private readonly object _lock = new();

    public PromptRepository(string? customPath = null)
    {
        if (!string.IsNullOrWhiteSpace(customPath))
        {
            _storageFilePath = customPath;
        }
        else
        {
            var baseDir = Environment.OSVersion.Platform == PlatformID.Win32NT
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PiewDesktop")
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".piew");

            _storageFilePath = Path.Combine(baseDir, "desktop-prompts.json");
        }

        EnsureLoaded();
    }

    private void EnsureLoaded()
    {
        lock (_lock)
        {
            _cache.Clear();
            var dir = Path.GetDirectoryName(_storageFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (File.Exists(_storageFilePath))
            {
                try
                {
                    var json = File.ReadAllText(_storageFilePath);
                    var list = JsonSerializer.Deserialize<List<PromptItem>>(json, JsonOptions);
                    if (list != null && list.Count > 0)
                    {
                        _cache.AddRange(list);
                        return;
                    }
                }
                catch
                {
                    // Gunakan fallback template jika file rusak
                }
            }

            // Inisialisasi awal dengan katalog template kanonis bawaan
            var builtins = PromptTemplateCatalog.GetCanonicalTemplates();
            _cache.AddRange(builtins);
            SaveToDiskInternal();
        }
    }

    private void SaveToDiskInternal()
    {
        try
        {
            var json = JsonSerializer.Serialize(_cache, JsonOptions);
            File.WriteAllText(_storageFilePath, json);
        }
        catch
        {
            // Abaikan kesalahan penulisan jika sistem file read-only
        }
    }

    public List<PromptItem> GetAll()
    {
        lock (_lock)
        {
            return _cache.OrderByDescending(p => p.IsFavorite)
                         .ThenByDescending(p => p.UpdatedAt)
                         .Select(Clone)
                         .ToList();
        }
    }

    public PromptItem? GetById(string id)
    {
        lock (_lock)
        {
            var found = _cache.FirstOrDefault(p => p.Id == id);
            return found != null ? Clone(found) : null;
        }
    }

    public void Save(PromptItem prompt)
    {
        lock (_lock)
        {
            var index = _cache.FindIndex(p => p.Id == prompt.Id);
            prompt.UpdatedAt = DateTime.UtcNow;
            if (index >= 0)
            {
                _cache[index] = Clone(prompt);
            }
            else
            {
                _cache.Insert(0, Clone(prompt));
            }
            SaveToDiskInternal();
        }
    }

    public bool Delete(string id)
    {
        lock (_lock)
        {
            var removed = _cache.RemoveAll(p => p.Id == id) > 0;
            if (removed)
            {
                SaveToDiskInternal();
            }
            return removed;
        }
    }

    public void ToggleFavorite(string id)
    {
        lock (_lock)
        {
            var item = _cache.FirstOrDefault(p => p.Id == id);
            if (item != null)
            {
                item.IsFavorite = !item.IsFavorite;
                item.UpdatedAt = DateTime.UtcNow;
                SaveToDiskInternal();
            }
        }
    }

    public List<PromptItem> Search(string? query, string? category = null)
    {
        lock (_lock)
        {
            var result = _cache.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(category) && category != "Semua")
            {
                result = result.Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim();
                result = result.Where(p =>
                    p.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    p.Role.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    p.Task.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    p.Tags.Any(t => t.Contains(q, StringComparison.OrdinalIgnoreCase)));
            }

            return result.OrderByDescending(p => p.IsFavorite)
                         .ThenByDescending(p => p.UpdatedAt)
                         .Select(Clone)
                         .ToList();
        }
    }

    private static PromptItem Clone(PromptItem item)
    {
        return new PromptItem
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Category = item.Category,
            Tags = new List<string>(item.Tags),
            Role = item.Role,
            Context = item.Context,
            Task = item.Task,
            NegativeConstraints = item.NegativeConstraints,
            OutputFormat = item.OutputFormat,
            Variables = new Dictionary<string, string>(item.Variables),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            IsFavorite = item.IsFavorite
        };
    }
}
