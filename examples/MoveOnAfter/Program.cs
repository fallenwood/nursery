using System;
using Nursery;

await using (var nursery = Nursery.Nursery.MoveOnAfter(TimeSpan.FromSeconds(3))) {
  nursery.StartSoon(async _ => {
    Console.WriteLine("  task1: started! sleeping for 5 seconds...");
    await nursery.Sleep(TimeSpan.FromSeconds(5));
    Console.WriteLine("  task1: exiting!");
  });

  nursery.StartSoon(async _ => {
    Console.WriteLine("  task2: started! sleeping for 2 seconds...");
    await nursery.Sleep(TimeSpan.FromSeconds(2));
    Console.WriteLine("  task2: exiting!");
  });
}

Console.WriteLine("parent: waiting for children to finish...");
