using System;
using System.Runtime.InteropServices;

namespace InteroperatingWithUnmanagedCode
{
    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(true)]
    public struct SystemPowerInformation
    {
        public uint MaxIdlenessAllowed;
        public uint Idleness;
        public uint TimeRemaining;
        public sbyte CoolingMode;

        public override string ToString() => $"MaxIdlenessAllowed: {MaxIdlenessAllowed}, Idleness: {Idleness}, " +
                                             $"TimeRemaining: {TimeSpan.FromTicks(TimeRemaining)}, CoolingMode: {CoolingMode}";
    }
}
