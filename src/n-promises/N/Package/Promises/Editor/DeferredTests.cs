#if N_PROMISES_TESTS
using System;
using N.Package.Test;
using N.Packages.Promises.Editor.Fixtures;
using NUnit.Framework;
using N.Package.Promises;
using N.Package.Promises.Infrastructure;
using UnityEngine;

namespace N.Packages.Promises.Editor
{
  public class DeferredTests : TestCase
  {
    [Test]
    public void TestDispatch()
    {
      var service = new AsyncTestService();
      var deferred = service.ResolveValue();

      var result = 0;
      deferred.Then((value) => { result = value; });
      Assert(result == 0);

      service.ForValueOne.Resolve(100);
      Assert(result == 0);

      service.ForValueTwo.Resolve(100);
      Assert(result == 0);

      PromiseWorker.Get().Update();
      Assert(result == 200);
    }

    [Test]
    public void TestDispatchUntypedTask()
    {
      var service = new AsyncTestService();
      var deferred = service.UnTypedTaskResult();

      var result = 0;
      deferred.Promise().Then((value) =>
        {
          Assert(value);
          result = 1;
        }, (err) =>
        {
          Debug.LogError(err);
          Unreachable();
        })
        .Dispatch();
      Assert(result == 0);

      service.ForValueOne.Resolve(100);
      Assert(result == 0);

      service.ForValueTwo.Resolve(100);
      Assert(result == 0);

      PromiseWorker.Get().Update();
      Assert(result == 1);
    }

    [Test]
    public void TestResolveCheck()
    {
      var service = new AsyncTestService();
      var deferred = service.ResolveValue();
      var result = 0;
      deferred.Then((value) => { result = value; });
      Assert(result == 0);

      service.ForValueOne.Resolve(100);
      Assert(result == 0);

      service.ForValueTwo.Resolve(100);
      Assert(result == 0);

      deferred.Check();
      Assert(result == 200);
    }

    [Test]
    public void TestRejectCheck()
    {
      var service = new AsyncTestService();
      var deferred = service.ResolveValue();
      var result = 0;

      deferred.Then((value) => { result = value; }, (err) => { result = -1; });
      Assert(result == 0);

      service.ForValueOne.Reject(new NotImplementedException());
      Assert(result == 0);

      service.ForValueTwo.Resolve(100);
      Assert(result == 0);

      deferred.Check();
      Assert(result == -1);
    }
  }
}

#endif