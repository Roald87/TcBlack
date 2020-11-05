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
        private DTE2 developmentToolsEnvironment;
        private bool loaded;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string solutionPath;
        private Type type;
        private Solution visualStudioSolution;
        private readonly string visualStudioVersion;

        public VisualStudioInstance(string visualStudioSolutionFilePath)
        {
            solutionPath = visualStudioSolutionFilePath;
            visualStudioVersion = FindVisualStudioVersion();
        }

        /// <summary>
        /// Loads the development tools environment
        /// </summary>
        public void Load(string twincatVersion)
        {
            loaded = true;

            try
            {
                LoadDevelopmentToolsEnvironment(visualStudioVersion, twincatVersion);
            }
            catch (Exception e)
            {
                string message = string.Format(
                    $"{e.Message} Error loading VS DTE version {visualStudioVersion}. "
                    + $"Is the correct version of Visual Studio installed?"
                );
                logger.Error(message);
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
                    logger.Error(message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Build a TwinCAT project.
        /// </summary>
        /// <param name="projectName">Path to plcproj file.</param>
        public void BuildProject(string projectName)
        {
            visualStudioSolution.SolutionBuild.BuildProject(
                "Debug", projectName, true
            );

            if (visualStudioSolution.SolutionBuild.LastBuildInfo > 0)
            {
                throw new ProjectBuildFailed();
            }
        }

        /// <summary>
        /// Closes the DTE and makes sure the VS process is completely shutdown
        /// </summary>
        public void Close()
        {
            if (loaded)
            {
                logger.Info(
                    "Closing the Visual Studio Development Tools Environment (DTE), "
                    + "please wait..."
                );
                // Makes sure that there are no visual studio processes left in the 
                // system if the user interrupts this program (for example by CTRL+C)
                System.Threading.Thread.Sleep(20000);
                developmentToolsEnvironment.Quit();
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
                logger.Info(
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
            logger.Info(
                "Loading the Visual Studio Development Tools Environment (DTE)..."
            );
            // have devenv.exe automatically close when launched using automation
            developmentToolsEnvironment = (DTE2)Activator.CreateInstance(type, true);
            developmentToolsEnvironment.UserControl = false;
            developmentToolsEnvironment.SuppressUI = true;
            developmentToolsEnvironment.ToolWindows.ErrorList.ShowErrors = true;
            developmentToolsEnvironment.ToolWindows.ErrorList.ShowMessages = true;
            developmentToolsEnvironment.ToolWindows.ErrorList.ShowWarnings = true;
            logger.Debug("Getting Tc automation settings");
            var tcAutomationSettings = 
                developmentToolsEnvironment.GetObject("TcAutomationSettings");
            tcAutomationSettings.SilentMode = true;
            // Uncomment this if you want to run a specific version of TwinCAT
            logger.Debug("Set remote manager version.");
            ITcRemoteManager remoteManager = 
                developmentToolsEnvironment.GetObject("TcRemoteManager");
            remoteManager.Version = remoteManagerVersion;
        }

        private void LoadSolution(string filePath)
        {
            logger.Debug($"Loading solution {filePath}");
            visualStudioSolution = developmentToolsEnvironment.Solution;
            visualStudioSolution.Open(filePath);
        }
    }
}
