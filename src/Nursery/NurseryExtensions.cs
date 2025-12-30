namespace Nursery;

using System;
using System.Threading;
using System.Threading.Tasks;

public static class NurseryExtensions {
  extension(Nursery) {
    public static Task Sleep(Nursery nursery, int milliseconds, CancellationToken token = default) => Task.Delay(milliseconds, token);
    public static Task Sleep(Nursery nursery, TimeSpan duration, CancellationToken token = default) => Task.Delay(duration, token);

    public static Nursery MoveOnAfter(TimeSpan timeout) {
      var nursery = new Nursery();
      nursery.CancelAfter(timeout);
      return nursery;
    }

    public static Nursery MoveOnAfter(Nursery parnetNursery, TimeSpan timeout) {
      var nursery = new Nursery(parnetNursery);
      nursery.CancelAfter(timeout);
      return nursery;
    }

    public static async Task FailAfter(TimeSpan timeout, Func<CancellationToken, Task> action) {
      using var cts = new CancellationTokenSource(timeout);
      try {
        await action(cts.Token);
      } catch (OperationCanceledException) when (cts.Token.IsCancellationRequested) {
        throw new TimeoutException($"Operation timed out after {timeout}");
      }
    }
  }
}
