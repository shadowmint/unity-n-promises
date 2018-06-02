using System;
using System.Collections.Generic;
using UnityEngine;

namespace N.Package.Promises.Infrastructure
{
  public class PromiseWorker : MonoBehaviour
  {
    private static PromiseWorker _instance;

    private readonly Queue<IPromise> _promises = new Queue<IPromise>();

    public static PromiseWorker Get()
    {
      if (_instance != null) return _instance;
      var container = new GameObject();
      container.transform.name = "N.Packages.Promises.PromiseWorker";

      try
      {
        DontDestroyOnLoad(container);
      }
      catch (Exception)
      {
      }

      container.hideFlags = HideFlags.HideInHierarchy;
      _instance = container.AddComponent<PromiseWorker>();
      return _instance;
    }

    public void Register(IPromise promise)
    {
      _promises.Enqueue(promise);
    }

    public void Update()
    {
      var count = _promises.Count;
      for (var i = 0; i < count; i++)
      {
        var promise = _promises.Dequeue();
        if (!promise.Check())
        {
          _promises.Enqueue(promise);
        }
      }
    }
  }
}