﻿using System;
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

        [Fact]
        public void BuildMockBrokenProjectShouldRaiseException()
        {
            string brokenProjectPath = Path.Combine(
                projectDirectory, "BrokenProjectForUnitTests", "PLC2", "PLC2.plcproj"
            );
            string failedBuildLogPath = Path.Combine(
                testDirectory, 
                "TcProjectBuildTestData", 
                "failedBuildWithExtraTextBelow.log"
            );
            var plcProject = new MockTcProjectBuilder(
                brokenProjectPath, failedBuildLogPath
            );
            Assert.Throws<ProjectBuildFailed>(() => plcProject.Build());
        }

        //// Only uncomment this if you want to test the real build process. 
        //// Takes ~30 s to complete.
        //[Fact]
        //public void BuildRealBrokenProjectShouldRaiseException()
        //{
        //    string brokenPlcProjectPath = Path.Combine(
        //        projectDirectory, "BrokenProjectForUnitTests", "PLC2", "PLC2.plcproj"
        //    );
        //    var plcProject = new TcProjectBuilder(brokenPlcProjectPath);
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

        private static readonly string testDataDirectory = Path.Combine(
            testDirectory, "TcProjectBuildTestData"
        );
        private static readonly string workingPlcProjectPath = Path.Combine(
            projectDirectory, "WorkingProjectForUnitTests", "PLC", "PLC.plcproj"
        );
        [Theory]
        [InlineData("succesfulBuild.log", false)]
        [InlineData("failedBuildWithExtraTextBelow.log", true)]
        [InlineData("firstBuildOkSecondBuildFailed.log", true)]
        public void CheckIfBuildFailedFromLogFile(string logFile, bool buildFailed)
        {
            TcProjectBuilder tcProject = new TcProjectBuilder(workingPlcProjectPath);
            string logFileContent = File.ReadAllText(
                Path.Combine(testDataDirectory, logFile)
            );
            bool actual = tcProject.BuildFailed(logFileContent);

            Assert.Equal(buildFailed, actual);
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
