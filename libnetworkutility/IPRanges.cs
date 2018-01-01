using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace libnetworkutility
{
    public class IPRanges : List<IPRange>, IComparable<IPRanges>
    {
        public class AddressEnumerator : IEnumerator<IPAddress>
        {
            private IPRanges Parent { get; set; }
            private int Range { get; set; } = 0;
            private IEnumerator<IPAddress> IPEnumerator { get; set; }
            private int Hash { get; set; }

            internal AddressEnumerator(IPRanges parent)
            {
                Parent = parent;
                Hash = parent.GetHashCode();

                IPEnumerator = Parent[Range].GetEnumerator();
            }

            public IPAddress Current
            {
                get
                {
                    if (Hash != Parent.GetHashCode())
                        throw new InvalidOperationException("The collection was modified after the enumerator was created.");

                    return IPEnumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    // TODO : Figure out how to trigger this from a unit test
                    if (Hash != Parent.GetHashCode())
                        throw new InvalidOperationException("The collection was modified after the enumerator was created.");

                    return IPEnumerator.Current;
                }
            }

            public void Dispose()
            {
                IPEnumerator.Dispose();
                IPEnumerator = null;
            }

            public bool MoveNext()
            {
                if (Hash != Parent.GetHashCode())
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");

                if(!IPEnumerator.MoveNext())
                {
                    if (Range >= (Parent.Count - 1))
                        return false;

                    Range++;

                    IPEnumerator.Dispose();
                    IPEnumerator = Parent[Range].GetEnumerator();
                }

                return true;
            }

            public void Reset()
            {
                if (Hash != Parent.GetHashCode())
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");

                Dispose();

                Range = 0;
                IPEnumerator = Parent[Range].GetEnumerator();
            }
        }

        public IPRanges() : base()
        {
        }

        public IPRanges(List<IPRange>ranges, bool normalize)
        {
            if (normalize)
            {
                foreach (var item in ranges)
                    Add(item);
            }
            else
                base.AddRange(ranges);
        }

        /// <summary>
        /// Insert an IP address to the range list
        /// </summary>
        /// <param name="address">The address to add</param>
        /// <remarks>
        /// The result of the output of this is normalized
        /// </remarks>
        public void Add(IPAddress address)
        {
            var addressInt = address.ToUInt32();

            for(var i=0; i<Count; i++)
            {
                var range = this[i];
                var start = range.Start.ToUInt32();
                var end = range.End.ToUInt32();

                // If the address is already in the list, do nothing
                if (addressInt >= start && addressInt <= end)
                    return;

                // If the address is at the start of a range, then  extend the start of the range forward by one
                if(addressInt == (start - 1))
                {
                    range.Start = address;
                    return;
                }

                // If the address can extend the end of a range, extend the current range and combine with the next
                // if appropriate
                if(addressInt == (end + 1))
                {
                    if(i < (Count - 1))
                    {
                        var nextRange = this[i + 1];
                        var nextStart = nextRange.Start.ToUInt32();
                        if(addressInt == (nextStart - 1))
                        {
                            range.End = nextRange.End;
                            base.Remove(nextRange);
                            return;
                        }
                    }

                    range.End = address;
                    return;
                }

                // If the address is less than the start of the next range, this is the insert point
                if(addressInt < start)
                {
                    Insert(
                        i, 
                        new IPRange
                        {
                            Start = address,
                            End = address
                        }
                    );
                    return;
                }
            }

            // Didn't find where to stick it, so append it
            Add(new IPRange
            {
                Start = address,
                End = address
            });
        }

        /// <summary>
        /// Remove an address from the range list
        /// </summary>
        /// <param name="address">The address to remove</param>
        /// <remarks>
        /// The range containing the value to remove is split into two if necessary. Massive
        /// fragmentation can have negative performance consequences.
        /// </remarks>
        public void Remove(IPAddress address)
        {
            for(var i=0; i<Count; i++)
            {
                var range = this[i];
                if(range.Contains(address))
                {
                    var start = range.Start.ToUInt32();
                    var end = range.End.ToUInt32();
                    var addressInt = address.ToUInt32();

                    // If the range contains one address, remove the range
                    if(addressInt == start && addressInt == end)
                    {
                        base.Remove(range);
                        return;
                    }

                    // If the address is at the start of the range, increase the start
                    if(addressInt == start)
                    {
                        range.Start = (start + 1).ToIPAddress();
                        return;
                    }

                    // If the address is at the end of the range, decrease the end
                    if (addressInt == end)
                    {
                        range.End = (end - 1).ToIPAddress();
                        return;
                    }

                    // Split the range at the given address while removing the address
                    Insert(
                        i,
                        new IPRange
                        {
                            Start = range.Start,
                            End = (addressInt - 1).ToIPAddress()
                        }
                    );
                    range.Start = (addressInt + 1).ToIPAddress();
                }
            }
        }

        public new void Add(IPRange range)
        {
            // Find all ranges in the current list which intersect or border with the range
            var intersecting =
                this.Where(x =>
                    range.Intersects(x) ||
                    range.Borders(x)
                )
                .ToList();

            // Remove those items from the list and update the range to add to contain the combined ranges
            foreach(var item in intersecting)
            {
                base.Remove(item);
                range = range.CombineWith(item);
            }

            // At this point, there should be no intersecting ranges left and the ranges should contain all
            // addresses where the intersections and the new range overlap.
            for (var i=0; i<Count; i++)
            {
                var current = this[i];

                // If the current range comes after the given range, then insert the given range
                if(current.Start.GreaterThan(range.End))
                {
                    Insert(i, range);
                    return;
                }
            }

            // If this point is reached, then the new range comes after all the rest.
            base.Add(range);
        }

        public new void Remove(IPRange range)
        {
            // Search for all intersecting items 
            var intersecting =
                this.Where(x =>
                    range.Intersects(x)
                )
                .ToList();

            // Remove all the intersected items
            foreach(var item in intersecting)
                base.Remove(item);
             
            // Find out what needs to be added after subtracting everything
            // TODO : Might be faster to just take the first and last item from the list
            var toAdd = intersecting
                .SelectMany(x =>
                    x.Subtract(range)
                )
                .ToList();

            // Add the items which are left. There should never be more than two
            foreach (var item in toAdd)
                Add(item);
        }

        public IPRanges Clone()
        {
            return new IPRanges(this.Select(x => x.Clone()).ToList(), false);
        }

        public IPRanges Minus(IPRanges ranges)
        {
            var result = Clone();
            result.Remove(ranges);
            return result;
        }

        public void Add(IPRanges ranges)
        {
            foreach (var item in ranges)
                Add(item);
        }

        public void Remove(IPRanges ranges)
        {
            foreach (var item in ranges)
                Remove(item);
        }

        public override bool Equals(object obj)
        {
            if(obj is IPRanges)
            {
                var ranges = (IPRanges)obj;
                if (ranges.Count != Count)
                    return false;

                for(var i=0; i<Count; i++)
                {
                    if (!ranges[i].Equals(this[i]))
                        return false;
                }

                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Join(",", this.Select(x => x.ToString()));
        }

        public override int GetHashCode()
        {
            int result = 0x55555555;        // Seed the result

            foreach (var range in this)
                result ^= range.GetHashCode();

            return result;
        }

        public int CompareTo(IPRanges other)
        {
            return Equals(other) ? 0 : -1;
        }

        public IEnumerator<IPAddress> GetAddressEnumerator()
        {
            return new AddressEnumerator(this);
        }

        public bool Contains(IPAddress address)
        {
            foreach(var range in this)
            {
                if (range.Contains(address))
                    return true;
            }
            return false;
        }
    }
}
