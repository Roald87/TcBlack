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
    }
}