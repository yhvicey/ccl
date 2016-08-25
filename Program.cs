using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CountCodeLine
{
    public class Program
    {
        private static string Usage { get; } =
            "Utility for counting code line's count.\n" +
            "Usage: ccl <RootFolder> <Suffix> [<Suffix>,...] [<Options>]\n" +
            "RootFolder: Folder to count.\n" +
            "Suffix: Suffix of code file.Start with '.'.\n" +
            "Options:\n" +
            "--ignore-blank, -I    Ignore blank lines.\n" +
            "--verbose, -V         Show process log.\n";

        private static string _rootFolder;

        private static List<string> _suffixs;

        private static bool _ignoreBlank;

        private static bool _verbose;

        private static void Main(string[] args)
        {
            var start = DateTime.Now;

            if (!ProcessArguments(args))
            {
                Console.WriteLine(Usage);
                return;
            }
             
            Console.WriteLine($"Total: {VisitDirectory(_rootFolder)} line(s).");

            Console.WriteLine($"Time: {(DateTime.Now - start).TotalMilliseconds:F0} ms.");
        }

        private static bool ProcessArguments(string[] args)
        {
            if (args.Length < 1)
            {
                return false;
            }

            _rootFolder = args[0].TrimEnd('\\', '/');

            _suffixs = args.Where(x => x.StartsWith(".")).Select(x => x.Split('.').Last()).ToList();

            _ignoreBlank = args.Any(x => x == "--ignore-blank" || x == "-I");

            _verbose = args.Any(x => x == "--verbose" || x == "-V");

            return true;
        }

        private static int VisitFile(string path)
        {
            try
            {
                var count = _ignoreBlank
                    ? File.ReadAllLines(path).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x).Count()
                    : File.ReadAllLines(path).Length;

                if (_verbose && count > 0)
                {
                    Console.WriteLine($"{path}: {count} line(s).");
                }

                return count;
            }
            catch (Exception)
            {
                if (_verbose)
                {
                    Console.WriteLine($"Can't read {path}.");
                }
                return 0;
            }
        }

        private static int VisitDirectory(string path)
        {
            try
            {
                var dirs = Directory.GetDirectories(path);
                var count = dirs.Sum(VisitDirectory);

                var files = Directory.GetFiles(path);
                count += files.Where(file => _suffixs.Any(x => x == file.Split('.').Last())).Sum(VisitFile);

                if (_verbose && count > 0)
                {
                    Console.WriteLine($"{path}: {count} line(s).");
                }
                return count;
            }
            catch (Exception)
            {
                if (_verbose)
                {
                    Console.WriteLine($"Can't read {path}.");
                }
                return 0;
            }
        }
    }
}
