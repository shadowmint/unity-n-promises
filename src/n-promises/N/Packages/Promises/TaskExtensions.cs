using System.Threading.Tasks;

namespace N.Packages.Promises
{
  public static class TaskExtensions
  {
    public static Promise Promise(this Task self)
    {
      return new Promise(self);
    }

    public static Promise<TResult> Promise<TResult>(this Task<TResult> self)
    {
      return new Promise<TResult>(self);
    }
  }
}