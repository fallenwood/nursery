using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

await Parent();

async Task EchoServer(Stream stream, Counter counter) {
  var ident = counter.Next();
  Console.WriteLine($"echo_server {ident}: started");

  try {
    var reader = PipeReader.Create(stream);
    while (true) {
      var result = await reader.ReadAsync();
      var readBuffer = result.Buffer;
      var length = (int)readBuffer.Length;
      if (length == 0 && result.IsCompleted) {
        break;
      }

      var message = Encoding.UTF8.GetString(readBuffer.ToArray());
      Console.WriteLine($"receiver: received {message}");

      reader.AdvanceTo(readBuffer.End);
    }
  } catch (Exception ex) {
    Console.WriteLine($"echo_server {ident}: crashed: {ex}");
  }
}

async Task Parent() {
  const int PORT = 12345;

  var listener = new System.Net.Sockets.TcpListener(new System.Net.IPAddress([0, 0, 0, 0]), PORT);
  listener.Start();

  var counter = new Counter();

  await using var nursery = new Nursery.Nursery();

  while (true) {
    var client = await listener.AcceptTcpClientAsync();
    var stream = client.GetStream();

    nursery.StartSoon(async _ => await EchoServer(stream, counter));
  }
}

sealed file class Counter {
  private int count = 0;

  public int Next() {
    return Interlocked.Increment(ref count);
  }
}
