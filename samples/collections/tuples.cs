using System.Collections.Generic;
using Avalanche.Utilities;

public class tuples
{
    public static void Run()
    {
        {
            // Stack allocated
            var pair_stack = new Pair<int, int>(1, 2);

            // Heap allocated
            var pair_heap = new PairObject<int, int>(1, 2);
        }

        {
            // Immutable
            var pair_immutable = new Pair<int, int>(1, 2);
            int hash_is_cached = pair_immutable.GetHashCode();

            // Mutable
            var pair_mutable = new PairMutable<int, int>(2, 3);
            pair_mutable.a = 4;
            pair_mutable.b = 6;
            int hash_is_calculated = pair_mutable.GetHashCode();
        }

        {
            // Create pairs
            var pair1 = new PairSet<int>(1, 2);
            var pair2 = new PairSet<int>(2, 1);

            // Get equality comparer singleton instance
            IEqualityComparer<PairSet<int>> equalsComparer = PairSet<int>.EqualityComparer.Instance;

            // Compare
            bool areEqual = equalsComparer.Equals(pair1, pair2);
        }

        {
            // Create pairs
            var pair1 = new Pair<int, int>(1, 2);
            var pair2 = new Pair<int, int>(2, 3);

            // Get equality comparer singleton instance
            IEqualityComparer<Pair<int, int>> equalsComparer = Pair<int, int>.EqualityComparer.Instance;

            // Compare
            bool areEqual = equalsComparer.Equals(pair1, pair2);
        }

        {
            var pair1 = new Pair<int, int>(1, 2);
            var pair2 = new Pair<int, int>(2, 3);
            // Compare
            bool areEqual = pair1.Equals(pair2);
        }

        {
            // Create pairs
            var pair1 = new Pair<int, int>(1, 2);
            var pair2 = new Pair<int, int>(2, 3);

            // Get equality comparer singleton instance
            IComparer<Pair<int, int>> comparer = Pair<int, int>.Comparer.Instance;

            // Compare
            int c = comparer.Compare(pair1, pair2);
        }

        {
            var pair1 = new Pair<int, int>(1, 2);
            var pair2 = new Pair<int, int>(2, 3);
            // Compare
            int c = pair1.CompareTo(pair2);
        }

        {
            // One argument
            var container = new ContainerMutableObject<string>("Hello");
            container.a = "New Hello";

            // Two arguments
            var pair = new PairMutable<int, int>(1, 2);

            // Three arguments
            var triple = new TripleSet<int>(5, 6, 7);

            // Four arguments
            var quad = new QuadObject<byte, byte, byte, byte>(1, 2, 3, 4);
        }
    }
}

