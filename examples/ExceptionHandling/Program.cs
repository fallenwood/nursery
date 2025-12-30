using System;
using System.Threading;
using System.Threading.Tasks;

await ParentAsync();

async Task Child1Async(CancellationToken token) {
  Console.WriteLine("  child1: started! sleeping now...");
  await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
  Console.WriteLine("  child1: exiting!");
}

async Task Child2Async(CancellationToken token) {
  Console.WriteLine("  child2: started! sleeping now...");
  await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
  Console.WriteLine("  child2: exiting!");
}

async Task Child3WithExceptionAsync(CancellationToken token) {
  Console.WriteLine("  child3: started! about to throw...");
  await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
  Console.WriteLine("  child3: throwing!");
  throw new InvalidOperationException("child3: something went wrong!");
}

async Task ParentAsync() {
  Console.WriteLine("parent: started!");

  try {
    await using (var nursery = new Nursery.Nursery()) {
      Console.WriteLine("parent: spawning child1...");
      nursery.StartSoon(Child1Async);

      Console.WriteLine("parent: spawning child2...");
      nursery.StartSoon(Child2Async);

      Console.WriteLine("parent: spawning child3...");
      nursery.StartSoon(Child3WithExceptionAsync);

      Console.WriteLine("parent: waiting for children to finish...");
    }
  } catch (InvalidOperationException ex) {
    Console.WriteLine($"parent: caught exception from child: {ex.Message}");
  }

  Console.WriteLine("parent: all done!");
}
