using EnvDTE;
using EnvDTE80;
using NLog;
using System;
using System.IO;
using System.Text.RegularExpressions;
using TCatSysManagerLib;

namespace TcBlack
{
    /// <summary>
    /// This class is used to instantiate the Visual Studio Development Tools 
    /// Environment (DTE) which is used to programatically access all the functions 
    /// in VS.
    /// </summary>
    /// <remarks>Source: https://github.com/tcunit/TcUnit </remarks>
    class VisualStudioInstance
    {
        private readonly string solutionPath;
        private Type type;
        private Solution visualStudioSolution;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private bool loaded;

        public VisualStudioInstance(string visualStudioSolutionFilePath)
        {
            solutionPath = visualStudioSolutionFilePath;
            VisualStudioVersion = FindVisualStudioVersion();
        }

        /// <summary>
        /// Loads the development tools environment
        /// </summary>
        public void Load(string twincatVersion)
        {
            loaded = true;

            try
            {
                LoadDevelopmentToolsEnvironment(VisualStudioVersion, twincatVersion);
            }
            catch (Exception e)
            {
                string message = string.Format(
                    $"{e.Message} Error loading VS DTE version {VisualStudioVersion}. "
                    + $"Is the correct version of Visual Studio installed?"
                );
                Logger.Error(message);
                throw;
            }

            if (!string.IsNullOrEmpty(solutionPath))
            {
                try
                {
                    LoadSolution(solutionPath);
                }
                catch (Exception e)
                {
                    string message = string.Format(
                        $"{e.Message} Error loading solution at \"{solutionPath}\". "
                        + $"Is the path correct?"
                    );
                    Logger.Error(message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Closes the DTE and makes sure the VS process is completely shutdown
        /// </summary>
        public void Close()
        {
            if (loaded)
            {
                Logger.Info(
                    "Closing the Visual Studio Development Tools Environment (DTE), "
                    + "please wait..."
                );
                // Makes sure that there are no visual studio processes left in the 
                // system if the user interrupts this program (for example by CTRL+C)
                //Thread.Sleep(20000);
                DevelopmentToolsEnvironment.Quit();
            }
            loaded = false;
        }

        /// <summary>
        /// Opens the main *.sln-file and finds the version of VS used for creation of 
        /// the solution
        /// </summary>
        /// <returns>The version of Visual Studio used to create the solution</returns>
        private string FindVisualStudioVersion()
        {
            string file;
            try
            {
                file = File.ReadAllText(solutionPath);
            }
            catch (ArgumentException)
            {
                return null;
            }

            string pattern = @"^VisualStudioVersion\s+=\s+(?<version>\d+\.\d+)";
            Match match = Regex.Match(file, pattern, RegexOptions.Multiline);

            if (match.Success)
            {
                Logger.Info(
                    $"Found visual studio version {match.Groups[1].Value} in solution file."
                );
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }

        private void LoadDevelopmentToolsEnvironment(
            string visualStudioVersion, string remoteManagerVersion
        )
        {
            // Make sure the DTE loads with the same version of Visual Studio as the
            // TwinCAT project was created in
            string VisualStudioProgId = "VisualStudio.DTE." + visualStudioVersion;
            type = Type.GetTypeFromProgID(VisualStudioProgId);
            Logger.Info(
                "Loading the Visual Studio Development Tools Environment (DTE)..."
            );
            // have devenv.exe automatically close when launched using automation
            DevelopmentToolsEnvironment = (DTE2)Activator.CreateInstance(type, true);
            DevelopmentToolsEnvironment.UserControl = false;
            DevelopmentToolsEnvironment.SuppressUI = true;
            DevelopmentToolsEnvironment.ToolWindows.ErrorList.ShowErrors = true;
            DevelopmentToolsEnvironment.ToolWindows.ErrorList.ShowMessages = true;
            DevelopmentToolsEnvironment.ToolWindows.ErrorList.ShowWarnings = true;
            Logger.Debug("Getting Tc automation settings");
            var tcAutomationSettings = 
                DevelopmentToolsEnvironment.GetObject("TcAutomationSettings");
            tcAutomationSettings.SilentMode = true;
            // Uncomment this if you want to run a specific version of TwinCAT
            Logger.Debug("Set remote manager version.");
            ITcRemoteManager remoteManager = 
                DevelopmentToolsEnvironment.GetObject("TcRemoteManager");
            remoteManager.Version = remoteManagerVersion;
        }

        private void LoadSolution(string filePath)
        {
            Logger.Debug($"Loading solution {filePath}");
            visualStudioSolution = DevelopmentToolsEnvironment.Solution;
            visualStudioSolution.Open(filePath);
        }

        public void BuildProject(string projectName)
        {
            visualStudioSolution.SolutionBuild.BuildProject(
                "Release|TwinCAT RT (x64)", projectName, true
            );
        }

        /// <returns>Returns null if no version was found</returns>
        public string VisualStudioVersion { get; private set; }

        public Project Project { get; private set; }

        public DTE2 DevelopmentToolsEnvironment { get; private set; }

    }
}
