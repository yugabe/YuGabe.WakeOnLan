if (args.Length == 1 && args[0] is var mac && (mac = new(mac.ToUpper().Where("0123456789ABCDEF".Contains).ToArray())).Length == 12)
{
    using var client = new System.Net.Sockets.UdpClient("255.255.255.255", 12287) { EnableBroadcast = true };
    await client.SendAsync(Enumerable.Repeat<byte>(255, 6).Concat(Enumerable.Repeat(Enumerable.Range(0, 6).Select(i => Convert.ToByte(mac.Substring(i * 2, 2), 16)), 16).SelectMany(b => b)).ToArray(), 102);
    Console.WriteLine($"Wake-on-LAN magic packet broadcast to MAC address {string.Join(':', mac.Chunk(2).Select(c => new string(c)))}.");
    return;
}

Console.WriteLine(
@"wol - Wake-on-LAN dotnet tool
Usage:
  wol <target MAC address>
      <target MAC address> is a string containing exactly 12 hexadecimal characters (0-9, A-F, case insensitive) and any additional non-whitespace delimiters (':', '-', '_' etc.).

Examples:
  wol 12:34:56:78:9A:BC
  wol 12345_6789!aBCdE*f
  wol 123456789ABCDEF

Remarks:
  For Wake-on-LAN to work, make sure that:
    - the current machine and the target machine are on the same local subnet, 
    - the UDP port 12287 or UDP broadcasting is not blocked any firewalls on the network (including Windows Firewall and routers/switches),
    - Wake-on-LAN is supported by the target machine, as not all motherboards/Ethernet cards support it, and some only when the computer is sleeping,
    - Wake-on-LAN is enabled by the target machine (usually in UEFI/BIOS).");
