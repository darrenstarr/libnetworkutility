using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace libnetworkutility.tests
{
    public class AdvancedBitArrayTests
    {
        [Fact]
        public void ConstructorZeroThrows()
        {
            Exception ex = Assert.Throws<ArgumentException>(() => new AdvancedBitArray(0));
            Assert.Equal("Bit array must be at least one bit long", ex.Message);
        }

        [Fact]
        public void ConstructorSetsZero()
        {
            var bits = new AdvancedBitArray(100);
            Assert.Equal(0, bits.BitsSet);

            foreach (var bit in bits)
                Assert.False((bool)bit);
        }

        [Fact]
        public void GetEnumeratorNotNull()
        {
            var bits = new AdvancedBitArray(100);
            Assert.NotNull(bits.GetEnumerator());
        }

        [Fact]
        public void BasicSetAndGet()
        {
            var bits = new AdvancedBitArray(100);
            for (var i = 0; i < bits.Count; i += 2)
                bits[i] = true;

            for (var i = 0; i < bits.Count; i += 2)
            {
                Assert.True(bits[i]);
                if (i < (bits.Count - 1))
                    Assert.False(bits[i + 1]);
            }
        }

        [Fact]
        public void GetAndSetPastEndThrows()
        {
            var bits = new AdvancedBitArray(100);
            Exception ex = Assert.Throws<ArgumentOutOfRangeException>(() => bits[100] = true);
            Assert.Matches("key must be a positive value less than [0-9]+", ex.Message);
            ex = Assert.Throws<ArgumentOutOfRangeException>(() => { var v = bits[100]; });
            Assert.Matches("key must be a positive value less than [0-9]+", ex.Message);
        }

        [Fact]
        public void SetTwice()
        {
            var bits = new AdvancedBitArray(1);
            var version = bits.Version;

            bits[0] = true;
            Assert.True(bits[0]);
            Assert.Equal(version + 1, bits.Version);

            bits[0] = true;
            Assert.True(bits[0]);
            Assert.Equal(version + 1, bits.Version);

            bits[0] = false;
            Assert.False(bits[0]);
            Assert.Equal(version + 2, bits.Version);

            bits[0] = false;
            Assert.False(bits[0]);
            Assert.Equal(version + 2, bits.Version);
        }

        [Fact]
        public void EnumeratorCurrentAndMoveNext()
        {
            var bits = new AdvancedBitArray(3);
            bits[0] = true;
            bits[2] = true;

            var enumerator = bits.GetEnumerator();
            Assert.True((bool)enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.False((bool)enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.True((bool)enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void EnumeratorReset()
        {
            var bits = new AdvancedBitArray(2);
            bits[0] = true;

            var enumerator = bits.GetEnumerator();
            Assert.True((bool)enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.False((bool)enumerator.Current);
            Assert.False(enumerator.MoveNext());
            Assert.False((bool)enumerator.Current);
            enumerator.Reset();
            Assert.True((bool)enumerator.Current);
        }

        [Fact]
        public void EnumeratorMoveNextThrowOnChange()
        {
            var bits = new AdvancedBitArray(3);
            bits[0] = true;
            bits[2] = true;

            var enumerator = bits.GetEnumerator();
            Assert.True((bool)enumerator.Current);
            Assert.True(enumerator.MoveNext());
            bits[2] = false;
            Exception ex = Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
            Assert.Equal("The collection was modified after the enumerator was created.", ex.Message);
        }

        [Fact]
        public void EnumeratorCurrentThrowOnChange()
        {
            var bits = new AdvancedBitArray(3);
            bits[0] = true;
            bits[2] = true;

            var enumerator = bits.GetEnumerator();
            Assert.True((bool)enumerator.Current);
            Assert.True(enumerator.MoveNext());
            bits[2] = false;
            Exception ex = Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            Assert.Equal("The collection was modified after the enumerator was created.", ex.Message);
        }

        [Fact]
        public void EnumeratorMoveNextThrowOnReset()
        {
            var bits = new AdvancedBitArray(3);
            bits[0] = true;
            bits[2] = true;

            var enumerator = bits.GetEnumerator();
            Assert.True((bool)enumerator.Current);
            Assert.True(enumerator.MoveNext());
            bits[2] = false;
            Exception ex = Assert.Throws<InvalidOperationException>(() => enumerator.Reset());
            Assert.Equal("The collection was modified after the enumerator was created.", ex.Message);
        }

        [Fact]
        public void BitsSet()
        {
            var bits = new AdvancedBitArray(3);
            bits[0] = true;
            bits[2] = true;

            Assert.Equal(2, bits.BitsSet);
        }

        [Fact]
        public void BitsUnset()
        {
            var bits = new AdvancedBitArray(3);
            bits[0] = true;
            bits[2] = true;

            Assert.Equal(1, bits.BitsUnset);
        }

        [Fact]
        public void IsSynchronized()
        {
            var bits = new AdvancedBitArray(3);
            Assert.False(bits.IsSynchronized);
        }

        [Fact]
        public void SyncRoot()
        {
            var bits = new AdvancedBitArray(3);
            Assert.Null(bits.SyncRoot);
        }

        [Fact]
        public void SetAndUnsetAll()
        {
            var bits = new AdvancedBitArray(100);
            Assert.Equal(100, bits.BitsUnset);

            bits.UnsetAll();
            Assert.Equal(0, bits.BitsSet);
            for (var i = 0; i < bits.Count; i++)
                Assert.False(bits[i]);

            bits.SetAll();
            Assert.Equal(100, bits.BitsSet);
            for (var i = 0; i < bits.Count; i++)
                Assert.True(bits[i]);
        }

        [Fact]
        public void CloneArray()
        {
            var bits = new AdvancedBitArray(100);
            for (var i = 0; i < bits.Count; i += 3)
                bits[i] = true;

            var cloned = (AdvancedBitArray)bits.Clone();
            Assert.NotNull(cloned);

            Assert.Equal(bits.Count, cloned.Count);
            Assert.Equal(bits.BitsSet, cloned.BitsSet);
            for (var i = 0; i < bits.Count; i++)
                Assert.Equal(bits[i], cloned[i]);

            bits[10] = !bits[10];
            Assert.NotEqual(bits[10], cloned[10]);
        }

        [Fact]
        public void CopyArrayFull()
        {
            var bits = new AdvancedBitArray(100);
            for (var i = 0; i < bits.Count; i += 3)
                bits[i] = true;

            var array = new bool[bits.Count];
            bits.CopyTo(array, 0);

            for (var i = 0; i < bits.Count; i++)
                Assert.Equal(bits[i], array[i]);
        }

        [Fact]
        public void CopyArrayDestinationIsNullThrows()
        {
            var bits = new AdvancedBitArray(100);
            for (var i = 0; i < bits.Count; i += 3)
                bits[i] = true;

            bool [] array = null;

            Assert.Throws<ArgumentNullException>(() => bits.CopyTo(array, 0));
        }

        [Fact]
        public void CopyArrayOutOfRangeThrows()
        {
            var bits = new AdvancedBitArray(100);
            for (var i = 0; i < bits.Count; i += 3)
                bits[i] = true;

            var array = new bool[bits.Count];
            Assert.Throws<ArgumentOutOfRangeException>(() => bits.CopyTo(array, -100));
        }

        [Fact]
        public void CopyArrayIndexPastEndThrows()
        {
            var bits = new AdvancedBitArray(100);
            for (var i = 0; i < bits.Count; i += 3)
                bits[i] = true;

            var array = new bool[bits.Count - 1];
            Exception ex = Assert.Throws<ArgumentException>(() => bits.CopyTo(array, 0));
            Assert.Equal("destination array is not long enough for operation", ex.Message);
        }

        [Fact]
        public void CopyArrayWrongArrayTypeThrows()
        {
            var bits = new AdvancedBitArray(100);
            for (var i = 0; i < bits.Count; i += 3)
                bits[i] = true;

            var array = new int[bits.Count];

            Exception ex = Assert.Throws<ArgumentException>(() => bits.CopyTo(array, 0));
            Assert.Equal("destination array is not a boolean array", ex.Message);
        }

        [Fact]
        public void CopyArrayPartial()
        {
            var bits = new AdvancedBitArray(100);
            for (var i = 0; i < bits.Count; i += 3)
                bits[i] = true;

            var array = new bool[50];
            bits.CopyTo(array, 50);

            for (var i = 0; i < 50; i++)
                Assert.Equal(bits[i + 50], array[i]);
        }

        [Fact]
        public void FindFirstZero()
        {
            var bits = new AdvancedBitArray(1000);
            bits.SetAll();
            Assert.Equal(-1, bits.FindFirst(false));

            bits[999] = false;
            Assert.Equal(999, bits.FindFirst(false));

            bits[100] = false;
            Assert.Equal(100, bits.FindFirst(false));

            bits[62] = false;
            Assert.Equal(62, bits.FindFirst(false));

            bits[0] = false;
            Assert.Equal(0, bits.FindFirst(false));
        }

        [Fact]
        public void FindFirstOne()
        {
            var bits = new AdvancedBitArray(1000);
            Assert.Equal(-1, bits.FindFirst(true));

            bits[999] = true;
            Assert.Equal(999, bits.FindFirst(true));

            bits[100] = true;
            Assert.Equal(100, bits.FindFirst(true));

            bits[62] = true;
            Assert.Equal(62, bits.FindFirst(true));

            bits[0] = true;
            Assert.Equal(0, bits.FindFirst(true));
        }

        [Fact]
        public void FindFirstZeroFromIndex()
        {
            var bits = new AdvancedBitArray(1000);
            bits.SetAll();
            Assert.Equal(-1, bits.FindFirst(false, 500));

            bits[999] = false;
            Assert.Equal(999, bits.FindFirst(false, 750));

            bits[501] = false;
            Assert.Equal(501, bits.FindFirst(false, 500));

            bits[100] = false;
            Assert.Equal(100, bits.FindFirst(false, 75));
            Assert.Equal(100, bits.FindFirst(false, 64));

            bits[62] = false;
            Assert.Equal(62, bits.FindFirst(false, 0));

            bits[0] = false;
            Assert.Equal(0, bits.FindFirst(false, 0));
        }

        [Fact]
        public void FindFirstOneFromIndex()
        {
            var bits = new AdvancedBitArray(1000);
            Assert.Equal(-1, bits.FindFirst(true, 500));

            bits[999] = true;
            Assert.Equal(999, bits.FindFirst(true, 750));

            bits[501] = true;
            Assert.Equal(501, bits.FindFirst(true, 500));

            bits[100] = true;
            Assert.Equal(100, bits.FindFirst(true, 75));
            Assert.Equal(100, bits.FindFirst(true, 64));

            bits[62] = true;
            Assert.Equal(62, bits.FindFirst(true, 0));

            bits[0] = true;
            Assert.Equal(0, bits.FindFirst(true, 0));
        }

        [Fact]
        public void FirstFirstFromIndexThrowsOnIndexTooLow()
        {
            var bits = new AdvancedBitArray(1000);
            Assert.Throws<ArgumentOutOfRangeException>(() => bits.FindFirst(true, -1));
        }

        [Fact]
        public void FirstFirstFromIndexThrowsOnIndexTooHigh()
        {
            var bits = new AdvancedBitArray(1000);
            Assert.Throws<ArgumentOutOfRangeException>(() => bits.FindFirst(true, 2000));
        }

        [Fact]
        public void SetRangesNone()
        {
            var bits = new AdvancedBitArray(1000);

            var good = new List<AdvancedBitArray.LongRange>();

            Assert.Equal(good, bits.SetRanges);
        }

        [Fact]
        public void SetRangesOne()
        {
            var bits = new AdvancedBitArray(1000);
            bits[500] = true;

            var good = new List<AdvancedBitArray.LongRange>
            {
                new AdvancedBitArray.LongRange
                {
                    Start = 500,
                    End = 500
                }
            };

            Assert.Equal(good, bits.SetRanges);
        }

        [Fact]
        public void SetRangesMultipleRanges()
        {
            var bits = new AdvancedBitArray(1000);
            for (var i = 10; i <= 100; i++)
                bits[i] = true;
            for (var i = 200; i <= 299; i++)
                bits[i] = true;

            var good = new List<AdvancedBitArray.LongRange>
            {
                new AdvancedBitArray.LongRange
                {
                    Start = 10,
                    End = 100
                },
                new AdvancedBitArray.LongRange
                {
                    Start = 200,
                    End = 299
                }
            };

            Assert.Equal(good, bits.SetRanges);
        }

        [Fact]
        public void UnsetRangesNone()
        {
            var bits = new AdvancedBitArray(1000);
            bits.SetAll();

            var good = new List<AdvancedBitArray.LongRange>();

            Assert.Equal(good, bits.UnsetRanges);
        }

        [Fact]
        public void UnsetRangesOne()
        {
            var bits = new AdvancedBitArray(1000);
            bits[500] = true;

            var good = new List<AdvancedBitArray.LongRange>
            {
                new AdvancedBitArray.LongRange
                {
                    Start = 0,
                    End = 499
                },
                new AdvancedBitArray.LongRange
                {
                    Start = 501,
                    End = 999
                }
            };

            Assert.Equal(good, bits.UnsetRanges);
        }

        [Fact]
        public void UnsetRangesMultipleRanges()
        {
            var bits = new AdvancedBitArray(1000);
            for (var i = 10; i <= 100; i++)
                bits[i] = true;
            for (var i = 200; i <= 299; i++)
                bits[i] = true;

            var good = new List<AdvancedBitArray.LongRange>
            {
                new AdvancedBitArray.LongRange
                {
                    Start = 0,
                    End = 9
                },
                new AdvancedBitArray.LongRange
                {
                    Start = 101,
                    End = 199
                },
                new AdvancedBitArray.LongRange
                {
                    Start = 300,
                    End = 999
                }
            };

            Assert.Equal(good, bits.UnsetRanges);
        }

        [Fact]
        public void LongRangeHashCode()
        {
            var a = new AdvancedBitArray.LongRange
            {
                Start = 0,
                End = 1000
            };

            var b = new AdvancedBitArray.LongRange
            {
                Start = 0,
                End = 1000
            };

            Assert.Equal(a.GetHashCode(), b.GetHashCode());

            var c = new AdvancedBitArray.LongRange
            {
                Start = 1,
                End = 1000
            };

            Assert.NotEqual(a.GetHashCode(), c.GetHashCode());
        }

        [Fact]
        public void LongRangeToString()
        {
            var range = new AdvancedBitArray.LongRange
            {
                Start = 1,
                End = 1000
            };

            Assert.Equal("1-1000", range.ToString());
        }
    }
}
