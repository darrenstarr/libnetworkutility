Function Get-NetworkPrefixParts
{
    Param(
        [string]$Prefix
    )

    [string[]]$parts = $Prefix.Split('/')

    if(
        ($null -eq $parts) -or 
        ($parts.Count -ne 2)
    ) {
        throw [System.ArgumentException]::new('Prefix must be formatted as x.x.x.x/y')
    }

    $ip = [System.Net.IPAddress]::None
    if(-not ([System.Net.IPAddress]::TryParse($parts[0], [ref]$ip))) {
        throw [System.ArgumentException]::new('Prefix must be formatted as x.x.x.x/y')
    }

    $length = 0
    try {
        $length = [Convert]::ToInt32($parts[1])
    } catch {
        throw [System.ArgumentException]::new('Prefix must be formatted as x.x.x.x/y')
    }

    return [PSCustomObject]@{
        Network = $ip
        Length = $length
    }
}

Function Get-IPAsUInt32
{
    Param(
        [string]$address
    )

    $x = [IPAddress]::Parse($address).GetAddressBytes() | ForEach-Object { [Convert]::ToUInt32($_) }
    
    return (
        ($x[0] -shl 24) -bor
        ($x[1] -shl 16) -bor
        ($x[2] -shl 8) -bor
        $x[3]
    )
}

Function Is-RouteMatch
{
    Param(
        [string]$NetworkPrefix,
        [int]$PrefixLength,
        [string]$Destination
    )

    [UInt32]$prefixUInt32 = Get-IPAsUInt32 -address $NetworkPrefix
    [UInt32]$destinationUInt32 = Get-IPAsUInt32 -address $Destination

    [uint32]$mask = [Convert]::ToUInt32(([Convert]::ToUInt64([UInt32]::MaxValue) -shl (32 - $PrefixLength)) -band [UInt32]::MaxValue) 

    return ($prefixUInt32 -band $mask) -eq ($destinationUInt32 -band $mask)
}

Function Get-OutgoingInterface
{
    Param(
        [string]$DestinationAddress
    )

    $rawRoutes = Get-NetRoute -AddressFamily IPv4
    [PSCustomObject[]]$normalizedRoutes = $rawRoutes | ForEach-Object {
        $parts = Get-NetworkPrefixParts -Prefix $_.DestinationPrefix

        [PSCustomObject]@{
            DestinationNetwork = $parts.Network
            PrefixLength = $parts.Length
            Metric = $_.RouteMetric
            ifIndex = $_.ifIndex
        }
    } | Where-Object { 
        Is-RouteMatch -NetworkPrefix $_.DestinationNetwork -PrefixLength $_.PrefixLength -Destination $DestinationAddress 
    } | Sort -Descending PrefixLength,Metric

    if(($null -eq $normalizedRoutes) -or $normalizedRoutes.Count -eq 0) {
        return $null
    }

    $addresses = Get-NetIPAddress -AddressFamily IPv4 -InterfaceIndex $normalizedRoutes[0].ifIndex

    if($null -eq $addresses) {
        return $null
    }

    $addresses[0].IPAddress
}

Get-OutgoingInterface -DestinationAddress '8.8.8.8' | ft
