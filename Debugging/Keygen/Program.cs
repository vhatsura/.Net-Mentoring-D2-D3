using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace Keygen
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var byteArray = BitConverter.GetBytes(DateTime.Now.Date.ToBinary());

            var interface2 = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
            if (interface2 == null) return;

            var key = interface2
                .GetPhysicalAddress()
                .GetAddressBytes()
                .Select((@byte, @int) => @byte ^ byteArray[@int])
                .Select(@int =>
                {
                    if (@int > 999)
                    {
                        return @int;
                    }
                    return @int * 10;
                })
                .ToArray();

            Console.WriteLine($"Key: {string.Join("-", key)}");
        }
    }
}
