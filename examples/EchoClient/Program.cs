using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

await ParentAsync();

async Task SenderAsync(Stream stream) {
  Console.WriteLine("sender: started!");

  var writer = PipeWriter.Create(stream);
  while (true) {
    var data = "async can sometimes be confusing, but I believe in you!";
    var bytes = Encoding.UTF8.GetBytes(data);

    Console.WriteLine($"sender: sending {data}");

    await writer.WriteAsync(bytes);
    await Task.Delay(TimeSpan.FromSeconds(1));
  }
}

async Task ReceiverAsync(Stream stream) {
  Console.WriteLine("receiver: started!");

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

  Environment.Exit(0);
}

async Task ParentAsync() {
  const int PORT = 12345;

  Console.WriteLine($"parent: connecting to 127.0.0.1:{PORT}");

  using var client = new System.Net.Sockets.TcpClient();
  await client.ConnectAsync(new System.Net.IPAddress([127, 0, 0, 1]), PORT);
  using var stream = client.GetStream();

  await using var nursery = new Nursery.Nursery();
  Console.WriteLine("parent: spawning sender...");
  nursery.StartSoon(async (_) => await SenderAsync(stream));

  Console.WriteLine("parent: spawning receiver...");
  nursery.StartSoon(async (_) => await ReceiverAsync(stream));
}
