using System.Diagnostics;
using Piew.Desktop.Core.Models;

namespace Piew.Desktop.Core.Services;

public sealed class TerminalRunner
{
    public async Task<VerificationReport> RunAuditAsync(string projectDirectory, string? testCommand = null, CancellationToken cancellationToken = default)
    {
        var report = new VerificationReport
        {
            ProjectRoot = projectDirectory
        };

        if (!Directory.Exists(projectDirectory))
        {
            report.RawOutput = $"Direktori proyek '{projectDirectory}' tidak ditemukan.";
            return report;
        }

        // 1. Periksa file yang dimodifikasi via git status / diff
        var gitDiffResult = await ExecuteProcessAsync("git", "status --porcelain", projectDirectory, cancellationToken);
        if (gitDiffResult.ExitCode == 0)
        {
            var lines = gitDiffResult.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.Length > 3)
                {
                    report.ChangedFiles.Add(trimmed[3..].Trim());
                }
            }
            report.ModifiedFilesCount = report.ChangedFiles.Count;
        }

        // 2. Jalankan perintah verifikasi jika diberikan
        if (!string.IsNullOrWhiteSpace(testCommand))
        {
            var parts = testCommand.Split(' ', 2);
            var cmd = parts[0];
            var args = parts.Length > 1 ? parts[1] : string.Empty;

            var testResult = await ExecuteProcessAsync(cmd, args, projectDirectory, cancellationToken);
            report.BuildPassed = testResult.ExitCode == 0;
            report.TestsPassed = testResult.ExitCode == 0;
            report.RawOutput = testResult.Output;
        }
        else
        {
            report.BuildPassed = true;
            report.TestsPassed = true;
            report.RawOutput = "Pemeriksaan pasif: tidak ada perintah tes yang dieksekusi.";
        }

        return report;
    }

    private static async Task<(int ExitCode, string Output)> ExecuteProcessAsync(
        string fileName,
        string arguments,
        string workingDirectory,
        CancellationToken cancellationToken)
    {
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process.Start();

            var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

            await process.WaitForExitAsync(cancellationToken);

            var stdout = await stdoutTask;
            var stderr = await stderrTask;

            return (process.ExitCode, $"{stdout}\n{stderr}".Trim());
        }
        catch (Exception ex)
        {
            return (-1, $"Eksekusi proses gagal: {ex.Message}");
        }
    }
}
