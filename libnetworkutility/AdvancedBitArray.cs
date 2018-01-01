using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace libnetworkutility
{
    public class AdvancedBitArray : ICollection, IEnumerable, ICloneable
    {
        public class Enumerator : IEnumerator
        {
            public int Position = 0;
            public AdvancedBitArray Parent { get; private set; }
            public long Version { get; private set; }
            internal Enumerator(AdvancedBitArray parent)
            {
                Parent = parent;
                Version = parent.Version;
            }

            public object Current
            {
                get
                {
                    if(Version != Parent.Version)
                        throw new InvalidOperationException("The collection was modified after the enumerator was created.");
                    return Parent[Position];
                }
            }
            public bool MoveNext()
            {
                if (Version != Parent.Version)
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");

                if (Position >= (Parent.Count - 1))
                    return false;

                Position++;
                return true;
            }

            public void Reset()
            {
                if (Version != Parent.Version)
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");

                Position = 0;
            }
        }

        public long Version { get; private set; } = 0;
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public int Count { get; private set; } = 0;

        ulong[] Bits { get; set; }

        public int BitsSet { get; private set; } = 0;

        public int BitsUnset
        {
            get
            {
                return Count - BitsSet;
            }
        }

        public bool IsSynchronized { get { return false; } }

        public object SyncRoot { get { return null; } }

        private AdvancedBitArray()
        {
        }

        public AdvancedBitArray(int count)
        {
            if (count == 0)
                throw new ArgumentException("Bit array must be at least one bit long");

            var arrayLength = (count / 64) + (((count & 0x3F) == 0) ? 0 : 1);
            Bits = new ulong[arrayLength];

            Count = count;

            UnsetAll();
        }

        public void SetAll()
        {
            for (var i = 0; i < Bits.Length; i++)
                Bits[i] = ulong.MaxValue;

            BitsSet = Count;
            Version++;
        }

        public void UnsetAll()
        {
            for (var i = 0; i < Bits.Length; i++)
                Bits[i] = 0;

            BitsSet = 0;
            Version++;
        }

        public object Clone()
        {
            return new AdvancedBitArray
            {
                Count = Count,
                BitsSet = BitsSet,
                Bits = Bits
                    .Select(x =>
                        x
                    )
                    .ToArray()
            };
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array is null");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index is less than zero.");

            if (array.Length < Count - index)
                throw new ArgumentException("destination array is not long enough for operation");

            if (!typeof(bool).IsAssignableFrom(array.GetType().GetElementType()))
                throw new ArgumentException("destination array is not a boolean array");

            var destination = (bool [])array;

            for (var i = index; i < Count; i++)
                destination[i - index] = this[i];
        }

        public long FindFirst(bool value)
        {
            return FindFirst(value, 0);
        }

        public long FindFirst(bool value, long start)
        {
            if (start < 0 || start >= Count)
                throw new ArgumentOutOfRangeException("start is not within the index range");

            long firstWord = start >> 6;

            var firstBitOffset = Convert.ToInt32(start & 0x3F);
            if(firstBitOffset != 0)
            {
                var x = Bits[firstWord];

                x <<= firstBitOffset;

                if (value)
                {
                    for (var k = firstBitOffset; k < 64; k++, x <<= 1)
                    {
                        if ((x & ((ulong)1 << 63)) != 0)
                            return (firstWord << 6) + k;
                    }
                }
                else
                {
                    for (var k = firstBitOffset; k < 64; k++, x <<= 1)
                    {
                        if ((x & ((ulong)1 << 63)) == 0)
                            return (firstWord << 6) + k;
                    }
                }

                firstWord++;
            }

            var wordFail = value ? ulong.MinValue : ulong.MaxValue;
            for (var i = firstWord; i < (Count >> 6); i++)
            {
                var x = Bits[i];
                if (x == wordFail)
                    continue;

                if (value)
                {
                    for (var k = 0; k < 64; k++, x <<= 1)
                    {
                        if ((x & ((ulong)1 << 63)) != 0)
                            return (i << 6) + k;
                    }
                }
                else
                {
                    for (var k = 0; k < 64; k++, x <<= 1)
                    {
                        if ((x & ((ulong)1 << 63)) == 0)
                            return (i << 6) + k;
                    }
                }
            }

            if ((Count & 0x3F) != 0)
            {
                var x = Bits[Count >> 6];
                if (value)
                {
                    for (var k = 0; k < (Count & 0x3F); k++, x <<= 1)
                    {
                        if ((x & ((ulong)1 << 63)) != 0)
                            return (Count & ~0x3F) + k;
                    }
                }
                else
                {
                    for (var k = 0; k < (Count & 0x3F); k++, x <<= 1)
                    {
                        if ((x & ((ulong)1 << 63)) == 0)
                            return (Count & ~0x3F) + k;
                    }
                }
            }

            return -1;
        }

        public class LongRange 
        {
            public LongRange()
            {
            }

            public long Start { get; set; }
            public long End { get; set; }

            public override bool Equals(object obj)
            {
                var range = obj as LongRange;
                return range != null &&
                       Start == range.Start &&
                       End == range.End;
            }

            public override int GetHashCode()
            {
                var hashCode = -1676728671;
                hashCode = hashCode * -1521134295 + Start.GetHashCode();
                hashCode = hashCode * -1521134295 + End.GetHashCode();
                return hashCode;
            }

            public override string ToString()
            {
                return Start.ToString() + "-" + End.ToString();
            }
        }

        public List<LongRange> SetRanges
        {
            get
            {
                var result = new List<LongRange>();

                var index = FindFirst(true);
                if (index == -1)
                    return result;

                while (index < Count)
                {
                    var nextStart = FindFirst(!this[index], index);
                    if (nextStart == -1)
                        nextStart = Count;

                    if (this[index])
                    {
                        result.Add(new LongRange
                        {
                            Start = index,
                            End = nextStart - 1
                        });
                    }

                    index = nextStart;
                }

                return result;
            }
        }

        public List<LongRange> UnsetRanges
        {
            get
            {
                var result = new List<LongRange>();

                var index = FindFirst(false);
                if (index == -1)
                    return result;

                while (index < Count)
                {
                    var nextStart = FindFirst(!this[index], index);
                    if (nextStart == -1)
                        nextStart = Count;

                    if (!this[index])
                    {
                        result.Add(new LongRange
                        {
                            Start = index,
                            End = nextStart - 1
                        });
                    }

                    index = nextStart;
                }

                return result;
            }
        }

        public bool this[long key]
        {
            get
            {
                if (key < 0 || key >= Count)
                    throw new ArgumentOutOfRangeException("key must be a positive value less than " + Count.ToString());

                var wordIndex = key >> 6;
                var bitMask = (ulong)0x1 << Convert.ToInt32(63 - (key & 0x3F));

                return (Bits[wordIndex] & bitMask) != 0;
            }

            set
            {
                if (key < 0 || key >= Count)
                    throw new ArgumentOutOfRangeException("key must be a positive value less than " + Count.ToString());

                var wordIndex = key >> 6;
                var bitMask = (ulong)0x1 << Convert.ToInt32(63 - (key & 0x3F));

                if (value)
                {
                    if ((Bits[wordIndex] & bitMask) == bitMask)
                        return;

                    BitsSet++;
                    Bits[wordIndex] |= bitMask;
                }
                else
                {
                    if ((Bits[wordIndex] & bitMask) == 0)
                        return;

                    BitsSet--;
                    Bits[wordIndex] &= ~bitMask;
                }
                Version++;
            }
        }
    }
}
