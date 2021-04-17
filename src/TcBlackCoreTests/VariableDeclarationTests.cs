using TcBlackCore;
using Xunit;

namespace TcBlackTests
{
    [Collection("Sequential")]
    public class VariableDeclarationTests
    {
        [Theory]
        [InlineData("variable1", "BOOL")]
        [InlineData("variable_1 ", "BOOL")]
        [InlineData("String1 ", " STRING")]
        [InlineData("var ", " LREAL ")]
        [InlineData(" number ", "    DINT")]
        [InlineData("   _internalVar4", "DWORD ")]
        [InlineData("   SHOUTING ", " INT ")]
        [InlineData("aSample1 ", "ARRAY[1..5] OF INT")]
        [InlineData("aSample1 ", "ARRAY[1..nInt - 1] OF INT")]
        [InlineData("aSample1 ", "ARRAY[nInt + 1..3] OF INT")]
        [InlineData("trigger ", "ARRAY[1..3] OF Tc2_Standard.R_TRIG")]
        [InlineData("pSample ", "POINTER TO INT")]
        [InlineData("refInt ", "REFERENCE TO INT")]
        [InlineData("aSample ", "ARRAY[*] OF INT")]
        [InlineData("typeclass ", "__SYSTEM.TYPE_CLASS")]
        [InlineData("fbSample ", "FB_Sample(nId_Init := 11, fIn_Init := 33.44)")]
        [InlineData(
            "afbSample2  ", 
            "ARRAY[0..1, 0..1] OF FB_Sample[(nId_Init:= 100, fIn_Init:= 123.456)]"
        )]
        [InlineData(
            "afbSample1",
            "ARRAY[0..1, 0..1] OF FB_Sample[" 
                + "(nId_Init:= 12, fIn_Init:= 11.22),"
                + "(nId_Init:= 13, fIn_Init:= 22.33)," 
                + "(nId_Init:= 14, fIn_Init:= 33.44)," 
                + "(nId_Init:= 15, fIn_Init:= 44.55)]"
        )]
        public void VarAndTypeVariousWhitespaceArrangements(
            string variable, string type
        )
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            VariableDeclaration varDecl = new VariableDeclaration(
                $"{variable}:{type};"
            );
            TcDeclaration expectedDecl = new TcDeclaration(
                variable.Trim(), 
                "", 
                VariableDeclaration.RemoveWhiteSpaceIfPossible(type), 
                "", 
                ""
            );
            AssertEquals(expectedDecl, varDecl.Tokenize());
        }

        [Theory]
        [InlineData("voltage ", "%I*", "INT")]
        [InlineData("Curr_2 ", "%I*", "BYTE")]
        [InlineData("   _nonSense  ", "%I* ", " BOOL  ")]
        [InlineData("  Bl12K  ", "      %I* ", " BOOL  ")]
        [InlineData("Uint", " %I* ", " UINT  ")]
        [InlineData(" RFRW_3 ", "  %I*", "REAL")]
        [InlineData(" __Asdf ", "  %I* ", " STRING  ")]
        [InlineData("voltage ", "%Q*", "UDINT")]
        [InlineData("voltage", " %Q* ", "T_MaxString")]
        [InlineData("_V", " %Q*    ", "    DWORD ")]
        [InlineData("I ", " %Q* ", "BOOL   ")]
        [InlineData("fResistance     ", "   %Q* ", "LREAL")]
        [InlineData("fResistance  ", "   %Q* ", "    UINT ")]
        [InlineData("bCurrent_3    ", " %M*     ", "   LREAL   ")]
        [InlineData("fInt", "%QX7.5", "    INT ")]
        [InlineData("sString", " %IW215     ", "  STRING")]
        [InlineData("_bBool  ", " %QB7 ", " BOOL  ")]
        [InlineData("bCurrent_3", " %MD48 ", "LREAL ")]
        [InlineData("Current", "%IW2.5.7.1 ", "DINT")]
        public void VarAllocationAndTypeVariousWhitespaceArangements
        (
            string variable, string allocation, string type)
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            VariableDeclaration varDecl = new VariableDeclaration(
                $"{variable} AT {allocation}:{type};"
            );
            TcDeclaration expectedDecl = new TcDeclaration(
                variable.Trim(), allocation.Trim(), type.Trim(), "", ""
            );
            AssertEquals(expectedDecl, varDecl.Tokenize());
        }

        [Theory]
        [InlineData("voltage ", "AT %I*", "INT", "3")]
        [InlineData("weight ", "", "LREAL", "3.14159")]
        [InlineData("weight ", "", "LREAL", "1E-5")]
        [InlineData("pid_controller ", "", "ST_Struct  ", " (nVar1:=1, nVar2:=4)")]
        [InlineData("Light", "", "photons  ", "2.4 ")]
        [InlineData("SomeWords ", "", "T_MaxString ", " 'Black quartz watch my vow.'")]
        [InlineData(" Character ", "", " STRING(1)", "' '")]
        [InlineData(
            "aSample_3  ", "", "ARRAY[1..2, 2..3, 3..4] OF INT ", "[2(0),4(4),2,3]"
        )]
        [InlineData(
            "aSample4", " AT %Q*", " ARRAY[1..3] OF ST_STRUCT1 ", 
            "[(n1:= 1, n2:= 10, n3:= 4723),\n"
            + "(n1:= 2, n2:= 0, n3:= 299),\n" 
            + "(n1:= 14, n2:= 5, n3:= 112)]"
        )]
        [InlineData("wsWSTRING ", "", "WSTRING", "\"abc\"")]
        [InlineData(
            "dtDATEANDTIME ", "", "DATE_AND_TIME    ", "DT#2017-02-20-11:07:00  "
        )]
        [InlineData("   tdTIMEOFDAY ", "", "  TIME_OF_DAY ", "    TOD#11:07:00")]
        [InlineData("nDINT", "", "DINT", "-12345")]
        [InlineData(" nDWORD", "", "DWORD", "16#6789ABCD")]
        [InlineData("sVar  ", "", "STRING(35)", "'This is a String'")]
        [InlineData(
            "stPoly1  ", "", "ST_Polygonline", 
            "(aStartPoint:=[3,3],aPoint1:=[5,2], aPoint2:=[7,3],aPoint3:=[8,5],aPoint4:=[5,7],aEndPoint:=[3,5])"
        )]
        public void VarAllocationTypeAndInitializationVariousWhitespaceArangements(
            string variable, string allocation, string type, string initialization
        )
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            VariableDeclaration varDecl = new VariableDeclaration(
                $"{variable}{allocation}:{type}:={initialization};"
            );
            string _allocation = allocation.Replace("AT", "");
            TcDeclaration expectedDecl = new TcDeclaration(
                variable.Trim(), 
                _allocation.Trim(),
                VariableDeclaration.RemoveWhiteSpaceIfPossible(type),
                VariableDeclaration.RemoveWhiteSpaceIfPossible(initialization), ""
            );

            AssertEquals(expectedDecl, varDecl.Tokenize());
        }

        [Theory]
        [InlineData(
            "Boolean  ", "AT %Q*", "BOOL", "TRUE", "// Very important comment"
        )]
        [InlineData("weight  ", "", "LREAL", "3.124", "// Comment with numbers 123  ")]
        [InlineData("  weight ", "", "  LREAL", "3.124", "  // $p€(|^[ characters")]
        [InlineData(" Pressure ", "", "  LREAL", "0.04", "  (* Chamber 1 pressure *)")]
        [InlineData("Name ", "", "STRING   ", "", "  (* Multi \n line \n comment *) ")]
        public void AllDeclarationsVariousWhitespaceArangements(
            string variable, 
            string allocation, 
            string type, 
            string initialization, 
            string comment
        )
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            VariableDeclaration varDecl = new VariableDeclaration(
                $"{variable}{allocation}:{type}:={initialization};{comment}"
            );
            string _allocation = allocation.Replace("AT", "");
            TcDeclaration expectedDecl = new TcDeclaration(
                variable.Trim(),
                _allocation.Trim(),
                VariableDeclaration.RemoveWhiteSpaceIfPossible(type.Trim()),
                VariableDeclaration.RemoveWhiteSpaceIfPossible(initialization),
                comment.Trim()
            );

            AssertEquals(expectedDecl, varDecl.Tokenize());
        }

        private void AssertEquals(TcDeclaration expected, TcDeclaration actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Allocation, actual.Allocation);
            Assert.Equal(expected.DataType, actual.DataType);
            Assert.Equal(expected.Initialization, actual.Initialization);
            Assert.Equal(expected.Comment, actual.Comment);
        }

        [Theory]
        [InlineData("   variable1  : BOOL      ;", "variable1 : BOOL;")]
        [InlineData(
            "deviceDown      AT     %QX0.2  :        BOOL ; ", 
            "deviceDown AT %QX0.2 : BOOL;"
        )]
        [InlineData("devSpeed:TIME:=T#10ms;", "devSpeed : TIME := T#10MS;")]
        [InlineData(
            "fbSample   :   FB_Sample(nId_Init := 11, fIn_Init := 33.44)   ;",
            "fbSample : FB_Sample(nId_Init:=11, fIn_Init:=33.44);"
        )]
        [InlineData(
            "var1   :   REAL := 8   ; // Comment", "var1 : REAL := 8; // Comment"
        )]
        [InlineData(
            "character   :   STRING(1) :=  ' ' ; ", "character : STRING(1) := ' ';"
        )]
        [InlineData(
            "MSG : INT := 253; // Do not put a double space, after a comma.",
            "MSG : INT := 253; // Do not put a double space, after a comma."
        )]
        [InlineData(
            "SomeArray : ARRAY[1..(n * (end + 1) - 4) / initial] OF REAL;",
            "SomeArray : ARRAY[1..(n * (end + 1) - 4) / initial] OF REAL;"
        )]
        [InlineData(
            "SomeArray : ARRAY[1..(n *(end +  1) -  4)/initial] OF REAL;",
            "SomeArray : ARRAY[1..(n * (end + 1) - 4) / initial] OF REAL;"
        )]
        [InlineData(
            "SomeArray : ARRAY[-10..-5] OF INT;",
            "SomeArray : ARRAY[-10..-5] OF INT;"
        )]
        [InlineData(
            "SomeArray : ARRAY[- 10..(2*-14)/SYSTEM.Number] OF INT;",
            "SomeArray : ARRAY[-10..(2 * -14) / SYSTEM.Number] OF INT;"
        )]
        [InlineData(
            "fbInst : FB_WithName(Name:='Text with spaces', number := 4);",
            "fbInst : FB_WithName(Name:='Text with spaces', number:=4);"
        )]
        [InlineData(
            "fbInst : FB_WithName(  Name:= \"Another text with spaces\",  num:=3.14) ;",
            "fbInst : FB_WithName(Name:=\"Another text with spaces\", num:=3.14);"
        )]
        [InlineData(
            "fbInst : FB_Name( Name:= \"Text with ' quote\" , truth := FALSE);",
            "fbInst : FB_Name(Name:=\"Text with ' quote\", truth:=FALSE);"
        )]
        [InlineData(
            "fbInst : FB_Name(Name:='The other \" quote' );",
            "fbInst : FB_Name(Name:='The other \" quote');"
        )]
        public void FormatVariableDeclaration(string unformattedCode, string expected)
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            VariableDeclaration variable = new VariableDeclaration(
                unformattedCode:unformattedCode
            );
            int indents = 0;
            Assert.Equal(expected, variable.Format(ref indents));
        }

        [Theory]
        [InlineData(
            "var1   :   REAL := 8   ; // Comment",
            "    var1 : REAL := 8; // Comment",
            1
        )]
        [InlineData(
            "deviceDown      AT     %QX0.2  :        BOOL ; ",
            "                    deviceDown AT %QX0.2 : BOOL;",
            5
        )]
        public void FormatVariableDeclarationWithIndentation(
            string unformattedCode, string expected, int indents)
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            VariableDeclaration variable = new VariableDeclaration(
                unformattedCode: unformattedCode
            );
            Assert.Equal(expected, variable.Format(ref indents));
        }

        [Theory]
        [InlineData(
            "FB_Sample( nId_Init := 11, fIn_Init := 33.44 )",
            "FB_Sample(nId_Init:=11,fIn_Init:=33.44)"
        )]
        [InlineData(
            "(aStartPoint:=[3, 3] ,aPoint1:=[    5,2], aPoint2:=[7,3],   aPoint3:=[8,5],aPoint4 := [5,7],aEndPoint   := [3,5]\n)",
            "(aStartPoint:=[3,3],aPoint1:=[5,2],aPoint2:=[7,3],aPoint3:=[8,5],aPoint4:=[5,7],aEndPoint:=[3,5])"
        )]
        [InlineData(
            "ARRAY[1..2,    2..3, 3..4] OF UINT",
            "ARRAY[1..2,2..3,3..4] OF UINT"
        )]
        [InlineData(
            "[ (n1:= 1, n2 := 10, n3:= 4723   ),\n"
            + " (n1:= 2, n2 := 0, n3:= 299) ,\n"
            + "( n1:=14, n2:= 5,  n3:=112)];",
            "[(n1:=1,n2:=10,n3:=4723),(n1:=2,n2:=0,n3:=299),(n1:=14,n2:=5,n3:=112)];"
        )]
        public void RemoveWhitespace(string input, string expected)
        {
            string actual = VariableDeclaration.RemoveWhiteSpaceIfPossible(input);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(
            "var: STRING(255):='This is a test string with ;';",
            "var : STRING(255) := 'This is a test string with ;';"
        )]
        [InlineData(
            "var: STRING(255):='This is a test() string with ;';",
            "var : STRING(255) := 'This is a test() string with ;';"
        )]
        [InlineData(
            "var: STRING(255):='This) is a test) string with ;';",
            "var : STRING(255) := 'This) is a test) string with ;';"
        )]
        [InlineData(
            "var: STRING(255):='This( is a test) string with ;';",
            "var : STRING(255) := 'This( is a test) string with ;';"
        )]
        [InlineData(
            "var: STRING(255):='This( is double \" test) string with ;';",
            "var : STRING(255) := 'This( is double \" test) string with ;';"
        )]
        [InlineData(
            "var: STRING(255):='This( is a $'asdf$' test) string with ;';",
            "var : STRING(255) := 'This( is a $'asdf$' test) string with ;';"
        )]
        [InlineData(
            "var: WSTRING(255):=\"This( is a ( test) string with ;\";",
            "var : WSTRING(255) := \"This( is a ( test) string with ;\";"
        )]
        [InlineData(
            "var: STRING(255):=\"This( is two singles '' test) string with ;\";",
            "var : STRING(255) := \"This( is two singles '' test) string with ;\";"
        )]
        [InlineData(
            "var: STRING(255):=\"This( is a $\" test) string with ;\";",
            "var : STRING(255) := \"This( is a $\" test) string with ;\";"
        )]
        public void SpecialCharStringInitialization(string unformattedCode, string expected)
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            VariableDeclaration variable = new VariableDeclaration(
                unformattedCode: unformattedCode
            );
            int indents = 0;
            Assert.Equal(expected, variable.Format(ref indents));
        }

        [Theory]
        [InlineData(
            "devSpeed:TIME:=T#10ms;",
            "devSpeed : TIME := T#10MS;"
        )]
        [InlineData(
            "devSpeed:time:=T#2d5h6m1s10ms;",
            "devSpeed : TIME := T#2D5H6M1S10MS;"
        )]
        [InlineData(
            "MSG : int := 253;",
            "MSG : INT := 253;"
        )]
        [InlineData(
            "SomeArray : array[1..(n * (end + 1) - 4) / initial] OF real;",
            "SomeArray : ARRAY[1..(n * (end + 1) - 4) / initial] OF REAL;"
        )]
        [InlineData(
            "ptr : pointer  to  dword ; ",
            "ptr : POINTER TO DWORD;"
        )]
        [InlineData(
            "ptr : pointer  to pointer to dword ; ",
            "ptr : POINTER TO POINTER TO DWORD;"
        )]
        public void UpperCaseKeywords(string unformattedCode, string expected)
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            VariableDeclaration variable = new VariableDeclaration(
                unformattedCode: unformattedCode
            );
            int indents = 0;
            Assert.Equal(expected, variable.Format(ref indents));
        }
    }
}