// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;

/// <summary>Extension methods for <see cref="IFieldDelegates"/>.</summary>
public static class FieldDelegatesExtensions_
{
    /// <summary>Clone</summary>
    public static FieldDelegates Clone(this IFieldDelegates fieldDelegates)
    {
        // Place result here
        FieldDelegates result = FieldDelegates.Create(fieldDelegates.RecordType, fieldDelegates.FieldType);
        // Assign
        result.FieldRead = fieldDelegates.FieldRead;
        result.FieldWrite = fieldDelegates.FieldWrite;
        result.RecreateWith = fieldDelegates.RecreateWith;
        result.FieldDescription = fieldDelegates.FieldDescription;
        // Return
        return result;
    }

    /// <summary>Create field delegates</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static FieldDelegates CreateFieldDelegates(this IFieldDescription fieldDescription)
    {
        //
        Type? recordType = fieldDescription.Record?.Type, fieldType = fieldDescription.Type;
        //
        if (recordType == null || fieldType == null) throw new ArgumentException(nameof(fieldDescription));
        //
        FieldDelegates fieldDelegates = FieldDelegates.Create(recordType, fieldType);
        //
        fieldDelegates.FieldDescription = fieldDescription;
        fieldDelegates.FieldRead = fieldDescription.TryCreateFieldReadDelegate(out Delegate? readField) ? readField : null;
        fieldDelegates.FieldWrite = fieldDescription.TryCreateFieldWriteDelegate(out Delegate? writeField) ? writeField : null;
        fieldDelegates.RecreateWith = fieldDescription.TryCreateRecreateWith(fieldDescription?.Record?.Construction as IConstructionDescription, out Delegate? recreateWith) ? recreateWith : null;
        //
        return fieldDelegates;
    }

    /// <summary>Create field delegates</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static FieldDelegates<Record, Field> CreateFieldDelegates<Record, Field>(this IFieldDescription fieldDescription_) => (FieldDelegates<Record, Field>)CreateFieldDelegates(fieldDescription: fieldDescription_);


}
