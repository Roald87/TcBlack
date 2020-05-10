using TcBlack;
using Xunit;

namespace TcBlackTests
{
    public class TcPouReaderTests
    {
        const string _tcProjectPath = "../../../../TwinCATBlack/PLC/";

        [Theory]
        [InlineData(_tcProjectPath + "Sum.TcPOU")]
        public void GetBasicTcPou(string path)
        {
            TcPouReader reader = new TcPouReader(path);
            TcPou actual = reader.Read();

            TcPou expected = new TcPou(
                declaration: 
                    "FUNCTION Sum : LREAL\r\n"
                    + "VAR_INPUT\r\n"
                    + "    var1 : LREAL;\r\n"
                    + "    var2 : LREAL;\r\n"
                    + "END_VAR\r\n"
                    + "VAR\r\n    total : LREAL;\r\nEND_VAR"
            );
            AssertEquals(expected, actual);
        }

        private void AssertEquals(TcPou expected, TcPou actual)
        {
            Assert.Equal(expected.Declaration, actual.Declaration);
        }
    }
}