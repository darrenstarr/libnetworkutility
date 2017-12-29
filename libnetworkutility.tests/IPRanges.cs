using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace libnetworkutility.tests
{
    public class IPRanges
    {
        [Fact]
        public void ConstructorNormalizeList()
        {
            var rangeList = new List<libnetworkutility.IPRange>
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.100"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            var ranges = new libnetworkutility.IPRanges(rangeList, true);

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void ConstructorDontNormalizeList()
        {
            var rangeList = new List<libnetworkutility.IPRange>
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.100"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            var ranges = new libnetworkutility.IPRanges(rangeList, false);

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.100"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            Assert.Equal(rangeList, ranges);
        }

        [Fact]
        public void AddAddressToEmpty()
        {
            var ranges = new libnetworkutility.IPRanges();
            ranges.Add(IPAddress.Parse("10.1.1.1"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.1")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void AddAddressNoChange()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.201"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void AddAddressToEndOfRangeNoNormalize()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.100"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void AddAddressToEndOfRangeNormalize()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.199"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };
        }

        [Fact]
        public void AddAddressToEndOfRangeLastItem()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.199"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void AddAddressToStartOfRange()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.190"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.190"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void InsertAddress()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.105"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.105"),
                    End = IPAddress.Parse("10.1.1.105")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void InsertAddressBeforeOthers()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.10"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.1"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.1")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.10"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void AddAddressToStartOfFirstRange()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.0"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.0"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void AppendAddress()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.200"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.200")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        void RemoveAddressFromEmpty()
        {
            var ranges = new libnetworkutility.IPRanges();

            ranges.Remove(IPAddress.Parse("10.1.1.200"));

            var good = new libnetworkutility.IPRanges
            {
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        void RemoveAddressNotInRanges()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                }
            };

            ranges.Remove(IPAddress.Parse("10.1.1.200"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        void RemoveAddressNotInStartOfRange()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.102"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            ranges.Remove(IPAddress.Parse("10.1.1.102"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.103"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        void RemoveAddressNotAtEndOfRange()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.102"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            ranges.Remove(IPAddress.Parse("10.1.1.100"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.102"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        void RemoveAddressFromSingleAddressRange()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.105"),
                    End = IPAddress.Parse("10.1.1.105")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.150"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            ranges.Remove(IPAddress.Parse("10.1.1.105"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.150"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        void RemoveAddressFromMiddleOfRange()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.102"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            ranges.Remove(IPAddress.Parse("10.1.1.105"));

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.102"),
                    End = IPAddress.Parse("10.1.1.104")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.106"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void AddRangeToEmpty()
        {
            var ranges = new libnetworkutility.IPRanges();

            var rangeToAdd = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.100"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            ranges.Add(rangeToAdd);

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void AddRangeWithExistingEclipse()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var rangeToAdd = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Add(rangeToAdd);

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void RemoveRangeAll()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                }
            };

            var toRemove = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.1"),
                End = IPAddress.Parse("10.1.1.10")
            };

            ranges.Remove(toRemove);

            Assert.Empty(ranges);
        }

        [Fact]
        public void RemoveRangeWholeRange()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.50"),
                End = IPAddress.Parse("10.1.1.200")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Remove(toRemove);

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void RemoveRangeOnePartialTop()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.50"),
                End = IPAddress.Parse("10.1.1.175")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.176"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Remove(toRemove);

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void RemoveRangeOnePartialBottom()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.101"),
                End = IPAddress.Parse("10.1.1.200")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Remove(toRemove);

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void RemoveRangeOnePartialMiddle()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.101"),
                End = IPAddress.Parse("10.1.1.190")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Remove(toRemove);

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void RemoveRangeMultipleWhole()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.50"),
                End = IPAddress.Parse("10.1.1.255")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                }
            };

            ranges.Remove(toRemove);

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void RemoveRangeMultipleOverlap()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new libnetworkutility.IPRange
            {
                Start = IPAddress.Parse("10.1.1.9"),
                End = IPAddress.Parse("10.1.1.240")
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.8")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.241"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Remove(toRemove);

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void AddRanges()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toAdd = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.11"),
                    End = IPAddress.Parse("10.1.1.55")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.208")
                }
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.208")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Add(toAdd);

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void RemoveRanges()
        {
            var ranges = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toAdd = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.8"),
                    End = IPAddress.Parse("10.1.1.55")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.208")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.212"),
                    End = IPAddress.Parse("10.1.1.213")
                }
            };

            var good = new libnetworkutility.IPRanges
            {
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.7")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.56"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.211")
                },
                new libnetworkutility.IPRange
                {
                    Start = IPAddress.Parse("10.1.1.214"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Remove(toAdd);

            Assert.Equal(good, ranges);
        }
    }
}
