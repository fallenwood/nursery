using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

var hostname = "bing.com";
var port = 443;

var endpoint = await HappyEyeballsAsync(hostname, port);

Console.WriteLine($"Connected to {hostname}:{port} with {((IPEndPoint)endpoint!).Address}");

async Task<EndPoint?> HappyEyeballsAsync(string hostname, int port) {
  Socket? result = null;
  async Task Try(AddressFamily addressFamily, string host, int port, Nursery.Nursery nursery) {
    var socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);

    await socket.ConnectAsync(host, port, nursery.CancelToken);

    nursery.Cancel();

    result = socket;
  }

  var addresses = await Dns.GetHostAddressesAsync(hostname);
  await using (var nursery = new Nursery.Nursery()) {
    foreach (var address in addresses) {
      nursery.StartSoon(async token => await Try(address.AddressFamily, hostname, port, nursery));
    }
  }

  return result?.RemoteEndPoint;
}
