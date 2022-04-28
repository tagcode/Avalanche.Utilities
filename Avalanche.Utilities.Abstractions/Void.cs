// Copyright (c) Toni Kalajainen 2022
namespace Avalanche;
using System;
using System.Diagnostics.CodeAnalysis;

// <docs>
/// <summary>Void type</summary>
public struct Void : IComparable, ICloneable, IComparable<Void>, IEquatable<Void>
{
    /// <summary>Unboxed</summary>
    public static Void Default => new Void();
    /// <summary>Boxed singleton</summary>
    static object box = new Void();
    /// <summary>Boxed singleton</summary>
    public static object Box => box;
    /// <summary>Calculate hash-code</summary>
    public int GetHashCode([DisallowNull] Void obj) => 0;
    /// <summary>Calculate hash-code</summary>
    public override int GetHashCode() => 0;
    /// <summary>Compare equality to <paramref name="other"/></summary>
    public override bool Equals(object? other) => other is Void;
    /// <summary>Compare equality to <paramref name="other"/>.</summary>
    bool IEquatable<Void>.Equals(Void other) => true;
    /// <summary>Compare to <paramref name="other"/>.</summary>
    int IComparable.CompareTo(object? other) => other is Void ? 0 : -1;
    /// <summary>Compare to <paramref name="other"/>.</summary>
    int IComparable<Void>.CompareTo(Void other) => 0;
    /// <summary>Clone</summary>
    public object Clone() => new Void();
    /// <summary>Print information</summary>
    public override string ToString() => "Void";

    /// <summary></summary>
    public static bool operator ==(Void left, Void right) => true;
    /// <summary></summary>
    public static bool operator !=(Void left, Void right) => false;
    /// <summary></summary>
    public static bool operator <(Void left, Void right) => false;
    /// <summary></summary>
    public static bool operator <=(Void left, Void right) => true;
    /// <summary></summary>
    public static bool operator >(Void left, Void right) => false;
    /// <summary></summary>
    public static bool operator >=(Void left, Void right) => true;
}
// </docs>
