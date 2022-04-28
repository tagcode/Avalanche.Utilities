// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalanche.Utilities.Record;

/// <summary>Compares an innter value of a container using a reader delegate.</summary>
public abstract class DelegateEqualityComparer : ReadOnlyAssignableClass, IEqualityComparer, IGraphEqualityComparer, ICyclical
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT2<object, Delegate, DelegateEqualityComparer> constructor = new(typeof(DelegateEqualityComparer<,>));

    /// <summary>Create comparer</summary>
    /// <param name="containerType"></param>
    /// <param name="valueType"></param>
    /// <param name="valueEqualityComparer"></param>
    /// <param name="fieldReader"></param>
    public static DelegateEqualityComparer Create(Type containerType, Type valueType, object valueEqualityComparer, Delegate fieldReader) => constructor.Create(containerType, valueType, valueEqualityComparer, fieldReader);

    /// <summary>Value equality comparer</summary>
    public abstract object ValueComparer { get; }
    /// <summary>Value reader function</summary>
    public abstract Delegate ValueReader { get; }

    /// <summary>Explicitly assigned <see cref="IsCyclical"/> value.</summary>
    protected bool isCyclical;
    /// <summary></summary>
    public virtual bool IsCyclical { get => isCyclical; set => this.AssertWritable().isCyclical = value; }

    /// <summary></summary>
    public abstract bool Equals(object? x, object? y, IGraphComparerContext2 context);
    /// <summary></summary>
    public abstract int GetHashCode([DisallowNull] object obj, IGraphComparerContext context);
    /// <summary></summary>
    public abstract new bool Equals(object? x, object? y);
    /// <summary></summary>
    public abstract int GetHashCode(object obj);

    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphEqualityComparer.Context2"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphComparerContext? setContext(IGraphComparerContext? context) { IGraphEqualityComparer.Context.Value = context; return context; }
    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphEqualityComparer.Context2"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphComparerContext2? setContext(IGraphComparerContext2? context) { IGraphEqualityComparer.Context2.Value = context; return context; }
}

/// <summary>Compares an innter value of a container using a reader delegate.</summary>
public abstract class DelegateEqualityComparer<Container> : DelegateEqualityComparer, IEqualityComparer<Container>, IGraphEqualityComparer<Container>
{
    /// <summary></summary>
    public abstract bool Equals(Container? x, Container? y);
    /// <summary></summary>
    public abstract bool Equals(Container? x, Container? y, IGraphComparerContext2 context);

    /// <summary></summary>
    public abstract int GetHashCode([DisallowNull] Container obj);
    /// <summary></summary>
    public abstract int GetHashCode([DisallowNull] Container obj, IGraphComparerContext context);
}

/// <summary>Compares an innter value of a container using a reader delegate.</summary>
public abstract class DelegateEqualityComparer_<Container, Value> : DelegateEqualityComparer<Container>
{
    /// <summary>Value equality comparer</summary>
    public override object ValueComparer => (object?)valueComparer ?? graphComparer!;
    /// <summary>Value reader function</summary>
    public override Delegate ValueReader => (Delegate?)valueReader ?? valueReader2!;

    /// <summary>Field value equality comparer</summary>
    protected object valueComparer;
    /// <summary>Field value equality comparer</summary>
    protected IEqualityComparer<Value>? comparer;
    /// <summary>Field value equality comparer</summary>
    protected IGraphEqualityComparer<Value>? graphComparer;
    /// <summary>Field reader function</summary>
    protected Func<Container, Value>? valueReader = null;
    /// <summary>Field reader function</summary>
    protected FieldRead<Container, Value>? valueReader2 = null;

    /// <summary>Create field comparer</summary>
    /// <param name="valueComparer"><![CDATA[IEqualityComparer<Value>]]> and/or <![CDATA[IGraphEqualityComparer<Value>]]></param>
    /// <param name="valueReader"><![CDATA[Func<Container, Value>]]> or <![CDATA[FieldRead<Container, Value>]]></param>
    public DelegateEqualityComparer_(object valueComparer, Delegate valueReader)
    {
        this.valueComparer = valueComparer ?? throw new ArgumentNullException(nameof(DelegateEqualityComparer_<Container, Value>.valueComparer));
        this.comparer = this.valueComparer as IEqualityComparer<Value>;
        this.graphComparer = this.valueComparer as IGraphEqualityComparer<Value>;

        // Assign value reader
        if (valueReader is Func<Container, Value> _1) { this.valueReader = _1; this.valueReader2 = null; }
        else if (valueReader is FieldRead<Container, Value> _2) { this.valueReader = null!; this.valueReader2 = _2; }
        else throw new ArgumentNullException(nameof(valueReader));

        // Cast to either failed
        if (valueComparer == null && graphComparer == null) throw new InvalidOperationException($"{nameof(DelegateEqualityComparer_<Container, Value>.valueComparer)} must implement {CanonicalName.Print(typeof(IEqualityComparer<Value>))} and/or {CanonicalName.Print(typeof(IGraphEqualityComparer<Value>))}");
        //
        //this.isCyclical |= graphComparer != null;
    }

    /// <summary></summary>
    public override bool Equals(object? x, object? y)
    {
        // Nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // 
        if (object.ReferenceEquals(x, y)) return true;
        // Cast
        if (x is Container xr && y is Container yr)
        {
            // Place values here
            Value xvalue, yvalue;
            // Read values
            if (valueReader != null) { xvalue = valueReader(xr); yvalue = valueReader(yr); }
            else if (valueReader2 != null) { xvalue = valueReader2(ref xr); yvalue = valueReader2(ref yr); }
            else throw new InvalidOperationException($"value reader is not assigned");

            // Graph comparison
            if (IsCyclical || graphComparer != null)
            {
                // Get previous context
                IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
                // Place here context
                IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
                try
                {
                    // Compare values
                    bool equals = graphComparer != null ? graphComparer!.Equals(xvalue, yvalue, context) : comparer!.Equals(xvalue, yvalue);
                    // Return
                    return equals;
                }
                finally
                {
                    // Revert thread local
                    IGraphEqualityComparer.Context2.Value = prevContext;
                }
            }
            // Regular comparison
            else
            {
                // Compare values
                bool equals = comparer!.Equals(xvalue, yvalue);
                // Return
                return equals;
            }
        }
        // Not records
        return x.Equals(y);
    }

    /// <summary></summary>
    public override bool Equals(object? x, object? y, IGraphComparerContext2 context)
    {
        // Nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // 
        if (object.ReferenceEquals(x, y)) return true;
        // Cast
        if (x is Container xr && y is Container yr)
        {
            // Place values here
            Value xvalue, yvalue;
            // Read values
            if (valueReader != null) { xvalue = valueReader(xr); yvalue = valueReader(yr); }
            else if (valueReader2 != null) { xvalue = valueReader2(ref xr); yvalue = valueReader2(ref yr); }
            else throw new InvalidOperationException($"value reader is not assigned");

            // Graph compare
            if (graphComparer != null) return graphComparer!.Equals(xvalue, yvalue, context);

            // Get previous context
            IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
            // Assign thread local
            IGraphEqualityComparer.Context2.Value = context;
            try
            {
                // Compare
                return comparer!.Equals(xvalue, yvalue);
            }
            finally
            {
                // Revert thread-local
                IGraphEqualityComparer.Context2.Value = prevContext;
            }
        }
        // Not records
        return x.Equals(y);
    }

    /// <summary></summary>
    public override int GetHashCode(object obj)
    {
        //
        if (obj is Container record)
        {
            // Place value here
            Value value;
            // Read values
            if (valueReader != null) value = valueReader(record);
            else if (valueReader2 != null) value = valueReader2(ref record);
            else throw new InvalidOperationException($"value reader is not assigned");

            // Got null
            if (value == null) return 0;

            // Graph comparison
            if (IsCyclical || graphComparer != null)
            {
                // Get previous context
                IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
                // Place here context
                IGraphComparerContext context = prevContext ?? setContext(new GraphComparerContext())!;
                try
                {
                    // Get value hash
                    int hash = graphComparer != null ? graphComparer!.GetHashCode(value, context) : comparer!.GetHashCode(value);
                    // Return
                    return hash;
                }
                finally
                {
                    // Revert thread local
                    IGraphEqualityComparer.Context.Value = prevContext;
                }
            }
            // Regular comparison
            else
            {
                // Get value hash
                int hash = comparer!.GetHashCode(value);
                // Return
                return hash;
            }
        }
        // Handle nulls
        else return obj.GetHashCode();
    }

    /// <summary></summary>
    public override int GetHashCode([DisallowNull] object obj, IGraphComparerContext context)
    {
        //
        if (obj is Container record)
        {
            // Place value here
            Value value;
            // Read values
            if (valueReader != null) value = valueReader(record);
            else if (valueReader2 != null) value = valueReader2(ref record);
            else throw new InvalidOperationException($"value reader is not assigned");

            // Got null
            if (value == null) return 0;

            // Graph hasher
            if (graphComparer != null) return graphComparer!.GetHashCode(value, context);

            // Get previous context
            IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
            // Assign thread local
            IGraphEqualityComparer.Context.Value = context;
            try
            {
                // Compare
                return comparer!.GetHashCode(value!);
            }
            finally
            {
                // Revert thread-local
                IGraphEqualityComparer.Context.Value = prevContext;
            }
        }
        // Handle nulls
        else return obj.GetHashCode();
    }
}

/// <summary>Compares an innter value of a container using a reader delegate.</summary>
public class DelegateEqualityComparer<Container, Value> : DelegateEqualityComparer_<Container, Value>
{
    /// <summary>Create field comparer</summary>
    /// <param name="comparer"><![CDATA[IEqualityComparer<Value>]]> and/or <![CDATA[IGraphEqualityComparer<Value>]]></param>
    /// <param name="valueReader"><![CDATA[Func<Container, Value>]]> or <![CDATA[FieldRead<Container, Value>]]></param>
    public DelegateEqualityComparer(object comparer, Delegate valueReader) : base(comparer, valueReader) { }

    /// <summary></summary>
    public override bool Equals(Container? xr, Container? yr)
    {
        // Handle nulls
        if (xr == null && yr == null) return true;
        if (xr == null || yr == null) return false;
        // 
        if (!typeof(Container).IsValueType && object.ReferenceEquals(xr, yr)) return true;

        // Place values here
        Value xvalue, yvalue;
        // Read values
        if (valueReader != null) { xvalue = valueReader(xr); yvalue = valueReader(yr); }
        else if (valueReader2 != null) { xvalue = valueReader2(ref xr); yvalue = valueReader2(ref yr); }
        else throw new InvalidOperationException($"value reader is not assigned");

        // Graph comparison
        if (IsCyclical || graphComparer != null)
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
            // Place here context
            IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
            try
            {
                // Compare values
                bool equals = graphComparer != null ? graphComparer!.Equals(xvalue, yvalue, context) : comparer!.Equals(xvalue, yvalue);
                // Return
                return equals;
            }
            finally
            {
                // Revert thread local
                IGraphEqualityComparer.Context2.Value = prevContext;
            }
        }
        // Regular comparison
        else
        {
            // Compare values
            bool equals = comparer!.Equals(xvalue, yvalue);
            // Return
            return equals;
        }
    }


    /// <summary></summary>
    public override bool Equals(Container? xr, Container? yr, IGraphComparerContext2 context)
    {
        // Handle nulls
        if (xr == null && yr == null) return true;
        if (xr == null || yr == null) return false;
        // 
        if (!typeof(Container).IsValueType && object.ReferenceEquals(xr, yr)) return true;

        // Place values here
        Value xvalue, yvalue;
        // Read values
        if (valueReader != null) { xvalue = valueReader(xr); yvalue = valueReader(yr); }
        else if (valueReader2 != null) { xvalue = valueReader2(ref xr); yvalue = valueReader2(ref yr); }
        else throw new InvalidOperationException($"value reader is not assigned");

        // Graph compare
        if (graphComparer != null) return graphComparer!.Equals(xvalue, yvalue, context);

        // Get previous context
        IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
        // Assign thread local
        IGraphEqualityComparer.Context2.Value = context;
        try
        {
            // Compare
            return comparer!.Equals(xvalue, yvalue);
        }
        finally
        {
            // Revert thread-local
            IGraphEqualityComparer.Context2.Value = prevContext;
        }
    }

    /// <summary></summary>
    public override int GetHashCode([DisallowNull] Container record)
    {
        // Handle nulls
        if (record == null) return 0;
        // Place value here
        Value value;
        // Read values
        if (valueReader != null) value = valueReader(record);
        else if (valueReader2 != null) value = valueReader2(ref record);
        else throw new InvalidOperationException($"value reader is not assigned");
        // Got null
        if (value == null) return 0;

        // Graph comparison
        if (IsCyclical || graphComparer != null)
        {
            // Get previous context
            IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
            // Place here context
            IGraphComparerContext context = prevContext ?? setContext(new GraphComparerContext())!;
            try
            {
                // Get value hash
                int hash = graphComparer != null ? graphComparer!.GetHashCode(value, context) : comparer!.GetHashCode(value);
                // Return
                return hash;
            }
            finally
            {
                // Revert thread local
                IGraphEqualityComparer.Context.Value = prevContext;
            }
        }
        // Regular comparison
        else
        {
            // Get value hash
            int hash = comparer!.GetHashCode(value);
            // Return
            return hash;
        }
    }

    /// <summary></summary>
    public override int GetHashCode([DisallowNull] Container record, IGraphComparerContext context)
    {
        // Handle nulls
        if (record == null) return 0;
        // Place value here
        Value value;
        // Read values
        if (valueReader != null) value = valueReader(record);
        else if (valueReader2 != null) value = valueReader2(ref record);
        else throw new InvalidOperationException($"value reader is not assigned");
        // Got null
        if (value == null) return 0;

        // Graph hasher
        if (graphComparer != null) return graphComparer!.GetHashCode(value, context);

        // Get previous context
        IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
        // Assign thread local
        IGraphEqualityComparer.Context.Value = context;
        try
        {
            // Compare
            return comparer!.GetHashCode(value!);
        }
        finally
        {
            // Revert thread-local
            IGraphEqualityComparer.Context.Value = prevContext;
        }
    }
}
