using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace libnetworkutility
{
    public class IPAddressPool
    {
        IPAddress FirstAddress { get; set; }

        AdvancedBitArray ReservedAddresses { get; set; }

        public IPRange Range
        {
            get
            {
                return new IPRange
                {
                    Start = FirstAddress,
                    End = FirstAddress.Offset(ReservedAddresses.Count - 1)
                };
            }
        }

        public long Available
        {
            get
            {
                return ReservedAddresses.BitsUnset;
            }
        }

        public IPAddressPool(NetworkPrefix prefix)
        {
            FirstAddress = prefix.BaseNetwork.Network;
            ReservedAddresses = new AdvancedBitArray(prefix.TotalAddresses);
        }

        public IPAddressPool(IPAddress firstAddress, int poolSize)
        {
            FirstAddress = firstAddress;
            ReservedAddresses = new AdvancedBitArray(poolSize);
        }

        private long BitIndex(IPAddress address)
        {
            return address.Minus(FirstAddress);
        }

        public void Reserve(IPAddress address)
        {
            lock (ReservedAddresses)
            {
                var index = BitIndex(address);
                if (index < 0 || index > ReservedAddresses.Count)
                    throw new ArgumentOutOfRangeException("Address is out of the scope of the pool");

                if (ReservedAddresses[index])
                    throw new ArgumentException("Address is already reserved");

                ReservedAddresses[index] = true;
            }
        }

        public void Reserve(IPRange range)
        {
            if(!Range.Eclipses(range))
                throw new ArgumentOutOfRangeException("Range is out of the scope of the pool");

            lock (ReservedAddresses)
            {
                var index = BitIndex(range.Start);

                for (var i = 0; i < range.Count; i++)
                {
                    if (ReservedAddresses[index + i])
                        throw new ArgumentException("Address is already reserved : " + range.Start.Offset(i).ToString());
                }

                for (var i = 0; i < range.Count; i++)
                    ReservedAddresses[index + i] = true;
            }
        }

        public void Reserve(IPRanges ranges)
        {
            foreach(var range in ranges)
            {
                if(!Range.Eclipses(range))
                    throw new ArgumentOutOfRangeException("Range is out of the scope of the pool : " + range.ToString());
            }

            lock (ReservedAddresses)
            {
                foreach (var range in ranges)
                {
                    var index = BitIndex(range.Start);

                    for (var i = 0; i < range.Count; i++)
                    {
                        if (ReservedAddresses[index + i])
                            throw new ArgumentException("Address is already reserved : " + range.Start.Offset(i).ToString());
                    }
                }

                foreach (var range in ranges)
                {
                    var index = BitIndex(range.Start);

                    for (var i = 0; i < range.Count; i++)
                        ReservedAddresses[index + i] = true;
                }
            }
        }

        public void Unreserve(IPAddress address)
        {
            lock (ReservedAddresses)
            {
                var index = BitIndex(address);
                if (index < 0 || index > ReservedAddresses.Count)
                    throw new ArgumentOutOfRangeException("Address is out of the scope of the pool");

                if (!ReservedAddresses[index])
                    throw new ArgumentException("Address is already unreserved");

                ReservedAddresses[index] = false;
            }
        }

        public void Unreserve(IPRange range)
        {
            if (!Range.Eclipses(range))
                throw new ArgumentOutOfRangeException("Range is out of the scope of the pool");

            lock (ReservedAddresses)
            {
                var index = BitIndex(range.Start);

                for (var i = 0; i < range.Count; i++)
                {
                    if (!ReservedAddresses[index + i])
                        throw new ArgumentException("Address is already unreserved : " + range.Start.Offset(i).ToString());
                }

                for (var i = 0; i < range.Count; i++)
                    ReservedAddresses[index + i] = false;
            }
        }

        public void Unreserve(IPRanges ranges)
        {
            foreach (var range in ranges)
            {
                if (!Range.Eclipses(range))
                    throw new ArgumentOutOfRangeException("Range is out of the scope of the pool : " + range.ToString());
            }

            lock (ReservedAddresses)
            {
                foreach (var range in ranges)
                {
                    var index = BitIndex(range.Start);

                    for (var i = 0; i < range.Count; i++)
                    {
                        if (!ReservedAddresses[index + i])
                            throw new ArgumentException("Address is already unreserved : " + range.Start.Offset(i).ToString());
                    }
                }

                foreach (var range in ranges)
                {
                    var index = BitIndex(range.Start);

                    for (var i = 0; i < range.Count; i++)
                        ReservedAddresses[index + i] = false;
                }
            }
        }

        public bool this[IPAddress key]
        {
            get
            {
                lock (ReservedAddresses)
                {
                    var index = BitIndex(key);
                    if (index < 0 || index > ReservedAddresses.Count)
                        throw new ArgumentOutOfRangeException("Address is out of the scope of the pool");

                    return ReservedAddresses[index];
                }
            }
            set
            {
                if (value)
                    Reserve(key);
                else
                    Unreserve(key);
            }
        }

        public IPAddress ReserveNextAddress()
        {
            lock (ReservedAddresses)
            {
                if (ReservedAddresses.BitsUnset == 0)
                    return null;

                var index = ReservedAddresses.FindFirst(false);
                if(index >= 0)
                {
                    ReservedAddresses[index] = true;
                    return FirstAddress.Offset(index);
                }
            }

            return null;
        }
    }
}
