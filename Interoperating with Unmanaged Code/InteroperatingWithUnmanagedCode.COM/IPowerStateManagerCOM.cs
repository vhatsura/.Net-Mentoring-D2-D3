using System;
using System.Runtime.InteropServices;

namespace InteroperatingWithUnmanagedCode.COM
{
    [ComVisible(true)]
    [Guid("83B34DC8-8975-427C-B0FD-0A139B7EBE90")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IPowerStateManagerCOM
    {
        DateTime GetLastSleepTime();

        DateTime GetLastWakeTime();
    }
}