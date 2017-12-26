using System;
using System.Net;
using Xunit;

namespace libnetworkutility.tests
{
    public class IPAddressExtension
    {
        [Fact]
        public void ThrowOnUInt32ForIPv6()
        {
            var x = IPAddress.Parse("FE80::1");
            Exception ex = Assert.Throws<ArgumentException>(() => x.ToUInt32());
            Assert.Equal("Attempting to execute ToUInt32() on non-IPv4 address", ex.Message);
        }

        [Fact]
        public void ConvertToUInt32ForIPv4()
        {
            var x = IPAddress.Parse("192.168.10.1");
            var uintValue = x.ToUInt32();
            Assert.Equal(0xC0A80A01, uintValue);
        }

        [Fact]
        public void FromUInt32ForIPv4()
        {
            var uintValue = (uint)0x0A0B0CFF;
            var x = uintValue.ToIPAddress();
            Assert.Equal(IPAddress.Parse("10.11.12.255"), x);
        }

        [Fact]
        public void HexString()
        {
            Assert.Equal("0A0B0CFF", IPAddress.Parse("10.11.12.255").ToHexString());
        }

        [Fact]
        public void LessThan()
        {
            var smaller = IPAddress.Parse("10.0.0.1");
            var larger = IPAddress.Parse("192.168.0.1");
            Assert.True(smaller.LessThan(larger));
            Assert.False(larger.LessThan(smaller));
            Assert.False(smaller.LessThan(smaller));
        }

        [Fact]
        public void LessThanOrEqual()
        {
            var smaller = IPAddress.Parse("10.0.0.1");
            var larger = IPAddress.Parse("192.168.0.1");
            Assert.True(smaller.LessThanOrEqual(larger));
            Assert.True(smaller.LessThanOrEqual(smaller));
            Assert.False(larger.LessThanOrEqual(smaller));
        }

        [Fact]
        public void GreaterThan()
        {
            var smaller = IPAddress.Parse("10.0.0.1");
            var larger = IPAddress.Parse("192.168.0.1");
            Assert.True(larger.GreaterThan(smaller));
            Assert.False(smaller.GreaterThan(larger));
            Assert.False(larger.GreaterThan(larger));
        }

        [Fact]
        public void GreaterThanOrEqual()
        {
            var smaller = IPAddress.Parse("10.0.0.1");
            var larger = IPAddress.Parse("192.168.0.1");
            Assert.True(larger.GreaterThanOrEqual(smaller));
            Assert.True(larger.GreaterThanOrEqual(larger));
            Assert.False(smaller.GreaterThanOrEqual(larger));
        }

        [Fact]
        public void Offset()
        {
            var original = IPAddress.Parse("10.0.0.1");
            var expectedUp = IPAddress.Parse("10.0.1.2");
            var expectedDown = IPAddress.Parse("9.255.255.254");
            Assert.Equal(expectedUp, original.Offset(257));
            Assert.Equal(expectedDown, original.Offset(-3));
        }
    }
}
