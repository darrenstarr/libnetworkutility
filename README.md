# libnetworkutility

## Introduction
This is a library of common functions for developing network related code.

## Classes

### IPAddressExtensions

Not really a class as opposed to extension methods for System.Net.IPAddress. These extensions :

* Simplify converting to and from UInt32 (IPAddress.ToUInt32(), uint32.ToIPAddress())
* Comparing IP addresses against each other numerically (IPAddress.GreatThan(), IPAddress.LessThan(), etc)
* Creating hexadecimal strings from addresses (IPAddress.ToHexString())
* Gets the source IP to use when contacting a destination address (IPAddress.GetSourceIP())

### IPRange

Represents a range of IP addresses from .Start to .End. This class includes 

* Range operations (Subtract, CombineWith, etc)
* Comparisons (Contains, ComesBefore, ComesAfter, etc)
* Utility (Split)

### IPRanges

Represents an ordered list of IP ranges. This class normalizes/optimizes Adds and Removes to reduce the number of items in the list.