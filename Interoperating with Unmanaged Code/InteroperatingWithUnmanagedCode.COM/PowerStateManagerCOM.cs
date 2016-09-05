using System;
using System.Runtime.InteropServices;

namespace InteroperatingWithUnmanagedCode.COM
{
    [ComVisible(true)]
    [Guid("E54F8365-D5EB-4C2B-A3FE-7C2B7EDF902A")]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class PowerStateManagerCOM : IPowerStateManagerCOM
    {
        private readonly PowerStateManager m_PowerManager = new PowerStateManager();

        public DateTime GetLastSleepTime() => m_PowerManager.GetLastSleepTime();
        public DateTime GetLastWakeTime() => m_PowerManager.GetLastWakeTime();
    }
}
