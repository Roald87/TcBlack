using System;
using System.Runtime.InteropServices;

namespace TcBlack
{
    enum ServiceCall
    {
        TooBusyCancelAll = -1,
        IsHandled = 0,
        PendingMsg_WaitDefProcess = 2,
        RetryLater = 99,
    }

    /// <summary>
    /// Prevents threading contention issues between external multi-threaded 
    /// applications and Visual Studio.
    /// </summary>
    /// <see cref="https://docs.microsoft.com/en-us/previous-versions/ms228772(v=vs.140)"/>
    public class MessageFilter : IOleMessageFilter
    {
        /// <summary>
        /// Start the filter.
        /// </summary>
        public static void Register()
        {
            IOleMessageFilter newFilter = new MessageFilter();
            CoRegisterMessageFilter(newFilter, out IOleMessageFilter oldFilter);
        }

        /// <summary>
        /// Done with the filter, close it. 
        /// </summary>
        public static void Revoke()
        {
            CoRegisterMessageFilter(null, out IOleMessageFilter oldFilter);
        }

        /// <summary>
        /// Handle incoming thread requests.
        /// </summary>
        /// <param name="dwCallType"></param>
        /// <param name="hTaskCaller"></param>
        /// <param name="dwTickCount"></param>
        /// <param name="lpInterfaceInfo"></param>
        /// <returns></returns>
        int IOleMessageFilter.HandleInComingCall(
            int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo
        )
        {
            return (int)ServiceCall.IsHandled;
        }

        /// <summary>
        /// Thread call was rejected, so try again.
        /// </summary>
        /// <param name="hTaskCallee"></param>
        /// <param name="dwTickCount"></param>
        /// <param name="dwRejectType"></param>
        /// <returns></returns>
        int IOleMessageFilter.RetryRejectedCall(
            IntPtr hTaskCallee, int dwTickCount, int dwRejectType
        )
        {
            if (dwRejectType == 2)
            {
                // Retry the thread call immediately if return >=0 & <100.
                return (int)ServiceCall.RetryLater;
            }
            return (int)ServiceCall.TooBusyCancelAll;
        }

        int IOleMessageFilter.MessagePending(
            IntPtr hTaskCallee, int dwTickCount, int dwPendingType
        )
        {
            return (int)ServiceCall.PendingMsg_WaitDefProcess;
        }

        /// <summary>
        /// Implement the IOleMessageFilter interface.
        /// </summary>
        /// <param name="newFilter"></param>
        /// <param name="oldFilter"></param>
        /// <returns></returns>
        [DllImport("Ole32.dll")]
        private static extern int CoRegisterMessageFilter(
            IOleMessageFilter newFilter, out IOleMessageFilter oldFilter
        );
    }

    [
        ComImport(), 
        Guid("00000016-0000-0000-C000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
    ]
    interface IOleMessageFilter
    {
        [PreserveSig]
        int HandleInComingCall(
            int dwCallType,
            IntPtr hTaskCaller,
            int dwTickCount,
            IntPtr lpInterfaceInfo
        );

        [PreserveSig]
        int RetryRejectedCall(
            IntPtr hTaskCallee,
            int dwTickCount,
            int dwRejectType
        );

        [PreserveSig]
        int MessagePending(
            IntPtr hTaskCallee,
            int dwTickCount,
            int dwPendingType
        );
    }
}
