using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArchiveExtractor;

internal static class ArchiveScanner
{
    private static readonly HashSet<string> SupportedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".zip",
            ".001",
            ".7z",
            ".rar",
            ".xz",
            ".tar"
        };

    public static string[] ScanArchives(string rootDir)
    {
        return Directory
            .EnumerateFiles(
                rootDir,
                "*",
                SearchOption.AllDirectories)
            .Where(IsSupportedArchive)
            .OrderBy(x => x)
            .ToArray();
    }

    private static bool IsSupportedArchive(string file)
    {
        string ext = Path.GetExtension(file);
        return SupportedExtensions.Contains(ext);
    }
}