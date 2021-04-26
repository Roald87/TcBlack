using TcBlackCLI;

namespace TcBlackTests
{
    public class MockTcProjectBuilder : TcProjectBuilder
    {
        public MockTcProjectBuilder(
            string projectPath, string buildLogPath
        ) : base(projectPath)
        {
            BuildLogFile = buildLogPath;
        }

        /// <summary>
        /// Doesn't run cmd.exe in the mock implementation.
        /// </summary>
        /// <param name="command">
        /// This argument doesn't have an effect in the mock implementation
        /// </param>
        protected override void ExecuteCommand(string command, bool verbose)
        {
        }
    }
}
