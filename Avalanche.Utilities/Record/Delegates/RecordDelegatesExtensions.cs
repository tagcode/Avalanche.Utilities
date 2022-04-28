// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;

/// <summary>Extension methods for <see cref="IRecordDelegates"/>.</summary>
public static class RecordDelegatesExtensions_
{
    /// <summary>Clone</summary>
    public static RecordDelegates Clone(this IRecordDelegates recordDelegates)
    {
        // Place result here
        RecordDelegates result = RecordDelegates.Create(recordDelegates.RecordType);
        // Assign
        result.RecordCreate = recordDelegates.RecordCreate;
        result.SetFieldDelegates(recordDelegates.FieldDelegates);
        result.RecordDescription = recordDelegates.RecordDescription;
        // Return
        return result;
    }

    /// <summary>Create record delegates.</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static IRecordDelegates CreateRecordDelegates(this IRecordDescription recordDescription)
    {
        //
        Type? recordType = recordDescription.Type;
        //
        if (recordType == null) throw new ArgumentException(nameof(recordDescription));
        //
        IRecordDelegates recordDelegates = RecordDelegates.Create(recordType);
        //
        recordDelegates.RecordCreate = ((IConstructionDescription)recordDescription.Construction!).TryCreateCreateFunc(out Delegate? createRecord) ? createRecord : null;
        recordDelegates.RecordDescription = recordDescription;
        //recordDelegates.FieldDelegates = 
        //
        return recordDelegates;
    }

    /// <summary>Create record delegates</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static IRecordDelegates<Record> CreateRecordDelegates<Record>(this IRecordDescription recordDescription_) => (IRecordDelegates<Record>)CreateRecordDelegates(recordDescription: recordDescription_);

}
