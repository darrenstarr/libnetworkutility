using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace libnetworkutility.tests
{
    public class IPRangesTests
    {
        [Fact]
        public void ConstructorNormalizeList()
        {
            var rangeList = new List<IPRange>
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.100"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            var ranges = new IPRanges(rangeList, true);

            var good = new IPRanges
            {
                new IPRange
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
            var rangeList = new List<IPRange>
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.100"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            var ranges = new IPRanges(rangeList, false);

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
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
            var ranges = new IPRanges();
            ranges.Add(IPAddress.Parse("10.1.1.1"));

            var good = new IPRanges
            {
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.201"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.100"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.199"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };
        }

        [Fact]
        public void AddAddressToEndOfRangeLastItem()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.199"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.190"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.190"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.200"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void AddAddresses()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            ranges.Add(new List<IPAddress> {
                IPAddress.Parse("10.1.1.105"),
                IPAddress.Parse("10.1.1.106"),
                IPAddress.Parse("10.1.1.107"),
                IPAddress.Parse("10.1.1.108")
            });

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.105"),
                    End = IPAddress.Parse("10.1.1.108")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void InsertAddress()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.105"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.105"),
                    End = IPAddress.Parse("10.1.1.105")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.10"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.1"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.1")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.10"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.0"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.0"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                }
            };

            ranges.Add(IPAddress.Parse("10.1.1.200"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.198")
                },
                new IPRange
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
            var ranges = new IPRanges();

            ranges.Remove(IPAddress.Parse("10.1.1.200"));

            var good = new IPRanges
            {
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        void RemoveAddressNotInRanges()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                }
            };

            ranges.Remove(IPAddress.Parse("10.1.1.200"));

            var good = new IPRanges
            {
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.102"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            ranges.Remove(IPAddress.Parse("10.1.1.102"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.102"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            ranges.Remove(IPAddress.Parse("10.1.1.100"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.99")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.105"),
                    End = IPAddress.Parse("10.1.1.105")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.150"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            ranges.Remove(IPAddress.Parse("10.1.1.105"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.102"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            ranges.Remove(IPAddress.Parse("10.1.1.105"));

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.102"),
                    End = IPAddress.Parse("10.1.1.104")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.106"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        void RemoveAddresses()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.102"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            ranges.Remove(new List<IPAddress> {
                IPAddress.Parse("10.1.1.105"),
                IPAddress.Parse("10.1.1.106"),
                IPAddress.Parse("10.1.1.107")
            });

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.102"),
                    End = IPAddress.Parse("10.1.1.104")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.108"),
                    End = IPAddress.Parse("10.1.1.199")
                }
            };

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void AddRangeToEmpty()
        {
            var ranges = new IPRanges();

            var rangeToAdd = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var good = new IPRanges
            {
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var rangeToAdd = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.100"),
                End = IPAddress.Parse("10.1.1.199")
            };

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                }
            };

            var toRemove = new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.50"),
                End = IPAddress.Parse("10.1.1.200")
            };

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.50"),
                End = IPAddress.Parse("10.1.1.175")
            };

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.176"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.101"),
                End = IPAddress.Parse("10.1.1.200")
            };

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.101"),
                End = IPAddress.Parse("10.1.1.190")
            };

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.50"),
                End = IPAddress.Parse("10.1.1.255")
            };

            var good = new IPRanges
            {
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toRemove = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.9"),
                End = IPAddress.Parse("10.1.1.240")
            };

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.8")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toAdd = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.11"),
                    End = IPAddress.Parse("10.1.1.55")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.208")
                }
            };

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.208")
                },
                new IPRange
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
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var toAdd = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.8"),
                    End = IPAddress.Parse("10.1.1.55")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.202"),
                    End = IPAddress.Parse("10.1.1.208")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.212"),
                    End = IPAddress.Parse("10.1.1.213")
                }
            };

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.7")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.56"),
                    End = IPAddress.Parse("10.1.1.100")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.191"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.211")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.214"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            ranges.Remove(toAdd);

            Assert.Equal(good, ranges);
        }

        [Fact]
        public void EqualsDifferentCount()
        {
            var a = new IPRanges(
                new List<IPRange> {
                    new IPRange
                    {
                        Start = IPAddress.Parse("10.1.1.1"),
                        End = IPAddress.Parse("10.1.1.10")
                    },
                    new IPRange
                    {
                        Start = IPAddress.Parse("10.1.1.50"),
                        End = IPAddress.Parse("10.1.1.100")
                    },
                    new IPRange
                    {
                        Start = IPAddress.Parse("10.1.1.101"),
                        End = IPAddress.Parse("10.1.1.200")
                    },
                    new IPRange
                    {
                        Start = IPAddress.Parse("10.1.1.210"),
                        End = IPAddress.Parse("10.1.1.255")
                    }
                },
                false
            );

            var b = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void EqualsDifferentRange()
        {
            var a = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.201")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var b = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void EqualsDifferentType()
        {
            var a = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.201")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var b = new List<IPRange>
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.False(a.Equals(b));
        }

        [Fact]
        public void ToStringNice()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.201")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.Equal("10.1.1.1-10.1.1.10,10.1.1.50-10.1.1.201,10.1.1.210-10.1.1.255", ranges.ToString());
        }

        [Fact]
        public void HashCode()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.201")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            int hash = 0x55555555;        // Seed the result

            foreach (var range in ranges)
                hash ^= range.GetHashCode();

            Assert.Equal(hash, ranges.GetHashCode());
        }

        [Fact]
        public void CompareToEqual()
        {
            var a = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var b = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.Equal(0, a.CompareTo(b));
        }

        [Fact]
        public void CompareToNotEqual()
        {
            var a = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            var b = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.220")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.NotEqual(0, a.CompareTo(b));
        }

        [Fact]
        public void GetAddressEnumerator()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.3")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.5"),
                    End = IPAddress.Parse("10.1.1.5")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.7"),
                    End = IPAddress.Parse("10.1.1.8")
                }
            };

            var enumerator = ranges.GetAddressEnumerator();
            Assert.NotNull(enumerator);
        }

        [Fact]
        public void AddressEnumeratorCurrent()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.3")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.5"),
                    End = IPAddress.Parse("10.1.1.5")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.7"),
                    End = IPAddress.Parse("10.1.1.8")
                }
            };

            var enumerator = ranges.GetAddressEnumerator();
            Assert.Equal(IPAddress.Parse("10.1.1.1"), enumerator.Current);
        }

        [Fact]
        public void AddressEnumeratorMoveNext()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.3")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.5"),
                    End = IPAddress.Parse("10.1.1.5")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.7"),
                    End = IPAddress.Parse("10.1.1.8")
                }
            };

            var enumerator = ranges.GetAddressEnumerator();
            Assert.Equal(IPAddress.Parse("10.1.1.1"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.2"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.3"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.5"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.7"), enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.8"), enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void AddressEnumeratorReset()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.3")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.5"),
                    End = IPAddress.Parse("10.1.1.5")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.7"),
                    End = IPAddress.Parse("10.1.1.8")
                }
            };

            var enumerator = ranges.GetAddressEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.True(enumerator.MoveNext());
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.5"), enumerator.Current);
            enumerator.Reset();
            Assert.Equal(IPAddress.Parse("10.1.1.1"), enumerator.Current);
        }

        [Fact]
        public void AddressEnumeratorMoveNextThrowsOnChange()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.3")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.5"),
                    End = IPAddress.Parse("10.1.1.5")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.7"),
                    End = IPAddress.Parse("10.1.1.8")
                }
            };

            var enumerator = ranges.GetAddressEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.2"), enumerator.Current);
            ranges[0].End = IPAddress.Parse("10.1.1.4");
            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [Fact]
        public void AddressEnumeratorResetThrowsOnChange()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.3")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.5"),
                    End = IPAddress.Parse("10.1.1.5")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.7"),
                    End = IPAddress.Parse("10.1.1.8")
                }
            };

            var enumerator = ranges.GetAddressEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.2"), enumerator.Current);
            ranges[0].End = IPAddress.Parse("10.1.1.4");
            Assert.Throws<InvalidOperationException>(() => enumerator.Reset());
        }

        [Fact]
        public void AddressEnumeratorCurrentThrowsOnChange()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.3")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.5"),
                    End = IPAddress.Parse("10.1.1.5")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.7"),
                    End = IPAddress.Parse("10.1.1.8")
                }
            };

            var enumerator = ranges.GetAddressEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(IPAddress.Parse("10.1.1.2"), enumerator.Current);
            ranges[0].End = IPAddress.Parse("10.1.1.4");
            Assert.Throws<InvalidOperationException>(() => { var x = enumerator.Current; } );
        }

        [Fact]
        public void Clone()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.3")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.5"),
                    End = IPAddress.Parse("10.1.1.5")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.7"),
                    End = IPAddress.Parse("10.1.1.8")
                }
            };

            var clone = ranges.Clone();

            Assert.Equal(ranges, clone);

            ranges[0].Start = IPAddress.Parse("10.1.1.0");

            Assert.NotEqual(ranges, clone);
        }

        [Fact]
        public void Contains()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.True(ranges.Contains(IPAddress.Parse("10.1.1.220")));
        }

        [Fact]
        public void DoesNotContain()
        {
            var ranges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.1"),
                    End = IPAddress.Parse("10.1.1.10")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.1.200")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.210"),
                    End = IPAddress.Parse("10.1.1.255")
                }
            };

            Assert.False(ranges.Contains(IPAddress.Parse("10.1.1.205")));
        }
    }
}
