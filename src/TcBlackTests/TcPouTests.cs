using System;
using System.IO;
using TcBlack;
using Xunit;

namespace TcBlackTests
{
    public class TcPouTests
    {
        [Theory]
        [InlineData("FB_InputSimple.TcPOU", "FB_ExpectedSimple.TcPOU")]
        [InlineData("FB_InputComplex.TcPOU", "FB_ExpectedComplex.TcPOU")]
        [InlineData(
            "FB_InputWithPropertiesAndMethods.TcPOU",
            "FB_ExpectedWithPropertiesAndMethods.TcPOU"
        )]
        public void LoadChangeAndSaveDeclaration(string fbInput, string fbExpected)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string testDataDirectory = Path.Combine(
                Directory.GetParent(workingDirectory).Parent.Parent.FullName, 
                "TcPouTestData"
            );
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
            // git keeps changing the line endings. In order to make sure it uses the 
            // correct line ending, I'll change them manually here.
            string workingDirectory = Environment.CurrentDirectory;
            string testDataDirectory = Path.Combine(
                Directory.GetParent(workingDirectory).Parent.Parent.FullName,
                "TcPouTestData"
            );
            string fileToFormat = Path.Combine(
                testDataDirectory, "FB_InputTabAndUnixLineEnd.TcPOU"
            );
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
            Assert.DoesNotContain("\r\n", expected); 
            Assert.DoesNotContain("\r\n", actual);
            Assert.Equal(expected, actual);
        }

        private void ReplaceWindowsLineEndingForUnixOnes(string filename)
        {
            string fileContent = File.ReadAllText(filename).Replace("\r\n", "\n");
            File.WriteAllText(filename, fileContent);
        }
    }
}