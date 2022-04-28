// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using Avalanche.Utilities.Provider;
using Avalanche.Utilities.Record;

/// <summary>Clones field from one record to another and (optionally) clones value.</summary>
public abstract class FieldCloner : ReadOnlyAssignableClass
{
    /// <summary></summary>
    static readonly ConstructorT2<FieldCloner> constructor = new(typeof(FieldCloner<,>));

    /// <summary></summary>
    public static FieldCloner Create(Type recordType, Type fieldType)
    {
        // Create cloner
        FieldCloner fieldCloner = constructor.Create(recordType, fieldType);
        // Return
        return fieldCloner;
    }

    /// <summary></summary>
    /// <param name="reader"><![CDATA[Func<Record, Field>]]> or <![CDATA[FieldRead<Record, Field>]]></param>
    /// <param name="writer"><![CDATA[Action<Record, Field>]]> or <![CDATA[FieldWrite<Record, Field>]]></param>
    /// <param name="cloner">(optional) provides cloner for values</param>
    /// <param name="clonerProvider">(optional) provides cloner for the values that come from reader.</param>
    public static FieldCloner Create(Delegate reader, Delegate? writer, ICloner? cloner = null, IProvider<Type, ICloner>? clonerProvider = null)
    {
        // Get type args
        Type[] typeArgs = reader.GetType().GetGenericArguments();
        // 
        Type recordType = typeArgs[0], fieldType = typeArgs[1];
        // Create cloner
        FieldCloner fieldCloner = constructor.Create(recordType, fieldType);
        //
        fieldCloner.SetReader(reader);
        if (writer != null) fieldCloner.SetWriter(writer);
        if (cloner != null) fieldCloner.SetCloner(cloner);
        if (clonerProvider != null) fieldCloner.SetClonerProvider(clonerProvider);
        // Return
        return fieldCloner;
    }

    /// <summary>Record type</summary>
    public abstract Type RecordType { get; }
    /// <summary></summary>
    public abstract Type FieldType { get; }
    /// <summary></summary>
    public abstract object Cloner { get; set; }
    /// <summary></summary>
    public abstract IProvider<Type, ICloner>? ClonerProvider { get; set; }
    /// <summary></summary>
    public abstract Delegate Reader { get; set; }
    /// <summary></summary>
    public abstract Delegate Writer { get; set; }
}

/// <summary>Extension methods for <see cref="FieldCloner"/>.</summary>
public static class FieldClonerExtensions
{
    /// <summary>Set reader</summary>
    /// <param name="reader"><![CDATA[Func<Record, Field>]]> or <![CDATA[FieldRead<Record, Field>]]></param>
    public static T SetReader<T>(this T cloner, Delegate reader) where T : FieldCloner { cloner.Reader = reader; return cloner; }
    /// <summary>Set writer</summary>
    /// <param name="writer"><![CDATA[Action<Record, Field>]]> or <![CDATA[FieldWrite<Record, Field>]]></param>
    public static T SetWriter<T>(this T cloner, Delegate writer) where T : FieldCloner { cloner.Writer = writer; return cloner; }
    /// <summary></summary>
    public static T SetCloner<T>(this T cloner, ICloner value) where T : FieldCloner { cloner.Cloner = value; return cloner; }
    /// <summary></summary>
    public static T SetClonerProvider<T>(this T cloner, IProvider<Type, ICloner>? value) where T : FieldCloner { cloner.ClonerProvider = value; return cloner; }
}

/// <summary>Field copier.</summary>
public abstract class FieldCloner<Record> : FieldCloner
{
    /// <summary>Copy <paramref name="srcRecord"/> to <paramref name="dstRecord"/>.</summary>
    public abstract void CloneField(ref Record srcRecord, ref Record dstRecord);
    /// <summary>Copy <paramref name="srcRecord"/> to <paramref name="dstRecord"/>.</summary>
    public abstract void CloneField(ref Record srcRecord, ref Record dstRecord, IGraphClonerContext context);

    /// <summary>Copy <paramref name="srcRecord"/> to <paramref name="dst"/>.</summary>
    public abstract void CloneField(ref Record srcRecord, ref object dst);
    /// <summary>Copy <paramref name="srcRecord"/> to <paramref name="dst"/>.</summary>
    public abstract void CloneField(ref Record srcRecord, ref object dst, IGraphClonerContext context);
}

/// <summary>Field copier</summary>
public class FieldCloner<Record, Field> : FieldCloner<Record>
{
    /// <summary></summary>
    protected Func<Record, Field>? reader0;
    /// <summary></summary>
    protected FieldRead<Record, Field>? reader1;
    /// <summary></summary>
    protected Action<Record, Field>? writer0;
    /// <summary></summary>
    protected FieldWrite<Record, Field>? writer1;
    /// <summary>(optional) cloner for the values that come from reader.</summary>
    protected object? cloner;
    /// <summary>(optional) cloner provider, that provides cloner for the values that come from reader.</summary>
    protected IProvider<Type, ICloner>? clonerProvider;

    /// <summary></summary>
    public override Type RecordType => typeof(Record);
    /// <summary></summary>
    public override Type FieldType => typeof(Field);
    /// <summary></summary>
    public override Delegate Reader
    {
        get => (Delegate)reader0! ?? reader1!;
        set
        {
            this.AssertWritable();
            if (value is Func<Record, Field> _reader0) { reader0 = _reader0; reader1 = null; }
            else if (value is FieldRead<Record, Field> _reader1) { reader1 = _reader1; reader0 = null; }
            else if (value == null) { reader1 = null; reader0 = null; }
            else throw new ArgumentNullException("Reader");
        }
    }
    /// <summary></summary>
    public override Delegate Writer
    {
        get => (Delegate)writer0! ?? writer1!;
        set
        {
            this.AssertWritable();
            if (value is Action<Record, Field> _writer0) { writer0 = _writer0; writer1 = null; }
            else if (value is FieldWrite<Record, Field> _writer1) { writer1 = _writer1; writer0 = null; }
            else if (value == null) { writer1 = null; writer0 = null; }
            else throw new ArgumentNullException("Writer");
        }
    }

    /// <summary></summary>
    public override object Cloner { get => cloner!; set => this.AssertWritable().cloner = (ICloner<Field>)value; }
    /// <summary></summary>
    public override IProvider<Type, ICloner>? ClonerProvider { get => clonerProvider; set => this.AssertWritable().clonerProvider = value; }

    /// <summary>Create field copier</summary>
    public FieldCloner() : base() { }

    /// <summary>Copy from record to record. Applies cloner if available.</summary>
    public override void CloneField(ref Record src, ref Record dst)
    {
        // Read
        Field value = reader1 != null ? reader1(ref src) : reader0 != null ? reader0(src)! : throw new InvalidOperationException($"Reader is not assigned");

        // Get cloner
        if (value != null && clonerProvider != null && clonerProvider.TryGetValue(value.GetType(), out ICloner cloner1))
        {
            // Clone value
            if (cloner is ICloner<Field> clonerT) value = clonerT.Clone(value);
            //
            else value = (Field)cloner1.Clone(value);
        }
        //
        else if (value != null && cloner is ICloner<Field> cloner2) value = cloner2.Clone(value);
        else if (value != null && cloner is ICloner cloner3) value = (Field)cloner3.Clone(value);

        // Write
        if (writer1 != null) writer1(ref dst, value); else if (writer0 != null) writer0(dst, value); else throw new InvalidOperationException("Writer is not assigned.");
    }

    /// <summary>Copy from record to record. Applies cloner if available.</summary>
    public override void CloneField(ref Record src, ref Record dst, IGraphClonerContext context)
    {
        // Read
        Field value = reader1 != null ? reader1(ref src) : reader0 != null ? reader0(src)! : throw new InvalidOperationException($"Reader is not assigned");

        // Get cloner
        if (value != null && clonerProvider != null && clonerProvider.TryGetValue(value.GetType(), out ICloner cloner1))
        {
            // Clone value
            if (cloner1 is IGraphCloner<Field> graphClonerT) value = graphClonerT.Clone(value, context);
            // Clone value
            else if (cloner1 is IGraphCloner graphCloner) value = (Field)graphCloner.Clone(value, context);
            // Clone value
            else if (cloner1 is ICloner<Field> clonerT) value = clonerT.Clone(value);
            //
            else value = (Field)cloner1.Clone(value);
        }
        //
        else if (value != null && cloner is IGraphCloner<Field> cloner4) value = cloner4.Clone(value, context);
        else if (value != null && cloner is IGraphCloner cloner5) value = (Field)cloner5.Clone(value, context);
        else if (value != null && cloner is ICloner<Field> cloner2) value = cloner2.Clone(value);
        else if (value != null && cloner is ICloner cloner3) value = (Field)cloner3.Clone(value);

        // Write
        if (writer1 != null) writer1(ref dst, value); else if (writer0 != null) writer0(dst, value); else throw new InvalidOperationException("Writer is not assigned.");
    }

    /// <summary>Copy to object pointer. Applies cloner if available.</summary>
    public override void CloneField(ref Record src, ref object dst)
    {
        // Read
        Field value = reader1 != null ? reader1(ref src) : reader0 != null ? reader0(src)! : throw new InvalidOperationException($"Reader is not assigned");

        // Get cloner
        if (value != null && clonerProvider != null && clonerProvider.TryGetValue(value.GetType(), out ICloner cloner1))
        {
            // Clone value
            if (cloner is ICloner<Field> clonerT) value = clonerT.Clone(value);
            //
            else value = (Field)cloner1.Clone(value);
        }
        //
        else if (value != null && cloner is ICloner<Field> cloner2) value = cloner2.Clone(value);
        else if (value != null && cloner is ICloner cloner3) value = (Field)cloner3.Clone(value);

        // Assign
        dst = (object)value!;
    }

    /// <summary>Copy from record to record. Applies cloner if available.</summary>
    public override void CloneField(ref Record src, ref object dst, IGraphClonerContext context)
    {
        // Read
        Field value = reader1 != null ? reader1(ref src) : reader0 != null ? reader0(src)! : throw new InvalidOperationException($"Reader is not assigned");

        // Get cloner
        if (value != null && clonerProvider != null && clonerProvider.TryGetValue(value.GetType(), out ICloner cloner1))
        {
            // Clone value
            if (cloner1 is IGraphCloner<Field> graphClonerT) value = graphClonerT.Clone(value, context);
            // Clone value
            else if (cloner1 is IGraphCloner graphCloner) value = (Field)graphCloner.Clone(value, context);
            // Clone value
            else if (cloner1 is ICloner<Field> clonerT) value = clonerT.Clone(value);
            //
            else value = (Field)cloner1.Clone(value);
        }
        //
        else if (value != null && cloner is IGraphCloner<Field> cloner4) value = cloner4.Clone(value, context);
        else if (value != null && cloner is IGraphCloner cloner5) value = (Field)cloner5.Clone(value, context);
        else if (value != null && cloner is ICloner<Field> cloner2) value = cloner2.Clone(value);
        else if (value != null && cloner is ICloner cloner3) value = (Field)cloner3.Clone(value);

        // Assign
        dst = (object)value!;
    }
}

