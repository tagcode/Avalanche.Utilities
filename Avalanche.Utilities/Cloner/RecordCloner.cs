// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Avalanche.Utilities.Provider;
using Avalanche.Utilities.Record;

/// <summary>Clones records.</summary>
public abstract class RecordCloner : ReadOnlyAssignableClass, ICloner, IGraphCloner, ICyclical
{
    /// <summary></summary>
    static readonly ConstructorT<RecordCloner> constructor = new(typeof(RecordCloner<>));

    /// <summary></summary>
    /// <returns><![CDATA[RecordCloner<T>]]></returns>
    public static RecordCloner Create(Type recordType) => constructor.Create(recordType);

    /// <summary>Record type</summary>
    public abstract Type RecordType { get; }

    /// <summary></summary>
    protected bool isCyclical;
    /// <summary>Providers cloners for fields.</summary>
    protected IProvider<Type, ICloner>? clonerProvider;

    /// <summary></summary>
    public virtual bool IsCyclical { get => isCyclical; set => this.AssertWritable().isCyclical = value; }
    /// <summary>Providers cloners for fields.</summary>
    public virtual IProvider<Type, ICloner>? ClonerProvider { get => clonerProvider; set => this.AssertWritable().clonerProvider = value; }

    /// <summary></summary>
    public abstract object Clone(object src);
    /// <summary></summary>
    public abstract object Clone(object src, IGraphClonerContext context);

    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphCloner.Context"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphClonerContext? setContext(IGraphClonerContext? context) { IGraphCloner.Context.Value = context; return context; }
}

/// <summary>Extension methods for <see cref="RecordCloner"/>.</summary>
public static class RecordClonerExtensions
{
    /// <summary></summary>
    public static T SetClonerProvider<T>(this T cloner, IProvider<Type, ICloner>? value) where T : RecordCloner { cloner.ClonerProvider = value; return cloner; }
}

/// <summary></summary>
/// <typeparam name="Record">Record type to draw fields from, can be class or value-type, interface, abstract type.</typeparam>
public class RecordCloner<Record> : RecordCloner, ICloner<Record>, IGraphCloner<Record>
{
    /// <summary>No args.</summary>
    static object[] no_args = new object[0];

    /// <summary>Record type</summary>
    public override Type RecordType => typeof(Record);

    /// <summary>Line for specific implementing record type.</summary>
    protected class Line
    {
        /// <summary>Implementing type</summary>
        public Type implType;
        /// <summary>Function that creates empty.</summary>
        public Func<object[], Record> create;

        /// <summary>Initial value cloners, each corresponds to argument in <see cref="create"/>.</summary>
        public List<FieldCloner<Record>> parameterCloners;
        /// <summary>After construction cloners.</summary>
        public List<FieldCloner<Record>> fieldCloners;
        /// <summary>Parent object</summary>
        protected RecordCloner recordCloner;

        /// <summary>Choose construction with the lowest parameter count, though prefer constructors that have parameters that don't have other way to write to</summary>
        /// <returns>Constructor</returns>
        /// <exception cref="ArgumentException">If no constructors were found.</exception>
        static IConstructionDescription ChooseConstruction(IEnumerable<IConstructionDescription> ctors)
        {
            // No constructions were found
            if (ctors == null) throw new ArgumentNullException(nameof(ctors));
            // Place here construction
            IConstructionDescription? constructionDescription = null;
            //
            long bestScore = long.MinValue;
            //
            foreach (IConstructionDescription ctor in ctors)
            {
                // Count score
                long score = 0L;
                // Count number of fields addressable (has parameter correlation or is writable)
                foreach (IFieldDescription field in ctor.Fields) if (field.Writer != null || ctor.FieldToParameter.ContainsKey(field)) score += 0x10000L;
                // Count number of parameters as negative
                score += 0x10000L - ctor.Parameters.Length;
                // Not better
                if (constructionDescription != null && score < bestScore) continue;
                // Assign
                constructionDescription = ctor;
                bestScore = score;
            }
            // No construction was found
            if (constructionDescription == null) throw new ArgumentException($"No constructors.");
            // Return
            return constructionDescription;
        }

        /// <summary></summary>
        public Line(Type implType, RecordCloner recordCloner)
        {
            //
            this.recordCloner = recordCloner;
            //
            this.implType = implType;
            // Get record descriptions
            IRecordDescription implDescription = RecordDescription.Cached[implType];
            IRecordDescription intfDescription = RecordDescription.Cached[typeof(Record)];
            // Create constructions
            ConstructionDescription[] constructions = implDescription.Constructors!.Select(ctor => ctor.CreateConstructionDescription()).ToArray();
            // Choose construction
            IConstructionDescription constructionDescription = ChooseConstruction(constructions); //.SetRecord(intfDescription).SetType(typeof(Record));
            {
                // 
                ConstructionDescription constructionDescription2 = constructionDescription.Clone().StripToParameters();
                // Parameter is unmatched
                if (constructionDescription2.UnmatchedParameters.Count > 0) throw new InvalidOperationException($"Unexpected unmatched parameters in {nameof(IConstructionDescription)}");
                // Lock description
                constructionDescription2.SetReadOnly();
                // Create delegate
                if (!RecordCreateFunc.TryCreateCreateFunc(constructionDescription2, out Delegate? createFunc, typeof(Record))) throw new InvalidOperationException($"Failed to create constructor for {CanonicalName.Print(implType)}");
                // Assign delegate
                this.create = (Func<object[], Record>)createFunc;
            }
            //
            fieldCloners = new List<FieldCloner<Record>>(constructionDescription.UnmatchedFields.Count);
            // Create field cloners
            foreach (IFieldDescription field in constructionDescription.Fields)
            {
                // Got matching parameter
                if (constructionDescription.FieldToParameter.ContainsKey(field)) continue;
                // Create reader
                if (!FieldRead.TryCreateFieldReadDelegate(field, out Delegate reader, typeof(Record), default)) throw new InvalidOperationException();
                // Create writer
                if (!FieldWrite.TryCreateFieldWriteDelegate(field, out Delegate writer, typeof(Record), default)) throw new InvalidOperationException();
                // Create field cloner
                FieldCloner<Record> fieldCloner = (FieldCloner<Record>)FieldCloner.Create(reader, writer, clonerProvider: recordCloner.ClonerProvider);
                // Assign
                fieldCloners.Add(fieldCloner);
            }

            // Assign parameter cloners
            parameterCloners = new List<FieldCloner<Record>>(constructionDescription.Parameters.Length);
            // Create parameter cloners (in order of fields)
            foreach (IFieldDescription field in constructionDescription.Fields)
            {
                // Get matching parameter
                if (!constructionDescription.FieldToParameter.TryGetValue(field, out IParameterDescription? parameter)) continue;
                // Create reader
                if (!FieldRead.TryCreateFieldReadDelegate(field, out Delegate reader, typeof(Record), default)) throw new InvalidOperationException();
                // Create field cloner
                FieldCloner<Record> fieldCloner = (FieldCloner<Record>)FieldCloner.Create(reader, null, clonerProvider: recordCloner.ClonerProvider);
                // Assign
                parameterCloners.Add(fieldCloner);
            }
        }
    }

    /// <summary>Implementation type specific infos</summary>
    protected ConcurrentDictionary<Type, Line> lines = new ConcurrentDictionary<Type, Line>();
    /// <summary>Creates implementation type specific info</summary>
    protected Func<Type, Line> lineCreate;

    /// <summary></summary>
    /// <exception cref="ArgumentException">If constructor was not found in record description</exception>
    public RecordCloner()
    {
        this.clonerProvider = Avalanche.Utilities.ClonerProvider.Cached;
        this.isCyclical = typeof(Record).IsAssignableTo(typeof(ICyclical));
        this.lineCreate = (Type type) => new Line(type, this);
    }

    /// <summary></summary>
    public Record Clone(in Record src)
    {
        // Got null
        if (src == null) return default!;
        // Move to cyclical 
        if (isCyclical)
        {
            // Get previous context
            IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
            // Place here context
            IGraphClonerContext context = prevContext ?? setContext(new GraphClonerContext())!;
            try
            {
                return Clone(src, context);
            }
            finally
            {
                // Revert to previous context
                IGraphCloner.Context.Value = prevContext;
            }
        }
        //
        Type type = src.GetType();
        //
        Line line = lines.GetOrAdd(type, lineCreate);
        // Take copy
        Record _src = src;
        //
        object[] arguments;
        // Constructor argument count
        int ctorArgCount = line.parameterCloners.Count;
        // No args needed in .ctor
        if (ctorArgCount == 0) arguments = no_args;
        // Clone .ctor args
        else
        {
            // Place args here
            arguments = new object[ctorArgCount];
            // Copy fields to args
            for (int i = 0; i < ctorArgCount; i++)
                line.parameterCloners[i].CloneField(ref _src, ref arguments[i]);
        }
        // Create record
        Record dst = line.create(arguments);

        // Clone fields
        for (int i = 0; i < line.fieldCloners.Count; i++)
            line.fieldCloners[i].CloneField(ref _src, ref dst);

        // Return 
        return dst;
    }

    /// <summary></summary>
    public override object Clone(object srcObject)
    {
        // Got null
        if (srcObject == null) return default!;
        // Move to cyclical 
        if (isCyclical)
        {
            // Get previous context
            IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
            // Place here context
            IGraphClonerContext context = prevContext ?? setContext(new GraphClonerContext())!;
            try
            {
                return Clone(srcObject, context);
            }
            finally
            {
                // Revert to previous context
                IGraphCloner.Context.Value = prevContext;
            }
        }
        //
        Record src = (Record)srcObject;
        //
        Type type = srcObject.GetType();
        //
        Line line = lines.GetOrAdd(type, lineCreate);
        //
        object[] arguments;
        // Constructor argument count
        int ctorArgCount = line.parameterCloners.Count;
        // No args needed in .ctor
        if (ctorArgCount == 0) arguments = no_args;
        // Clone .ctor args
        else
        {
            // Place args here
            arguments = new object[ctorArgCount];
            // Copy fields to args
            for (int i = 0; i < ctorArgCount; i++)
                line.parameterCloners[i].CloneField(ref src, ref arguments[i]);
        }
        // Create record
        Record dst = line.create(arguments);

        // Clone fields
        for (int i = 0; i < line.fieldCloners.Count; i++)
            line.fieldCloners[i].CloneField(ref src, ref dst);

        // Return 
        return dst!;
    }


    /// <summary></summary>
    public Record Clone(in Record src, IGraphClonerContext context)
    {
        // Got null
        if (src == null) return default!;
        // Exists in context
        if (context.TryGet(src, out Record? _dst)) return (Record)_dst!;
        //
        Type type = src.GetType();
        //
        Line line = lines.GetOrAdd(type, lineCreate);


        // Take copy
        Record _src = src;
        // Place arguments here
        object[] arguments;
        // Constructor parameter count
        int parameterCount = line.parameterCloners.Count;
        // No args needed in .ctor
        if (parameterCount == 0) arguments = no_args;
        // Clone .ctor args
        else
        {
            // Place args here
            arguments = new object[parameterCount];
            // Copy fields to args
            for (int i = 0; i < parameterCount; i++)
                line.parameterCloners[i].CloneField(ref _src, ref arguments[i], context);
        }
        // Create record
        Record dst = line.create(arguments);
        // Assign to context
        if (!typeof(Record).IsValueType) context.Add(src!, dst!);

        // Clone fields
        for (int i = 0; i < line.fieldCloners.Count; i++)
            line.fieldCloners[i].CloneField(ref _src, ref dst, context);

        // Return 
        return dst;

    }

    /// <summary></summary>
    public override object Clone(object srcObject, IGraphClonerContext context)
    {
        // Got null
        if (srcObject == null) return default!;
        // Exists in context
        if (context.TryGet(srcObject, out object? _dst)) return _dst!;
        //
        Record src = (Record)srcObject;
        //
        Type type = srcObject.GetType();
        //
        Line line = lines.GetOrAdd(type, lineCreate);
        //
        object[] arguments;
        // Constructor argument count
        int ctorArgCount = line.parameterCloners.Count;
        // No args needed in .ctor
        if (ctorArgCount == 0) arguments = no_args;
        // Clone .ctor args
        else
        {
            // Place args here
            arguments = new object[ctorArgCount];
            // Copy fields to args
            for (int i = 0; i < ctorArgCount; i++)
                line.parameterCloners[i].CloneField(ref src, ref arguments[i], context);
        }
        // Create record
        Record dst = line.create(arguments);
        // Assign to context
        if (!srcObject.GetType().IsValueType) context.Add(src!, dst!);

        // Clone fields
        for (int i = 0; i < line.fieldCloners.Count; i++)
            line.fieldCloners[i].CloneField(ref src, ref dst, context);

        // Return 
        return dst!;
    }
}
