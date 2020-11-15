using System.Collections.Generic;
using CommandLine;

namespace TcBlackCore
{
    /// <summary>
    /// Options for command line interface.
    /// </summary>
    public class Options
    {
        [Option(
            'f',
            "file",
            HelpText = "TcPOU/TcIO file(s) to format.",
            SetName = "files"
        )]
        public IEnumerable<string> File { get; set; }
        [Option(
            'p',
            "project",
            Default = "",
            HelpText = "Plc project to format.",
            SetName = "files"
        )]
        public string Project { get; set; }

        [Option(
            Default = false,
            HelpText =
                "Compiles project before and after formatting, in order to check "
                + "if the code has changed. WARNING: Takes > 30 seconds!"
        )]
        public bool Safe { get; set; }

        [Option(
            Default = false,
            HelpText =
                "Shows the commands which are used to build the project. "
                + "Currently only has an effect when --safe option is used."
        )]
        public bool Verbose { get; set; }

        [Option(
            Default = "",
            HelpText = "Override the indentation found in the file(s)."
        )]
        public string Indentation { get; set; }

        [Option(
            HelpText = "Overrides the line ending of all files with Windows' \\r\\n"
        )]
        public bool WindowsLineEnding { get; set; }
        [Option(
            HelpText = "Overrides the line ending of all files with UNIX' \\n."
        )]
        public bool UnixLineEnding { get; set; }
    }
}
