using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using CommandLine;

namespace TcBlack
{
    class Program
    {
        /// <summary>
        /// Options for command line interface.
        /// </summary>
        class Options
        {
            [Option(
                HelpText = "TcPOU/TcIO file(s) to format.",
                SetName = "FilesToFormat"
            )]
            public IEnumerable<string> Filenames { get; set; }
            [Option(
                Default = "",
                HelpText = "Plc project to format.",
                SetName = "FilesToBuild"
            )]
            public string Project { get; set; }

            [Option(
                Default = false,
                HelpText =
                    "Compiles project before and after formatting, in order to check "
                    + "if the code has changed. WARNING: Takes > 30 seconds!"
            )]
            public bool Safe { get; set; }

            [Option(
                Default = false,
                HelpText = "Outputs build info. Has no effect in non-safe mode."
            )]
            public bool Verbose { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    string[] filenames = FilesToFormat(options);

                    string fileListForCommandPrompt = string.Join(
                        "\n",
                        filenames.Select(filename => $"  - {filename}").ToArray()
                    );

                    if (options.Safe)
                    {
                        Console.WriteLine(
                            $"\nFormatting {filenames.Length} file(s) "
                            + $"in safe mode:\n{fileListForCommandPrompt}\n"
                        );
                        SafeFormat(filenames, options.Verbose);
                    }
                    else
                    {
                        Console.WriteLine(
                            $"\nFormatting {filenames.Length} file(s) "
                            + $"in fast non-safe mode:\n{fileListForCommandPrompt}\n"
                        );
                        try
                        {
                            CreateBackups(options.Filenames.ToArray());
                        }
                        catch (FileNotFoundException)
                        {
                            Console.WriteLine(
                                $"One of the files doesn't exist. " +
                                $"Check the filesnames and try again."
                            );
                            return;
                        }
                        FormatAll(filenames);
                    }
                });
        }

        /// <summary>
        /// Return all the files which should be formatted.
        /// </summary>
        /// <param name="options">Input from the user.</param>
        /// <returns>Array with full path to the files.</returns>
        static string[] FilesToFormat(Options options)
        {
            if (options.Project.Length > 0)
            {
                Regex extensions = new Regex(@"(TcPOU|TcIO)$");
                string projectDirectory = Path.GetDirectoryName(options.Project);
                return Directory.EnumerateFiles(
                    projectDirectory, "*.*", SearchOption.AllDirectories
                ).Where(f => extensions.IsMatch(f)).ToArray();
            }
            else
            {
                return options.Filenames.ToArray();
            }
        }

        /// <summary>
        /// Compiles project before and after formatting to check if nothing changed.
        /// It does this by comparing the compile hash.
        /// </summary>
        /// <param name="options">Input from the command line.</param>
        static void SafeFormat(string[] filenames, bool verbose)
        {
            Console.WriteLine("Building project before formatting.");
            TcProjectBuilder tcProject = new TcProjectBuilder(filenames.First());
            string hashBeforeFormat = string.Empty;
            try
            {
                hashBeforeFormat = tcProject.Build(verbose).Hash;
            }
            catch(ProjectBuildFailed)
            {
                Console.WriteLine(
                    "Initial project build failed! No formatting will be done."
                );
                return;
            }

            List<Backup> backups;
            try
            {
                backups = CreateBackups(filenames);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(
                    $"One of the files doesn't exist. " +
                    $"Check the filesnames and try again."
                );
                return;
            }
            FormatAll(filenames);

            Console.WriteLine("Building project after formatting.");
            string hashAfterFormat = string.Empty;
            try
            {
                hashAfterFormat = tcProject.Build(verbose).Hash;
            }
            catch(ProjectBuildFailed)
            {
                Console.WriteLine(
                    "Project build failed after formatting! Undoing it."
                );
                backups.ForEach(backup => backup.Restore().Delete());
                return;
            }

            if (hashBeforeFormat != hashAfterFormat)
            {
                Console.WriteLine(
                    "Something when wrong during formatting! Undoing it."
                );
                backups.ForEach(backup => backup.Restore().Delete());
            }
            else
            {
                Console.WriteLine("All done and everything looks OK!");
            }
        }

        /// <summary>
        /// Reformat all TcPou files.
        /// </summary>
        /// <param name="filenames">Filesnames which should be formatted.</param>
        static void FormatAll(string[] filenames)
        {
            foreach (string filename in filenames)
            {
                new TcPou(filename).FormatDeclaration().FormatImplementation().Save();
            }
            Console.WriteLine($"Formatted {filenames.Length} file(s).");
        }

        /// <summary>
        /// Creates .bak files of the files.
        /// </summary>
        /// <param name="filenames">File to back-up.</param>
        /// <returns>List of files which were backed-up and can be restored.</returns>
        static List<Backup> CreateBackups(string[] filenames)
        {
            return filenames.Select(filename => new Backup(filename)).ToList();
        }
    }
}
