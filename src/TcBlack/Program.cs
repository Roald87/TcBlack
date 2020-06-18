using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace TcBlack
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    string files = string.Join(
                        "\n", 
                        o.Filenames.Select(filename => $"  - {filename}").ToArray()
                    );

                    if (o.Safe)
                    {
                        Console.WriteLine(
                            $"\nFormatting file(s) in safe mode:\n{files}\n"
                        );
                        SafeFormat(o);
                    }
                    else
                    {
                        Console.WriteLine(
                            $"\nFormatting file(s) in fast non-safe mode:\n{files}\n"
                        );
                        try
                        {
                            CreateBackups(o.Filenames.ToArray());
                        }
                        catch (FileNotFoundException)
                        {
                            Console.WriteLine(
                                $"One of the files doesn't exist. " +
                                $"Check the filesnames and try again."
                            );
                            return;
                        }
                        FormatAll(o.Filenames.ToArray());
                    }
                });
        }

        /// <summary>
        /// Compiles project before and after formatting to check if nothing changed.
        /// It does this by comparing the compile hash.
        /// </summary>
        /// <param name="options">Input from the command line.</param>
        static void SafeFormat(Options options)
        {
            Console.WriteLine("Building project before formatting.");
            TcProjectBuilder tcProject = new TcProjectBuilder(
                options.Filenames.First()
            );
            string hashBeforeFormat = string.Empty;
            try
            {
                hashBeforeFormat = tcProject.Build(options.Verbose).Hash;
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
                backups = CreateBackups(options.Filenames.ToArray());
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(
                    $"One of the files doesn't exist. " +
                    $"Check the filesnames and try again."
                );
                return;
            }
            FormatAll(options.Filenames.ToArray());

            string hashAfterFormat = string.Empty;
            try
            {
                hashAfterFormat = tcProject.Build(options.Verbose).Hash;
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
        }

        /// <summary>
        /// Reformat all TcPou files.
        /// </summary>
        /// <param name="filenames">Filesnames which should be formatted.</param>
        static void FormatAll(string[] filenames)
        {
            foreach (string filename in filenames)
            {
                new TcPou(filename).Format().Save();
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

        /// <summary>
        /// Options for command line interface.
        /// </summary>
        class Options
        {
            [Option(
                'f', "filenames",
                HelpText = "File(s) you want to reformat.",
                Required = true
            )]
            public IEnumerable<string> Filenames { get; set; }

            [Option(
                's', "safe",
                HelpText =
                    "Compiles project before and after formatting, in order to check "
                    + "if the code has changed. WARNING: Takes > 30 seconds!"
            )]
            public bool Safe { get; set; }

            [Option(
                'v', "verbose",
                HelpText = "Outputs build info. Has no effect in non-safe mode."
            )]
            public bool Verbose { get; set; }
        }
    }
}
