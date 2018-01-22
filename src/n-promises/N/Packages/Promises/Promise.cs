using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using N.Packages.Promises.Errors;
using N.Packages.Promises.Infrastructure;
using UnityEngine;

namespace N.Packages.Promises
{
  public class Promise : Promise<bool>
  {
    public Promise(Task task) : base(task as Task<bool>)
    {
    }

    public Promise Then(Action onSuccess, Action<Exception> onError = null)
    {
      base.Then((_) => onSuccess(), onError);
      return this;
    }
  }

  public class Promise<TResult> : IPromise
  {
    private PromiseState _state = PromiseState.Pending;

    private readonly Task<TResult> _task;

    private readonly List<Action<TResult>> _success = new List<Action<TResult>>();

    private readonly List<Action<Exception>> _errors = new List<Action<Exception>>();

    public Promise(Task<TResult> task)
    {
      _task = task;
    }

    public Promise<TResult> Then(Action<TResult> onSuccess, Action<Exception> onError = null)
    {
      if (_state != PromiseState.Pending) return this;

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

    public bool Check()
    {
      if (_state != PromiseState.Pending) return true;
      if (!_task.IsCompleted) return false;

      if (_task.IsFaulted)
      {
        Failed();
        return true;
      }

      if (_task.IsCanceled)
      {
        Failed(new TaskCancelledException());
      }

      Success();
      return true;
    }

    public Promise<TResult> ResolveWhenDone()
    {
      var worker = PromiseWorker.Get();
      worker.Register(this);
      return this;
    }

    private void Failed(Exception exception = null)
    {
      _state = PromiseState.Rejected;
      _errors.ForEach(i => Safe(i, exception ?? _task.Exception));
      Clear();
    }

    private void Success()
    {
      _state = PromiseState.Resolved;
      _success.ForEach(i => Safe(i, _task.Result));
      Clear();
    }

    private void Safe<T>(Action<T> action, T value)
    {
      try
      {
        action?.Invoke(value);
      }
#if !UNITY_EDITOR
      catch (Exception error)
      {
        Debug.LogException(error);
      }
#else
      catch (Exception)
      {
      }
#endif
    }

    private void Clear()
    {
      _success.Clear();
      _errors.Clear();
    }
  }
}