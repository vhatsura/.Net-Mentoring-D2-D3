using System;
using System.Runtime.InteropServices;

namespace InteroperatingWithUnmanagedCode
{
    public sealed class PowerStateManager
    {
        public DateTime GetLastSleepTime()
        {
            return GetLastTime(InformationLevel.LastSleepTime);
        }

        public DateTime GetLastWakeTime()
        {
            return GetLastTime(InformationLevel.LastWakeTime);
        }

        public SystemBatteryState GetSystemBatteryState()
        {
            throw new NotImplementedException();
        }

        public SystemPowerInformation GetSystemPowerInformation()
        {
            throw new NotImplementedException();
        }

        public void ReserveHibernationFile()
        {
            throw new NotImplementedException();
        }

        public void RemoveHibernationFile()
        {
            throw new NotImplementedException();
        }

        public void SuspendSystem(bool hibernate)
        {
            throw new NotImplementedException();
        }

        private DateTime GetLastTime(InformationLevel level)
        {
            var outputBufferSize = Marshal.SizeOf<ulong>();
            var outputBuffer = Marshal.AllocHGlobal(outputBufferSize);

            PowerStateManagement.CallNtPowerInformation((int) level, IntPtr.Zero, 0, outputBuffer, (uint) outputBufferSize);

            var ticks = Marshal.ReadInt64(outputBuffer);
            Marshal.FreeHGlobal(outputBuffer);

            var startupTime = PowerStateManagement.GetTickCount64() * 10000;

            var date = DateTime.UtcNow - TimeSpan.FromTicks((long) startupTime) + TimeSpan.FromTicks(ticks);

            return date;
        }

        private class PowerStateManagement
        {
            /// <summary>
            /// Retrieves the number of milliseconds that have elapsed since the system was started.
            /// </summary>
            /// <returns>The number of milliseconds.</returns>
            [DllImport("kernel32")]
            public static extern ulong GetTickCount64();

            /// <summary>
            /// Sets or retrieves power information.
            /// </summary>
            /// <param name="informationLevel">The information level requested.</param>
            /// <param name="inputBuffer">A pointer to an optional input buffer. The data type of this buffer depends on
            ///  the information level requested in the InformationLevel parameter.</param>
            /// <param name="inputBufferSize">The size of the input buffer, in bytes.</param>
            /// <param name="outputBuffer">A pointer to an optional output buffer.</param>
            /// <param name="outputBufferSize">The size of the output buffer, in bytes. Depending on the information level
            ///  requested, this may be a variably sized buffer.</param>
            /// <returns></returns>
            [DllImport("powrprof.dll")]
            public static extern uint CallNtPowerInformation(
                int informationLevel,
                IntPtr inputBuffer,
                uint inputBufferSize,
                [Out] IntPtr outputBuffer,
                uint outputBufferSize);

            [DllImport("powrprof.dll")]
            public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
        }
    }
}
