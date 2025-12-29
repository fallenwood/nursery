using System;
using System.Threading.Tasks;

await ParentAsync();

async Task Child1Async() {
  Console.WriteLine("  child1: started! sleeping now...");
  await Task.Delay(TimeSpan.FromSeconds(1));
  Console.WriteLine("  child1: exiting!");
}

async Task Child2Async() {
  Console.WriteLine("  child2: started! sleeping now...");
  await Task.Delay(TimeSpan.FromSeconds(1));
  Console.WriteLine("  child2: exiting!");
}

async Task ParentAsync() {
  Console.WriteLine("parent: started!");

  await using (var nursery = new Nursery.Nursery()) {
    Console.WriteLine("parent: spawning child1...");
    nursery.StartSoon(async (_) => await Child1Async());
    Console.WriteLine("parent: spawning child2...");

    nursery.StartSoon(async _ => await Child2Async());
    Console.WriteLine("parent: waiting for children to finish...");
  }

  Console.WriteLine("parent: all done!");
}
