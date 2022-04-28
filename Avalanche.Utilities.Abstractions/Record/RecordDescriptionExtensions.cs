// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Reflection;

/// <summary>Extension methods for <see cref="IRecordDescription"/>.</summary>
public static class RecordDescriptionExtensions
{
    /// <summary>Set record name</summary>
    public static T SetName<T>(this T recordDescription, object value) where T : IRecordDescription { recordDescription.Name = value; return recordDescription; }
    /// <summary>Set field type</summary>
    public static T SetType<T>(this T recordDescription, Type value) where T : IRecordDescription { recordDescription.Type = value; return recordDescription; }
    /// <summary>Set record constructor.</summary>
    public static T SetConstructors<T>(this T recordDescription, IConstructorDescription[]? value) where T : IRecordDescription { recordDescription.Constructors = value; return recordDescription; }
    /// <summary>Set record deconstructor as <see cref="ValueTuple"/>: <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]></summary>
    public static T SetDeconstructor<T>(this T recordDescription, Object? value) where T : IRecordDescription { recordDescription.Deconstructor = value; return recordDescription; }
    /// <summary>Set fields from <paramref name="value"/> and make a copy.</summary>
    public static T SetFields<T>(this T recordDescription, IEnumerable<IFieldDescription> value) where T : IRecordDescription { recordDescription.Fields = value.ToArray(); return recordDescription; }
    /// <summary>Set Annotations, such as <see cref="Attribute"/></summary>
    public static T SetAnnotations<T>(this T recordDescription, IEnumerable<object> value) where T : IRecordDescription { recordDescription.Annotations = value.ToArray(); return recordDescription; }
    /// <summary>Record construction strategy</summary>
    public static T SetConstruction<T>(this T recordDescription, IConstructionDescription? construction) where T : IRecordDescription { recordDescription.Construction = construction; return recordDescription; }

    /// <summary>Put into read-only state.</summary>
    /// <param name="applyFields">Apply fields into readonly state too</param>
    public static R SetReadOnlyDeep<R>(this R recordDescription, bool applyFields) where R : IRecordDescription
    {
        // Assign as read-only
        if (recordDescription is IReadOnly @readonly) @readonly.ReadOnly = true;
        //
        if (applyFields)
        {
            // Get fields
            var fields = recordDescription.Fields;
            //
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    // Assign as read-only
                    if (field is IReadOnly _field) _field.ReadOnly = true;
                }
            }
        }
        //
        return recordDescription;
    }

}
