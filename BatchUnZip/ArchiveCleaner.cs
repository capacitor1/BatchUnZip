using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ArchiveExtractor;

internal static class ArchiveCleaner
{
    public static void DeleteArchiveSet(string archiveFile)
    {
        string ext =
            Path.GetExtension(archiveFile);

        TryDelete(archiveFile);

        if (ext.Equals(".zip",
                StringComparison.OrdinalIgnoreCase))
        {
            DeleteZipVolumes(archiveFile);
        }
        else if (ext.Equals(".001",
                     StringComparison.OrdinalIgnoreCase))
        {
            DeleteNumericVolumes(archiveFile);
        }
    }

    private static void DeleteZipVolumes(
        string archiveFile)
    {
        string dir =
            Path.GetDirectoryName(archiveFile)!;

        string baseName =
            Path.GetFileNameWithoutExtension(
                archiveFile);

        Regex regex = new(
            "^" +
            Regex.Escape(baseName) +
            @"\.z\d+$",
            RegexOptions.IgnoreCase);

        foreach (string file in Directory.EnumerateFiles(dir))
        {
            string fileName = Path.GetFileName(file);

            if (!regex.IsMatch(fileName))
                continue;

            TryDelete(file);
        }
    }

    private static void DeleteNumericVolumes(
        string archiveFile)
    {
        string dir =
            Path.GetDirectoryName(archiveFile)!;

        string baseName =
            Path.GetFileNameWithoutExtension(
                archiveFile);

        Regex regex = new(
            "^" +
            Regex.Escape(baseName) +
            @"\.\d+$",
            RegexOptions.IgnoreCase);

        foreach (string file in Directory.EnumerateFiles(dir))
        {
            string fileName = Path.GetFileName(file);

            if (!regex.IsMatch(fileName))
                continue;

            TryDelete(file);
        }
    }

    private static void TryDelete(string file)
    {
        try
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
        catch
        {
            // ignore
        }
    }
}