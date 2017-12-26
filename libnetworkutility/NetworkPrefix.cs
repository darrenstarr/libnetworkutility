using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace libnetworkutility
{
    public class NetworkPrefix
    {
        public IPAddress Network { get; set; }
        public int Length { get; set; }
        public int TotalAddresses { get { return (1 << (32 - Length)); } }

        public NetworkPrefix BaseNetwork
        {
            get
            {
                return new NetworkPrefix
                {
                    Network = AsRange().Start,
                    Length = Length
                };
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as NetworkPrefix;
            if (obj == null)
                return false;

            return Network.Equals(other.Network) && Length == other.Length;
        }
        public override string ToString()
        {
            return Network.ToString() + "/" + Length.ToString();
        }
        public override int GetHashCode()
        {
            return Network.GetHashCode() ^ Length.GetHashCode();
        }
        public bool Contains(IPAddress address)
        {
            if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                // TODO : Support IPv6 at some point if interesting
                System.Diagnostics.Debug.WriteLine("Address families other than IPv4 not supported at this time");
                return false;
            }

            try
            {
                var prefixAsUInt32 = Network.ToUInt32();
                var addressAsUInt32 = address.ToUInt32();

                return (prefixAsUInt32 >> (32 - Length)) == (addressAsUInt32 >> (32 - Length));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }
        public IPRange AsRange()
        {
            try
            {
                var prefixAsUInt32 = Network.ToUInt32();
                var mask = 0xFFFFFFFF << (32 - Length);
                return new IPRange
                {
                    Start = (prefixAsUInt32 & mask).ToIPAddress(),
                    End = ((prefixAsUInt32 & mask) | (~mask)).ToIPAddress()
                };
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        static Regex ipv4PrefixExpression = new Regex(@"^\s*(?<ip>(((1?[0-9]{1,2})|(2([0-4][0-9])|(5[0-5])))\.){3}((1?[0-9]{1,2})|(2([0-4][0-9])|(5[0-5]))))\/(?<length>(3[0-2])|([0-2]?[0-9]))\s*$", RegexOptions.Compiled);
        public static NetworkPrefix Parse(string text)
        {
            var m = ipv4PrefixExpression.Match(text);
            if (!m.Success)
                return null;

            IPAddress address;
            if (!IPAddress.TryParse(m.Groups["ip"].Value, out address))
                return null;

            int length = Convert.ToInt32(m.Groups["length"].Value);

            return new NetworkPrefix
            {
                Network = address,
                Length = length
            };
        }
        public IPAddress SubnetMask
        {
            get
            {
                var value = 0xFFFFFFFF << (32 - Length);
                return new IPAddress(new byte[] {
                    Convert.ToByte(value >> 24),
                    Convert.ToByte((value >> 16) & 0xFF),
                    Convert.ToByte((value >> 8) & 0xFF),
                    Convert.ToByte(value & 0xFF)
                });
            }
        }
    }
}
