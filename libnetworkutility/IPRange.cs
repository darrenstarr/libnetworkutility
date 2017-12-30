using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace libnetworkutility
{
    public class IPRange : IEnumerable<IPAddress>, IComparable<IPRange>
    {
        public class Enumerator : IEnumerator<IPAddress>
        {
            private IPRange Parent { get; set; }
            private long Position { get; set; } = 0;
            private int Hash { get; set; }

            internal Enumerator(IPRange parent)
            {
                Parent = parent;
                Hash = Parent.GetHashCode();
            }

            public object Current
            {
                get
                {
                    // TODO : Figure out how to trigger this from a unit test
                    if (Hash != Parent.GetHashCode())
                        throw new InvalidOperationException("The collection was modified after the enumerator was created.");

                    return Parent.Start.Offset(Position);
                }
            }

            IPAddress IEnumerator<IPAddress>.Current
            {
                get
                {
                    if (Hash != Parent.GetHashCode())
                        throw new InvalidOperationException("The collection was modified after the enumerator was created.");

                    return Parent.Start.Offset(Position);
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (Hash != Parent.GetHashCode())
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");

                if (Position >= (Parent.Count - 1))
                    return false;

                Position++;
                return true;
            }

            public void Reset()
            {
                if (Hash != Parent.GetHashCode())
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");

                Position = 0;
            }
        };

        public IPAddress Start { get; set; }

        public IPAddress End { get; set; }

        public IPRange()
        {
            Start = IPAddress.Any;
            End = IPAddress.Any;
        }

        public IPRange(IPAddress start, IPAddress end)
        {
            if (start.GreaterThan(end))
                throw new ArgumentException("start must be less than or equal to end");

            Start = start;
            End = end;
        }

        public bool Contains(IPAddress address)
        {
            return address.GreaterThanOrEqual(Start) && address.LessThanOrEqual(End);
        }

        public bool Intersects(IPRange range)
        {
            return (
                Contains(range.Start) ||
                Contains(range.End) ||
                range.Contains(Start) ||
                range.Contains(End)
            );
        }

        public bool Borders(IPRange range)
        {
            return (
                BordersNext(range) ||
                BordersPrevious(range)
            );
        }

        public bool BordersNext(IPRange range)
        {
            return End.Offset(1).Equals(range.Start);
        }

        public bool BordersPrevious(IPRange range)
        {
            return range.BordersNext(this);
        }

        public override string ToString()
        {
            return Start.ToString() + "-" + End.ToString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as IPRange;
            if (other == null)
                return false;

            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

        public IPRanges Split(IPAddress splitAt)
        {
            // Convert the addresses to UInt32
            var start = Start.ToUInt32();
            var end = End.ToUInt32();
            var at = splitAt.ToUInt32();

            if (
                (end - start) == 1 || 
                at <= start || 
                at > end
                )
            {
                return new IPRanges
                {
                    new IPRange
                    {
                        Start = Start,
                        End = End
                    }
                };
            }

            return new IPRanges( 
                new List<IPRange>
                {
                    new IPRange
                    {
                        Start = Start,
                        End = (at - 1).ToIPAddress()
                    },
                    new IPRange
                    {
                        Start = at.ToIPAddress(),
                        End = End
                    }
                },
                false
            );
        }

        public IPRange CombineWith(IPRange other)
        {
            if (!Intersects(other) && !Borders(other))
                throw new ArgumentException("Can't combine two ranges which do not intersect or touch");

            return new IPRange
            {
                Start = Start.Min(other.Start),
                End = End.Max(other.End)
            };
        }

        public long Count
        {
            get
            {
                return Convert.ToInt64(End.ToUInt32()) - Convert.ToInt64(Start.ToUInt32()) + 1;
            }
        }

        public IPRanges Subtract(IPRange other)
        {
            // If the other range doesn't overlap this range, then just return this range
            if (!Intersects(other))
            {
                return new IPRanges
                {
                    this
                };
            }

            var start = Start.ToUInt32();
            var end = End.ToUInt32();
            var otherStart = other.Start.ToUInt32();
            var otherEnd = other.End.ToUInt32();

            // If the other range eclipses this range, then return an empty list
            if (
                (otherStart <= start) && 
                (otherEnd >= end)
            )
                return new IPRanges();

            // If the other range overlaps the start of this range, then clip the start
            if (
                (otherStart <= start) &&
                (otherEnd >= start)
            )
            {
                return new IPRanges {
                    new IPRange
                    {
                        Start = (otherEnd + 1).ToIPAddress(),
                        End = End
                    }
                };
            }

            // If the other range overlaps the end of this range, then clip the end
            if (
                (otherStart <= end) &&
                (otherEnd >= end)
            )
            {
                return new IPRanges
                {
                    new IPRange
                    {
                        Start = Start,
                        End = (otherStart - 1).ToIPAddress()
                    }
                };
            }

            // If the other range is in the middle of this range, then create two new ranges as a result
            return new IPRanges
            {
                new IPRange
                {
                    Start = Start,
                    End = (otherStart - 1).ToIPAddress()
                },
                new IPRange
                {
                    Start = (otherEnd + 1).ToIPAddress(),
                    End = End
                }
            };
        }

        public bool ComesBefore(IPRange other)
        {
            return other.Start.GreaterThan(End);
        }

        public bool ComesAfter(IPRange other)
        {
            return Start.GreaterThan(other.End);
        }

        public bool Eclipses(IPRange other)
        {
            return (
                Start.LessThanOrEqual(other.Start) &&
                End.GreaterThanOrEqual(other.End)
            );
        }

        public IEnumerator<IPAddress> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public int CompareTo(IPRange other)
        {
            if (Equals(other))
                return 0;

            if (Start.LessThan(other.Start))
                return -1;
            else if (Start.GreaterThan(other.Start))
                return 1;

            return -1;
        }
    }
}
