using System.Runtime.InteropServices;

namespace InteroperatingWithUnmanagedCode
{
    [StructLayout(LayoutKind.Sequential)]
    [ComVisible(true)]
    public struct SystemBatteryState
    {
        public bool AcOnLine;
        public bool BatteryPresent;
        public bool Charging;
        public bool Discharging;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public bool[] Spare1;
        public int MaxCapacity;
        public int RemainingCapacity;
        public int Rate;
        public int EstimatedTime;
        public int DefaultAlert1;
        public int DefaultAlert2;

        public override string ToString() => $"AcOnLine: {AcOnLine}, BatteryPresent: {BatteryPresent}, " +
                                             $"Charging: {Charging}, Discharging: {Discharging}, " +
                                             $"Spare1 {string.Join("|", Spare1)}, " +
                                             $"MaxCapacity: {MaxCapacity}, RemainingCapacity: {RemainingCapacity}, " +
                                             $"Rate: {Rate}, EstimatedTime: {EstimatedTime}, " +
                                             $"DefaultAlert1: {DefaultAlert1}, DefaultAlert2: {DefaultAlert2}";
    }
}
