// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

/// <summary>
/// Bijection Dictionary is a Dictionary that has no values or keys, only 1:1 of values. 
/// These value/keys will be called with left and right side.
/// 
/// Each value can exist only once on a each side
/// </summary>
public abstract class BijectionMap : ICloneable, IEnumerable, IReadOnly
{
    /// <summary></summary>
    static readonly ConstructorT2<BijectionMap> constructor = new(typeof(BijectionMap<,>));
    /// <summary></summary>
    static readonly ConstructorT2<object, object, BijectionMap> constructor2 = new(typeof(BijectionMap<,>._));
    /// <summary></summary>
    public static BijectionMap Create(Type keyType, Type valueType) => constructor.Create(keyType, valueType);
    /// <summary></summary>
    public static BijectionMap Create(Type keyType, Type valueType, object leftComparer, object rightComparer) => constructor2.Create(keyType, valueType, leftComparer, rightComparer);

    /// <summary>Left side type</summary>
    public abstract Type LeftType { get; }
    /// <summary>Right side type</summary>
    public abstract Type RightType { get; }

    /// <summary>Is read-only state</summary>
    protected bool @readonly;
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException("Read-only"); @readonly = value; } }

    /// <summary></summary>
    public abstract object Clone();
    /// <summary></summary>
    public IEnumerator GetEnumerator() => getEnumerator();
    /// <summary></summary>
    protected abstract IEnumerator getEnumerator();
}

/// <summary>
/// Bijection Dictionary is a Dictionary that has no values or keys, only 1:1 of values. 
/// These value/keys will be called with left and right side.
/// 
/// Each value can exist only once on a each side
/// </summary>
/// <typeparam name="L">Left-side key</typeparam>
/// <typeparam name="R">Right-side key</typeparam>
public class BijectionMap<L, R> : BijectionMap, IEnumerable<KeyValuePair<L, R>> where L : notnull where R : notnull
{
    /// <summary>The keys of tableLeft are left-side-values and values are right-side-values.</summary>
    protected Dictionary<L, R> tableLeft;
    /// <summary>The keys of tableRight are right-side-values and values on it are left-side-values.</summary>
    protected Dictionary<R, L> tableRight;
    /// <summary>Left key comparer.</summary>
    protected IEqualityComparer<L> leftComparer;
    /// <summary>Right key comparer.</summary>
    protected IEqualityComparer<R> rightComparer;
    /// <summary>Left key comparer.</summary>
    public IEqualityComparer<L> LeftComparer { get => leftComparer; protected set => leftComparer = value; }
    /// <summary>Right key comparer.</summary>
    public IEqualityComparer<R> RightComparer { get => rightComparer; protected set => rightComparer = value; }

    /// <summary>Left side type</summary>
    public override Type LeftType => typeof(L);
    /// <summary>Right side type</summary>
    public override Type RightType => typeof(R);

    /// <summary>Test if is empty.</summary>
    public bool IsEmpty => tableLeft.Count == 0;
    /// <summary>Get entry count.</summary>
    public int Count => tableLeft.Count;

    /// <summary>Index an element with left-side key.</summary>
    /// <param name="left"></param>
    /// <returns>right</returns>
    public R this[L left] { get => GetRight(left); set => Put(left, value); }

    /// <summary>Create bijection map.</summary>
    public BijectionMap()
    {
        tableLeft = new Dictionary<L, R>();
        tableRight = new Dictionary<R, L>();
        this.leftComparer = EqualityComparer<L>.Default;
        this.rightComparer = EqualityComparer<R>.Default;
    }

    /// <summary>Create bijection map with <paramref name="leftComparer"/> and <paramref name="rightComparer"/>.</summary>
    /// <param name="leftComparer">(optional) comparer</param>
    /// <param name="rightComparer">(optional) comparer</param>
    public BijectionMap(IEqualityComparer<L>? leftComparer, IEqualityComparer<R>? rightComparer)
    {
        this.leftComparer = leftComparer ?? EqualityComparer<L>.Default;
        this.rightComparer = rightComparer ?? EqualityComparer<R>.Default;
        tableLeft = new Dictionary<L, R>(LeftComparer);
        tableRight = new Dictionary<R, L>(RightComparer);
    }

    /// <summary>Create bijection map with default comparers, copy initial values from <paramref name="copyFrom"/>.</summary>
    /// <param name="copyFrom"></param>
    public BijectionMap(IEnumerable<KeyValuePair<L, R>> copyFrom)
    {
        if (copyFrom is BijectionMap<L, R> map)
        {
            this.leftComparer = map.LeftComparer;
            this.rightComparer = map.RightComparer;
        }
        else
        {
            this.leftComparer = EqualityComparer<L>.Default;
            this.rightComparer = EqualityComparer<R>.Default;
        }
        tableLeft = new Dictionary<L, R>(LeftComparer);
        tableRight = new Dictionary<R, L>(RightComparer);
        AddAll(copyFrom);
    }

    /// <summary>Add contents from <paramref name="source"/>.</summary>
    /// <param name="source"></param>
    public BijectionMap<L, R> AddAll(IEnumerable<KeyValuePair<L, R>> source)
    {
        // Assert writable
        this.AssertWritable();
        // Add all
        foreach (var entry in source)
        {
            tableLeft[entry.Key] = entry.Value;
            tableRight[entry.Value] = entry.Key;
        }
        // Return
        return this;
    }

    /// <summary>Retain all entries whose left side is within <paramref name="leftValuesToRetain"/>.</summary>
    /// <param name="leftValuesToRetain"></param>
    public BijectionMap<L, R> RetainAllLeft(IEnumerable<L> leftValuesToRetain)
    {
        // Assert writable
        this.AssertWritable();
        // Convert to collection
        ICollection<L> leftValuesToRetain_ = (leftValuesToRetain as ICollection<L>) ?? new HashSet<L>(leftValuesToRetain);
        // Values to remove
        ICollection<L> remove = new List<L>(Count);
        // Test each value whether to keep or remove
        foreach (L lValue in tableLeft.Keys)
        {
            // Retain
            if (leftValuesToRetain_.Contains(lValue)) continue;
            // Add to remove list
            remove.Add(lValue);
        }
        // Remove values
        foreach (L lValue in remove) RemoveWithLeft(lValue);
        // Return
        return this;
    }

    /// <summary>Retain all entries whose right side is within <paramref name="rightValuesToRetain"/>.</summary>
    /// <param name="rightValuesToRetain"></param>
    public BijectionMap<L, R> RetainAllRight(IEnumerable<R> rightValuesToRetain)
    {
        // Assert writable
        this.AssertWritable();
        // Convert to collection
        ICollection<R> rightValuesToRetain_ = (rightValuesToRetain as ICollection<R>) ?? new HashSet<R>(rightValuesToRetain);
        // Probably not the best of implementations
        ICollection<R> remove = new List<R>(Count);
        foreach (R rValue in tableRight.Keys)
        {
            // Retain
            if (rightValuesToRetain_.Contains(rValue)) continue;
            // Add to remove list
            remove.Add(rValue);
        }
        // Remove values
        foreach (R rValue in remove) RemoveWithRight(rValue);
        // Return
        return this;
    }

    /// <summary>Check if left values contain <paramref name="leftValue"/>.</summary>
    /// <param name="leftValue"></param>
    /// <returns></returns>
    public bool ContainsLeft(L leftValue) => tableLeft.ContainsKey(leftValue);

    /// <summary>Check if right values contain <paramref name="rightValue"/>.</summary>
    /// <param name="rightValue"></param>
    /// <returns></returns>
    public bool ContainsRight(R rightValue) => tableRight.ContainsKey(rightValue);

    /// <summary>Test if contains a pair.</summary>
    /// <param name="leftValue"></param>
    /// <param name="rightValue"></param>
    /// <returns></returns>
    public bool Contains(L leftValue, R rightValue)
    {
        // Assert not null
        if (leftValue == null || rightValue == null) return false;
        // 
        if (!tableLeft.TryGetValue(leftValue, out R? r)) return false;
        // 
        return RightComparer.Equals(r, rightValue);
    }

    /// <summary>Put pair into map.</summary>
    /// <param name="leftValue"></param>
    /// <param name="rightValue"></param>
    public void Put(L leftValue, R rightValue)
    {
        // Assert writable
        this.AssertWritable();
        // Assert not null
        if (leftValue == null) throw new ArgumentNullException(nameof(leftValue));
        if (rightValue == null) throw new ArgumentNullException(nameof(rightValue));
        // Remove possible old entry
        if (tableLeft.TryGetValue(leftValue, out R? oldR)) tableRight.Remove(oldR);
        if (tableRight.TryGetValue(rightValue, out L? oldL)) tableLeft.Remove(oldL);
        // Assign
        tableLeft[leftValue] = rightValue;
        tableRight[rightValue] = leftValue;
    }

    /// <summary>Remove (left, right) pair.</summary>
    /// <param name="leftValue"></param>
    /// <param name="rightValue"></param>
    /// <returns></returns>
    public bool Remove(L leftValue, R rightValue)
    {
        // Assert writable
        this.AssertWritable();
        // Assert not null
        if (leftValue == null || rightValue == null) return false;
        //
        if (!tableRight.TryGetValue(rightValue, out L? oldL)) return false;
        if (!LeftComparer.Equals(oldL, leftValue)) return false;
        tableRight.Remove(rightValue);
        tableLeft.Remove(oldL);
        return true;
    }

    /// <summary>Get value on the left with right key.</summary>
    /// <param name="rightValue"></param>
    /// <returns></returns>
    public L GetLeft(R rightValue) => tableRight[rightValue];

    /// <summary>Try get value on the left with right key.</summary>
    /// <param name="rightValue"></param>
    /// <param name="leftValue"></param>
    /// <returns></returns>
    public bool TryGetLeft(R rightValue, out L? leftValue) => tableRight.TryGetValue(rightValue, out leftValue);

    /// <summary>Get value on the right with left key.</summary>
    /// <param name="leftValue"></param>
    /// <returns></returns>
    public R GetRight(L leftValue) => tableLeft[leftValue];

    /// <summary>Try get value on the right with left key.</summary>
    /// <param name="leftValue"></param>
    /// <param name="rightValue"></param>
    /// <returns></returns>
    public bool TryGetRight(L leftValue, out R? rightValue) => tableLeft.TryGetValue(leftValue, out rightValue);

    /// <summary>Remove with left key.</summary>
    /// <param name="leftValue"></param>
    /// <returns></returns>
    public R? RemoveWithLeft(L leftValue)
    {
        // Assert writable
        this.AssertWritable();
        // 
        R? oldR = default(R);
        if (tableLeft.TryGetValue(leftValue, out oldR))
        {
            tableRight.Remove(oldR);
            tableLeft.Remove(leftValue);
        }
        // 
        return oldR;
    }

    /// <summary>Remove with right key.</summary>
    /// <param name="rightValue"></param>
    /// <returns></returns>
    public L? RemoveWithRight(R rightValue)
    {
        // Assert writable
        this.AssertWritable();
        //
        L? oldL = default(L);
        //
        if (tableRight.TryGetValue(rightValue, out oldL))
        {
            tableLeft.Remove(oldL);
            tableRight.Remove(rightValue);
        }
        // 
        return oldL;
    }

    /// <summary>Get set of left values</summary>
    /// <returns>Left values</returns>
    public ICollection<L> GetLeftSet() => tableLeft.Keys;

    /// <summary>Get set of right values</summary>
    /// <returns>Right values</returns>
    public ICollection<R> GetRightSet() => tableRight.Keys;

    /// <summary>Get left-to-right Dictionary</summary>
    /// <returns>left-to-right Dictionary</returns>
    public Dictionary<L, R> GetLeftToRightDictionary() => tableLeft;

    /// <summary>Get right-to-left Dictionary</summary>
    /// <returns>right-to-left Dictionary</returns>
    public Dictionary<R, L> GetRightToLeftDictionary() => tableRight;

    /// <summary>Clear entriees.</summary>
    public virtual void Clear()
    {
        // Assert writable
        this.AssertWritable();
        //
        tableLeft.Clear();
        tableRight.Clear();
    }

    /// <summary>Print info.</summary>
    public override string ToString()
    {
        int count = 0;
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        foreach (var e in tableLeft)
        {
            if (count++ > 0) sb.Append(", ");
            sb.Append(e.Key);
            sb.Append("=");
            sb.Append(e.Value);
        }
        sb.Append("]");
        return sb.ToString();
    }

    /// <summary>Create clone.</summary>
    public override object Clone()
    {
        BijectionMap<L, R> clone = new BijectionMap<L, R>(tableLeft.Comparer, tableRight.Comparer);
        clone.AddAll(this);
        return clone;
    }

    /// <summary>Enumerate pairs.</summary>
    public new IEnumerator<KeyValuePair<L, R>> GetEnumerator() => tableLeft.GetEnumerator();
    /// <summary>Enumerate pairs.</summary>
    protected override IEnumerator getEnumerator() => tableLeft.GetEnumerator();

    /// <summary>Workaround</summary>
    public class _ : BijectionMap<L, R>
    {
        /// <summary></summary>
        public _(object leftComparer, object rightComparer) : base((IEqualityComparer<L>)leftComparer, (IEqualityComparer<R>)rightComparer) { }
    }

}
