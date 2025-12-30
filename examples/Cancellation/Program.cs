using System;
using System.Threading.Tasks;
using Nursery;

Console.WriteLine("===========");
Console.WriteLine("Example 1");

Console.WriteLine("starting...");
try {
  await using (var nursery5 = Nursery.Nursery.MoveOnAfter(TimeSpan.FromSeconds(5))) {
    await using (var nursery10 = Nursery.Nursery.MoveOnAfter(nursery5, TimeSpan.FromSeconds(10))) {
      await nursery10.Sleep(TimeSpan.FromSeconds(20));
      Console.WriteLine("sleep finished without error");
    }
    Console.WriteLine("move_on_after(10) finished without error");
  }
  Console.WriteLine("move_on_after(5) finished without error");
} catch (OperationCanceledException) {
  Console.WriteLine("caught OperationCanceledException as expected");
}

Console.WriteLine("===========");
Console.WriteLine("Example 2");

await Main();

async Task Sleeper(Nursery.Nursery nursery) {
  Console.WriteLine("sleeper: starting 20s sleep");
  await nursery.Sleep(TimeSpan.FromSeconds(20));
  Console.WriteLine("sleeper: finished 20s sleep");
}

async Task Main() {
  await using var nursery = new Nursery.Nursery();
  nursery.StartSoon(_ => Sleeper(nursery));
  await nursery.Sleep(TimeSpan.FromSeconds(2));
  nursery.Cancel();
  Console.WriteLine("main: cancelled nursery");
}
