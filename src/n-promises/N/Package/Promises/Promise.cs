using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using N.Package.Promises.Errors;
using N.Package.Promises.Infrastructure;
using UnityEngine;

namespace N.Package.Promises
{
  public class Promise : Promise<bool>
  {
    private readonly TaskCompletionSource<bool> _deferred;

    private readonly Task _task;

    public Promise(Task task) : base(null)
    {
      _task = task;
      _deferred = new TaskCompletionSource<bool>();
      Task = _deferred.Task;
    }

    public Promise Then(Action onSuccess, Action<Exception> onError = null)
    {
      base.Then((_) => onSuccess(), onError);
      return this;
    }

    public override bool Check()
    {
      if (State != PromiseState.Pending) return true;

      if (_task == null)
      {
        Failed(new Exception("Task was NULL for Promise"));
        return true;
      }

      if (!_task.IsCompleted) return false;

      if (_task.IsFaulted)
      {
        Failed(_task.Exception);
        return true;
      }

      if (_task.IsCanceled)
      {
        Failed(new TaskCancelledException());
      }

      Success();
      return true;
    }

    protected override void Success()
    {
      _deferred.SetResult(true);
      base.Success();
    }

    protected override void Failed(Exception exception)
    {
      _deferred.SetException(exception);
      base.Failed(exception);
    }
  }

  public class Promise<TResult> : IPromise
  {
    protected PromiseState State = PromiseState.Pending;

    protected Task<TResult> Task;

    private readonly List<Action<TResult>> _success = new List<Action<TResult>>();

    private readonly List<Action<Exception>> _errors = new List<Action<Exception>>();

    public Promise(Task<TResult> task)
    {
      Task = task;
    }

    public Promise<TResult> Then(Action<TResult> onSuccess, Action<Exception> onError = null)
    {
      if (State != PromiseState.Pending) return this;

      if (onSuccess != null)
      {
        _success.Add(onSuccess);
      }

      if (onError != null)
      {
        _errors.Add(onError);
      }

      return this;
    }

    public virtual bool Check()
    {
      if (State != PromiseState.Pending) return true;

      if (Task == null)
      {
        Failed(new Exception("Task was NULL for Promise"));
        return true;
      }

      if (!Task.IsCompleted) return false;

      if (Task.IsFaulted)
      {
        Failed();
        return true;
      }

      if (Task.IsCanceled)
      {
        Failed(new TaskCancelledException());
      }

      Success();
      return true;
    }

    [Obsolete]
    public Promise<TResult> ResolveWhenDone()
    {
      var worker = PromiseWorker.Get();
      worker.Register(this);
      return this;
    }

    public Promise<TResult> Dispatch()
    {
      var worker = PromiseWorker.Get();
      worker.Register(this);
      return this;
    }

    protected virtual void Failed(Exception exception = null)
    {
      State = PromiseState.Rejected;
      _errors.ForEach(i => Safe(i, exception ?? Task.Exception));
      Clear();
    }

    protected virtual void Success()
    {
      State = PromiseState.Resolved;
      _success.ForEach(i => Safe(i, Task.Result));
      Clear();
    }

    private void Safe<T>(Action<T> action, T value)
    {
      try
      {
        action?.Invoke(value);
      }
      catch (Exception error)
      {
        // Pass tests, but still log in play mode in the editor.
        var runningTests = Application.isEditor && !Application.isPlaying;
        if (!runningTests) {
          Debug.LogException(error);
        }
      }
    }

    private void Clear()
    {
      _success.Clear();
      _errors.Clear();
    }
  }
}
