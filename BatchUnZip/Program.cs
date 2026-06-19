using System;
using System.Text;

namespace ArchiveExtractor;

internal static class Program
{
    static int Main(string[] args)
    {
        Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;
        if (args.Length != 1)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("BatchUnZip <Directory>");
            return 1;
        }

        string rootDir = Path.GetFullPath(args[0]);

        if (!Directory.Exists(rootDir))
        {
            Console.WriteLine($"Directory not found: {rootDir}");
            return 1;
        }

        var archives = ArchiveScanner.ScanArchives(rootDir);
        HashSet<string> failed = new();

        while(archives.Length > 0)
        {
            foreach (string archive in archives)
            {
                string outputDir = archive + ".dec_temp";

                bool success = SevenZipExtractor.Extract(
                    archive,
                    outputDir,
                    out int exitCode);

                Console.WriteLine(
                    $"[7-zip] Processed '{Path.GetFileName(archive)}' with exit code {exitCode}");

                if (success)
                {
                    try
                    {
                        ArchiveCleaner.DeleteArchiveSet(archive);
                        Directory.Move(
                            archive + ".dec_temp",
                            archive);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"[Program] Rename failed: {ex.Message}");
                    }
                }
                else
                {
                    failed.Add(archive);
                }
            }
            archives = ArchiveScanner.ScanArchives(rootDir)
                .Where(x => !failed.Contains(x))
                .ToArray();
        }

        return 0;
    }
}