using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace libnetworkutility.tests
{
    public class IPAddressPoolTests
    {
        [Fact]
        public void ConstructPoolLiteral()
        {
            var pool = new IPAddressPool(IPAddress.Parse("10.1.1.0"), 256);

            var goodRange = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.0"),
                End = IPAddress.Parse("10.1.1.255")
            };

            Assert.Equal(goodRange, pool.Range);
        }

        [Fact]
        public void ConstructPoolNetworkPrefix()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.1.0/23"));

            var goodRange = new IPRange
            {
                Start = IPAddress.Parse("10.1.0.0"),
                End = IPAddress.Parse("10.1.1.255")
            };

            Assert.Equal(goodRange, pool.Range);
        }

        [Fact]
        public void ReserveAddresses()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            Assert.False(pool[IPAddress.Parse("10.1.2.255")]);
            pool.Reserve(IPAddress.Parse("10.1.2.255"));
            Assert.True(pool[IPAddress.Parse("10.1.2.255")]);

            Assert.False(pool[IPAddress.Parse("10.1.3.25")]);
            pool.Reserve(IPAddress.Parse("10.1.3.25"));
            Assert.True(pool[IPAddress.Parse("10.1.3.25")]);
        }

        [Fact]
        public void ReserveReservedAddressThrows()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.1.0/23"));

            pool.Reserve(IPAddress.Parse("10.1.1.255"));
            Assert.True(pool[IPAddress.Parse("10.1.1.255")]);

            Assert.Throws<ArgumentException>(() => pool.Reserve(IPAddress.Parse("10.1.1.255")));
        }

        [Fact]
        public void ReserveInvalidAddressThrows()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.1.0/23"));

            Assert.Throws<ArgumentOutOfRangeException>(() => pool.Reserve(IPAddress.Parse("10.1.3.1")));
        }

        [Fact]
        public void GetInvalidAddressThrows()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.1.0/23"));

            Assert.Throws<ArgumentOutOfRangeException>(() => { var x = pool[IPAddress.Parse("10.1.3.1")]; });
        }

        [Fact]
        public void ReserveByIndex()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.1.0/23"));

            pool[IPAddress.Parse("10.1.1.5")] = true;
            Assert.True(pool[IPAddress.Parse("10.1.1.5")]);

            pool[IPAddress.Parse("10.1.1.5")] = false;
            Assert.False(pool[IPAddress.Parse("10.1.1.5")]);
        }

        [Fact]
        public void ReserveRange()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.2.50"),
                End = IPAddress.Parse("10.1.3.120")
            };

            pool.Reserve(range);
            Assert.Equal(185, pool.Available);

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.0"),
                    End = IPAddress.Parse("10.1.2.49")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.3.121"),
                    End = IPAddress.Parse("10.1.3.255")
                }
            };

            var enumerator = good.GetAddressEnumerator();
            do
            {
                Assert.False(pool[enumerator.Current]);
            } while (enumerator.MoveNext());

            foreach (var address in range)
                Assert.True(pool[address]);
        }

        [Fact]
        public void ReserveThrowsOnOutOfRangeWayTooLow()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.50"),
                End = IPAddress.Parse("10.1.1.120")
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => pool.Reserve(range));
        }

        [Fact]
        public void ReserveThrowsOnOutOfRangeTooLow()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.50"),
                End = IPAddress.Parse("10.1.2.120")
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => pool.Reserve(range));
        }

        [Fact]
        public void ReserveThrowsOnOutOfRangeTooHigh()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.3.50"),
                End = IPAddress.Parse("10.1.4.120")
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => pool.Reserve(range));
        }

        [Fact]
        public void ReserveThrowsOnOutOfRangeWayTooHigh()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.4.50"),
                End = IPAddress.Parse("10.1.4.120")
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => pool.Reserve(range));
        }

        [Fact]
        public void ReserveThrowsOnOutOfRangeConflicts()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var range = new IPRange
            {
                Start = IPAddress.Parse("10.1.2.50"),
                End = IPAddress.Parse("10.1.2.120")
            };

            pool.Reserve(IPAddress.Parse("10.1.2.100"));
            Assert.Throws<ArgumentException>(() => pool.Reserve(range));
        }

        [Fact]
        public void ReserveRanges()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var ranges = new IPRanges
            {
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.50"),
                    End = IPAddress.Parse("10.1.2.120")
                },
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.150"),
                    End = IPAddress.Parse("10.1.2.152")
                }
            };

            pool.Reserve(ranges);
            Assert.Equal(438, pool.Available);

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.0"),
                    End = IPAddress.Parse("10.1.2.49")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.121"),
                    End = IPAddress.Parse("10.1.2.149")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.153"),
                    End = IPAddress.Parse("10.1.3.255")
                }
            };

            var enumerator = good.GetAddressEnumerator();
            do
            {
                Assert.False(pool[enumerator.Current]);
            } while (enumerator.MoveNext());

            enumerator = ranges.GetAddressEnumerator();
            do
            {
                Assert.True(pool[enumerator.Current]);
            } while (enumerator.MoveNext());
        }

        [Fact]
        public void ReserveRangesThrowsOnOutOfRange()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var ranges = new IPRanges
            {
                new IPRange {
                    Start = IPAddress.Parse("10.1.1.50"),
                    End = IPAddress.Parse("10.1.2.120")
                },
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.150"),
                    End = IPAddress.Parse("10.1.2.152")
                }
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => pool.Reserve(ranges));
        }

        [Fact]
        public void ReserveRangesThrowsOnAlreadyReserved()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var ranges = new IPRanges
            {
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.50"),
                    End = IPAddress.Parse("10.1.2.120")
                },
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.150"),
                    End = IPAddress.Parse("10.1.2.152")
                }
            };

            pool.Reserve(IPAddress.Parse("10.1.2.55"));

            Assert.Throws<ArgumentException>(() => pool.Reserve(ranges));
        }

        [Fact]
        public void UnreserveAddress()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var ranges = new IPRanges
            {
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.50"),
                    End = IPAddress.Parse("10.1.2.120")
                },
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.150"),
                    End = IPAddress.Parse("10.1.2.152")
                }
            };

            pool.Reserve(ranges);
            Assert.True(pool[IPAddress.Parse("10.1.2.51")]);
            pool.Unreserve(IPAddress.Parse("10.1.2.51"));
            Assert.False(pool[IPAddress.Parse("10.1.2.51")]);
        }

        [Fact]
        public void UnreserveAddressThrowsOnAlreadyUnreserved()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var ranges = new IPRanges
            {
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.50"),
                    End = IPAddress.Parse("10.1.2.120")
                },
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.150"),
                    End = IPAddress.Parse("10.1.2.152")
                }
            };

            Assert.Throws<ArgumentException>(() => pool.Unreserve(IPAddress.Parse("10.1.2.51")));
            pool.Reserve(ranges);
            Assert.True(pool[IPAddress.Parse("10.1.2.51")]);
            pool.Unreserve(IPAddress.Parse("10.1.2.51"));
            Assert.False(pool[IPAddress.Parse("10.1.2.51")]);
            Assert.Throws<ArgumentException>(() => pool.Unreserve(IPAddress.Parse("10.1.2.51")));
        }

        [Fact]
        public void UnreserveAddressThrowsOnOutOfRange()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var ranges = new IPRanges
            {
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.50"),
                    End = IPAddress.Parse("10.1.2.120")
                },
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.150"),
                    End = IPAddress.Parse("10.1.2.152")
                }
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => pool.Unreserve(IPAddress.Parse("10.1.1.51")));
        }

        [Fact]
        public void UnreserveAddressRange()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var ranges = new IPRanges
            {
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.50"),
                    End = IPAddress.Parse("10.1.2.120")
                },
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.150"),
                    End = IPAddress.Parse("10.1.2.152")
                }
            };

            var unreserveRange = new IPRange
            {
                Start = IPAddress.Parse("10.1.2.55"),
                End = IPAddress.Parse("10.1.2.60")
            };

            pool.Reserve(ranges);
            pool.Unreserve(unreserveRange);

            foreach (var address in unreserveRange)
                Assert.False(pool[address]);
        }

        [Fact]
        public void UnreserveAddressRangeThrowsOnConflict()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var ranges = new IPRanges
            {
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.50"),
                    End = IPAddress.Parse("10.1.2.120")
                },
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.150"),
                    End = IPAddress.Parse("10.1.2.152")
                }
            };

            var unreserveRange = new IPRange
            {
                Start = IPAddress.Parse("10.1.2.55"),
                End = IPAddress.Parse("10.1.2.60")
            };

            pool.Reserve(ranges);
            pool.Unreserve(IPAddress.Parse("10.1.2.57"));
            Assert.Throws<ArgumentException>(() => pool.Unreserve(unreserveRange));
        }

        [Fact]
        public void UnreserveAddressRangeThrowsOnOutOfRange()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var ranges = new IPRanges
            {
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.50"),
                    End = IPAddress.Parse("10.1.2.120")
                },
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.150"),
                    End = IPAddress.Parse("10.1.2.152")
                }
            };

            var unreserveRange = new IPRange
            {
                Start = IPAddress.Parse("10.1.1.55"),
                End = IPAddress.Parse("10.1.2.60")
            };

            pool.Reserve(ranges);
            Assert.Throws<ArgumentOutOfRangeException>(() => pool.Unreserve(unreserveRange));
        }

        [Fact]
        public void UnreserveAddressRanges()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var ranges = new IPRanges
            {
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.50"),
                    End = IPAddress.Parse("10.1.2.120")
                },
                new IPRange {
                    Start = IPAddress.Parse("10.1.2.150"),
                    End = IPAddress.Parse("10.1.2.170")
                }
            };

            var unreserveRanges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.55"),
                    End = IPAddress.Parse("10.1.2.60")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.160"),
                    End = IPAddress.Parse("10.1.2.165")
                }
            };

            pool.Reserve(ranges);

            var enumerator = unreserveRanges.GetAddressEnumerator();
            do
            {
                Assert.True(pool[enumerator.Current]);
            } while (enumerator.MoveNext());
            enumerator.Dispose();

            pool.Unreserve(unreserveRanges);

            enumerator = unreserveRanges.GetAddressEnumerator();
            do
            {
                Assert.False(pool[enumerator.Current]);
            } while (enumerator.MoveNext());
            enumerator.Dispose();

            var shouldBeTrue = ranges.Minus(unreserveRanges);

            enumerator = shouldBeTrue.GetAddressEnumerator();
            do
            {
                Assert.True(pool[enumerator.Current]);
            } while (enumerator.MoveNext());
            enumerator.Dispose();
        }

        [Fact]
        public void UnreserveAddressRangesThrowsOnOutOfRange()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var unreserveRanges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.1.55"),
                    End = IPAddress.Parse("10.1.2.60")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.160"),
                    End = IPAddress.Parse("10.1.2.165")
                }
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => pool.Unreserve(unreserveRanges));
        }

        [Fact]
        public void UnreserveAddressRangesThrowsOnAlreadyUnreserved()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var unreserveRanges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.55"),
                    End = IPAddress.Parse("10.1.2.60")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.160"),
                    End = IPAddress.Parse("10.1.2.165")
                }
            };

            Assert.Throws<ArgumentException>(() => pool.Unreserve(unreserveRanges));
        }

        [Fact]
        public void ReserveNextAddress()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            Assert.Equal(IPAddress.Parse("10.1.2.0"), pool.ReserveNextAddress());
            Assert.Equal(IPAddress.Parse("10.1.2.1"), pool.ReserveNextAddress());
            Assert.Equal(IPAddress.Parse("10.1.2.2"), pool.ReserveNextAddress());
            Assert.Equal(IPAddress.Parse("10.1.2.3"), pool.ReserveNextAddress());
        }

        [Fact]
        public void ReserveNextAddressFailsWhenOutOfAddresses()
        {
            var pool = new IPAddressPool(IPAddress.Parse("10.1.2.1"), 3);

            Assert.Equal(IPAddress.Parse("10.1.2.1"), pool.ReserveNextAddress());
            Assert.Equal(IPAddress.Parse("10.1.2.2"), pool.ReserveNextAddress());
            Assert.Equal(IPAddress.Parse("10.1.2.3"), pool.ReserveNextAddress());
            Assert.Null(pool.ReserveNextAddress());
        }


        [Fact]
        public void ReservedAddressRangesMultiple()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var reserveRanges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.55"),
                    End = IPAddress.Parse("10.1.2.60")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.160"),
                    End = IPAddress.Parse("10.1.2.165")
                }
            };

            pool.Reserve(reserveRanges);

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.55"),
                    End = IPAddress.Parse("10.1.2.60")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.160"),
                    End = IPAddress.Parse("10.1.2.165")
                }
            };

            Assert.Equal(good, pool.ReservedAddressRanges);
        }

        [Fact]
        public void UnreservedAddressRangesMultiple()
        {
            var pool = new IPAddressPool(NetworkPrefix.Parse("10.1.2.0/23"));

            var reserveRanges = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.55"),
                    End = IPAddress.Parse("10.1.2.60")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.160"),
                    End = IPAddress.Parse("10.1.2.165")
                }
            };

            pool.Reserve(reserveRanges);

            var good = new IPRanges
            {
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.0"),
                    End = IPAddress.Parse("10.1.2.54")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.61"),
                    End = IPAddress.Parse("10.1.2.159")
                },
                new IPRange
                {
                    Start = IPAddress.Parse("10.1.2.166"),
                    End = IPAddress.Parse("10.1.3.255")
                }
            };

            Assert.Equal(good, pool.UnreservedAddressRanges);
        }
    }
}
