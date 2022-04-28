// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Reflection;
using System.Collections;
using System.Reflection;

/// <summary>Enumerates non-null field values.</summary>
public abstract class FieldValues : IEnumerable
{
    /// <summary></summary>
    static readonly ConstructorT<IEnumerable<FieldInfo>, object, FieldValues> constructor = new(typeof(FieldValues<>));
    /// <summary></summary>
    public FieldValues Create(Type type, IEnumerable<FieldInfo> fields, object instance) => constructor.Create(type, fields, instance);

    /// <summary></summary>
    protected object Instance;
    /// <summary>Fields that implement <see cref="ValueType"/> or <![CDATA[IEnumerable<T>]]></summary>
    protected IEnumerable<FieldInfo> Fields;
    /// <summary></summary>
    protected abstract Type ValueType { get; }

    /// <summary></summary>
    public FieldValues(IEnumerable<FieldInfo> fields, object instance)
    {
        //
        this.Fields = fields;
        //
        this.Instance = instance;
    }

    /// <summary></summary>
    public abstract IEnumerator GetEnumerator();
}

/// <summary>Enumerates non-null field values.</summary>
public class FieldValues<T> : FieldValues, IEnumerable<T>
{
    /// <summary></summary>
    protected override Type ValueType => typeof(T);
    /// <summary>Create enumerator that enumerates <typeparamref name="T"/> typed fields from <paramref name="instance"/>.</summary>
    public FieldValues(IEnumerable<FieldInfo> fields, object instance) : base(fields, instance) { }

    /// <summary></summary>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        foreach (var pi in Fields)
        {
            // Get value
            object? value = pi.GetValue(Instance);
            // Got TT
            if (value is T tt) yield return tt;
            // Got IEnumerable<TT>
            else if (value is IEnumerable<T> enumr)
                foreach (T tt_ in enumr)
                    yield return tt_;
        }
    }

    /// <summary></summary>
    public override IEnumerator GetEnumerator()
    {
        foreach (var pi in Fields)
        {
            // Get value
            object? value = pi.GetValue(Instance);
            // Got TT
            if (value is T tt) yield return tt;
            // Got IEnumerable<TT>
            else if (value is IEnumerable<T> enumr) foreach (T tt_ in enumr) yield return tt_;
        }
    }
}
