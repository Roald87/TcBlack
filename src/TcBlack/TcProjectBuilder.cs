using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TcBlack
{
    public class ProjectBuildFailed : Exception
    {
        public ProjectBuildFailed()
        {
        }
    }

    /// <summary>
    /// Builds a TwinCAT project using the devenv.
    /// </summary>
    public class TcProjectBuilder
    {
        private readonly string devenvPath;
        private readonly string slnPath;
        private readonly string projectPath;
        private readonly string tcVersion;
        protected string buildLogFile = "build.log";

        public TcProjectBuilder(string projectOrTcPouPath)
        {
            tcVersion = GetTwinCatVersionFromTsprojFile(projectOrTcPouPath);
            projectPath = GetParentPath(projectOrTcPouPath, ".plcproj");
            slnPath = GetParentPath(projectOrTcPouPath, ".sln");
            string vsVersion = GetVsVersion(slnPath);
            devenvPath = GetDevEnvPath(vsVersion);
        }

        /// <summary>
        /// Tries to get the version number.
        /// </summary>
        /// <param name="projectOrTcPouPath"></param>
        /// <returns></returns>
        private string GetTwinCatVersionFromTsprojFile(string projectOrTcPouPath)
        {
            string tcVersion = "";
            try
            {
                string tsprojPath = GetTsprojPath(projectOrTcPouPath);
                tcVersion = GetTwinCatVersion(tsprojPath);
            }
            catch (FileNotFoundException)
            {
            }

            return tcVersion;
        }

        /// <summary>
        /// Return the path to the *.tsp(p)roj file.
        /// </summary>
        /// <param name="projectOrTcPouPath">Path to start the search from.</param>
        /// <returns>Path to the *.tsp(p)roj file or FileNotFoundException.</returns>
        private string GetTsprojPath(string projectOrTcPouPath)
        {
            string tsprojPath = "";
            string[] tsprojExtensions = new string[] { ".tsproj", ".tspproj" };
            foreach (string tsprojExtension in tsprojExtensions)
            {
                try
                {
                    tsprojPath = GetParentPath(projectOrTcPouPath, tsprojExtension);
                }
                catch(FileNotFoundException)
                {
                }
            }

            return tsprojPath;
        }

        /// <summary>
        /// Searched through the current and its parent paths for the first occurence
        /// of a file with given extension
        /// </summary>
        /// <param name="extension">The extension to look for.</param>
        /// <returns>Path to the directory with given extension.</returns>
        private string GetParentPath(string startingPath, string extension)
        {
            string path = "";
            string parentPath = Path.GetDirectoryName(startingPath);
            string exceptionMessage = (
                $"Unable to find a {extension} file in any of the "
                + $"parent folders of {startingPath}."
            );

            while (true)
            {
                try
                {
                    IEnumerable<string> filesWithExtension = Directory.EnumerateFiles(
                            parentPath, $"*{extension}"
                        ).Where(
                            x => x.Substring(x.Length - extension.Length) == extension
                        );
                    path = filesWithExtension.Single();
                    break;
                }
                catch (Exception ex) when (
                    ex is DirectoryNotFoundException || ex is ArgumentException
                )
                {
                    throw new FileNotFoundException(exceptionMessage);
                }
                catch (InvalidOperationException)
                {
                }

                try
                {
                    parentPath = Directory.GetParent(parentPath).ToString();
                }
                catch (NullReferenceException)
                {
                    throw new FileNotFoundException(exceptionMessage);
                }
            }

            return path;
        }

        /// <summary>
        /// Return the Visual Studio version from the solution file.
        /// </summary>
        /// <param name="slnPath">Path the solution file.</param>
        /// <returns>Major and minor version number of Visual Studio.</returns>
        private string GetVsVersion(string slnPath)
        {
            string file;
            try
            {
                file = File.ReadAllText(slnPath);
            }
            catch (ArgumentException)
            {
                return "";
            }

            string pattern = @"^VisualStudioVersion\s+=\s+(?<version>\d+\.\d+)";
            Match match = Regex.Match(
                file, pattern, RegexOptions.Multiline
            );

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Return the TwinCAT version from the tsproj file.
        /// </summary>
        /// <param name="tsprojPath">Path the tsproj file.</param>
        /// <returns>Version number of TwinCAT.</returns>
        private string GetTwinCatVersion(string tsprojPath)
        {
            string file;
            try
            {
                file = File.ReadAllText(tsprojPath);
            }
            catch (ArgumentException)
            {
                return "";
            }

            string pattern = "TcVersion=\"(\\d\\.\\d\\.\\d{4}\\.\\d+)\"";
            Match match = Regex.Match(
                file, pattern, RegexOptions.Multiline
            );

            return match.Success ? match.Groups[1].Value : "";
        }

        /// <summary>
        /// Return the path to devenv.com of the given Visual Studio version.
        /// </summary>
        /// <param name="vsVersion">
        /// Visual Studio version to get the devenv.com path of.
        /// </param>
        /// <returns>
        /// The path to devenv.com of the given Visual Studio version.
        /// </returns>
        private string GetDevEnvPath(string vsVersion)
        {
            RegistryKey rkey = Registry.LocalMachine
                .OpenSubKey(
                    @"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\SxS\VS7", false
                );

            try
            {
                return Path.Combine(
                    rkey.GetValue(vsVersion).ToString(), 
                    "Common7", 
                    "IDE", 
                    "devenv.com"
                );
            }
            catch (NullReferenceException)
            {
                return "";
            }
        }

        /// <summary>
        /// Build the project file.
        /// </summary>
        public TcProjectBuilder Build(bool verbose)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string buildScript = Path.Combine(
                currentDirectory, "BuildTwinCatProject.bat"
            );

            ExecuteCommand(
                $"{buildScript} \"{devenvPath}\" \"{slnPath}\" \"{projectPath}\"",
                verbose
            );

            string buildLog = File.ReadAllText(buildLogFile);
            if (BuildFailed(buildLog))
            {
                throw new ProjectBuildFailed();
            }

            return this;
        }

        /// <summary>
        /// Reads the last line from the build.log file to see if the build failed.
        /// </summary>
        /// <param name="buildLog">The </param>
        public bool BuildFailed(string buildLog)
        {
            string pattern = 
                @"(?:========== Build: )(\d+)(?:[a-z \-,]*)(\d+)(?:[a-z \-,]*)";
            MatchCollection matches = Regex.Matches(buildLog, pattern);
            if (matches.Count > 0)
            {
                var lastMatch = matches[matches.Count - 1];
                int buildsFailed = Convert.ToInt16(lastMatch.Groups[2].Value);

                return buildsFailed != 0;
            }

            return false;
        }

        /// <summary>
        /// Get the hash of the compiled project. Hash doesn't change when whitespaces
        /// are adjusted or comments are added/removed.
        /// </summary>
        public string Hash {
            get
            {
                if (projectPath.Length == 0)
                {
                    return "";
                }
                try
                {
                    string projectDirectory = Path.GetDirectoryName(projectPath);
                    string compileDirectory = Path.Combine(
                        projectDirectory, "_CompileInfo"
                    );
                    string latestCompileInfo = new DirectoryInfo(compileDirectory)
                        .GetFiles()
                        .OrderByDescending(f => f.LastWriteTime)
                        .First()
                        .Name;
                    return Path.GetFileNameWithoutExtension(latestCompileInfo);
                }
                catch (DirectoryNotFoundException)
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Execute a command in the windown command prompt cmd.exe.
        /// Source: https://stackoverflow.com/a/5519517/6329629
        /// </summary>
        /// <param name="command"></param>
        protected virtual void ExecuteCommand(string command, bool verbose)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            var process = Process.Start(processInfo);

            if (verbose)
            {
                process.OutputDataReceived += (object sender, DataReceivedEventArgs e)
                    => Console.WriteLine("output >> " + e.Data);
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (object sender, DataReceivedEventArgs e)
                    => Console.WriteLine("error >> " + e.Data);
                process.BeginErrorReadLine();
            }

            process.WaitForExit();
            if (verbose)
            {
                Console.WriteLine("ExitCode: {0}", process.ExitCode);
            }
            process.Close();
        }
    }
}
