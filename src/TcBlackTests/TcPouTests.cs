using System;
using System.IO;
using TcBlack;
using Xunit;

namespace TcBlackTests
{
    public class TcPouTests
    {
        const string _tcProjectPath = "../../../../WorkingProjectForUnitTests/PLC/POUs/";

        [Theory]
        [InlineData(_tcProjectPath + "Sum.TcPOU")]
        public void ReadDeclaration(string path)
        {
            TcPou tcPou = new TcPou(path);

            string expected = new string(
                    "FUNCTION Sum : LREAL\r\n"
                    + "VAR_INPUT\r\n"
                    + "    var1 : LREAL;\r\n"
                    + "    var2 : LREAL;\r\n"
                    + "END_VAR\r\n"
                    + "VAR\r\n    total : LREAL;\r\nEND_VAR"
            );
            Assert.Equal(expected, tcPou.Declaration);
        }

        [Theory]
        [InlineData("FB_InputSimple.TcPOU", "FB_ExpectedSimple.TcPOU")]
        [InlineData("FB_InputComplex.TcPOU", "FB_ExpectedComplex.TcPOU")]
        [InlineData(
            "FB_InputTabAndUnixLineEnd.TcPOU", "FB_ExpectedTabAndUnixLineEnd.TcPOU"
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