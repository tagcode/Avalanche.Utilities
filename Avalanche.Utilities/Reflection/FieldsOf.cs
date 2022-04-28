// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Reflection;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

/// <summary>Lists public non-static fields of a record type.</summary>
public abstract class FieldsOf : IEnumerable<FieldInfo>
{
    /// <summary></summary>
    static readonly ConstructorT2<FieldsOf> constructor = new(typeof(FieldsOf<,>));
    /// <summary></summary>
    public static FieldsOf Create(Type recordType, Type fieldType) => constructor.Create(recordType, fieldType);
    /// <summary></summary>
    public abstract Type RecordType { get; }
    /// <summary></summary>
    public abstract Type FieldType { get; }
    /// <summary>Fields that implement field type</summary>
    public abstract FieldInfo[] Fields { get; }
    /// <summary></summary>
    public IEnumerator<FieldInfo> GetEnumerator() => ((IEnumerable<FieldInfo>)Fields).GetEnumerator();
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Fields).GetEnumerator();
}

/// <summary>Lists public non-static fields of a record type.</summary>
public class FieldsOf<Record, Field> : FieldsOf
{
    /// <summary></summary>
    static FieldsOf<Record, Field> instance = new();
    /// <summary></summary>
    public static FieldsOf<Record, Field> Instance => instance;

    /// <summary>Fields that implement Field or <![CDATA[IEnumerable<Field>]]></summary>
    public static FieldInfo[] fields;
    /// <summary>Fields that implement Field or <![CDATA[IEnumerable<Field>]]></summary>
    public override FieldInfo[] Fields => fields;
    /// <summary></summary>
    public override Type RecordType => typeof(Record);
    /// <summary></summary>
    public override Type FieldType => typeof(Field);

    /// <summary></summary>
    static FieldsOf()
    {
        // Get all fields
        FieldInfo[] allFields = typeof(Record).GetFields();
        // Filter
        IEnumerable<FieldInfo> filtered = Filter(allFields, typeof(Field));
        //
        fields = filtered.ToArray();
    }

    /// <summary>Filter applicable fields</summary>
    public static IEnumerable<FieldInfo> Filter(IEnumerable<FieldInfo> fields, Type filterType)
    {
        //
        foreach (FieldInfo fi in fields)
        {
            // Not visible
            if (!fi.IsPublic) continue;
            // Static
            if (fi.IsStatic) continue;
            //
            if (fi.GetCustomAttribute(typeof(IgnoreDataMemberAttribute)) != null) continue;
            //
            if (fi.FieldType.IsAssignableTo(filterType)) yield return fi;
            //
            else if (TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(fi.FieldType, typeof(IEnumerable<>), 0, out Type elementType) && elementType.IsAssignableTo(filterType)) yield return fi;
        }
    }
}
