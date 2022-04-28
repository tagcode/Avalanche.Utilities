using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using Avalanche.Utilities.Record;

public class clonerprovider
{
    public static void Run()
    {
        {
            IProvider<Type, ICloner> clonerProvider = ClonerProvider.Create;
        }
        {
            IProvider<Type, ICloner> clonerProvider = ClonerProvider.Cached;
        }
        {
            ICloner<int[]> cloner = (ICloner<int[]>)ClonerProvider.Create[typeof(int[])];
            int[] clone = cloner.Clone(new int[] { 1, 2, 3 });
        }
        {
            ICloner<IList<int>> cloner = (ICloner<IList<int>>)ClonerProvider.Cached[typeof(IList<int>)];
            IList<int> clone = cloner.Clone(new int[] { 1, 2, 3 });
        }
        {
            ICloner<MyRecord> cloner = (ICloner<MyRecord>)ClonerProvider.Cached[typeof(MyRecord)];
            MyRecord clone = cloner.Clone(new MyRecord(1));
        }
        {
            // Create cloner
            ICloner<Node> cloner = (ICloner<Node>)ClonerProvider.Cached[typeof(Node)];
            // Create graph
            Node node1 = new Node(1);
            Node node2 = new Node(2);
            Node node3 = new Node(3);
            node1.Edges.Add(node2);
            node2.Edges.Add(node3);
            node3.Edges.Add(node1);
            // Clone graph
            Node clone1 = cloner.Clone(node1);
        }
    }

    public class Node : IRecord, ICyclical
    {
        [IgnoreDataMember] public bool IsCyclical { get => Edges.Count > 0; set { } }
        public readonly int Id;
        public List<Node> Edges = new List<Node>();
        public Node(int id) => Id = id;
    }

    public record MyRecord(int Id) : IRecord;
}
