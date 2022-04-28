using System;
using System.Threading;
using System.Threading.Tasks;
using Avalanche.Utilities;

class disposebelatable
{
    public static void Run()
    {
        // Create something disposable
        MyClass obj = new MyClass();
        // Belate handle for closure
        IDisposable belateHandle = obj.BelateDispose();
        // Pass 'obj' for concurrent thread
        Task.Run(() =>
        {
            // Do work
            Thread.Sleep(1000);
            //
            belateHandle.Dispose();
        });
        // Do work with 'obj'...

        // Dispose 'obj'.
        obj.Dispose();
    }

    public class MyClass : DisposeAttachable, IDisposeBelatable
    {
    }
}
