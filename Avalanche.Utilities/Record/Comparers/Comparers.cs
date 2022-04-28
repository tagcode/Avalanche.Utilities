// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>A record of comparers for a type.</summary>
public record Comparers : ReadOnlyAssignableRecord, IComparers
{
    /// <summary>ctor</summary>
    static readonly ConstructorT<Comparers> constructor = new(typeof(Comparers<>));
    /// <summary>Create <see cref="Comparers{T}"/></summary>
    public static Comparers Create(Type type) => constructor.Create(type);

    /// <summary>Data type interface or type</summary>
    Type type = null!;
    /// <summary>Does the type have cyclical structure.</summary>
    bool isCyclical;
    /// <summary>Equality comparer for datatype instances</summary>
    protected IEqualityComparer equalityComparer = null!;
    /// <summary>Comparer for datatype instances</summary>
    protected IComparer comparer = null!;
    /// <summary></summary>
    protected IGraphEqualityComparer graphEqualityComparer = null!;
    /// <summary></summary>
    protected IGraphComparer graphComparer = null!;

    /// <summary>Equality comparer for datatype instances</summary>
    protected virtual Comparers setType(Type type) { this.AssertWritable().type = type; return this; }
    /// <summary>Assign info about possible cyclical structure of <see cref="type"/>.</summary>
    protected virtual Comparers setIsCyclical(bool value) { this.AssertWritable().isCyclical = value; return this; }
    /// <summary>Equality comparer for datatype instances</summary>
    protected virtual Comparers setEqualityComparer(IEqualityComparer value) { this.AssertWritable().equalityComparer = value; return this; }
    /// <summary>Comparer for datatype instances</summary>
    protected virtual Comparers setComparer(IComparer value) { this.AssertWritable().comparer = value; return this; }
    /// <summary></summary>
    protected virtual Comparers setGraphEqualityComparer(IGraphEqualityComparer value) { this.AssertWritable().graphEqualityComparer = value; return this; }
    /// <summary></summary>
    protected virtual Comparers setGraphComparer(IGraphComparer value) { this.AssertWritable().graphComparer = value; return this; }

    /// <summary><![CDATA[IEqualityComparer<T>]]></summary>
    public virtual object EqualityComparerT { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    /// <summary><![CDATA[IComparer<T>]]></summary>
    public virtual object ComparerT { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    /// <summary><![CDATA[IGraphEqualityComparer<T>]]></summary>
    public virtual object GraphEqualityComparerT { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    /// <summary><![CDATA[IGraphComparer<T>]]></summary>
    public virtual object GraphComparerT { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    /// <summary>Equality comparer for datatype instances</summary>
    protected virtual Comparers setEqualityComparerT(object value) => throw new NotSupportedException();
    /// <summary>Comparer for datatype instances</summary>
    protected virtual Comparers setComparerT(object value) => throw new NotSupportedException();
    /// <summary></summary>
    protected virtual Comparers setGraphEqualityComparerT(object value) => throw new NotSupportedException();
    /// <summary></summary>
    protected virtual Comparers setGraphComparerT(object value) => throw new NotSupportedException();

    /// <summary>Data type interface or type</summary>
    public Type Type { get => type; set => setType(value); }
    /// <summary>Can data type be cyclic (contain IDataType fields).</summary>
    public bool IsCyclical { get => isCyclical; set => setIsCyclical(value); }
    /// <summary>Equality comparer for datatype instances</summary>
    public IEqualityComparer EqualityComparer { get => equalityComparer; set => setEqualityComparer(value); }
    /// <summary>Comparer for datatype instances</summary>
    public IComparer Comparer { get => comparer; set => setComparer(value); }
    /// <summary></summary>
    public IGraphEqualityComparer GraphEqualityComparer { get => graphEqualityComparer; set => setGraphEqualityComparer(value); }
    /// <summary></summary>
    public IGraphComparer GraphComparer { get => graphComparer; set => setGraphComparer(value); }
}

/// <summary>A record of comparers for <typeparamref name="T"/>.</summary>
public record Comparers<T> : Comparers, IComparers<T>
{
    /// <summary>.ctor</summary>
    public Comparers() : base() => this.Type = typeof(T);

    /// <summary>Equality comparer for datatype instances</summary>
    protected new IEqualityComparer<T> equalityComparer = null!;
    /// <summary>Comparer for datatype instances</summary>
    protected new IComparer<T> comparer = null!;
    /// <summary>Equality comparer for datatype instances</summary>
    protected new IGraphEqualityComparer<T> graphEqualityComparer = null!;
    /// <summary>Comparer for datatype instances</summary>
    protected new IGraphComparer<T> graphComparer = null!;

    /// <summary><![CDATA[IEqualityComparer<T>]]></summary>
    public override object EqualityComparerT { get => equalityComparer; set => setEqualityComparerT(value); }
    /// <summary><![CDATA[IComparer<T>]]></summary>
    public override object ComparerT { get => comparer; set => setComparerT(value); }
    /// <summary><![CDATA[IGraphEqualityComparer<T>]]></summary>
    public override object GraphEqualityComparerT { get => graphEqualityComparer; set => setGraphEqualityComparerT(value); }
    /// <summary><![CDATA[IGraphComparer<T>]]></summary>
    public override object GraphComparerT { get => graphComparer; set => setGraphComparerT(value); }

    /// <summary>Equality comparer for datatype instances</summary>
    protected override Comparers setEqualityComparerT(object value) { this.AssertWritable().equalityComparer = (IEqualityComparer<T>)value; return this; }
    /// <summary>Comparer for datatype instances</summary>
    protected override Comparers setComparerT(object value) { this.AssertWritable().comparer = (IComparer<T>)value; return this; }
    /// <summary></summary>
    protected override Comparers setGraphEqualityComparerT(object value) { this.AssertWritable().graphEqualityComparer = (IGraphEqualityComparer<T>)value; return this; }
    /// <summary></summary>
    protected override Comparers setGraphComparerT(object value) { this.AssertWritable().graphComparer = (IGraphComparer<T>)value; return this; }

    /// <summary>Equality comparer for datatype instances</summary>
    public new IEqualityComparer<T> EqualityComparer { get => equalityComparer; set => setEqualityComparerT(value); }
    /// <summary>Comparer for datatype instances</summary>
    public new IComparer<T> Comparer { get => comparer; set => setComparerT(value); }
    /// <summary>Equality comparer for datatype instances</summary>
    public new IGraphEqualityComparer<T> GraphEqualityComparer { get => graphEqualityComparer; set => setGraphEqualityComparerT(value); }
    /// <summary>Comparer for datatype instances</summary>
    public new IGraphComparer<T> GraphComparer { get => graphComparer; set => setGraphComparerT(value); }
}

