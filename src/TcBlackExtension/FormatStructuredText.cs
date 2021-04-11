using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;
using TcBlackCore;
using TCatSysManagerLib;

namespace TcBlackExtension2
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class FormatStructuredText
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = 
            new Guid("2331eac3-39e5-4347-b678-a146d49c0a07");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatStructuredText"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private FormatStructuredText(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? 
                throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static FormatStructuredText Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in FormatStructuredText's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory
                .SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(
                (typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new FormatStructuredText(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte.ActiveWindow.ProjectItem.Object is ITcPlcDeclaration)
            {
                ITcPlcDeclaration declaration =
                  (ITcPlcDeclaration)dte.ActiveWindow.ProjectItem.Object;

                int indents = 0;
                string text = declaration.DeclarationText;
                TcBlackCore.Globals.indentation = text.Contains("\t") ? "\t" : "    ";
                TcBlackCore.Globals.lineEnding = "\r\n";
                string formatedCode = new CompositeCode(text)
                    .Tokenize()
                    .Format(ref indents);
                declaration.DeclarationText = formatedCode;
            }
        }
    }
}
