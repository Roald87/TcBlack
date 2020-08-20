using System.IO;
using TcBlack;
using Xunit;

namespace TcBlackTests
{
    public class TcProjectBuilderTests
    {
        [Fact]
        public void GetHashOfProjectWithHash()
        {
            var plcProject = new TcProjectBuilder(
                "../../../../WorkingProjectForUnitTests/PLC/PLC.plcproj"
            );

            Assert.Equal("7526D772-C42C-771C-E7F5-8B6DA4DF5F84", plcProject.Hash);
        }

        [Fact]
        public void InitializeWithNonExistingPathRaiseException()
        {
            Assert.Throws<FileNotFoundException>(
                () => new TcProjectBuilder("Non/Existing/Path/PLC.plcproj")
            );
        }

        [Fact]
        public void BuildMockBrokenProjectShouldRaiseException()
        {
            var plcProject = new MockTcProjectBuilder(
                "../../../../BrokenProjectForUnitTests/PLC2/PLC2.plcproj",
                "../../../TcProjectBuildTestData/failedBuildWithExtraTextBelow.log"
            );
            Assert.Throws<ProjectBuildFailed>(() => plcProject.Build(verbose:true));
        }

        //// Only uncomment this if you want to test the real build process. 
        //// Takes 15 s to complete.
        //[Fact]
        //public void BuildRealBrokenProjectShouldRaiseException()
        //{
        //    var plcProject = new TcProjectBuilder(
        //        "../../../../BrokenProjectForUnitTests/PLC2/PLC2.plcproj"
        //    );
        //    Assert.Throws<ProjectBuildFailed>(() => plcProject.Build(verbose: true));
        //}

        [Theory]
        [InlineData("PLC.plcproj")]
        [InlineData("Non/Existing/Path/PLC.plcproj")]
        public void TryGetHashOfNonExistingProject(string projectPath)
        {
            Assert.Throws<FileNotFoundException>(
                ()=> new TcProjectBuilder(projectPath)
            );
        }

        const string testDataPath = "../../../TcProjectBuildTestData";
        [Theory]
        [InlineData("../../../TcProjectBuildTestData/succesfulBuild.log", false)]
        [InlineData("../../../TcProjectBuildTestData/failedBuildWithExtraTextBelow.log", true)]
        [InlineData("../../../TcProjectBuildTestData/firstBuildOkSecondBuildFailed.log", true)]
        public void CheckIfBuildFailedFromLogFile(string logFilePath, bool buildFailed)
        {
            TcProjectBuilder tcProject = new TcProjectBuilder(
                "../../../../WorkingProjectForUnitTests/PLC/PLC.plcproj"
            );
            string logFileContent = File.ReadAllText(logFilePath);
            bool actual = tcProject.BuildFailed(logFileContent);

            Assert.Equal(buildFailed, actual);
        }

        [Theory]
        [InlineData("../../../../WorkingProjectForUnitTests/PLC/POUs/Sum.TcPOU")]
        [InlineData("../../../../WorkingProjectForUnitTests/PLC/POUs/MAIN.TcPOU")]
        public void GetProjectHashFromSingleTcPouFilename(string filename)
        {
            var plcProject = new TcProjectBuilder(filename);

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
            string tempPlcProjFile = "../../../../../UnitTest.plcproj";
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
    }
}
