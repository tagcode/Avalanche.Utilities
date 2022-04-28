// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Avalanche.Utilities.Record;

/// <summary>Compares an innter value of a container using a reader delegate.</summary>
public abstract class DelegateComparer : ReadOnlyAssignableClass, IComparer, IGraphComparer, ICyclical
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT2<object, Delegate, DelegateComparer> constructor = new(typeof(DelegateComparer<,>));

    /// <summary>Create comparer</summary>
    /// <param name="containerType"></param>
    /// <param name="valueType"></param>
    /// <param name="valueComparer"><![CDATA[IComparer<Value>]]></param>
    /// <param name="valueReader"><![CDATA[Func<Container, Value>]]></param>
    public static DelegateComparer Create(Type containerType, Type valueType, object valueComparer, Delegate valueReader) => constructor.Create(containerType, valueType, valueComparer, valueReader);

    /// <summary>Value equality comparer</summary>
    public abstract object ValueComparer { get; }
    /// <summary>Value reader function</summary>
    public abstract Delegate ValueReader { get; }

    /// <summary>Explicitly assigned <see cref="IsCyclical"/> value.</summary>
    protected bool isCyclical;
    /// <summary></summary>
    public virtual bool IsCyclical { get => isCyclical; set => this.AssertWritable().isCyclical = value; }

    /// <summary></summary>
    public abstract int Compare(object? x, object? y);
    /// <summary></summary>
    public abstract int Compare(object? x, object? y, IGraphComparerContext2 context);

    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphComparer.Context2"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphComparerContext2? setContext(IGraphComparerContext2? context) { IGraphComparer.Context2.Value = context; return context; }
}

/// <summary>Compares an innter value of a container using a reader delegate.</summary>
public abstract class DelegateComparer<Container> : DelegateComparer, IComparer<Container>, IGraphComparer<Container>
{
    /// <summary></summary>
    public abstract int Compare(Container? x, Container? y);
    /// <summary></summary>
    public abstract int Compare(Container? x, Container? y, IGraphComparerContext2 context);
}

/// <summary>Compares an innter value of a container using a reader delegate.</summary>
public abstract class DelegateComparer_<Container, Value> : DelegateComparer<Container>
{
    /// <summary>Value equality comparer</summary>
    public override object ValueComparer => valueComparer;
    /// <summary>Value reader function</summary>
    public override Delegate ValueReader => (Delegate?)valueReader ?? valueReader2!;

    /// <summary>Field value equality comparer</summary>
    protected object valueComparer;
    /// <summary>Field value equality comparer</summary>
    protected IComparer<Value>? comparer;
    /// <summary>Field value equality comparer</summary>
    protected IGraphComparer<Value>? graphComparer;
    /// <summary>Field reader function</summary>
    protected Func<Container, Value>? valueReader = null;
    /// <summary>Field reader function</summary>
    protected FieldRead<Container, Value>? valueReader2 = null;

    /// <summary>Create field comparer</summary>
    /// <param name="valueComparer">Field value comparer as <![CDATA[IComparer<Value>]]></param>
    /// <param name="valueReader">Reader as <![CDATA[Func<Container, Value>]]> or <![CDATA[FieldRead<Container, Value>]]></param>
    public DelegateComparer_(object valueComparer, Delegate valueReader)
    {
        this.valueComparer = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
        this.comparer = valueComparer as IComparer<Value>;
        this.graphComparer = valueComparer as IGraphComparer<Value>;

        // Assign value reader
        if (valueReader is Func<Container, Value> _1) { this.valueReader = _1; this.valueReader2 = null; }
        else if (valueReader is FieldRead<Container, Value> _2) { this.valueReader = null!; this.valueReader2 = _2; }
        else throw new ArgumentNullException(nameof(valueReader));

        // Cast to either failed
        if (comparer == null && graphComparer == null) throw new InvalidOperationException($"{nameof(valueComparer)} must implement {CanonicalName.Print(typeof(IComparer<Value>))} and/or {CanonicalName.Print(typeof(IGraphComparer<Value>))}");

        //this.isCyclical |= this.graphComparer != null;
    }

    /// <summary></summary>
    public override int Compare(object? _x, object? _y)
    {
        // Same reference
        if (object.ReferenceEquals(_x, _y)) return 0;
        // Check nulls
        if (_x == null && _y == null) return 0;
        if (_x == null) return -1;
        if (_y == null) return 1;
        //
        if (_x is Container x && _y is Container y)
        {
            // Place values here
            Value xvalue, yvalue;
            // Read values
            if (valueReader != null) { xvalue = valueReader(x); yvalue = valueReader(y); }
            else if (valueReader2 != null) { xvalue = valueReader2(ref x); yvalue = valueReader2(ref y); }
            else throw new InvalidOperationException($"value reader is not assigned");

            // Graph comparison
            if (IsCyclical || graphComparer != null)
            {
                // Get previous context
                IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
                // Place here context
                IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
                try
                {
                    // Compare values
                    int d = graphComparer != null ? graphComparer!.Compare(xvalue, yvalue, context) : comparer!.Compare(xvalue, yvalue);
                    // Return
                    return d;
                }
                finally
                {
                    // Revert thread local
                    IGraphComparer.Context2.Value = prevContext;
                }
            }
            // Regular comparison
            else
            {
                // Compare values
                int d = comparer!.Compare(xvalue, yvalue);
                // Return
                return d;
            }
        }
        // Cannot discern
        else return 0;
    }

    /// <summary></summary>
    public override int Compare(object? _x, object? _y, IGraphComparerContext2 context)
    {
        // Same reference
        if (object.ReferenceEquals(_x, _y)) return 0;
        // Check nulls
        if (_x == null && _y == null) return 0;
        if (_x == null) return -1;
        if (_y == null) return 1;
        //
        if (_x is Container x && _y is Container y)
        {
            // Place values here
            Value xvalue, yvalue;
            // Read values
            if (valueReader != null) { xvalue = valueReader(x); yvalue = valueReader(y); }
            else if (valueReader2 != null) { xvalue = valueReader2(ref x); yvalue = valueReader2(ref y); }
            else throw new InvalidOperationException($"value reader is not assigned");

            // Graph compare
            if (graphComparer != null) return graphComparer!.Compare(xvalue, yvalue, context);

            // Get previous context
            IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
            // Assign thread local
            IGraphComparer.Context2.Value = context;
            try
            {
                // Compare
                return comparer!.Compare(xvalue, yvalue);
            }
            finally
            {
                // Revert thread-local
                IGraphComparer.Context2.Value = prevContext;
            }
        }
        // Cannot discern
        else return 0;
    }
}

/// <summary>Compares an innter value of a container using a reader delegate.</summary>
public class DelegateComparer<Container, Value> : DelegateComparer_<Container, Value>
{
    /// <summary>Create field comparer</summary>
    /// <param name="valueComparer">Field value comparer as <![CDATA[IComparer<Value>]]></param>
    /// <param name="valueReader">Reader as <![CDATA[Func<Container, Value>]]> or <![CDATA[FieldRead<Container, Value>]]></param>
    public DelegateComparer(object valueComparer, Delegate valueReader) : base(valueComparer, valueReader) { }

    /// <summary></summary>
    public override int Compare(Container? x, Container? y)
    {
        // Same reference
        if (!typeof(Container).IsValueType && object.ReferenceEquals(x, y)) return 0;
        // Check nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        // Place values here
        Value xvalue, yvalue;
        // Read values
        if (valueReader != null) { xvalue = valueReader(x); yvalue = valueReader(y); }
        else if (valueReader2 != null) { xvalue = valueReader2(ref x); yvalue = valueReader2(ref y); }
        else throw new InvalidOperationException($"value reader is not assigned");

        // Graph comparison
        if (IsCyclical || graphComparer != null)
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
            // Place here context
            IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
            try
            {
                // Compare values
                int d = graphComparer != null ? graphComparer!.Compare(xvalue, yvalue, context) : comparer!.Compare(xvalue, yvalue);
                // Return
                return d;
            }
            finally
            {
                // Revert thread local
                IGraphComparer.Context2.Value = prevContext;
            }
        }
        // Regular comparison
        else
        {
            // Compare values
            int d = comparer!.Compare(xvalue, yvalue);
            // Return
            return d;
        }
    }

    /// <summary></summary>
    public override int Compare(Container? x, Container? y, IGraphComparerContext2 context)
    {
        // Same reference
        if (!typeof(Container).IsValueType && object.ReferenceEquals(x, y)) return 0;
        // Check nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        // Place values here
        Value xvalue, yvalue;
        // Read values
        if (valueReader != null) { xvalue = valueReader(x); yvalue = valueReader(y); }
        else if (valueReader2 != null) { xvalue = valueReader2(ref x); yvalue = valueReader2(ref y); }
        else throw new InvalidOperationException($"value reader is not assigned");

        // Graph compare
        if (graphComparer != null) return graphComparer!.Compare(xvalue, yvalue, context);

        // Get previous context
        IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
        // Assign thread local
        IGraphComparer.Context2.Value = context;
        try
        {
            // Compare
            return comparer!.Compare(xvalue, yvalue);
        }
        finally
        {
            // Revert thread-local
            IGraphComparer.Context2.Value = prevContext;
        }
    }
}

