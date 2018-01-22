using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace N.Packages.Promises.Editor.Fixtures
{
  public class NaiveAsyncService
  {
    private Deferred _deferred;

    public Task DoLongRunningTask()
    {
      _deferred = new Deferred();
      return _deferred.Task;
    }

    public void Resolve()
    {
      _deferred.Resolve();
    }
  }
}