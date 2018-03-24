using System.Threading.Tasks;

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