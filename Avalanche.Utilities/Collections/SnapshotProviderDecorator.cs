// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Decorates elements of <![CDATA[IEnumerable<T>]]>. 
/// When decoree is <![CDATA[ISnapshotProvider<T>]]>, it detects source changes and can redecorate contents. 
/// Can be combined with caching provider to return same decoration references to recurring elements.
/// 
/// This class is designed to use <see cref="ISnapshotProvider{T}"/> such as <see cref="ArrayList{T}"/> as source. 
/// It can be used with other <see cref="IEnumerable{T}"/>, but the performance is lower.
/// </summary>
public class SnapshotProviderDecorator<T> : IEnumerable<T>, IDecoration, ICache, ISnapshotProvider<T>
{
    /// <summary></summary>
    protected IEnumerable<T> source;
    /// <summary></summary>
    public bool IsDecoration { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public object? Decoree { get => source; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    bool ICache.IsCache { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    bool ICached.IsCached { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public T[] Snapshot { get => createArray(); set => throw new NotImplementedException(); }

    /// <summary></summary>
    protected (IList<T> sourceList, T[] array) snapshot = default!;

    /// <summary>Optional where filter</summary>
    protected Func<T, bool>? where;
    /// <summary>Optional selector</summary>
    protected Func<T, T>? selector;
    /// <summary>Optional post process</summary>
    protected Action<T[]>? postProcess;

    /// <summary></summary>
    protected virtual T[] createArray()
    {
        // 
        (IList<T> sourceList, T[] array) prev = snapshot;
        // Get source list
        IList<T> sourceList = ArrayUtilities.GetSnapshot(source);
        // Source has remained same
        if (prev.sourceList != null && prev.array != null && object.ReferenceEquals(sourceList, prev.sourceList)) return prev.array;
        // Assign as is
        if (sourceList is T[] sourceArray && where == null && selector == null && postProcess == null) { snapshot = (sourceList, sourceArray); return sourceArray; }
        // Create new result
        List<T> resultList = new List<T>(sourceList.Count);
        //
        for (int i=0; i<sourceList.Count; i++)
        {
            // Get element
            T element = sourceList[i];
            // Where rules out
            if (where != null && !where(element)) continue;
            // Selector
            if (selector != null) element = selector(element);
            // Add to result
            resultList.Add(element);
        }
        // Create array
        T[] resultArray = resultList.ToArray();
        // Post-process
        if (postProcess != null) postProcess(resultArray);
        // Assign
        snapshot = (sourceList, resultArray);
        // Return 
        return resultArray;
    }

    /// <summary></summary>
    /// <param name="source"></param>
    /// <param name="selector">Optional selector</param>
    /// <param name="where">Optional where filter</param>
    public SnapshotProviderDecorator(IEnumerable<T> source, Func<T, bool>? where = null, Func<T, T>? selector = null, Action<T[]>? postProcess = null)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
        this.where = where;
        this.selector = selector;
        this.postProcess = postProcess;
    }

    /// <summary>Invalidate cached array</summary>
    /// <param name="deep">If true, invalidates elements as well</param>
    void ICached.InvalidateCache(bool deep)
    {
        var _copy = snapshot;
        snapshot = default;
        if (deep && _copy.array != null)
        {
            foreach (T element in _copy.array)
                if (element is ICached cached)
                    cached.InvalidateCache(true);
        }
    }

    /// <summary></summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Snapshot).GetEnumerator();
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => Snapshot.GetEnumerator();

    /// <summary></summary>
    public override string ToString() => source.ToString()+".Cached()";
}
