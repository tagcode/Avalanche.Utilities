// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Reflection;
using System.Collections;
using System.Reflection;

/// <summary>Enumerates non-null property values.</summary>
public abstract class PropertyValues : IEnumerable
{
    /// <summary></summary>
    static readonly ConstructorT<IEnumerable<PropertyInfo>, object, PropertyValues> constructor = new(typeof(PropertyValues<>));
    /// <summary></summary>
    public PropertyValues Create(Type type, IEnumerable<PropertyInfo> properties, object instance) => constructor.Create(type, properties, instance);

    /// <summary></summary>
    protected object Instance;
    /// <summary>Properties that implement <see cref="ValueType"/> or <![CDATA[IEnumerable<T>]]></summary>
    protected IEnumerable<PropertyInfo> Properties;
    /// <summary></summary>
    protected abstract Type ValueType { get; }

    /// <summary></summary>
    public PropertyValues(IEnumerable<PropertyInfo> properties, object instance)
    {
        //
        this.Properties = properties;
        //
        this.Instance = instance;
    }

    /// <summary></summary>
    public abstract IEnumerator GetEnumerator();
}

/// <summary>Enumerates non-null property values.</summary>
public class PropertyValues<T> : PropertyValues, IEnumerable<T>
{
    /// <summary></summary>
    protected override Type ValueType => typeof(T);
    /// <summary>Create enumerator that enumerates <typeparamref name="T"/> typed propertys from <paramref name="instance"/>.</summary>
    public PropertyValues(IEnumerable<PropertyInfo> properties, object instance) : base(properties, instance) { }

    /// <summary></summary>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        foreach (var pi in Properties)
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
        foreach (var pi in Properties)
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
