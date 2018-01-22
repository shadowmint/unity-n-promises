# unity-n-promises

Async/await enabled support for Task in C#.

Notice that you should never run `ContinueWith`, because that may run
in a different thread, and the Unity api is not thread safe.

## Usage

See the tests in the `Editor/` folder for each class for more usage examples.

### Promises

Convert a Task into a Promise like this:

    Task Foo() { ... }
    var promise = Foo().Promise();

Then add a completion condition:

    promise.Then(() => { .... }, (error) => { ... });

You can periodically check for resolution like this:

    void Update() {
      promise.Check()
    }

...or automatically resolve a promise async:

    promise.Then(() => { .... }, (error) => { ... }).ResolveWhenDone();

### Deferred

Deferred is a same-thread task source, which you can use like this:

    class AsyncTestService
    {
      public readonly Deferred<int> ForValueOne = new Deferred<int>();
      public readonly Deferred<int> ForValueTwo = new Deferred<int>();
    }

    ...

    instance.ForValueOne.Resolve(1);
    instance.ForValueTwo.Reject(new Exception("Not valid value!"));

Notice you can now use async / await syntax safely:

    class AsyncTestService
    {
      public readonly Deferred<int> ForValueOne = new Deferred<int>();
      public readonly Deferred<int> ForValueTwo = new Deferred<int>();

      public Promise<int> ResolveValue()
      {
        return new Promise<int>(Resolve()).ResolveWhenDone();
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
    }

### Coroutines

Coroutines are of dubious value once you have async/await, but they're
useful sometimes. You can invoke them safely like this, which automatically
spawns an abortable worker:

    AsyncWorker.Run(Spin).Then(() => { ... }, Debug.LogException).ResolveWhenDone();

## Install

From your unity project folder:

    npm init
    npm install shadowmint/unity-n-promises --save
    echo Assets/packages >> .gitignore
    echo Assets/packages.meta >> .gitignore

The package and all its dependencies will be installed in
your Assets/packages folder.

## Development

Setup and run tests:

    npm install
    npm install ..
    cd test
    npm install

Remember that changes made to the test folder are not saved to the package
unless they are copied back into the source folder.

To reinstall the files from the src folder, run `npm install ..` again.

### Tests

All tests are wrapped in `#if ...` blocks to prevent test spam.

You can enable tests in: Player settings > Other Settings > Scripting Define Symbols

The test key for this package is: `N_PROMISES_TESTS`
