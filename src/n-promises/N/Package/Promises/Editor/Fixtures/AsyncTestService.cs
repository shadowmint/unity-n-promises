using System.Threading.Tasks;
using N.Package.Promises;

namespace N.Packages.Promises.Editor.Fixtures
{
  internal class AsyncTestService
  {
    public readonly Deferred<int> ForValueOne = new Deferred<int>();
    public readonly Deferred<int> ForValueTwo = new Deferred<int>();

    public Promise<int> ResolveValue()
    {
      return new Promise<int>(Resolve()).Dispatch();
    }

    private async Task<int> Resolve()
    {
      var v1 = await Value1();
      var v2 = await Value2();
      return v1 + v2;
    }

    private async Task<int> Value1()
    {
      var value = await ForValueOne.Task;
      return value;
    }

    private async Task<int> Value2()
    {
      var value = await ForValueTwo.Task;
      return value;
    }

    public async Task UnTypedTaskResult()
    {
      await Resolve();
    }
  }
}