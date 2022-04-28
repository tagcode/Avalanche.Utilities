using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Avalanche.Utilities;
using static System.Console;

class graphcomparable
{
    public static void Run()
    {
        {
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

            // Create graph comparer
            IComparer<Node> comparer = GraphComparer<Node>.Instance;
            // Compare 
            WriteLine(comparer.Compare(graph1_1, graph2_1)); // 0
            WriteLine(comparer.Compare(graph1_1, graph2_2)); // -1

            // Create graph comparer
            IEqualityComparer<Node> equalityComparer = GraphEqualityComparer<Node>.Instance;
            // Compare 
            WriteLine(equalityComparer.Equals(graph1_1, graph2_1)); // True
            WriteLine(equalityComparer.Equals(graph1_1, graph2_2)); // False
        }
    }

    /// <summary>Graph node</summary>
    public class Node : IGraphComparable<Node>, IGraphEqualityComparable<Node>, ICyclical
    {
        /// <summary>Is possibly cyclical node.</summary>
        [IgnoreDataMember] public bool IsCyclical { get => Edges.Count > 0; set { } }
        /// <summary>Id</summary>
        public readonly int Id;
        /// <summary>Forward edges/summary>
        public readonly List<Node> Edges = new List<Node>();

        /// <summary>Create node</summary>
        public Node(int id) => Id = id;

        /// <summary>Compare order to <paramref name="other"/></summary>
        public int CompareTo(Node? other, IGraphComparerContext2 context)
        {
            // Same reference
            if (this == other) return 0;
            if (other == null) return -1;
            // Is already hashed
            if (!context.Add(this, other)) return 0;
            // Id
            if (Id < other.Id) return -1;
            if (Id > other.Id) return 1;
            // Get edge count
            int c1 = Edges.Count, c2 = other.Edges.Count;
            // Compare edges
            for (int i = 0; i < Math.Min(c1, c2); i++)
            {
                int d = Edges[i].CompareTo(other.Edges[i], context);
                if (d != 0) return d;
            }
            //
            if (c1 < c2) return -1;
            if (c1 > c2) return 1;
            // Equal
            return 0;
        }

        /// <summary>Compare equality to <paramref name="other"/>.</summary>
        public bool EqualTo(Node? other, IGraphComparerContext2 context)
        {
            // Same reference
            if (this == other) return true;
            if (other == null) return false;
            // Is already hashed
            if (!context.Add(this, other)) return true;
            // Id
            if (Id != other.Id) return false;
            // Get edge count
            int c1 = Edges.Count, c2 = other.Edges.Count;
            // Count mismatch
            if (c1 != c2) return false;
            // Compare edges
            for (int i = 0; i < Math.Min(c1, c2); i++)
            {
                if (!Edges[i].EqualTo(other.Edges[i], context)) return false;
            }
            // Equal
            return true;
        }

        /// <summary>Calculate hash-code</summary>
        public int GetHashCode(IGraphComparerContext context)
        {
            // Is already hashed
            if (!context.Add(this)) return 0;
            // Init
            int hash = unchecked((int)2166136261);
            // Hash in id
            hash ^= unchecked(Id);
            // Hash in edges
            foreach (Node n in Edges) hash = (hash * 16777619) ^ n.GetHashCode(context);
            // Return
            return hash;
        }
    }
}
