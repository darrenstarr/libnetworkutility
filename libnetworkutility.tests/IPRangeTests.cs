using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace libnetworkutility.tests
{
    public class IPRangeTests
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
        public void IntersectsRangeOverlapsOther()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.100")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.20"),
                End = IPAddress.Parse("10.1.1.89")
            };

            Assert.True(range.Intersects(other));
        }

        [Fact]
        public void IntersectsOtherOverlapsRange()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.20"),
                End = IPAddress.Parse("10.1.1.89")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.100")
            };

            Assert.True(range.Intersects(other));
        }

        [Fact]
        public void IntersectsRangeEqualsOther()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.20"),
                End = IPAddress.Parse("10.1.1.89")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.20"),
                End = IPAddress.Parse("10.1.1.89")
            };

            Assert.True(range.Intersects(other));
        }

        [Fact]
        public void IntersectsRangeIntoOther()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.89")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.20"),
                End = IPAddress.Parse("10.1.1.100")
            };

            Assert.True(range.Intersects(other));
        }

        [Fact]
        public void IntersectsOtherIntoRange()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.20"),
                End = IPAddress.Parse("10.1.1.100")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.89")
            };

            Assert.True(range.Intersects(other));
        }

        [Fact]
        public void IntersectsDoesntIntersect()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.20"),
                End = IPAddress.Parse("10.1.1.100")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.10")
            };

            Assert.False(range.Intersects(other));
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
        public void RangeEqualsWrongType()
        {
            var x = new IPRange
            {
                Start = IPAddress.Parse("10.0.0.1"),
                End = IPAddress.Parse("10.0.0.255")
            };

            Assert.False(x.Equals(12));
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

        [Fact]
        public void Compare()
        {
            var a = new IPRange
            {
                Start = IPAddress.Parse("10.0.0.1"),
                End = IPAddress.Parse("10.0.0.255")
            };

            var b = new IPRange
            {
                Start = IPAddress.Parse("10.1.0.1"),
                End = IPAddress.Parse("10.2.0.255")
            };

            var c = new IPRange
            {
                Start = IPAddress.Parse("10.0.0.0"),
                End = IPAddress.Parse("10.0.0.40")
            };

            Assert.Equal(-1, a.CompareTo(b));
            Assert.Equal(0, a.CompareTo(a));
            Assert.Equal(1, b.CompareTo(a));
            Assert.Equal(1, a.CompareTo(c));
        }

        [Fact]
        public void RangeSplit()
        {
            var x = new IPRange
            {
                Start = IPAddress.Parse("10.0.0.1"),
                End = IPAddress.Parse("10.0.0.1")
            };

            var good1 = new IPRanges
            {
                x
            };

            Assert.Equal(good1, x.Split(IPAddress.Parse("10.0.0.1")));
            Assert.Equal(good1, x.Split(IPAddress.Parse("192.168.1.1")));

            var y = new IPRange
            {
                Start = IPAddress.Parse("10.0.0.22"),
                End = IPAddress.Parse("192.168.1.10")
            };

            var good2 = new IPRanges
            {
                y
            };

            Assert.Equal(good2, y.Split(IPAddress.Parse("10.0.0.22")));
            Assert.Equal(good2, y.Split(IPAddress.Parse("10.0.0.1")));
            Assert.Equal(good2, y.Split(IPAddress.Parse("201.1.2.3")));

            var good3 = new IPRanges(
                new List<IPRange>
                {
                    new IPRange
                    {
                        Start = IPAddress.Parse("10.0.0.22"),
                        End = IPAddress.Parse("192.168.1.9")
                    },
                    new IPRange
                    {
                        Start = IPAddress.Parse("192.168.1.10"),
                        End = IPAddress.Parse("192.168.1.10")
                    }
                }, 
                false
            );

            Assert.Equal(good3, y.Split(IPAddress.Parse("192.168.1.10")));

            var good4 = new IPRanges(
                new List<IPRange>
                {
                    new IPRange
                    {
                        Start = IPAddress.Parse("10.0.0.22"),
                        End = IPAddress.Parse("172.16.1.0")
                    },
                    new IPRange
                    {
                        Start = IPAddress.Parse("172.16.1.1"),
                        End = IPAddress.Parse("192.168.1.10")
                    }
                },
                false
            );

            Assert.Equal(good4, y.Split(IPAddress.Parse("172.16.1.1")));
        }

        [Fact]
        void BordersNextTrue()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.99")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            Assert.True(range.BordersNext(other));
        }

        [Fact]
        void BordersNextFalseGap()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.99")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.101"),
                End = IPAddress.Parse("10.1.1.199")
            };

            Assert.False(range.BordersNext(other));
        }

        [Fact]
        void BordersPreviousTrue()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.99")
            };

            Assert.True(range.BordersPrevious(other));
        }

        [Fact]
        void BordersPreviousFalseGap()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.102"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.99")
            };

            Assert.False(range.BordersPrevious(other));
        }

        [Fact]
        void BordersTrueForward()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.99")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            Assert.True(range.Borders(other));
        }

        [Fact]
        void BordersTrueBack()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.99")
            };

            Assert.True(range.Borders(other));
        }

        [Fact]
        void BordersFalseForward()
        {
            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.99")
            };

            var other = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.102"),
                End = IPAddress.Parse("10.1.1.199")
            };

            Assert.False(range.Borders(other));
        }

        [Fact]
        void BordersFalseBack()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.102"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.99")
            };

            Assert.False(range.Borders(other));
        }

        [Fact]
        public void CombineRangeEqualsOther()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var good = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            Assert.Equal(good, range.CombineWith(other));
        }

        [Fact]
        public void CombineRangeEclipsesOther()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.200")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var good = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.200")
            };

            Assert.Equal(good, range.CombineWith(other));
        }

        [Fact]
        public void CombineRangeEndToEndLowerFirst()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.99")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var good = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.199")
            };

            Assert.Equal(good, range.CombineWith(other));
        }

        [Fact]
        public void CombineRangeEndToEndUpperFirst()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.99")
            };

            var good = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.199")
            };

            Assert.Equal(good, range.CombineWith(other));
        }

        [Fact]
        public void CombineRangeOverlappingLowerFirst()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.101")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var good = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.199")
            };

            Assert.Equal(good, range.CombineWith(other));
        }

        [Fact]
        public void CombineRangeOverlappingUpperFirst()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.101")
            };

            var good = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.199")
            };

            Assert.Equal(good, range.CombineWith(other));
        }

        [Fact]
        public void CombineRangeThrowsIfNotTouching()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.105"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.101")
            };

            Exception ex = Assert.Throws<ArgumentException>(() => range.CombineWith(other));
            Assert.Equal("Can't combine two ranges which do not intersect or touch", ex.Message);
        }

        [Fact]
        public void CountEqualsAll()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("0.0.0.0"),
                End = IPAddress.Parse("255.255.255.255")
            };

            Assert.Equal(Math.Pow(2, 32), range.Count);
        }

        [Fact]
        public void CountOne()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.1")
            };

            Assert.Equal(1, range.Count);
        }

        [Fact]
        public void CountOneHundred()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.100")
            };

            Assert.Equal(100, range.Count);
        }

        [Fact]
        public void SubtractEclipse()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.80")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.100")
            };

            var good = new libnetworkutility.IPRanges
            {
            };

            Assert.Equal(good, range.Subtract(other));
        }

        [Fact]
        public void SubtractEqual()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.80")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.80")
            };

            var good = new libnetworkutility.IPRanges
            {
            };

            Assert.Equal(good, range.Subtract(other));
        }

        [Fact]
        public void SubtractClipStartAligned()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.80")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.19")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.20"),
                    End = IPAddress.Parse("10.1.1.80")
                }
            };

            Assert.Equal(good, range.Subtract(other));
        }

        [Fact]
        public void SubtractClipStartUnaligned()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.80")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.19")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.20"),
                    End = IPAddress.Parse("10.1.1.80")
                }
            };

            Assert.Equal(good, range.Subtract(other));
        }

        [Fact]
        public void SubtractClipEndAligned()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.80")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.61"),
                End = IPAddress.Parse("10.1.1.80")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.10"),
                    End = IPAddress.Parse("10.1.1.60")
                }
            };

            Assert.Equal(good, range.Subtract(other));
        }

        [Fact]
        public void SubtractClipEndUnaligned()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.80")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.61"),
                End = IPAddress.Parse("10.1.1.85")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.10"),
                    End = IPAddress.Parse("10.1.1.60")
                }
            };

            Assert.Equal(good, range.Subtract(other));
        }

        [Fact]
        public void SubtractNone()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.80")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.82"),
                End = IPAddress.Parse("10.1.1.85")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.10"),
                    End = IPAddress.Parse("10.1.1.80")
                }
            };

            Assert.Equal(good, range.Subtract(other));
        }

        [Fact]
        public void SubtractMiddle()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.10"),
                End = IPAddress.Parse("10.1.1.80")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.20"),
                End = IPAddress.Parse("10.1.1.59")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.10"),
                    End = IPAddress.Parse("10.1.1.19")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.60"),
                    End = IPAddress.Parse("10.1.1.80")
                }
            };

            Assert.Equal(good, range.Subtract(other));
        }

        [Fact]
        public void ComesBeforeNoOverlap()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.200"),
                End = IPAddress.Parse("10.1.1.255")
            };

            Assert.True(range.ComesBefore(other));
            Assert.False(other.ComesBefore(other));
        }

        [Fact]
        public void ComesBeforeOverlap()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.201")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.200"),
                End = IPAddress.Parse("10.1.1.255")
            };

            Assert.False(range.ComesBefore(other));
            Assert.False(other.ComesBefore(other));
        }

        [Fact]
        public void ComesAfterNoOverlap()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.200"),
                End = IPAddress.Parse("10.1.1.255")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            Assert.True(range.ComesAfter(other));
            Assert.False(other.ComesAfter(other));
        }

        [Fact]
        public void ComesAfterOverlap()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.200"),
                End = IPAddress.Parse("10.1.1.255")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.203")
            };

            Assert.False(range.ComesAfter(other));
            Assert.False(other.ComesAfter(other));
        }

        [Fact]
        public void EclipsesEqual()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.200")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.200")
            };

            Assert.True(range.Eclipses(other));
        }

        [Fact]
        public void EclipsesAtStart()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.200")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.200")
            };

            Assert.True(range.Eclipses(other));
        }

        [Fact]
        public void EclipsesEqualAtEnd()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.255")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.200")
            };

            Assert.True(range.Eclipses(other));
        }

        [Fact]
        public void EclipsesBothSides()
        {
            var range = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.255")
            };

            var other = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.200")
            };

            Assert.True(range.Eclipses(other));
        }

        [Fact]
        public void CompareToEqual()
        {
            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.255")
            };

            var other = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.255")
            };

            Assert.Equal(0, range.CompareTo(other));
        }

        [Fact]
        public void CompareToLesser()
        {
            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.255")
            };

            var other = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.0"),
                End = IPAddress.Parse("10.1.1.255")
            };

            Assert.Equal(1, range.CompareTo(other));
        }

        [Fact]
        public void CompareToGreater()
        {
            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.255")
            };

            var other = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.2"),
                End = IPAddress.Parse("10.1.1.255")
            };

            Assert.Equal(-1, range.CompareTo(other));
        }

        [Fact]
        public void GetEnumerator()
        {
            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.3")
            };

            var enumerator = range.GetEnumerator();
            Assert.NotNull(enumerator);
        }

        [Fact]
        public void EnumeratorCurrent()
        {
            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.3")
            };

            var enumerator = range.GetEnumerator();
            Assert.Equal(IPAddress.Parse("10.1.1.1"), enumerator.Current);
        }

        [Fact]
        public void EnumeratorMoveNext()
        {
            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.3")
            };

            var enumerator = range.GetEnumerator();
            Assert.Equal(IPAddress.Parse("10.1.1.1"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.2"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.3"), enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void EnumeratorReset()
        {
            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.3")
            };

            var enumerator = range.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.2"), enumerator.Current);
            enumerator.Reset();
            Assert.Equal(IPAddress.Parse("10.1.1.1"), enumerator.Current);
        }

        [Fact]
        public void EnumeratorMoveNextThrowsOnChange()
        {
            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.3")
            };

            var enumerator = range.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.2"), enumerator.Current);

            range.Start = IPAddress.Parse("10.1.1.0");
            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [Fact]
        public void EnumeratorResetThrowsOnChange()
        {
            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.3")
            };

            var enumerator = range.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.2"), enumerator.Current);

            range.Start = IPAddress.Parse("10.1.1.0");
            Assert.Throws<InvalidOperationException>(() => enumerator.Reset());
        }

        [Fact]
        public void EnumeratorCurrentThrowsOnChange()
        {
            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.3")
            };

            var enumerator = range.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.2"), enumerator.Current);

            range.Start = IPAddress.Parse("10.1.1.0");
            Assert.Throws<InvalidOperationException>(() => { var x = enumerator.Current; } );
        }

    }
}
