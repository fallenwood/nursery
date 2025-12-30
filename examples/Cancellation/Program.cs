using System;
using Nursery;

Action<string> print = Console.WriteLine;

print("starting...");
await using (var nursery5 = Nursery.Nursery.MoveOnAfter(TimeSpan.FromSeconds(5))) {
  await using (var nursery10 = Nursery.Nursery.MoveOnAfter(nursery5, TimeSpan.FromSeconds(10))) {
    await NurseryExtensions.Sleep(TimeSpan.FromSeconds(20), nursery10.CancelToken);
    print("sleep finished without error");
  }
  print("move_on_after(10) finished without error");
}
print("move_on_after(5) finished without error");
