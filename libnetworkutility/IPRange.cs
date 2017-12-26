using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace libnetworkutility
{
    public class IPRange
    {
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
    }
}
