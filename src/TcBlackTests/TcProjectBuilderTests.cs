using System;
using System.IO;
using TcBlack;
using Xunit;

namespace TcBlackTests
{
    public class TcProjectBuilderTests
    {
        private static readonly string currentDirectory = Environment.CurrentDirectory;
        private static readonly string testDirectory = 
            Directory.GetParent(currentDirectory).Parent.FullName;
        private static readonly string projectDirectory = 
            Directory.GetParent(testDirectory).FullName;

        [Fact]
        public void GetHashOfProjectWithHash()
        {
            var plcProject = new TcProjectBuilder(Path.Combine(
                projectDirectory, "WorkingProjectForUnitTests", "PLC", "PLC.plcproj"
            ));

            Assert.Equal("7526D772-C42C-771C-E7F5-8B6DA4DF5F84", plcProject.Hash);
        }

        [Fact]
        public void InitializeWithNonExistingPathRaiseException()
        {
            Assert.Throws<FileNotFoundException>(
                () => new TcProjectBuilder("Non/Existing/Path/PLC.plcproj")
            );
        }

        // Warning: Takes ~30 s to complete.
        [StaFact]
        public void BuildRealBrokenProjectShouldRaiseException()
        {
            string brokenPlcProjectPath = Path.Combine(
                projectDirectory, "BrokenProjectForUnitTests", "PLC2", "PLC2.plcproj"
            );
            var plcProject = new TcProjectBuilder(brokenPlcProjectPath);
            Assert.Throws<ProjectBuildFailed>(() => plcProject.Build());
        }

        // Warning: takes ~30 s to complete.
        [StaFact]
        public void BuildRealWorkingProjectShouldMakeNewCompiledFile()
        {
            string workingPlcProjectPath = Path.Combine(
                projectDirectory, "WorkingProjectForUnitTests", "PLC", "PLC.plcproj"
            );
            var plcProject = new TcProjectBuilder(workingPlcProjectPath);
            var hash = plcProject.Build().Hash;
            string workingProjectDirectory = Path.GetDirectoryName(
                workingPlcProjectPath
            );
            var compileDate = File.GetLastWriteTime(Path.Combine(
                workingProjectDirectory, "_CompileInfo", $"{hash}.compileinfo"
            ));
            Assert.Equal(
                compileDate,
                DateTime.Now,
                new TimeSpan(hours: 0, minutes: 1, seconds: 0)
            );
        }

        [Theory]
        [InlineData("PLC.plcproj")]
        [InlineData("Non/Existing/Path/PLC.plcproj")]
        public void TryGetHashOfNonExistingProject(string projectPath)
        {
            Assert.Throws<FileNotFoundException>(
                ()=> new TcProjectBuilder(projectPath)
            );
        }

        private static readonly string workingProjectPouDirectory = Path.Combine(
            projectDirectory, "WorkingProjectForUnitTests", "PLC", "POUs"
        );
        [Theory]
        [InlineData("Sum.TcPOU")]
        [InlineData("MAIN.TcPOU")]
        public void GetProjectHashFromSingleTcPouFilename(string filename)
        {
            string path = Path.Combine(workingProjectPouDirectory, filename);
            var plcProject = new TcProjectBuilder(path);

            Assert.Equal("7526D772-C42C-771C-E7F5-8B6DA4DF5F84", plcProject.Hash);
        }

        [Fact]
        public void TryToBuildProjectWithoutPlcprojFile()
        {
            string path = "C:/Program Files";
            var exception = Assert.Throws<FileNotFoundException>(
                () => new TcProjectBuilder(path)
            );
            Assert.Equal(
                $"Unable to find a .plcproj file in any of the parent folders of "
                + $"{path}.",
                exception.Message
            );
        }

        [Fact]
        public void TryToBuildProjectWithoutSlnFile()
        {
            string tempPlcProjFile = Path.Combine(
                projectDirectory, "../UnitTest.plcproj"
            );
            if (!File.Exists(tempPlcProjFile))
            {
                File.Create(tempPlcProjFile).Close();
            }
            var exception = Assert.Throws<FileNotFoundException>(
                () => new TcProjectBuilder(tempPlcProjFile)
            );
            Assert.Equal(
                $"Unable to find a .sln file in any of the parent folders of " 
                + $"{tempPlcProjFile}.",
                exception.Message
            );
            File.Delete(tempPlcProjFile);
        }


        [Fact]
        public void FindOnlyExactExtensionForThreeCharacterExtensions()
        {
            //var plcProject = new TcProjectBuilder(
            //    @"C:\Users\roald\Source\Repos\TcBlack\src\ShowcaseProject\PLC3\POUs\FB_Child.TcPOU"
            //);
            string projectPath = Path.GetFullPath(
                "../../../WorkingProjectForUnitTests/PLC/plcproj"
            );
            var exception = Record.Exception(() => new TcProjectBuilder(projectPath));
            Assert.Null(exception);
        }
    }
}
