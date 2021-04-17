using System;
using System.IO;
using TcBlackCLI;
using Xunit;

namespace TcBlackTests
{
    [Collection("Sequential")]
    public class TcPouTests
    {
        private static string workingDirectory = Environment.CurrentDirectory;
        private static string testDataDirectory = Path.Combine(
            Directory.GetParent(workingDirectory).Parent.FullName,
            "TcPouTestData"
        );

        [Theory]
        [InlineData("FB_InputSimple.TcPOU", "FB_ExpectedSimple.TcPOU")]
        [InlineData("FB_InputComplex.TcPOU", "FB_ExpectedComplex.TcPOU")]
        [InlineData(
            "FB_InputWithPropertiesAndMethods.TcPOU",
            "FB_ExpectedWithPropertiesAndMethods.TcPOU"
        )]
        [InlineData("FB_InputWithEmptyVars.TcPOU", "FB_ExpectedWithEmptyVars.TcPOU")]
        public void LoadChangeAndSaveDeclaration(string fbInput, string fbExpected)
        {
            string fileToFormat = Path.Combine(testDataDirectory, fbInput);
            Backup backup = new Backup(fileToFormat);

            new TcPou(fileToFormat).Format().Save();

            string expectedFile = Path.Combine(testDataDirectory, fbExpected);
            string expected = File.ReadAllText(expectedFile);
            string actual = File.ReadAllText(fileToFormat);
            backup.Restore().Delete();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FormatFileWithUnixTypeLineEnd()
        {
            string fileToFormat = Path.Combine(
                testDataDirectory, "FB_InputTabAndUnixLineEnd.TcPOU"
            );
            // git keeps changing the line endings. In order to make sure it uses the 
            // correct line ending, I'll change them manually here.
            ReplaceWindowsLineEndingForUnixOnes(fileToFormat);
            Backup backup = new Backup(fileToFormat);

            new TcPou(fileToFormat).Format().Save();

            string expectedFile = Path.Combine(
                testDataDirectory, "FB_ExpectedTabAndUnixLineEnd.TcPOU"
            );
            ReplaceWindowsLineEndingForUnixOnes(expectedFile);
            string expected = File.ReadAllText(expectedFile);
            string actual = File.ReadAllText(fileToFormat);
            backup.Restore().Delete();
            Assert.DoesNotContain("\r\n", expected, StringComparison.Ordinal); 
            Assert.DoesNotContain("\r\n", actual, StringComparison.Ordinal);
            Assert.Equal(expected, actual);
        }

        [Fact]
        private void OverrideIndentationOfFile()
        {
            string fileToFormat = Path.Combine(
                testDataDirectory, "FB_InputOverrideIndentation.TcPOU"
            );
            Backup backup = new Backup(fileToFormat);

            new TcPou(fileToFormat, indentation:"  ").Format().Save();

            string expectedFile = Path.Combine(
                testDataDirectory, "FB_ExpectedOverrideIndentation.TcPOU"
            );
            string expected = File.ReadAllText(expectedFile);
            string actual = File.ReadAllText(fileToFormat);
            backup.Restore().Delete();
            Assert.Equal(expected, actual);
        }

        [Fact]
        private void OverrideLineBreakOfFile()
        {
            string fileToFormat = Path.Combine(
                testDataDirectory, "FB_InputOverrideLineEnding.TcPOU"
            );
            Backup backup = new Backup(fileToFormat);

            new TcPou(fileToFormat, windowsLineEnding:false).Format().Save();

            string expectedFile = Path.Combine(
                testDataDirectory, "FB_ExpectedOverrideLineEnding.TcPOU"
            );
            ReplaceWindowsLineEndingForUnixOnes(expectedFile);
            string expected = File.ReadAllText(expectedFile);
            string actual = File.ReadAllText(fileToFormat);
            backup.Restore().Delete();
            Assert.Equal(expected, actual);
        }

        [Fact]
        private void OverrideLineBreakAndIndentationOfFile()
        {
            string fileToFormat = Path.Combine(
                testDataDirectory, "FB_InputOverrideLineEndingAndIndentation.TcPOU"
            );
            ReplaceWindowsLineEndingForUnixOnes(fileToFormat);
            Backup backup = new Backup(fileToFormat);

            new TcPou(fileToFormat, windowsLineEnding:true, indentation:"    ")
                .Format()
                .Save();

            string expectedFile = Path.Combine(
                testDataDirectory, "FB_ExpectedOverrideLineEndingAndIndentation.TcPOU"
            );
            string expected = File.ReadAllText(expectedFile);
            string actual = File.ReadAllText(fileToFormat);
            backup.Restore().Delete();
            Assert.Equal(expected, actual);
        }

        private void ReplaceWindowsLineEndingForUnixOnes(string filename)
        {
            string fileContent = File.ReadAllText(filename).Replace("\r\n", "\n");
            File.WriteAllText(filename, fileContent);
        }
    }
}