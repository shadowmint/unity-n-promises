using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace N.Package.Promises
{
  public class AsyncWorker : MonoBehaviour
  {
    [Tooltip("Set this true to abort this worker, whatever it is")]
    public bool Abort;

    private bool _running;

    private Func<IEnumerator> Task { get; set; }

    private Deferred Deferred { get; set; }

    public static Promise Run(Func<IEnumerator> task, string name = "AsyncWorker", bool dontDestroyOnLoad = true, bool hideInHierarchy = true)
    {
      var container = new GameObject();
      container.transform.name = name;

      if (dontDestroyOnLoad)
      {
        DontDestroyOnLoad(container);
      }

      if (hideInHierarchy)
      {
        container.hideFlags = HideFlags.HideInHierarchy;
      }

      var worker = container.AddComponent<AsyncWorker>();
      worker.Task = task;
      worker.Deferred = new Deferred();
      return new Promise(worker.Deferred.Task);
    }

    public static Task RunAsync(Func<IEnumerator> task, string name = "AsyncWorker", bool dontDestroyOnLoad = true, bool hideInHierarchy = true)
    {
      var container = new GameObject();
      container.transform.name = name;

      if (dontDestroyOnLoad)
      {
        DontDestroyOnLoad(container);
      }

      if (hideInHierarchy)
      {
        container.hideFlags = HideFlags.HideInHierarchy;
      }
      
      var worker = container.AddComponent<AsyncWorker>();
      worker.Task = task;
      worker.Deferred = new Deferred();
      worker.Deferred.Task.Dispatch();
      return worker.Deferred.Task;
    }

    public void Start()
    {
      StartCoroutine(CoroutineWrapper());
    }

    private IEnumerator CoroutineWrapper()
    {
      var task = Task?.Invoke();
      if (task == null) yield break;

      _running = true;

      while (true)
      {
        object current;
        try
        {
          if (task.MoveNext() == false)
          {
            break;
          }

          current = task.Current;
        }
        catch (Exception error)
        {
          Deferred.Reject(error);
          break;
        }

        yield return current;
      }

      Deferred.Resolve();
      _running = false;
    }

    public void Update()
    {
      if (!_running || Abort)
      {
        Destroy(gameObject);
      }
    }
  }
}