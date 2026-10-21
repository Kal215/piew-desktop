using Piew.Desktop.Core.Models;
using Piew.Desktop.Core.Services;
using Xunit;

namespace Piew.Desktop.Core.Tests;

public class PromptServicesTests
{
    [Fact]
    public void PromptCompiler_ShouldCompile5PillarsAndCalculateTokens()
    {
        var item = new PromptItem
        {
            Title = "Test Prompt",
            Role = "Senior Architect",
            Context = "High performance web app",
            Task = "Design a database schema",
            NegativeConstraints = "Do not use MongoDB",
            OutputFormat = "SQL DDL scripts"
        };

        var result = PromptCompilerService.Compile(item);

        Assert.NotNull(result.FullPromptText);
        Assert.Contains("### ROLE & IDENTITAS", result.FullPromptText);
        Assert.Contains("### BATASAN MUTLAK (NEGATIVE CONSTRAINTS)", result.FullPromptText);
        Assert.Contains("Do not use MongoDB", result.FullPromptText);
        Assert.True(result.EstimatedTokens > 0);
        Assert.Equal(100, result.QualityScore);
    }

    [Fact]
    public void PromptCompiler_ShouldSubstituteVariables()
    {
        var item = new PromptItem
        {
            Title = "Variable Prompt",
            Task = "Refactor module {{module_name}} to support {{feature_name}}"
        };

        var runtimeVars = new Dictionary<string, string>
        {
            ["module_name"] = "AuthService",
            ["feature_name"] = "Passkey 2FA"
        };

        var result = PromptCompilerService.Compile(item, runtimeVars);

        Assert.Contains("Refactor module AuthService to support Passkey 2FA", result.FullPromptText);
    }

    [Fact]
    public void PromptRepository_ShouldLoadCanonicalTemplatesAndSupportSearch()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"piew_test_prompts_{Guid.NewGuid():N}.json");
        try
        {
            var repo = new PromptRepository(tempFile);
            var all = repo.GetAll();

            Assert.True(all.Count >= 6);

            var searchResult = repo.Search("Architect");
            Assert.NotEmpty(searchResult);

            var newPrompt = new PromptItem
            {
                Title = "Custom Unit Test Prompt",
                Task = "Write unit tests"
            };
            repo.Save(newPrompt);

            var retrieved = repo.GetById(newPrompt.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("Custom Unit Test Prompt", retrieved.Title);

            repo.Delete(newPrompt.Id);
            Assert.Null(repo.GetById(newPrompt.Id));
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
    public void McpToolRegistry_ShouldHave10BuiltinTools()
    {
        var tools = McpToolRegistry.GetBuiltinTools();
        Assert.Equal(10, tools.Count);
        Assert.Contains(tools, t => t.Id == "firecrawl-search");
        Assert.Contains(tools, t => t.Id == "git-diff-auditor");
        Assert.Contains(tools, t => t.Id == "security-scanner");
    }
}
