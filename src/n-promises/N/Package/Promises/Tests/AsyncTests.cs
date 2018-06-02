#if N_PROMISES_TESTS
using System.Collections;
using System.Threading.Tasks;
using N.Package.Test.Runtime;
using UnityEngine;

namespace N.Package.Promises.Tests
{
  public class AsyncTests : RuntimeTest
  {
    private Deferred _deferred;
    private float _elapsed;

    [RuntimeTest]
    public void TestAsyncCoroutine()
    {
      SomeAsyncFunction().Dispatch();
    }

    private async Task SomeAsyncFunction()
    {
      Log("Starting async function");

      Log("Waiting for coroutine");
      await AsyncWorker.RunAsync(SomeCoroutine);

      Log("Waiting for deferred");
      await SomeExternalAsyncFunction();

      Log("Woo! All async stuff worked");
      Completed();
    }

    private Task SomeExternalAsyncFunction()
    {
      _elapsed = 0f;
      _deferred = new Deferred();
      return _deferred.Task;
    }

    private IEnumerator SomeCoroutine()
    {
      yield return new WaitForSeconds(1f);
    }

    public void Update()
    {
      if (_deferred == null) return;
      _elapsed += Time.deltaTime;

      if (_elapsed < 2f) return;
      _deferred.Resolve();
      _deferred = null;
    }
  }
}

#endif