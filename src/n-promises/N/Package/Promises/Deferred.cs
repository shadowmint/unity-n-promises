using System;
using System.Threading.Tasks;

namespace N.Package.Promises
{
  public class Deferred : Deferred<bool>
  {
    public new Task Task => InnerTask;

    public void Resolve()
    {
      base.Resolve(true);
    }
  }

  public class Deferred<TResult>
  {
    private readonly TaskCompletionSource<TResult> _taskSource;

    public Task<TResult> Task => InnerTask;

    protected Task<TResult> InnerTask { get; }

    public Deferred()
    {
      _taskSource = new TaskCompletionSource<TResult>();
      InnerTask = _taskSource.Task;
    }

    public void Resolve(TResult value)
    {
      _taskSource.TrySetResult(value);
    }

    public void Reject(Exception error)
    {
      _taskSource.TrySetException(error);
    }
  }
}