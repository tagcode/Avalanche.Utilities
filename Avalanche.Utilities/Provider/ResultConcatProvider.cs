// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;

/// <summary>Concatenates result values.</summary>
public class ResultConcatProvider<Key, Value> : ProviderBase<Key, IEnumerable<Value>>
{
    /// <summary>Immutable snapshot provider</summary>
    protected IEnumerable<IProvider<Key, IEnumerable<Value>>> providerEnumerable;
    /// <summary>Immutable snapshot provider</summary>
    public virtual IEnumerable<IProvider<Key, IEnumerable<Value>>> ProviderEnumerable => providerEnumerable;

    /// <summary></summary>
    /// <param name="providerEnumerable">Immutable snapshot provider</param>
    public ResultConcatProvider(IEnumerable<IProvider<Key, IEnumerable<Value>>> providerEnumerable)
    {
        this.providerEnumerable = providerEnumerable ?? throw new ArgumentNullException(nameof(providerEnumerable));
    }

    /// <summary>Query and concatenate results</summary>
    public override bool TryGetValue(Key query, out IEnumerable<Value> values)
    {
        // Get snapshots
        IList<IProvider<Key, IEnumerable<Value>>> _providers = ArrayUtilities.GetSnapshot(ProviderEnumerable);
        // Init
        values = null!;
        // Number of queries that were ok (true) and failed (false)
        int okCount = 0, failedCount = 0;
        // Get snapshot of file provicers
        foreach (var _provider in _providers)
        {
            // Get Lines
            if (!_provider.TryGetValue(query, out IEnumerable<Value> _values)) { failedCount++; continue; } else okCount++;
            // No values
            if (_values is ICollection<Value> collection && collection.Count == 0) continue;
            // Add to result
            values = values == null ? _values : values.Concat(_values);
        }
        // Nothing was queried (true but empty result)
        if (okCount == 0 && failedCount == 0) { values = Array.Empty<Value>(); return false; }
        // All were failed (false result)
        if (okCount == 0 && failedCount > 0) { values = null!; return false; }
        // All or some returned results (concat results)
        if (values == null) values = Array.Empty<Value>();
        return true;
    }

    /// <summary>Version that removes overlapping results</summary>
    public class Distinct : ResultConcatProvider<Key, Value>
    {
        /// <summary>Value equality comparer</summary>
        protected IEqualityComparer<Value> valueEqualityComparer;

        /// <summary></summary>
        /// <param name="providerEnumerable">Immutable snapshot provider</param>
        public Distinct(IEnumerable<IProvider<Key, IEnumerable<Value>>> providerEnumerable, IEqualityComparer<Value>? valueEqualityComparer = null) : base(providerEnumerable) 
        {
            this.valueEqualityComparer = valueEqualityComparer ?? EqualityComparer<Value>.Default;
        }

        /// <summary>Query and concatenate results</summary>
        public override bool TryGetValue(Key query, out IEnumerable<Value> values)
        {
            // Get snapshots
            IList<IProvider<Key, IEnumerable<Value>>> _providers = ArrayUtilities.GetSnapshot(ProviderEnumerable);
            // Place results here, maintain order
            StructList10<Value> result = new(valueEqualityComparer);
            // Lazy initialized table for detecting duplicates
            HashSet<Value>? set = null!;
            // Number of queries that were ok (true) and failed (false)
            int okCount = 0, failedCount = 0;
            // Get snapshot of file provicers
            foreach (var _provider in _providers)
            {
                // Get files
                if (!_provider.TryGetValue(query, out IEnumerable<Value> _values)) { failedCount++; continue; }
                // Ok
                okCount++;
                // No values
                if (_values is ICollection<Value> collection && collection.Count == 0) continue;
                // Add values
                foreach (Value _value in _values)
                {
                    // Create hash set for detecting duplicates
                    if (result.Count >= result.StackCount && set == null) { set = new HashSet<Value>(valueEqualityComparer); for (int i = 0; i < result.Count; i++) set.Add(result[i]); }
                    // Detected duplicate
                    if (set == null ? result.Contains(_value) : set.Contains(_value)) continue;
                    // Add
                    result.Add(_value);
                    if (set != null) set.Add(_value);
                }
            }
            // Nothing was queried (true but empty result)
            if (okCount == 0 && failedCount == 0) { values = Array.Empty<Value>(); return false; }
            // All were failed (false result)
            if (okCount == 0 && failedCount > 0) { values = null!; return false; }
            // All or some returned results (concat results)
            values = result.ToArray();
            return true;
        }
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"Concatenation({string.Join(", ", providerEnumerable)})";
}
