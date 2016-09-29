using System;
using Logger;
using System.Linq;
using System.Net.NetworkInformation;

namespace Keygen
{
    [LoggerPostSharpAspect]
    public class Keygen : IKeygen
    {
        public string GenerateKey(DateTime date)
        {
            var byteArray = BitConverter.GetBytes(date.Date.ToBinary());
            var interface2 = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
            if (interface2 == null) return string.Empty;

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

            return string.Join("-", key);
        }
    }
}
