using TCatSysManagerLib;

namespace TcBlack
{
    /// <summary>
    /// This class provides the functionality to access the TwinCAT automation 
    /// interface, which is a complement to the VS DTE and that gives access to certain 
    /// TwinCAT specific functions integrated into Visual Studio
    /// </summary>
    /// <remarks>Source: https://github.com/tcunit/TcUnit </remarks>
    class AutomationInterface
    {
        private ITcConfigManager configManager = null;

        public AutomationInterface(EnvDTE.Project project)
        {
            ITcSysManager = (ITcSysManager10)project.Object;
            configManager = ITcSysManager.ConfigurationManager;
            PlcTreeItem = ITcSysManager.LookupTreeItem(
                Constants.PLC_CONFIGURATION_SHORTCUT
            );
            RoutesTreeItem = ITcSysManager.LookupTreeItem(
                Constants.RT_CONFIG_ROUTE_SETTINGS_SHORTCUT
            );
        }

        public AutomationInterface(VisualStudioInstance vsInst) : this(vsInst.Project)
        { }

        public ITcSysManager10 ITcSysManager { get; } = null;

        public ITcSmTreeItem PlcTreeItem { get; } = null;

        public ITcSmTreeItem RoutesTreeItem { get; } = null;

        public string ActiveTargetPlatform
        {
            get => configManager.ActiveTargetPlatform;
            set => configManager.ActiveTargetPlatform = value;
        }

        public string TargetNetId
        {
            get => ITcSysManager.GetTargetNetId();
            set => ITcSysManager.SetTargetNetId(value);
        }
    }
}
