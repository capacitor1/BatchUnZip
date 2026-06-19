using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ArchiveExtractor;

internal static class SevenZipExtractor
{
    public static bool Extract(
        string archivePath,
        string outputDir,
        out int exitCode)
    {
        string? password = null;

        if (RequiresPassword(archivePath))
        {
            Console.WriteLine(
                "[7-zip] Password required. Input password:");

            password = Console.ReadLine() ?? string.Empty;
        }

        return ExecuteExtract(
            archivePath,
            outputDir,
            password,
            out exitCode);
    }

    private static bool RequiresPassword(string archivePath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "7z.exe",

            Arguments =
                $"l -slt -p -sccUTF-8 \"{archivePath}\"",

            RedirectStandardOutput = true,
            RedirectStandardError = true,

            UseShellExecute = false,
            CreateNoWindow = true,

            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        using var process = Process.Start(psi)!;

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        string text = output + Environment.NewLine + error;

        return IsEncryptedArchive(text);
    }
    private static bool IsEncryptedArchive(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        return
            text.Contains(
                "Encrypted = +",
                StringComparison.OrdinalIgnoreCase)

            ||

            text.Contains(
                "Cannot open encrypted archive. Wrong password?",
                StringComparison.OrdinalIgnoreCase)

            ||

            text.Contains(
                "Can not open encrypted archive. Wrong password?",
                StringComparison.OrdinalIgnoreCase);
    }

    private static bool ExecuteExtract(
        string archivePath,
        string outputDir,
        string? password,
        out int exitCode)
    {
        string arguments =
            $"x -y -sccUTF-8 \"{archivePath}\" -o\"{outputDir}\"";

        if (!string.IsNullOrEmpty(password))
        {
            arguments += $" -p\"{password}\"";
        }

        var psi = new ProcessStartInfo
        {
            FileName = "7z.exe",

            Arguments = arguments,

            RedirectStandardOutput = true,
            RedirectStandardError = true,

            UseShellExecute = false,
            CreateNoWindow = true,

            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        using var process = Process.Start(psi)!;

        process.StandardOutput.ReadToEnd();
        process.StandardError.ReadToEnd();

        process.WaitForExit();

        exitCode = process.ExitCode;

        return exitCode == 0;
    }
}