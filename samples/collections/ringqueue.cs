using System;
using Avalanche.Utilities;
using static System.Console;

public class ringqueue
{
    public static void Run()
    {
        {
            var queue = new RingQueue<int>();
        }
        {
            var queue =
                (RingQueue<int>)
                RingQueue.Create(typeof(int));
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            WriteLine(queue.Dequeue()); // 1
            WriteLine(queue.Dequeue()); // 2
            WriteLine(queue.Dequeue()); // 3
        }

        {
            var queue = new RingQueue<int>(capacity: 1);
            queue.AllowGrow = true;
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            WriteLine(queue.Capacity); // 11
            WriteLine(queue.Dequeue()); // 1
            WriteLine(queue.Dequeue()); // 2
            WriteLine(queue.Dequeue()); // 3
        }

        {
            try
            {
                var queue = new RingQueue<int>(capacity: 1);
                queue.AllowGrow = false;
                queue.Enqueue(1);
                queue.Enqueue(2); // InvalidOperationException
            }
            catch (InvalidOperationException)
            {
                // Could not grow (AllowGrow = false)
            }
        }
        {
            var queue = new RingQueue<int>(capacity: 1);
            WriteLine(queue.Capacity); // 1
            queue.AllowGrow = false;
            queue.Capacity = 10;
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            WriteLine(queue.Capacity); // 10
            queue.TrimExcess();
            WriteLine(queue.Capacity); // 3
            queue.EnsureCapacity(10);
            WriteLine(queue.Capacity); // 10
        }

        {
            var queue = new RingQueue<string>().Enqueue("A").Enqueue("B").Enqueue("C");
            queue[0] = "AA";
            WriteLine(queue.Dequeue()); // AA
            WriteLine(queue.Dequeue()); // B
            WriteLine(queue.Dequeue()); // C
        }

        {
            var queue = new RingQueue<string>().Enqueue("A").SetReadOnly();
        }

        {
            var queue = new RingQueue<string>().Enqueue("A").Enqueue("B").Enqueue("C");
            queue.Skip(2);
            WriteLine(queue.Dequeue()); // C
        }

        {
            var queue = new RingQueue<string>().Enqueue("A").Enqueue("B").Enqueue("C");
            string[] array = queue.Dequeue(3); // [ A, B, C ]
        }

        {
            var queue = new RingQueue<string>().Enqueue("A").Enqueue("B").Enqueue("C");
            WriteLine(queue.Peek()); // A
            WriteLine(queue[0]); // A
            WriteLine(queue[1]); // B
        }
        {
            var queue = new RingQueue<string>().Enqueue("A").Enqueue("B").Enqueue("C");
            if (queue.TryPeek(out string? value0)) WriteLine(value0); // A
            if (queue.TryDequeue(out string? value1)) WriteLine(value1); // A
        }
        {
            var queue = new RingQueue<string>().Enqueue("A").Enqueue("B").Enqueue("C");
            WriteLine(queue.IndexOf("B")); // 1
        }
        {
            var queue = new RingQueue<string>().Enqueue("A").Enqueue("B").Enqueue("C");
            WriteLine(queue.BinarySearch("B", StringComparer.Ordinal)); // 1
        }
        {
            var queue = new RingQueue<string>().Enqueue("A").Enqueue("B").Enqueue("C");
            foreach (var str in queue) WriteLine(str); // A, B, C
        }

    }
}

