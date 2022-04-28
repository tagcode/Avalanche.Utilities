using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalanche.Utilities;
using static System.Console;

public class pipe
{
    public static void Run()
    {
        {
            Pipe<int> pipe = new Pipe<int>();
        }
        {
            Pipe<int> pipe = (Pipe<int>)Pipe.Create(typeof(int));
        }
        {
            IProducerConsumerCollection<int> pipe = new Pipe<int>();
            pipe.TryAdd(1);
            pipe.TryTake(out int value);
        }
        {
            var pipe = new Pipe<int>();
            using IDisposable handle = pipe.Subscribe(new Observer());
            pipe.TryAdd(1);
        }
        {
            var pipe = new Pipe<int>();
            pipe.TryAdd(1);
            pipe.TryAdd(2);
            int[] array = pipe.ToArray();
        }

        {
            Pipe<int> pipe = new Pipe<int>();

            Task.Factory.StartNew(() =>
            {
                Parallel.For(0, 200, i =>
                {
                    for (int ix = 0; ix < 10000; ix++)
                        pipe.TryAdd(i * 100000 + ix);
                });
                pipe.Dispose();
            });

            List<int> receivedCopy = new List<int>();
            foreach (int ii in pipe)
            {
                receivedCopy.Add(ii);
            }

            int count = 0;
            foreach (int ii in pipe)
                count++;

            WriteLine($"first count = {receivedCopy.Count}, second count={count}");
        }
        {
            Pipe<int> pipe = new Pipe<int>();
            pipe.Dispose();
        }
    }

    public class Observer : IObserver<int>
    {
        public void OnCompleted() => WriteLine("Completed");
        public void OnError(Exception error) => WriteLine(error);
        public void OnNext(int value) => WriteLine($"OnValue: {value}");
    }
}

