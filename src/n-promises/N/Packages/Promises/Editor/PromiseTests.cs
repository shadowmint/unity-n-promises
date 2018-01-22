#if N_PROMISES_TESTS
using System;
using N.Package.Core.Tests;
using N.Packages.Promises.Editor.Fixtures;
using NUnit.Framework;

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
  }
}

#endif