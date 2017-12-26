using System;
using System.Linq;
using System.Net;

namespace libnetworkutility
{
    public static class IPAddressExtension
    {
        public static uint ToUInt32(this IPAddress address)
        {
            if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                throw new ArgumentException("Attempting to execute ToUInt32() on non-IPv4 address");

            var uints = address.GetAddressBytes().Select(x => Convert.ToUInt32(x)).ToList();
            return (
                    (uints[0] << 24) |
                    (uints[1] << 16) |
                    (uints[2] << 8) |
                    uints[3]
                );
        }

        public static IPAddress ToIPAddress(this uint value)
        {
            var bytes = new byte[]
            {
                Convert.ToByte(value >> 24),
                Convert.ToByte((value >> 16) & 0xFF),
                Convert.ToByte((value >> 8) & 0xFF),
                Convert.ToByte(value & 0xFF)
            };

            return new IPAddress(bytes);
        }

        public static string ToHexString(this IPAddress address)
        {
            return address.ToUInt32().ToString("X8");
        }

        public static bool LessThan(this IPAddress me, IPAddress them)
        {
            var meInt = me.ToUInt32();
            var themInt = them.ToUInt32();
            return meInt < themInt;
        }

        public static bool LessThanOrEqual(this IPAddress me, IPAddress them)
        {
            var meInt = me.ToUInt32();
            var themInt = them.ToUInt32();
            return meInt <= themInt;
        }

        public static bool GreaterThan(this IPAddress me, IPAddress them)
        {
            var meInt = me.ToUInt32();
            var themInt = them.ToUInt32();
            return meInt > themInt;
        }

        public static bool GreaterThanOrEqual(this IPAddress me, IPAddress them)
        {
            var meInt = me.ToUInt32();
            var themInt = them.ToUInt32();
            return meInt >= themInt;
        }
        public static IPAddress Offset(this IPAddress me, int offset)
        {
            var meInt = me.ToUInt32();
            if (offset >= 0)
                meInt += Convert.ToUInt32(offset);
            else
                meInt -= Convert.ToUInt32(Math.Abs(offset));
            return meInt.ToIPAddress();
        }
    }
}