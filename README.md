# WakeOnLan / "wol"

A no-nonsense, simple Wake-on-LAN implementation, that can be invoked via the command 'wol'.

## Usage
```
wol <target MAC address>
```

`<target MAC address>` is a string containing exactly 12 hexadecimal characters (0-9, A-F, case insensitive) and any additional non-whitespace delimiters (':', '-', '_' etc.).

## Examples

`wol 12:34:56:78:9A:BC`

`wol 12345_6789!aBCdE*f`

`wol 123456789ABCDEF`

## Remarks
For Wake-on-LAN to work, make sure that:
- the current machine and the target machine are on the same local subnet, 
  - this is tantamount, as there are certain features that might make you use a different subnet even if you don't realize (Windows' "Projecting to this PC", for example, creates a virtualized Wi-Fi Direct adapter on a different subnet); there's no plans currently to support choosing which adapter to use specifically, but it would be possible to set the IP address to send the packet from (thus, indirectly, choosing the adapter to use) -- feel free to send a PR if you want to add this to the client,
- the UDP port 12287 or UDP broadcasting is not blocked any firewalls on the network (including Windows Firewall and routers/switches),
- Wake-on-LAN is supported by the target machine, as not all motherboards/Ethernet cards support it, and some only when the computer is sleeping,
- Wake-on-LAN is enabled by the target machine (usually in UEFI/BIOS).

## Notes regarding the source
The source code is essentially the following two lines of code:

```csharp
using var client = new System.Net.Sockets.UdpClient("255.255.255.255", 12287) { EnableBroadcast = true };
await client.SendAsync(Enumerable.Repeat<byte>(255, 6).Concat(Enumerable.Repeat(Enumerable.Range(0, 6).Select(i => Convert.ToByte(mac.Substring(i * 2, 2), 16)), 16).SelectMany(b => b)).ToArray(), 102);
```

In plain words, the 6 bytes `FF FF FF FF FF FF` are followed by the target MAC address 16 times, and sent to the UDP broadcast address on port 12287.

This is preceded by validation, and the documentation if wrong input is detected.
