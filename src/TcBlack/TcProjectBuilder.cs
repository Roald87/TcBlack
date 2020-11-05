using System;
using System.Collections.Generic;
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
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly string slnPath;
        private readonly string projectPath;
        private readonly string tcVersion;
        private static VisualStudioInstance vsInstance = null;

        public TcProjectBuilder(string projectOrTcPouPath)
        {
            tcVersion = GetTwinCatVersionFromTsprojFile(projectOrTcPouPath);
            projectPath = GetParentPath(projectOrTcPouPath, ".plcproj");
            slnPath = GetParentPath(projectOrTcPouPath, ".sln");
        }

        /// <summary>
        /// Tries to get the version number.
        /// </summary>
        /// <param name="projectOrTcPouPath"></param>
        /// <returns></returns>
        private string GetTwinCatVersionFromTsprojFile(string projectOrTcPouPath)
        {
            string version = "";
            try
            {
                string tsprojPath = GetTsprojPath(projectOrTcPouPath);
                version = GetTwinCatVersion(tsprojPath);
            }
            catch (FileNotFoundException)
            {
            }

            return version;
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
        /// Build the project file.
        /// </summary>
        public TcProjectBuilder Build()
        {
            TryLoadSolution();
            TryBuildTwinCatProject();

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        private void TryLoadSolution()
        {
            Logger.Info("Starting solution...");
            try
            {
                MessageFilter.Register();
                vsInstance = new VisualStudioInstance(slnPath);
                vsInstance.Load(tcVersion);
            }
            catch
            {
                // Detailed error messages output by vsInstance.Load()
                Logger.Error("Solution load failed");  
                CleanUp();
                throw new ProjectBuildFailed();
            }
        }

        private void TryBuildTwinCatProject()
        {
            Logger.Info("Building TwinCAT project...");
            vsInstance.BuildProject(projectPath);
        }

        /// <summary>
        /// Cleans the system resources (the VS DTE)
        /// </summary>
        private static void CleanUp()
        {
            try
            {
                vsInstance.Close();
            }
            catch
            {
            }

            Logger.Info("Exiting application...");
            MessageFilter.Revoke();
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
    }
}
