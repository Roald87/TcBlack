using System;
using System.Diagnostics;
using System.IO;

namespace TcBlack
{
    class Program
    {
        static void Main(string[] args)
        {
            string devenvPath = "\"C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/Common7/IDE/devenv.exe\"";
            string slnPath = "\"C:/Users/roald/Source/Repos/TcBlack/src/TcBlack.sln\"";
            string projectPath = "\"TwinCATBlack/PLC/PLC.plcproj\"";
            BuildTwinCatProject(devenvPath, slnPath, projectPath);
            
            Console.ReadKey();
        }

        static void BuildTwinCatProject(string devenvPath, string slnPath, string projectPath)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string buildScript = Path.Combine(
                currentDirectory, "BuildTwinCatProject.bat"
            );

            ExecuteCommand($"{buildScript} {devenvPath} {slnPath} {projectPath}");
        }

        /// <summary>
        /// Source: https://stackoverflow.com/a/5519517/6329629
        /// </summary>
        /// <param name="command"></param>
        static void ExecuteCommand(string command)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            var process = Process.Start(processInfo);

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                Console.WriteLine("output>>" + e.Data);
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                Console.WriteLine("error>>" + e.Data);
            process.BeginErrorReadLine();

            process.WaitForExit();

            Console.WriteLine("ExitCode: {0}", process.ExitCode);
            process.Close();
        }
    }
}
