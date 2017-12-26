using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace libnetworkutility.tests
{
    public class IPRange
    {
        [Fact]
        public void Constructor()
        {
            var start = IPAddress.Parse("10.0.0.1");
            var end = IPAddress.Parse("10.0.0.255");

            var x = new libnetworkutility.IPRange(start, end);
            Assert.Equal(start, x.Start);
            Assert.Equal(end, x.End);
            Assert.Equal(start.ToString() + "-" + end.ToString(), x.ToString());

            Exception ex = Assert.Throws<ArgumentException>(() => new libnetworkutility.IPRange(end, start));
            Assert.Equal("start must be less than or equal to end", ex.Message);

            var y = new libnetworkutility.IPRange();
            Assert.Equal(IPAddress.Any, y.Start);
            Assert.Equal(IPAddress.Any, y.End);
        }

        [Fact]
        public void Contains()
        {
            var start = IPAddress.Parse("10.0.0.1");
            var end = IPAddress.Parse("10.0.0.255");

            var testWithin = IPAddress.Parse("10.0.0.125");
            var testWithout = IPAddress.Parse("10.1.1.1");

            var x = new libnetworkutility.IPRange
            {
                Start = start,
                End = end
            };

            Assert.True(x.Contains(testWithin));
            Assert.False(x.Contains(testWithout));
        }

        [Fact]
        public void RangeEquals()
        {
            var x = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.0.0.1"),
                End = IPAddress.Parse("10.0.0.255")
            };

            var good = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.0.0.1"),
                End = IPAddress.Parse("10.0.0.255")
            };

            Assert.True(good.Equals(x));

            var bad1 = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.0.0.1"),
                End = IPAddress.Parse("10.0.0.254")
            };

            Assert.False(bad1.Equals(x));

            var bad2 = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.0.0.2"),
                End = IPAddress.Parse("10.0.0.255")
            };

            Assert.False(bad2.Equals(x));

            Assert.False(x.Equals(null));
        }

        [Fact]
        public void HashCode()
        {
            var x = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.0.0.1"),
                End = IPAddress.Parse("10.0.0.255")
            };

            var h = x.Start.GetHashCode() ^ x.End.GetHashCode();
            Assert.Equal(h, x.GetHashCode());
        }
    }
}
