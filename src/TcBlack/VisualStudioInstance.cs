using EnvDTE80;
using NLog;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
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
        private string @filePath = null;
        private Type type = null;
        private EnvDTE.Solution visualStudioSolution = null;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private bool loaded = false;

        public VisualStudioInstance(string @visualStudioSolutionFilePath)
        {
            filePath = visualStudioSolutionFilePath;
            VisualStudioVersion = FindVisualStudioVersion();
        }

        public VisualStudioInstance(int vsVersionMajor, int vsVersionMinor)
        {
            VisualStudioVersion = 
                $"{vsVersionMajor.ToString()}.{vsVersionMinor.ToString()}";
        }

        /// <summary>
        /// Loads the development tools environment
        /// </summary>
        public void Load()
        {
            loaded = true;

            try
            {
                LoadDevelopmentToolsEnvironment(VisualStudioVersion);
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

            if (!string.IsNullOrEmpty(@filePath))
            {
                try
                {
                    LoadSolution(@filePath);
                    LoadProject();
                }
                catch (Exception e)
                {
                    string message = string.Format(
                        $"{e.Message} Error loading solution at \"{filePath}\". "
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
                Thread.Sleep(20000);
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
            /* Find visual studio version */
            string file;
            try
            {
                file = File.ReadAllText(@filePath);
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
                    "In Visual Studio solution file, found visual studio version "
                    + $"{match.Groups[1].Value}"
                );
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }

        private void LoadDevelopmentToolsEnvironment(string visualStudioVersion)
        {
            // Make sure the DTE loads with the same version of Visual Studio as the
            // TwinCAT project was created in
            string VisualStudioProgId = "VisualStudio.DTE." + visualStudioVersion;
            type = Type.GetTypeFromProgID(VisualStudioProgId);
            Logger.Info(
                "Loading the Visual Studio Development Tools Environment (DTE)..."
            );
            DevelopmentToolsEnvironment = (DTE2)Activator.CreateInstance(type);
            // have devenv.exe automatically close when launched using automation
            DevelopmentToolsEnvironment.UserControl = false;
            DevelopmentToolsEnvironment.SuppressUI = true;
            DevelopmentToolsEnvironment.ToolWindows.ErrorList.ShowErrors = true;
            DevelopmentToolsEnvironment.ToolWindows.ErrorList.ShowMessages = true;
            DevelopmentToolsEnvironment.ToolWindows.ErrorList.ShowWarnings = true;
            var tcAutomationSettings = 
                DevelopmentToolsEnvironment.GetObject("TcAutomationSettings");
            tcAutomationSettings.SilentMode = true;
            // Uncomment this if you want to run a specific version of TwinCAT
            ITcRemoteManager remoteManager = 
                DevelopmentToolsEnvironment.GetObject("TcRemoteManager");
            remoteManager.Version = "3.1.4022.32";
        }

        private void LoadSolution(string filePath)
        {
            visualStudioSolution = DevelopmentToolsEnvironment.Solution;
            visualStudioSolution.Open(@filePath);
        }

        private void LoadProject()
        {
            Project = visualStudioSolution.Projects.Item(1);
        }

        public void CleanSolution()
        {
            visualStudioSolution.SolutionBuild.Clean(true);
        }

        /// <returns>Returns null if no version was found</returns>
        public string VisualStudioVersion { get; private set; }

        public EnvDTE.Project Project { get; private set; }

        public DTE2 DevelopmentToolsEnvironment { get; private set; }

        public ErrorItems ErrorItems =>
            DevelopmentToolsEnvironment.ToolWindows.ErrorList.ErrorItems;
    }
}
