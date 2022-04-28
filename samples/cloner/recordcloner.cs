using System.Collections.Generic;
using System.Runtime.Serialization;
using Avalanche.Utilities;
using Avalanche.Utilities.Record;
using static System.Console;

public class recordcloner
{
    public static void Run()
    {
        {
            RecordCloner<MyRecord> cloner =
                new RecordCloner<MyRecord>()
                .SetReadOnly();
        }
        {
            RecordCloner<MyRecord> cloner =
                (RecordCloner<MyRecord>)
                RecordCloner.Create(typeof(MyRecord))
                .SetReadOnly();
        }
        {
            RecordCloner<MyRecord> cloner =
                new RecordCloner<MyRecord>()
                .SetClonerProvider(ClonerProvider.Cached)
                .SetReadOnly();
        }
        {
            // Create cloner
            RecordCloner<MyRecord> cloner = new RecordCloner<MyRecord>();
            // Create record
            MyRecord record1 = new MyRecord(1);
            // Clone record
            MyRecord clone = cloner.Clone(record1);
        }

        {
            // Create cloner
            RecordCloner<Node> cloner =
                new RecordCloner<Node>()
                .SetCyclical(true)
                .SetClonerProvider(ClonerProvider.Cached)
                .SetReadOnly();
            // Create graph
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            node1.Edges.Add(node2);
            node2.Edges.Add(node3);
            node3.Edges.Add(node1);
            // Clone graph
            Node clone1 = cloner.Clone(node1);
            // Compare object references
            WriteLine(clone1 == node1); // false
        }
    }

    public class Node : IRecord, ICyclical
    {
        /// <summary>Is possibly cyclical node.</summary>
        [IgnoreDataMember] public bool IsCyclical { get => Edges.Count > 0; set { } }
        public readonly int Id;
        public List<Node> Edges = new List<Node>();
        public Node(int id) => Id = id;
    }

    public record MyRecord(int Id) : IRecord;

}
