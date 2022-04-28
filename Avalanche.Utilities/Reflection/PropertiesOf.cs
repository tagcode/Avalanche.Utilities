// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Reflection;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

/// <summary>Lists public non-static properties of a record type.</summary>
public abstract class PropertiesOf : IEnumerable<PropertyInfo>
{
    /// <summary></summary>
    static readonly ConstructorT2<PropertiesOf> constructor = new(typeof(PropertiesOf<,>));
    /// <summary></summary>
    public static PropertiesOf Create(Type recordType, Type fieldType) => constructor.Create(recordType, fieldType);
    /// <summary></summary>
    public abstract Type RecordType { get; }
    /// <summary></summary>
    public abstract Type PropertyType { get; }
    /// <summary>Properties that implement field type</summary>
    public abstract PropertyInfo[] Properties { get; }
    /// <summary></summary>
    public IEnumerator<PropertyInfo> GetEnumerator() => ((IEnumerable<PropertyInfo>)Properties).GetEnumerator();
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Properties).GetEnumerator();
}

/// <summary>Lists public non-static properties of a record type.</summary>
public class PropertiesOf<Record, Property> : PropertiesOf
{
    /// <summary></summary>
    static PropertiesOf<Record, Property> instance = new();
    /// <summary></summary>
    public static PropertiesOf<Record, Property> Instance => instance;

    /// <summary>Properties that implement Field or <![CDATA[IEnumerable<Field>]]></summary>
    public static PropertyInfo[] properties;
    /// <summary>Properties that implement Field or <![CDATA[IEnumerable<Field>]]></summary>
    public override PropertyInfo[] Properties => properties;
    /// <summary></summary>
    public override Type RecordType => typeof(Record);
    /// <summary></summary>
    public override Type PropertyType => typeof(Property);

    /// <summary></summary>
    static PropertiesOf()
    {
        // Get all properties
        PropertyInfo[] allProperties = typeof(Record).GetProperties();
        // Filter
        IEnumerable<PropertyInfo> filtered = Filter(allProperties, typeof(Property));
        //
        properties = filtered.ToArray();
    }

    /// <summary>Filter applicable properties</summary>
    public static IEnumerable<PropertyInfo> Filter(IEnumerable<PropertyInfo> properties, Type filterType)
    {
        //
        foreach (PropertyInfo pi in properties)
        {
            // No getter
            if (pi.GetGetMethod() == null) continue;
            // Not visible
            if (!pi.GetGetMethod()!.IsPublic) continue;
            // Static
            if (pi.GetGetMethod()!.IsStatic) continue;
            // Index
            if (pi.GetIndexParameters() != null && pi.GetIndexParameters().Length > 0) continue;
            //
            if (pi.GetCustomAttribute(typeof(IgnoreDataMemberAttribute)) != null) continue;
            //
            if (pi.PropertyType.IsAssignableTo(filterType)) yield return pi;
            //
            else if (TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(pi.PropertyType, typeof(IEnumerable<>), 0, out Type elementType) && elementType.IsAssignableTo(filterType)) yield return pi;
        }
    }
}
