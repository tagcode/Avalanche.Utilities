using System.Collections.Generic;
using Avalanche.Utilities;
using static System.Console;

class recordcomparer
{
    public static void Run()
    {
        {
            // Create field comparer
            DelegateComparer<MyRecord, int> idComparer = new DelegateComparer<MyRecord, int>(Comparer<int>.Default, (MyRecord n) => n.Id);
            // Create record comparer
            RecordComparer<MyRecord> recordComparer = new RecordComparer<MyRecord>().SetComparers(idComparer).SetReadOnly();
            // Compare
            WriteLine(recordComparer.Compare(new MyRecord(1), new MyRecord(1))); // 0
            WriteLine(recordComparer.Compare(new MyRecord(1), new MyRecord(2))); // -1
        }
        {
            RecordComparer recordComparer = RecordComparer.Create(typeof(MyRecord));
        }

        {
            // Create record comparer
            RecordComparer<Node> recordComparer = new RecordComparer<Node>().SetCyclical(true);
            // Id comparer
            DelegateComparer<Node, int> idComparer = new DelegateComparer<Node, int>(Comparer<int>.Default, (Node n) => n.Id);
            // Children comparer
            DelegateComparer<Node, List<Node>> edgeComparer = new DelegateComparer<Node, List<Node>>(new EnumerableGraphComparer<List<Node>, Node>(recordComparer).SetCyclical(true), (Node n) => n.Edges).SetCyclical(true);
            // Assign field comparers
            recordComparer.SetComparers(idComparer, edgeComparer).SetReadOnly();

            // Create graph 1
            Node graph1_1 = new Node(1);
            Node graph1_2 = new Node(2);
            Node graph1_3 = new Node(3);
            graph1_1.Edges.Add(graph1_2);
            graph1_2.Edges.Add(graph1_3);
            graph1_3.Edges.Add(graph1_1);

            // Create graph 2
            Node graph2_1 = new Node(1);
            Node graph2_2 = new Node(2);
            Node graph2_3 = new Node(3);
            graph2_1.Edges.Add(graph2_2);
            graph2_2.Edges.Add(graph2_3);
            graph2_3.Edges.Add(graph2_1);

            // Compare two graphs for content and topology differences
            WriteLine(recordComparer.Compare(graph1_1, graph2_1)); // 0
            WriteLine(recordComparer.Compare(graph1_1, graph2_2)); // -1
        }

        {
            // Create field comparer
            DelegateEqualityComparer<MyRecord, int> idEqualityComparer = new DelegateEqualityComparer<MyRecord, int>(EqualityComparer<int>.Default, (MyRecord n) => n.Id);
            // Create record comparer
            RecordEqualityComparer<MyRecord> recordEqualityComparer = new RecordEqualityComparer<MyRecord>().SetComparers(idEqualityComparer).SetReadOnly();
            // Compare
            WriteLine(recordEqualityComparer.Equals(new MyRecord(1), new MyRecord(1))); // True
            WriteLine(recordEqualityComparer.Equals(new MyRecord(1), new MyRecord(2))); // False
        }
        {
            RecordEqualityComparer recordEqualityComparer = RecordEqualityComparer.Create(typeof(MyRecord));
        }

        {
            // Create record comparer
            RecordEqualityComparer<Node> recordEqualityComparer = new RecordEqualityComparer<Node>().SetCyclical(true);
            // Id comparer
            DelegateEqualityComparer<Node, int> idEqualityComparer = new DelegateEqualityComparer<Node, int>(EqualityComparer<int>.Default, (Node n) => n.Id);
            // Children comparer
            DelegateEqualityComparer<Node, List<Node>> edgeEqualityComparer = new DelegateEqualityComparer<Node, List<Node>>(new EnumerableGraphEqualityComparer<List<Node>, Node>(recordEqualityComparer).SetCyclical(true), (Node n) => n.Edges).SetCyclical(true);
            // Assign field comparers
            recordEqualityComparer.SetComparers(idEqualityComparer, edgeEqualityComparer).SetReadOnly();

            // Create graph 1
            Node graph1_1 = new Node(1);
            Node graph1_2 = new Node(2);
            Node graph1_3 = new Node(3);
            graph1_1.Edges.Add(graph1_2);
            graph1_2.Edges.Add(graph1_3);
            graph1_3.Edges.Add(graph1_1);

            // Create graph 2
            Node graph2_1 = new Node(1);
            Node graph2_2 = new Node(2);
            Node graph2_3 = new Node(3);
            graph2_1.Edges.Add(graph2_2);
            graph2_2.Edges.Add(graph2_3);
            graph2_3.Edges.Add(graph2_1);

            // Compare two graphs for content and topology differences
            WriteLine(recordEqualityComparer.Equals(graph1_1, graph2_1)); // 0
            WriteLine(recordEqualityComparer.GetHashCode(graph1_1)); // -335010009
            WriteLine(recordEqualityComparer.GetHashCode(graph2_1)); // -335010009
        }

    }

    public class Node
    {
        public readonly int Id;
        public readonly List<Node> Edges = new List<Node>();
        public Node(int id) => Id = id;
    }

    public record MyRecord(int Id);

}
