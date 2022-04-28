// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Avalanche.Utilities.Provider;

/// <summary>Creates <![CDATA[Action<Record, Record>]]> that copies every field from one record to another.</summary>
public static class RecordCopyAction
{
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    static readonly IProvider<IRecordDescription, Delegate> create = Providers.Func<IRecordDescription, Delegate>(TryCreateRecordCopyAction);
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    static readonly IProvider<IRecordDescription, IResult<Delegate>> createResult = create.ResultCaptured();
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    static readonly IProvider<IRecordDescription, IResult<Delegate>> cachedResult = createResult.WeakCached();
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    static readonly IProvider<IRecordDescription, Delegate> cached = cachedResult.ResultOpened();
    /// <summary>Creates from type.</summary>
    static readonly IProvider<Type, Delegate> createFromType = RecordDescription.CreateResult.Concat(createResult).ResultOpened();
    /// <summary>Creates and caches from type.</summary>
    static readonly IProvider<Type, Delegate> cachedFromType = RecordDescription.CachedResult.Concat(cachedResult).ResultOpened();

    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    public static IProvider<IRecordDescription, Delegate> Create => create;
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    public static IProvider<IRecordDescription, IResult<Delegate>> CreateResult => createResult;
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    public static IProvider<IRecordDescription, IResult<Delegate>> CachedResult => cachedResult;
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    public static IProvider<IRecordDescription, Delegate> Cached => cached;
    /// <summary>Creates from type.</summary>
    public static IProvider<Type, Delegate> CreateFromType => createFromType;
    /// <summary>Creates and caches from type.</summary>
    public static IProvider<Type, Delegate> CachedFromType => cachedFromType;

    /// <summary>Create <see cref="Action{Record, Record}"/> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecordCopyAction(this IRecordDescription record, [NotNullWhen(true)] out Delegate @delegate)
    {
        // Create LambdaExpression
        if (!TryCreateRecordCopyExpression(record, out LambdaExpression? expression)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <see cref="Action{Record, Record}"/> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecordCopyAction(this IRecordDescription record, [NotNullWhen(true)] out Delegate @delegate, Type? delegateRecordType)
    {
        // Create LambdaExpression
        if (!TryCreateRecordCopyExpression(record, out LambdaExpression? expression, delegateRecordType)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <see cref="Action{Record, Record}"/> expression</summary>
    /// <param name="recordDescription">Record type to clone</param>
    /// <param name="expression">Expression for <see cref="Action{Record, Record}"/>.</param>
    public static bool TryCreateRecordCopyExpression(IRecordDescription recordDescription, [NotNullWhen(true)] out LambdaExpression? expression, Type? delegateRecordType = default)
    {
        // Record Type
        Type recordType = recordDescription.Type;
        //
        if (delegateRecordType == null) delegateRecordType = recordType;
        // Record arguments
        ParameterExpression srcRecordArgument = Expression.Parameter(delegateRecordType, "srcRecord"), dstRecordArgument = Expression.Parameter(delegateRecordType, "dstRecord"); ;
        Expression srcRecordArgument_ = srcRecordArgument.Type.Equals(recordType) ? srcRecordArgument : Expression.Convert(srcRecordArgument, recordType);
        Expression dstRecordArgument_ = dstRecordArgument.Type.Equals(recordType) ? dstRecordArgument : Expression.Convert(dstRecordArgument, recordType);
        //
        List <Expression> assignments = new(recordDescription.Fields.Length);
        // Choose expression for each field
        for (int fieldIndex = 0; fieldIndex < recordDescription.Fields.Length; fieldIndex++)
        {
            // Get field
            IFieldDescription field = recordDescription.Fields[fieldIndex];
            // Get reader and writer
            if (!FieldRead.TryCreateFieldReadExpression(field, out LambdaExpression? readerExpression) ||
                !FieldWrite.TryCreateFieldWriteExpression(field, out LambdaExpression? writerExpression)) { expression = null; return false; }
            // Read
            Expression valueExpression = Expression.Invoke(readerExpression, srcRecordArgument_);
            // Write
            Expression writeExpression = Expression.Invoke(writerExpression, dstRecordArgument_, valueExpression);
            //
            assignments.Add(writeExpression);
        }

        //
        BlockExpression body = Expression.Block(assignments);
        // Choose delegate type
        Type delegateType = typeof(Action<,>).MakeGenericType(delegateRecordType, delegateRecordType);
        // Compile
        expression = Expression.Lambda(delegateType, body, srcRecordArgument, dstRecordArgument);
        // Return
        return true;
    }

}
