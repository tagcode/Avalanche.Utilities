// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;

/// <summary>Extension methods for <see cref="IRecordDelegates"/>.</summary>
public static class RecordDelegatesExtensions
{
    /// <summary><![CDATA[Func<object[], Record>]]></summary>
    public static T SetRecordCreate<T>(this T recordDelegates, Delegate? value) where T : IRecordDelegates { recordDelegates.RecordCreate = value; return recordDelegates; }
    /// <summary>Field delegates</summary>
    public static T SetFieldDelegates<T>(this T recordDelegates, IEnumerable<IFieldDelegates>? value) where T : IRecordDelegates { recordDelegates.FieldDelegates = value?.ToArray(); return recordDelegates; }
    /// <summary>></summary>
    public static T SetRecordDescription<T>(this T fieldDelegates, IRecordDescription? value) where T : IRecordDelegates { fieldDelegates.RecordDescription = value; return fieldDelegates; }
}
