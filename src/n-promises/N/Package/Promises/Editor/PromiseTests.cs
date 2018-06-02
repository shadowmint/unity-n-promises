#if N_PROMISES_TESTS
using System;
using N.Package.Test;
using N.Packages.Promises.Editor.Fixtures;
using NUnit.Framework;
using N.Package.Promises;

namespace N.Packages.Promises.Editor
{
  public class PromiseTests : TestCase
  {
    [Test]
    public void TestHandleNaiveTask()
    {
      var resolved = false;
      var service = new NaiveAsyncService();
      var task = service.DoLongRunningTask().Promise().Then(() => { resolved = true; });
      Assert(!task.Check());

      service.Resolve();
      Assert(task.Check());
      Assert(resolved);
    }

    [Test]
    public void TestUnexpectedException()
    {
      var service = new NaiveAsyncService();
      var task = service.DoLongRunningTask().Promise().Then(() => { throw new NotImplementedException(); });
      Assert(!task.Check());

      service.Resolve();
      Assert(task.Check());
    }

    [Test]
    public void TestNullTaskResultsInValidPromise()
    {
      var resolved = false;
      var failed = false;
      
      var promise = new Promise(null).Then(() => { resolved = true; }, e => { failed = true; });
      
      // Is immediately completed
      Assert(promise.Check());
      
      // A null task is always a failed promise
      Assert(!resolved);
      Assert(failed);
    }
  }
}

#endif