using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CountCodeLine
{
    public class Program
    {
        private static string Usage { get; } =
@"Utility for counting code line's count.
    Usage: ccl <RootFolder> <Suffix> [<Suffix>,...] [<Options>]
    Options:
        --blank-lines, -b       Count blank lines.
        --hidden-folders, -f    Count hidden folders (Start with '.').
        --verbose, -v           Show process log.";

        private static string _rootFolder;

        private static readonly List<string> Suffixs = new List<string>();

        private static bool _countBlank;

        private static bool _countHidden;

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

        private static void PrintUsage()
        {
            Console.WriteLine(Usage);
            Environment.Exit(0);
        }

        private static bool ProcessArguments(IReadOnlyList<string> args)
        {
            if (args.Count < 1) PrintUsage();

            var index = 0;
            foreach (var arg in args)
            {
                if (index == 0) _rootFolder = args[index];
                else
                {
                    switch (arg)
                    {
                        case "--blank-lines":
                        case "-b":
                        {
                            _countBlank = true;
                            continue;
                        }
                        case "--hidden-folders":
                        case "-f":
                        {
                            _countHidden = true;
                            continue;
                        }
                        case "--verbose":
                        case "-v":
                        {
                            _verbose = true;
                            continue;
                        }
                        default:
                        {
                            Suffixs.Add(arg);
                            continue;
                        }
                    }
                }
                index++;
            }

            return true;
        }

        private static int VisitFile(string path)
        {
            try
            {
                var count = _countBlank
                    ? File.ReadAllLines(path).Length
                    : File.ReadAllLines(path).Count(x => !string.IsNullOrWhiteSpace(x));

                if (_verbose && count > 0) Console.WriteLine($"{path}: {count} line(s).");

                return count;
            }
            catch (Exception ex)
            {
                if (_verbose) Console.WriteLine($"Can't read {path}. Error: {ex}");
                return 0;
            }
        }

        private static int VisitDirectory(string path)
        {
            try
            {
                var dirs = _countHidden
                    ? Directory.GetDirectories(path)
                    : Directory.GetDirectories(path)
                        .Where(dir => !dir.Split('/', '\\').LastOrDefault()?.StartsWith(".") ?? false);
                var count = dirs.Sum(VisitDirectory);

                var files = Directory.GetFiles(path).Where(file => Suffixs.Any(suffix => suffix == file.Split('.').LastOrDefault()));
                count += files.Sum(VisitFile);

                if (_verbose && count > 0) Console.WriteLine($"{path}: {count} line(s).");
                return count;
            }
            catch (Exception ex)
            {
                if (_verbose) Console.WriteLine($"Can't read {path}. Error: {ex}");
                return 0;
            }
        }
    }
}
