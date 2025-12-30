namespace Nursery;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed class Nursery(int capacity, CancellationToken cancellationToken)
    : IAsyncDisposable {
  private readonly List<Task> tasks = new(capacity: capacity);
  private readonly CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
  private readonly Lock @lock = new();

  public Nursery()
      : this(capacity: 0, default) { }
  
  public Nursery(Nursery parentNursery)
      : this(capacity: 0, parentNursery.CancelToken) { }

  public CancellationToken CancelToken => cts.Token;

  public void StartSoon(Func<CancellationToken, Task> action) {
    lock (@lock) {
      var task = Task.Run(async () => {
        try {
          await action(cts.Token);
        } catch (TaskCanceledException) {
        } catch (OperationCanceledException) {
        } catch (Exception) {
          cts.Cancel();
          throw;
        }
      }, cts.Token);

      tasks.Add(task);
    }
  }

  public void StartSoon(Func<Task> action) => StartSoon(_ => action());

  public async Task WaitAllAsync() {
    while (true) {
      Task[] tasksToWait;
      lock (@lock) {
        if (tasks.All(t => t.IsCompleted)) {
          break;
        }

        tasksToWait = [.. tasks];
      }

      if (tasksToWait.Length > 0) {
        await Task.WhenAll(tasksToWait);
      } else {
        break;
      }
    }
  }

  public void Cancel() => cts.Cancel();

  public void CancelAfter(TimeSpan timeout) => cts.CancelAfter(timeout);

  public void CancelAfter(int millisecondsDelay) => cts.CancelAfter(millisecondsDelay);

  public async ValueTask DisposeAsync() {
    try {
      await Task.WhenAll(tasks);
    } catch {
      cts.Cancel();
      throw;
    } finally {
      cts.Dispose();
    }
  }
}
