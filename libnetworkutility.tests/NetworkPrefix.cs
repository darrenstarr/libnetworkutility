using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace libnetworkutility.tests
{
    public class NetworkPrefix
    {
        [Fact]
        public void BaseNetwork()
        {
            var x = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("192.168.255.255"),
                Length = 21
            };

            var baseNetwork = x.BaseNetwork;
            Assert.Equal(IPAddress.Parse("192.168.248.0"), baseNetwork.Network);
            Assert.Equal(21, baseNetwork.Length);
        }

        [Fact]
        public void PrefixEquals()
        {
            var x = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("10.0.0.0"),
                Length = 16
            };

            var good = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("10.0.0.0"),
                Length = 16
            };

            Assert.True(good.Equals(x));

            var bad1 = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("192.168.0.0"),
                Length = 16
            };

            Assert.False(bad1.Equals(x));

            var bad2 = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("10.0.0.0"),
                Length = 17
            };

            Assert.False(bad2.Equals(x));

            Assert.False(x.Equals(null));
        }

        [Fact]
        public void PrefixAsString()
        {
            var x = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("10.11.12.255"),
                Length = 16
            };

            Assert.Equal("10.11.12.255/16", x.ToString());
        }

        [Fact]
        public void PrefixGetHashCode()
        {
            var x = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("10.11.12.255"),
                Length = 16
            };

            var h = x.Network.GetHashCode() ^ x.Length.GetHashCode();
            Assert.Equal(h, x.GetHashCode());
        }

        [Fact]
        public void ContainsIPv6()
        {
            var badAddress = IPAddress.Parse("FE80::1");
            var x = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("10.11.12.255"),
                Length = 16
            };
            Assert.False(x.Contains(badAddress));
        }

        [Fact]
        public void ContainsAddress()
        {
            var x = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("10.11.12.255"),
                Length = 16
            };

            Assert.True(x.Contains(IPAddress.Parse("10.11.1.1")));
            Assert.True(x.Contains(IPAddress.Parse("10.11.12.254")));
            Assert.True(x.Contains(IPAddress.Parse("10.11.0.1")));

            Assert.False(x.Contains(IPAddress.Parse("10.10.0.1")));
            Assert.False(x.Contains(IPAddress.Parse("192.168.1.1")));

            var y = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("FE80::1"),
                Length = 33
            };

            Assert.False(y.Contains(IPAddress.Parse("10.1.1.1")));
        }

        [Fact]
        public void PrefixAsRange()
        {
            var x = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("10.11.12.255"),
                Length = 16
            };

            var good = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.11.0.0"),
                End = IPAddress.Parse("10.11.255.255")
            };

            Assert.Equal(good, x.AsRange());

            var y = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("FE80::1"),
                Length = 33
            };

            Assert.Null(y.AsRange());
        }

        [Fact]
        public void Parse()
        {
            var good = new libnetworkutility.NetworkPrefix
            {
                Network = IPAddress.Parse("10.11.12.128"),
                Length = 25
            };

            Assert.Equal(good, libnetworkutility.NetworkPrefix.Parse("10.11.12.128/25"));

            Assert.Null(libnetworkutility.NetworkPrefix.Parse("10.256.12.128/25"));
            Assert.Null(libnetworkutility.NetworkPrefix.Parse("2001:1:FEED::6/25"));

            // TODO : Try to cause IPAddress.Parse() to fail 
        }

        [Fact]
        public void SubnetMask()
        {
            Assert.Equal(IPAddress.Parse("255.255.255.0"), libnetworkutility.NetworkPrefix.Parse("10.100.1.0/24").SubnetMask);
            Assert.Equal(IPAddress.Parse("255.255.255.128"), libnetworkutility.NetworkPrefix.Parse("10.100.1.0/25").SubnetMask);
            Assert.Equal(IPAddress.Parse("255.0.0.0"), libnetworkutility.NetworkPrefix.Parse("10.100.1.0/8").SubnetMask);
        }

        [Fact]
        public void TotalAddresses()
        {
            Assert.Equal(Math.Pow(2, 24), libnetworkutility.NetworkPrefix.Parse("10.100.1.0/8").TotalAddresses);
            Assert.Equal(Math.Pow(2, 8), libnetworkutility.NetworkPrefix.Parse("10.100.1.0/24").TotalAddresses);
            Assert.Equal(Math.Pow(2, 15), libnetworkutility.NetworkPrefix.Parse("10.100.1.0/17").TotalAddresses);
        }
    }
}
