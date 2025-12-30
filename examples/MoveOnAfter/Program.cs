using System;
using Nursery;

await using (var nursery = Nursery.Nursery.MoveOnAfter(TimeSpan.FromSeconds(3))) {
  nursery.StartSoon(async token => {
    Console.WriteLine("  task1: started! sleeping for 5 seconds...");
    await NurseryExtensions.Sleep(TimeSpan.FromSeconds(5), token);
    Console.WriteLine("  task1: exiting!");
  });

  nursery.StartSoon(async token => {
    Console.WriteLine("  task2: started! sleeping for 2 seconds...");
    await NurseryExtensions.Sleep(TimeSpan.FromSeconds(2), token);
    Console.WriteLine("  task2: exiting!");
  });
}

Console.WriteLine("parent: waiting for children to finish...");
