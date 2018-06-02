using System.Threading.Tasks;

namespace N.Package.Promises
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

    /// <summary>
    /// Simply dispatch a task to resolve itself later, without worrying about the related promise.
    /// </summary>
    public static void Dispatch(this Task self)
    {
      self.Promise().Dispatch();
    }

    /// <summary>
    /// Simply dispatch a task to resolve itself later, without worrying about the related promise.
    /// Note that in this case the return value is simply discarded.
    /// </summary>
    public static void Dispatch<TResult>(this Task<TResult> self)
    {
      self.Promise().Dispatch();
    }
  }
}