using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using CommandLine;
using TcBlackCore;

[assembly: CLSCompliant(true)]
namespace TcBlackCLI
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                string[] filenames;
                try
                {
                    filenames = FilesToFormat(options);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"Unable to find file {options.Project}");
                    return;
                }

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
                    SafeFormat(filenames, options);
                }
                else
                {
                    Console.WriteLine(
                        $"\nFormatting {filenames.Length} file(s) "
                        + $"in fast non-safe mode:\n{fileListForCommandPrompt}\n"
                    );
                    try
                    {
                        CreateBackups(options.File.ToArray());
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine(
                            $"One of the files doesn't exist. " +
                            $"Check the filenames and try again."
                        );
                        return;
                    }
                    FormatAll(filenames, options);
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
                return options.File.ToArray();
            }
        }

        /// <summary>
        /// Compiles project before and after formatting to check if nothing changed.
        /// It does this by comparing the compile hash.
        /// </summary>
        /// <param name="filenames">Files to format.</param>
        /// <param name="options">Input from the command line.</param>
        static void SafeFormat(string[] filenames, Options options)
        {
            Console.WriteLine("Building project before formatting.");
            TcProjectBuilder tcProject;
            try
            {
                tcProject = new TcProjectBuilder(filenames.First());
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"{ex.Message}\nCancelling build.");
                return;
            }

            string hashBeforeFormat = string.Empty;
            try
            {
                hashBeforeFormat = tcProject.Build(options.Verbose).Hash;
            }
            catch(ProjectBuildFailedException)
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
                    $"Check the filenames and try again."
                );
                return;
            }
            FormatAll(filenames, options);

            Console.WriteLine("Building project after formatting.");
            string hashAfterFormat = string.Empty;
            try
            {
                hashAfterFormat = tcProject.Build(options.Verbose).Hash;
            }
            catch(ProjectBuildFailedException)
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
        /// Reformat all TcPou and TcIO files.
        /// </summary>
        /// <param name="filenames">Files to format.</param>
        /// <param name="options">Options to use for the formatting.</param>
        static void FormatAll(string[] filenames, Options options)
        {
            foreach (string filename in filenames)
            {
                if (
                    options.Indentation.Length > 0 
                    && (options.WindowsLineEnding || options.UnixLineEnding)
                )
                {
                    new TcPou(filename, options.Indentation, options.WindowsLineEnding)
                        .Format()
                        .Save();
                }
                else if (options.Indentation.Length > 0)
                {
                    new TcPou(filename, options.Indentation).Format().Save();
                }
                else if (options.WindowsLineEnding || options.UnixLineEnding)
                {
                    new TcPou(filename, options.WindowsLineEnding).Format().Save();
                }
                else
                {
                    new TcPou(filename).Format().Save();
                }
            }
            Console.WriteLine($"Formatted {filenames.Count()} file(s).");
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
