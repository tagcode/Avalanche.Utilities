using System;
using System.Collections.Generic;
using Avalanche.Utilities;
using static System.Console;

class graphclonable
{
    public static void Run()
    {
        {
            IGraphClonerContext graphClonerContext = new GraphClonerContext();
        }

        {
            // Create cloner
            ICloner<MyRecord> recordCloner = Cloner<MyRecord>.Instance;

            // Create record
            MyRecord myRecord = new MyRecord(1, "Abc-123");
            // Clone record
            MyRecord clone1 = recordCloner.Clone(myRecord);
            // Compare object references
            WriteLine(myRecord == clone1); // false
        }

        {
            // Create graph
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            node1.Edges.Add(node2);
            node2.Edges.Add(node3);
            node3.Edges.Add(node1);
            Node[] graph = new Node[] { node1, node2, node3 };

            {
                // Create cloner
                IGraphCloner<Node> nodeCloner = Cloner<Node>.Instance;
                // Clone graph
                Node clone1 = nodeCloner.Clone(node1, new GraphClonerContext());
                // Compare object references
                WriteLine(clone1 == node1); // false
            }

            {
                // Clone graph
                Node clone1 = (Node)node1.Clone(new GraphClonerContext());
                // Compare object references
                WriteLine(clone1 == node1); // false
            }

            {
                // Create cloner
                ICloner<Node> nodeCloner = Cloner<Node>.Instance;
                // Clone node
                Node clonedNode1 = nodeCloner.Clone(node1);
                // Compare object references
                WriteLine(clonedNode1 == node1); // false
            }
            {
                // Clone node
                Node clonedNode1 = (Node)node1.Clone();
                // Compare object references
                WriteLine(clonedNode1 == node1); // false
            }


        }
    }

    public class MyRecord : ICloneable
    {
        /// <summary></summary>
        public readonly int Id;
        /// <summary></summary>
        public readonly string Name;

        /// <summary></summary>
        public MyRecord(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>Clone</summary>
        public object Clone() => new MyRecord(Id, Name);
    }

    /// <summary>Graph node</summary>
    public class Node : IGraphCloneable, ICloneable
    {
        /// <summary>Id</summary>
        public readonly int Id;
        /// <summary>Forward edges/summary>
        public readonly List<Node> Edges = new List<Node>();

        /// <summary>Create node</summary>
        public Node(int id) => Id = id;

        /// <summary>Clone</summary>
        public object Clone(IGraphClonerContext context)
        {
            // Get existing clone
            if (context.TryGet(this, out Node clone0)) return clone0;
            // Create clone
            Node clone = new Node(Id);
            // Add to context
            context.Add(this, clone);
            // Hash in edges
            foreach (Node n in Edges) clone.Edges.Add((Node)n.Clone(context));
            // Return
            return clone;
        }

        /// <summary>Clone</summary>
        public object Clone()
        {
            // Get previous context
            IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
            // Place here context
            IGraphClonerContext context = prevContext ?? setContext(new GraphClonerContext())!;
            try
            {
                return Clone(context);
            }
            finally
            {
                // Revert to previous context
                IGraphCloner.Context.Value = prevContext;
            }
        }
        /// <summary>Assign <paramref name="context"/> to <see cref="IGraphCloner.Context"/> and return it.</summary>
        /// <returns><paramref name="context"/></returns>
        static IGraphClonerContext? setContext(IGraphClonerContext context) { IGraphCloner.Context.Value = context; return context; }
    }
}
