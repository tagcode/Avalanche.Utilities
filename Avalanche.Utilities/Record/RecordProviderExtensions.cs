// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using Avalanche.Utilities.Provider;

/// <summary>Extension methods for <see cref="IRecordProviders"/>.</summary>
public static class RecordProviderExtensions
{
    /// <summary>Get specific record delegate.</summary>
    public static ValueResult<IRecordDelegates<Record>> GetRecordDelegates<Record>(this IRecordProviders provider)
    {
        // Query
        IResult<IRecordDelegates> result1 = provider.RecordDelegatesByType[typeof(Record)];
        // Error
        if (result1.Status != ResultStatus.Ok) return ValueResult<IRecordDelegates<Record>>.CopyFrom(result1);
        // 
        if (result1.Value is not IRecordDelegates<Record> casted) return new ValueResult<IRecordDelegates<Record>> { Status = ResultStatus.Error, Error = new InvalidCastException() };
        // Copy result
        return new ValueResult<IRecordDelegates<Record>> { Status = ResultStatus.Ok, Value = casted };
    }

    /// <summary>Get specific record delegate.</summary>
    public static ValueResult<Delegate> GetCreateRecordDelegate(this IRecordProviders provider, Type recordType)
    {
        // Query
        IResult<IRecordDescription> result1 = provider.RecordDescription[recordType];
        // Error
        if (result1.Status != ResultStatus.Ok) return ValueResult<Delegate>.CopyFrom(result1);
        // Query
        IResult<Delegate> result2 = provider.RecordCreate[result1.Value];
        // Copy result
        return ValueResult<Delegate>.CopyFrom(result2);
    }


    /// <summary>Get specific field delegate.</summary>
    public static ValueResult<Delegate> GetFieldRead(this IRecordProviders provider, Type recordType, string fieldName)
    {
        // Query
        IResult<IRecordDescription> result1 = provider.RecordDescription[recordType];
        // Error
        if (result1.Status != ResultStatus.Ok) return ValueResult<Delegate>.CopyFrom(result1);
        // Find field description
        if (!result1.Value!.Fields.TryGetByName(fieldName, out IFieldDescription fieldDescription)) return new ValueResult<Delegate> { Status = ResultStatus.NoResult };
        // Query
        IResult<Delegate> result2 = provider.FieldRead[fieldDescription];
        // Copy result
        return ValueResult<Delegate>.CopyFrom(result2);
    }

    /// <summary>Get specific field delegate.</summary>
    public static ValueResult<Delegate> GetFieldRead(this IResult<IFieldDelegates> result1)
    {
        // Error
        if (result1.Status != ResultStatus.Ok) return ValueResult<Delegate>.CopyFrom(result1);
        //
        Delegate? @delegate = result1.Value!.FieldRead;
        // Find field description
        if (@delegate == null) return new ValueResult<Delegate> { Status = ResultStatus.NoResult };
        // Ok result
        return new ValueResult<Delegate> { Status = ResultStatus.Ok, Value = @delegate };
    }

    /// <summary>Get specific field delegate.</summary>
    public static ValueResult<Delegate> GetFieldWrite(this IResult<IFieldDelegates> result1)
    {
        // Error
        if (result1.Status != ResultStatus.Ok) return ValueResult<Delegate>.CopyFrom(result1);
        //
        Delegate? @delegate = result1.Value!.FieldWrite;
        // Find field description
        if (@delegate == null) return new ValueResult<Delegate> { Status = ResultStatus.NoResult };
        // Ok result
        return new ValueResult<Delegate> { Status = ResultStatus.Ok, Value = @delegate };
    }

    /// <summary>Get specific field delegate.</summary>
    public static ValueResult<Delegate> GetRecreateWith(this IResult<IFieldDelegates> result1)
    {
        // Error
        if (result1.Status != ResultStatus.Ok) return ValueResult<Delegate>.CopyFrom(result1);
        //
        Delegate? @delegate = result1.Value!.RecreateWith;
        // Find field description
        if (@delegate == null) return new ValueResult<Delegate> { Status = ResultStatus.NoResult };
        // Ok result
        return new ValueResult<Delegate> { Status = ResultStatus.Ok, Value = @delegate };
    }

    /// <summary>Get specific field delegate.</summary>
    public static ValueResult<Delegate> GetFieldWrite(this IRecordProviders provider, Type recordType, string fieldName)
    {
        // Query
        IResult<IRecordDescription> result1 = provider.RecordDescription[recordType];
        // Error
        if (result1.Status != ResultStatus.Ok) return ValueResult<Delegate>.CopyFrom(result1);
        // Find field description
        if (!result1.Value!.Fields.TryGetByName(fieldName, out IFieldDescription fieldDescription)) return new ValueResult<Delegate> { Status = ResultStatus.NoResult };
        // Query
        IResult<Delegate> result2 = provider.FieldWrite[fieldDescription];
        // Copy result
        return ValueResult<Delegate>.CopyFrom(result2);
    }

    /// <summary>Get specific field delegate.</summary>
    public static ValueResult<Delegate> GetRecreateWith(this IRecordProviders provider, Type recordType, string fieldName)
    {
        // Query
        IResult<IRecordDescription> result1 = provider.RecordDescription[recordType];
        // Error
        if (result1.Status != ResultStatus.Ok) return ValueResult<Delegate>.CopyFrom(result1);
        // Find field description
        if (!result1.Value!.Fields.TryGetByName(fieldName, out IFieldDescription fieldDescription)) return new ValueResult<Delegate> { Status = ResultStatus.NoResult };
        // Query
        IResult<Delegate> result2 = provider.RecreateWith[fieldDescription];
        // Copy result
        return ValueResult<Delegate>.CopyFrom(result2);
    }

    /// <summary>Get all field delegates.</summary>
    public static ValueResult<IFieldDelegates> GetFieldDelegates(this IRecordProviders provider, Type recordType, string fieldName)
    {
        // Query
        IResult<IRecordDescription> result1 = provider.RecordDescription[recordType];
        // Error
        if (result1.Status != ResultStatus.Ok) return ValueResult<IFieldDelegates>.CopyFrom(result1);
        // Find field description
        if (!result1.Value!.Fields.TryGetByName(fieldName, out IFieldDescription fieldDescription)) return new ValueResult<IFieldDelegates> { Status = ResultStatus.NoResult };
        // Query
        IResult<IFieldDelegates> result2 = provider.FieldDelegates[fieldDescription];
        // Copy result
        return ValueResult<IFieldDelegates>.CopyFrom(result2);
    }

    /// <summary>Get all field delegates.</summary>
    public static ValueResult<IFieldDelegates<Record, Field>> GetFieldDelegates<Record, Field>(this IRecordProviders provider, string fieldName)
    {
        // Query
        IResult<IRecordDescription> result1 = provider.RecordDescription[typeof(Record)];
        // Error
        if (result1.Status != ResultStatus.Ok) return ValueResult<IFieldDelegates<Record, Field>>.CopyFrom(result1);
        // Find field description
        if (!result1.Value!.Fields.TryGetByName(fieldName, out IFieldDescription fieldDescription)) return new ValueResult<IFieldDelegates<Record, Field>> { Status = ResultStatus.NoResult };
        // Query
        IResult<IFieldDelegates> result2 = provider.FieldDelegates[fieldDescription];
        // Copy result
        return ValueResult<IFieldDelegates<Record, Field>>.CopyFrom(result2);
    }
}
