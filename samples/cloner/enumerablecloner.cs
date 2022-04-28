using System;
using System.Collections.Generic;
using System.Linq;
using Avalanche.Utilities;
using static System.Console;

public class enumerablecloner
{
    public static void Run()
    {
        // Enumerable Cloner
        {
            // Create cloner
            ICloner<IEnumerable<int>> cloner = new EnumerableCloner<int>().SetReadOnly();
            // Create list
            List<int> list = new List<int> { 1, 2, 3 };
            // Create clone
            IEnumerable<int> clone = cloner.Clone(list);
            // Print clone
            Print(clone);
        }
        {
            ICloner<IEnumerable<int>> cloner =
                (ICloner<IEnumerable<int>>)
                EnumerableCloner.Create(typeof(int)).SetReadOnly();
            // Create list
            List<int> list = new List<int> { 1, 2, 3 };
            // Create clone
            IEnumerable<int> clone = cloner.Clone(list);
            // Print clone
            Print(clone);
        }

        // List Cloner
        {
            // Create cloner
            ICloner<List<int>> cloner = new ListCloner<List<int>, int>().SetReadOnly();

            // Create list
            List<int> list = new List<int> { 1, 2, 3 };

            // Create clone
            List<int> clone = cloner.Clone(list);
            // Print clone
            Print(clone);
        }
        {
            // Create cloner
            ICloner<List<int>> cloner =
                (ICloner<List<int>>)
                ListCloner.Create(typeof(List<int>)).SetReadOnly();
            // Create list
            List<int> list = new List<int> { 1, 2, 3 };
            // Create clone
            List<int> clone = cloner.Clone(list);
            // Print clone
            Print(clone);
        }
        {
            // Create cloner
            ICloner<IList<int>> cloner =
                new ListCloner<IList<int>, int, List<int>>()
                .SetReadOnly();

            // Create list
            IList<int> list = new List<int> { 1, 2, 3 };

            // Create clone
            IList<int> clone = cloner.Clone(list);
            // Print clone
            Print(clone);
        }
        {
            // Create cloner
            ICloner<IList<int>> cloner =
                (ICloner<IList<int>>)
                ListCloner.Create(typeof(IList<int>), typeof(List<int>))
                .SetReadOnly();
            // Create list
            IList<int> list = new List<int> { 1, 2, 3 };
            // Create clone
            IList<int> clone = cloner.Clone(list);
            // Print clone
            Print(clone);
        }

        // Array Cloner
        {
            // Create cloner
            ICloner<int[]> cloner = new ArrayCloner<int>().SetReadOnly();

            // Create array
            int[] list = new int[] { 1, 2, 3 };

            // Create clone
            int[] clone = cloner.Clone(list);
            // Print clone
            Print(clone);
        }
        {
            // Create cloner
            ICloner<int[]> cloner =
                (ICloner<int[]>)
                ArrayCloner.Create(typeof(int)).SetReadOnly();
            // Create array
            int[] list = new int[] { 1, 2, 3 };
            // Create clone
            int[] clone = cloner.Clone(list);
            // Print clone
            Print(clone);
        }

        // ThrowOnNull
        {
            // Create cloner
            ICloner<int[]> cloner =
                (ICloner<int[]>)
                ArrayCloner.Create(typeof(int))
                .SetThrowOnNull(false)
                .SetReadOnly();
            // Create clone (of null)
            int[]? clone = cloner.Clone(null!); // -> null
        }
        {
            try
            {
                // Create cloner
                ICloner<int[]> cloner =
                    (ICloner<int[]>)
                    ArrayCloner.Create(typeof(int))
                    .SetThrowOnNull(true)
                    .SetReadOnly();
                // Create clone (of null)
                int[]? clone = cloner.Clone(null!);
            }
            catch (ArgumentNullException)
            {
            }
        }

        // Deep Cloner
        {
            // Create record cloner
            ICloner<MyRecord> recordCloner = (ICloner<MyRecord>)RecordCloner.Create(typeof(MyRecord));
            // Create array cloner
            ICloner<MyRecord[]> cloner =
                (ICloner<MyRecord[]>)
                ArrayCloner.Create(typeof(MyRecord))
                .SetElementCloner(recordCloner)
                .SetReadOnly();
            // Create record
            MyRecord myRecord = new MyRecord { Id = 1 };
            // Create array
            MyRecord[] array = new[] { myRecord };
            // Create array clone
            MyRecord[] clones = cloner.Clone(array);
            // Assert not same reference
            WriteLine(Object.ReferenceEquals(clones, array));       // False
            WriteLine(Object.ReferenceEquals(clones[0], array[0])); // False
        }

        // Graph Cloner
        {
            // Create node cloner
            ICloner<Node> nodeCloner = (ICloner<Node>)RecordCloner.Create(typeof(Node)).SetCyclical(true);
            // Create array cloner
            ICloner<Node[]> graphCloner =
                (ICloner<Node[]>)
                ArrayCloner.Create(typeof(Node))
                .SetElementCloner(nodeCloner)
                .SetCyclical(true)
                .SetReadOnly();
            // Create graph
            Node node1 = new Node { Id = 1 };
            Node node2 = new Node { Id = 2 };
            Node node3 = new Node { Id = 3 };
            node1.Edges.Add(node2);
            node2.Edges.Add(node3);
            node3.Edges.Add(node1);
            Node[] graph = new[] { node1, node2, node3 };
            // Clone
            Node[] clone = graphCloner.Clone(graph);
            // Assert not same reference
            WriteLine(Object.ReferenceEquals(graph, clone));       // False
            WriteLine(Object.ReferenceEquals(graph[0], clone[0])); // False
        }

    }

    static void Print<T>(IEnumerable<T> enumr) => Console.WriteLine(String.Join(", ", enumr));
    static void PrintArray<T>(IEnumerable<IEnumerable<T>> enumr) => Console.WriteLine($"[{String.Join("], [", enumr.Select(array => String.Join(", ", array)))}]");


    public class Node
    {
        public int Id;
        public List<Node> Edges = new List<Node>();
    }

    public record MyRecord
    {
        public int Id;
    }
}
