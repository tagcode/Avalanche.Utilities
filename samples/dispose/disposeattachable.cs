using System;
using System.Threading;
using Avalanche.Utilities;

class disposeattachable
{
    public static void Run()
    {
        {
            // Create something disposable
            IDisposable s = new Semaphore(1, 1);
            // Create obj and attach disposable
            MyClass obj = new MyClass().AttachDisposable(s);
            // Dispose 'obj' and attached 's'
            obj.Dispose();
        }
        {
            // Create something disposable
            IDisposable s = new Semaphore(1, 1);
            // Create obj and attach disposable
            MyClass obj = new MyClass().AttachDisposable(s);
            obj.RemoveDisposable(s);
        }
        {
            MyClass obj = new MyClass().AddDisposeAction((MyClass m) => { });
        }
    }

    public class MyClass : DisposeAttachable, IDisposeAttachable
    {
    }

    public class MyClassB : DisposeAttachable, IDisposeAttachable
    {
        /// <summary>Override this to dispose managed resources</summary>
        /// <param name="errors">list that can be instantiated and where errors can be added</param>
        /// <exception cref="Exception">any exception is captured and aggregated with other errors</exception>
        protected override void InnerDispose<Exceptions>(ref Exceptions errors)
        {
        }

        /// <summary>Override this to dispose unmanaged resources.</summary>
        /// <param name="errors">list that can be instantiated and where errors can be added</param>
        /// <exception cref="Exception">any exception is captured and aggregated with other errors</exception>
        protected override void InnerDisposeUnmanaged<Exceptions>(ref Exceptions errors)
        {
        }
    }
}
