// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalanche.Utilities.Provider;

/// <summary>
/// Enumerable cloner. 
/// 
/// Reads <see cref="IEnumerable"/> and <see cref="IEnumerable{T}"/>, and outputs array.
/// 
/// If <see cref="ElementCloner"/> is assigned, makes deep copy, otherwise a shallow.
/// 
/// Implements also <see cref="IGraphCloner"/> for cyclic object graphs.
/// 
/// Supports two-phased initialization. <see cref="ElementCloner"/> can be assigned after initialization, and then can be set immutable with <see cref="IReadOnly"/>.
/// </summary>
/// <remarks> TODO The cyclic implementation is not correct. It should first allocate, then assign to context, and then resolve elements.</remarks>
public abstract class EnumerableCloner : ReadOnlyAssignableClass, ICloner, IGraphCloner, ICyclical
{
    /// <summary></summary>
    static readonly ConstructorT<EnumerableCloner> constructor = new(typeof(EnumerableCloner<>));

    /// <summary></summary>
    public static EnumerableCloner Create(Type elementType) => constructor.Create(elementType);

    /// <summary></summary>
    /// <param name="elementCloner">Optional element cloner as <see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>. If assigned, makes deep clone of each element. If not assigned, makes shallow clone.</param>
    public static EnumerableCloner Create(Type elementType, object? elementCloner = null)
    {
        // Create cloner
        EnumerableCloner cloner = constructor.Create(elementType);
        // Assign elementCloner
        if (elementCloner != null) cloner.ElementCloner = elementCloner;
        // Return
        return cloner;
    }

    /// <summary>Optional element cloner as <see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>. If assigned, makes deep clone of each element. If not assigned, makes shallow clone.</summary>
    protected object? elementCloner;
    /// <summary>Throw <see cref="ArgumentNullException"/> on 'null' source value, otherwise pass 'null'</summary>
    protected bool throwOnNull;

    /// <summary>Is cyclic value</summary>
    protected bool isCyclical;
    /// <summary>Is cyclic value</summary>
    public bool IsCyclical { get => isCyclical; set => this.AssertWritable().isCyclical = value; }

    /// <summary>Element type</summary>
    public abstract Type ElementType { get; }
    /// <summary>Throw <see cref="ArgumentNullException"/> on 'null' source value, otherwise pass 'null'</summary>
    public bool ThrowOnNull { get => throwOnNull; set => this.AssertWritable().throwOnNull = value; }

    /// <summary>Set element cloner</summary>
    /// <param name="elementCloner"><see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>.</param>
    protected abstract void setElementCloner(object? elementCloner);

    /// <summary>
    /// Optional element cloner as <see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>. 
    /// If assigned, makes deep clone of each element. If not assigned, makes shallow clone. 
    /// </summary>
    public object? ElementCloner { get => elementCloner; set => this.AssertWritable().setElementCloner(value); }

    /// <summary>Copy enumerable as array</summary>
    /// <param name="src"><see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/>.</param>
    /// <returns><![CDATA[T[]]]></returns>
    public abstract object Clone(object src);
    /// <summary></summary>
    public abstract object Clone(object src, IGraphClonerContext context);

    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphCloner.Context"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphClonerContext? setContext(IGraphClonerContext? context) { IGraphCloner.Context.Value = context; return context; }
}

/// <summary>Extension methods for <see cref="EnumerableCloner"/>.</summary>
public static class EnumerableClonerExtensions
{
    /// <summary>Set policy whether to throw <see cref="ArgumentNullException"/> on 'null' source value, otherwise pass 'null'</summary>
    public static T SetThrowOnNull<T>(this T cloner, bool throwOnNull) where T : EnumerableCloner { cloner.ThrowOnNull = throwOnNull; return cloner; }

    /// <summary>Set element cloner. If assigned, makes deep clone of each element. If not assigned, makes shallow clone.</summary>
    /// <param name="elementCloner">element cloner as <see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>. If assigned, makes deep clone of each element. If not assigned, makes shallow clone. </param>
    public static T SetElementCloner<T>(this T cloner, object? elementCloner) where T : EnumerableCloner { cloner.ElementCloner = elementCloner; return cloner; }
}

/// <summary>Facade for creating array cloners.</summary>
public static class ArrayCloner
{
    /// <summary></summary>
    static readonly ConstructorT<EnumerableCloner> constructor = new(typeof(ArrayCloner<>));
    /// <summary>Create array cloner</summary>
    /// <param name="elementCloner">Optional element cloner as <see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>. If assigned, makes deep clone of each element. If not assigned, makes shallow clone.</param>
    public static EnumerableCloner Create(Type elementType, object? elementCloner = null)
    {
        // Create cloner
        EnumerableCloner cloner = constructor.Create(elementType);
        // Assign elementCloner
        if (elementCloner != null) cloner.ElementCloner = elementCloner;
        // Return
        return cloner;
    }
}

/// <summary>Facade for creating array cloners.</summary>
public static class ListCloner
{
    /// <summary></summary>
    static readonly ConstructorT3<EnumerableCloner> constructor = new(typeof(ListCloner<,,>));

    /// <summary>Create list cloner</summary>
    /// <param name="listType">List type, e.g. <see cref="IList{T}"/> or <see cref="List{T}"/>.</param>
    /// <param name="implementationType">Optional implementation type, used if <paramref name="listType"/> is abstract.</param>
    /// <param name="elementCloner">Optional element cloner as <see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>. If assigned, makes deep clone of each element. If not assigned, makes shallow clone.</param>
    public static EnumerableCloner Create(Type listType, Type? implementationType = null, object? elementCloner = null)
    {
        // Get element type
        if (!TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(listType, typeof(IList<>), 0, out Type elementType)) throw new ArgumentException($"{nameof(listType)}");
        // Choose implementation type
        if (implementationType == null) implementationType = listType.IsInterface || listType.IsAbstract ? typeof(List<>).MakeGenericType(elementType) : listType;
        // Create cloner
        EnumerableCloner cloner = constructor.Create(listType, elementType, implementationType);
        // Assign elementCloner
        if (elementCloner != null) cloner.ElementCloner = elementCloner;
        // Return
        return cloner;
    }
}

/// <summary></summary>
/// <typeparam name="Element"></typeparam>
public abstract class EnumerableCloner_<Element> : EnumerableCloner
{
    /// <summary>Element type</summary>
    public override Type ElementType => typeof(Element);

    /// <summary>Set element cloner</summary>
    /// <param name="elementCloner"><see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>.</param>
    protected override void setElementCloner(object? elementCloner)
    {
        // Assign null
        if (elementCloner == null) this.elementCloner = null;
        // Assign as is
        else if (elementCloner is ICloner || elementCloner is ICloner<IEnumerable<Element>> || elementCloner is ICloner<Element[]> || elementCloner is IGraphCloner || elementCloner is IGraphCloner<IEnumerable<Element>> || elementCloner is IGraphCloner<Element[]>) this.elementCloner = elementCloner;
        // Assign IProvider
        else if (elementCloner is IProvider) this.elementCloner = elementCloner;
        // Assign IProvider<>
        else if (elementCloner is IProvider<Type, ICloner> || elementCloner is IProvider<Type, IGraphCloner>) this.elementCloner = elementCloner;
        // Cannot assign
        else throw new ArgumentException("Unsupported type", nameof(elementCloner));

        // Assign cyclicity
        isCyclical |= elementCloner is IGraphCloner graphCloner1 ? graphCloner1.IsCyclical : elementCloner is IGraphCloner<Element> graphCloner2 ? graphCloner2.IsCyclical : false;
    }

    /// <summary>Clone elements from <paramref name="src"/> to <paramref name="list"/>.</summary>
    /// <param name="src"><see cref="IEnumerable"/>, <see cref="IEnumerable{T}"/> to read elements from</param>
    /// <param name="builder"></param>
    /// <param name="list">Optional list to write to. If null a <see cref="List{T}"/> is created.</param>
    /// <returns><paramref name="list"/> or new list</returns>
    /// <exception cref="ArgumentNullException">If throwOnNull is enabled</exception>
    protected virtual T? CloneElements<T>(object src, Func<IList<Element>?, T?> builder, IList<Element>? list = null) where T : IEnumerable<Element>
    {
        // Throw or pass 'null'
        if (src == null) return throwOnNull ? throw new ArgumentNullException(nameof(src)) : default!;
        // Place context here
        IGraphClonerContext? context = null;
        // Move to cyclical 
        if (isCyclical || elementCloner is IGraphCloner<Element> || elementCloner is IGraphCloner)
        {
            // Get previous context
            IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
            // Place here context
            context = prevContext ?? setContext(new GraphClonerContext())!;
            try
            {
                return CloneElements(src, builder, context);
            }
            finally
            {
                // Revert to previous context
                IGraphCloner.Context.Value = prevContext;
            }
        }

        //
        int initialCount = 10;
        //
        if (src is ICollection __collection) initialCount = __collection.Count;
        //
        else if (src is ICollection<Element> __collectionT) initialCount = __collectionT.Count;
        // Create list if needed.
        if (list == null) list = new List<Element>(initialCount);

        // Shallow copy (reference pass)
        if (elementCloner == null)
        {
            // Iterate
            if (src is IEnumerable<Element> enumrT)
                foreach (Element element in enumrT) list.Add(element);
            else if (src is IEnumerable enumr)
                foreach (object element in enumr) list.Add((Element)element);
        }
        // ICloner<T>
        else if (elementCloner is ICloner<Element> clonerT)
        {
            // Iterate
            if (src is IEnumerable<Element> enumrT)
                foreach (Element element in enumrT) list.Add(clonerT.Clone(element));
            else if (src is IEnumerable enumr)
                foreach (object element in enumr) list.Add(clonerT.Clone((Element)element));
        }
        // ICloner
        else if (elementCloner is ICloner cloner)
        {
            // Iterate
            if (src is IEnumerable<Element> enumrT)
                foreach (Element element in enumrT) list.Add((Element)cloner.Clone(element!));
            else if (src is IEnumerable enumr)
                foreach (object element in enumr) list.Add((Element)cloner.Clone(element!));
        }
        // IProvider
        else if (elementCloner is IProvider clonerProvider)
        {
            // Iterate
            if (src is IEnumerable<Element> enumrT)
                foreach (Element elementT in enumrT)
                {
                    // No value
                    if (elementT == null) { list.Add(default!); continue; }
                    // Get element specific cloner
                    if (!clonerProvider.TryGetValue(elementT.GetType(), out object cloner_)) throw new ArgumentException($"Could not get cloner for \"{elementT.GetType()}\".");
                    // ICloner<T>
                    if (cloner_ is ICloner<Element> clonerT_) { list.Add(clonerT_.Clone(elementT)); continue; }
                    // ICloner
                    else if (cloner_ is ICloner cloner__) { list.Add((Element)cloner__.Clone(elementT)); continue; }
                    // IGraphCloner<T>
                    else if (cloner_ is IGraphCloner<Element> graphClonerT_)
                    {
                        // Get previous context
                        IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
                        // Place here context
                        context = prevContext ?? setContext(new GraphClonerContext())!;
                        try
                        {
                            // Clone
                            list.Add(graphClonerT_.Clone(elementT, context));
                            // Done
                            continue;
                        }
                        finally
                        {
                            // Revert to previous context
                            IGraphCloner.Context.Value = prevContext;
                        }
                    }
                    // IGraphCloner
                    else if (cloner_ is IGraphCloner graphCloner_)
                    {
                        // Get previous context
                        IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
                        // Place here context
                        context = prevContext ?? setContext(new GraphClonerContext())!;
                        try
                        {
                            // Clone
                            list.Add((Element)graphCloner_.Clone(elementT, context));
                            // Done
                            continue;
                        }
                        finally
                        {
                            // Revert to previous context
                            IGraphCloner.Context.Value = prevContext;
                        }
                    }
                    //
                    else throw new InvalidOperationException($"Provider did not return cloner, got {cloner_}");
                }
            // Iterate
            else if (src is IEnumerable enumr)
                foreach (object element in enumr)
                {
                    // No value
                    if (element == null) { list.Add(default!); continue; }
                    // Get element specific cloner
                    if (!clonerProvider.TryGetValue(element.GetType(), out object cloner_)) throw new ArgumentException($"Could not get cloner for \"{element.GetType()}\".");
                    // ICloner<T>
                    if (cloner_ is ICloner<Element> clonerT_ && element is Element elementT) { list.Add(clonerT_.Clone(elementT)); continue; }
                    // ICloner
                    else if (cloner_ is ICloner cloner__) { list.Add((Element)cloner__.Clone(element)); continue; }
                    // IGraphCloner<T>
                    else if (cloner_ is IGraphCloner<Element> graphClonerT_ && element is Element elementT_)
                    {
                        // Get previous context
                        IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
                        // Place here context
                        context = context ?? prevContext ?? setContext(new GraphClonerContext())!;
                        try
                        {
                            // Clone
                            list.Add(graphClonerT_.Clone(elementT_, context));
                            // Done
                            continue;
                        }
                        finally
                        {
                            // Revert to previous context
                            IGraphCloner.Context.Value = prevContext;
                        }
                    }
                    // IGraphCloner
                    else if (cloner_ is IGraphCloner graphCloner_)
                    {
                        // Get previous context
                        IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
                        // Place here context
                        context = context ?? prevContext ?? setContext(new GraphClonerContext())!;
                        try
                        {
                            // Clone
                            list.Add((Element)graphCloner_.Clone(element, context));
                            // Done
                            continue;
                        }
                        finally
                        {
                            // Revert to previous context
                            IGraphCloner.Context.Value = prevContext;
                        }
                    }
                    //
                    else throw new InvalidOperationException($"Provider did not return cloner, got {cloner_}");
                }

        }
        //
        else throw new InvalidOperationException($"Element cloner unusable: {elementCloner}");

        // Build
        T? result = builder(list);
        // Add to context
        if (context != null && result != null) context.Add(src, result);
        // Return list
        return result;
    }

    /// <summary>Graph clone</summary>
    /// <summary>Clone elements from <paramref name="src"/> to <paramref name="list"/>.</summary>
    /// <param name="src"><see cref="IEnumerable"/>, <see cref="IEnumerable{T}"/> to read elements from</param>
    /// <param name="list">Optional list to write to. If null a <see cref="List{T}"/> is created.</param>
    /// <returns><paramref name="list"/> or new list</returns>
    /// <exception cref="ArgumentNullException">If throwOnNull is enabled</exception>
    protected virtual T? CloneElements<T>(object src, Func<IList<Element>?, T?> builder, IGraphClonerContext context, IList<Element>? list = null) where T : IEnumerable<Element>
    {
        // Throw or pass 'null'
        if (src == null) return throwOnNull ? throw new ArgumentNullException(nameof(src)) : default!;
        // Exists in context
        if (context.TryGet(src, out object? _dst)) return (T)_dst!;
        //
        int initialCount = 10;
        //
        if (src is ICollection __collection) initialCount = __collection.Count;
        //
        else if (src is ICollection<Element> __collectionT) initialCount = __collectionT.Count;
        // Create list if needed.
        if (list == null) list = new List<Element>(initialCount);

        // Get previous context
        IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
        // Assign context
        IGraphCloner.Context.Value = context;

        try
        {
            // Shallow copy (reference or value pass)
            if (elementCloner == null)
            {
                // Iterate
                if (src is IEnumerable<Element> enumrT)
                    foreach (Element element in enumrT) list.Add(element);
                else if (src is IEnumerable enumr)
                    foreach (object element in enumr) list.Add((Element)element);
            }
            // IGraphCloner<T>
            else if (elementCloner is IGraphCloner<Element> graphClonerT)
            {
                // Iterate
                if (src is IEnumerable<Element> enumrT)
                    foreach (Element element in enumrT) list.Add(graphClonerT.Clone(element, context));
                else if (src is IEnumerable enumr)
                    foreach (object element in enumr) list.Add(graphClonerT.Clone((Element)element, context));
            }
            // IGraphCloner<T>
            else if (elementCloner is IGraphCloner graphCloner)
            {
                // Iterate
                if (src is IEnumerable<Element> enumrT)
                    foreach (Element element in enumrT) list.Add((Element)graphCloner.Clone(element!, context));
                else if (src is IEnumerable enumr)
                    foreach (object element in enumr) list.Add((Element)graphCloner.Clone(element, context));
            }
            // ICloner<T>
            else if (elementCloner is ICloner<Element> clonerT)
            {
                // Iterate
                if (src is IEnumerable<Element> enumrT)
                    foreach (Element element in enumrT) list.Add(clonerT.Clone(element));
                else if (src is IEnumerable enumr)
                    foreach (object element in enumr) list.Add(clonerT.Clone((Element)element));
            }
            // ICloner
            else if (elementCloner is ICloner cloner)
            {
                // Iterate
                if (src is IEnumerable<Element> enumrT)
                    foreach (Element element in enumrT) list.Add((Element)cloner.Clone(element!));
                else if (src is IEnumerable enumr)
                    foreach (object element in enumr) list.Add((Element)cloner.Clone(element!));
            }
            // IProvider
            else if (elementCloner is IProvider clonerProvider)
            {
                // Iterate
                if (src is IEnumerable<Element> enumrT)
                    foreach (Element elementT in enumrT)
                    {
                        // No value
                        if (elementT == null) { list.Add(default!); continue; }
                        // Get element specific cloner
                        if (!clonerProvider.TryGetValue(elementT.GetType(), out object cloner_)) throw new ArgumentException($"Could not get cloner for \"{elementT.GetType()}\".");
                        // ICloner<T>
                        if (cloner_ is ICloner<Element> clonerT_) { list.Add(clonerT_.Clone(elementT)); continue; }
                        // ICloner
                        else if (cloner_ is ICloner cloner__) { list.Add((Element)cloner__.Clone(elementT)); continue; }
                        // IGraphCloner<T>
                        else if (cloner_ is IGraphCloner<Element> graphClonerT_) { list.Add(graphClonerT_.Clone(elementT, context)); continue; }
                        // IGraphCloner
                        else if (cloner_ is IGraphCloner graphCloner_) { list.Add((Element)graphCloner_.Clone(elementT, context)); continue; }
                        //
                        else throw new InvalidOperationException($"Provider did not return cloner, got {cloner_}");
                    }
                else if (src is IEnumerable enumr)
                    foreach (object element in enumr)
                    {
                        // No value
                        if (element == null) { list.Add(default!); continue; }
                        // Get element specific cloner
                        if (!clonerProvider.TryGetValue(element.GetType(), out object cloner_)) throw new ArgumentException($"Could not get cloner for \"{element.GetType()}\".");
                        // IGraphCloner<T>
                        if (cloner_ is IGraphCloner<Element> graphClonerT_ && element is Element elementT_) { list.Add(graphClonerT_.Clone(elementT_, context)); continue; }
                        // IGraphCloner
                        else if (cloner_ is IGraphCloner graphCloner_) { list.Add((Element)graphCloner_.Clone(element, context)); continue; }
                        // ICloner<T>
                        else if (cloner_ is ICloner<Element> clonerT_ && element is Element elementT) { list.Add(clonerT_.Clone(elementT)); continue; }
                        // ICloner
                        else if (cloner_ is ICloner cloner__) { list.Add((Element)cloner__.Clone(element)); continue; }
                        //
                        else throw new InvalidOperationException($"Provider did not return cloner, got {cloner_}");
                    }
            }
        }
        finally
        {
            // Revert to previous context
            IGraphCloner.Context.Value = prevContext;
        }

        // Build
        T? result = builder(list);
        // Add to context
        if (result != null) context.Add(src, result);
        // Return list
        return result;
    }
}

/// <summary></summary>
/// <typeparam name="Element"></typeparam>
public class EnumerableCloner<Element> :
    EnumerableCloner_<Element>,
    ICloner<IEnumerable>, IGraphCloner<IEnumerable>,
    ICloner<IEnumerable<Element>>, IGraphCloner<IEnumerable<Element>>
{
    /// <summary>Method that finalizes list to output type</summary>
    Func<IList<Element>?, IEnumerable<Element>?> builder = (IList<Element>? list) => list;

    /// <summary></summary>
    public EnumerableCloner() { }
    /// <summary></summary>
    /// <param name="elementCloner">Optional element cloner as <see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>. If assigned, makes deep clone of each element. If not assigned, makes shallow clone.</param>
    public EnumerableCloner(object? elementCloner) => setElementCloner(elementCloner);

    /// <summary></summary>
    public override object Clone(object src) => CloneElements(src, builder)!;
    /// <summary>Graph clone</summary>
    public override object Clone(object src, IGraphClonerContext context) => CloneElements(src, builder, context)!;
    /// <summary></summary>
    public virtual IEnumerable Clone(in IEnumerable src) => CloneElements(src, builder)!;
    /// <summary></summary>
    public virtual IEnumerable Clone(in IEnumerable src, IGraphClonerContext context) => CloneElements(src, builder, context)!;
    /// <summary></summary>
    public virtual IEnumerable<Element> Clone(in IEnumerable<Element> src) => CloneElements(src, builder)!;
    /// <summary></summary>
    public virtual IEnumerable<Element> Clone(in IEnumerable<Element> src, IGraphClonerContext context) => CloneElements(src, builder, context)!;
}

/// <summary></summary>
/// <typeparam name="Element"></typeparam>
public class ArrayCloner<Element> : EnumerableCloner_<Element>,
    ICloner<IEnumerable>, IGraphCloner<IEnumerable>,
    ICloner<IEnumerable<Element>>, IGraphCloner<IEnumerable<Element>>,
    ICloner<Element[]>, IGraphCloner<Element[]>
{
    /// <summary>Method that finalizes list to output type</summary>
    Func<IList<Element>?, Element[]?> builder = (IList<Element>? list) => list?.ToArray();

    /// <summary></summary>
    public ArrayCloner() { }
    /// <summary></summary>
    /// <param name="elementCloner">Optional element cloner as <see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>. If assigned, makes deep clone of each element. If not assigned, makes shallow clone.</param>
    public ArrayCloner(object? elementCloner) => setElementCloner(elementCloner);

    /// <summary></summary>
    public override object Clone(object src) => CloneElements(src, builder)!;
    /// <summary>Graph clone</summary>
    public override object Clone(object src, IGraphClonerContext context) => CloneElements(src, builder, context)!;
    /// <summary></summary>
    public virtual IEnumerable Clone(in IEnumerable src) => CloneElements(src, builder)!;
    /// <summary></summary>
    public virtual IEnumerable Clone(in IEnumerable src, IGraphClonerContext context) => CloneElements(src, builder, context)!;
    /// <summary></summary>
    public virtual IEnumerable<Element> Clone(in IEnumerable<Element> src) => CloneElements(src, builder)!;
    /// <summary></summary>
    public virtual IEnumerable<Element> Clone(in IEnumerable<Element> src, IGraphClonerContext context) => CloneElements(src, builder, context)!;
    /// <summary></summary>
    public virtual Element[] Clone(in Element[] src) => CloneElements(src, builder)!;
    /// <summary></summary>
    public virtual Element[] Clone(in Element[] src, IGraphClonerContext context) => CloneElements(src, builder, context)!;
}

/// <summary></summary>
/// <typeparam name="T">The implemented <see cref="ICloner{T}"/> type</typeparam>
/// <typeparam name="Element"></typeparam>
/// <typeparam name="ListImplementation"></typeparam>
public class ListCloner<T, Element, ListImplementation> :
    EnumerableCloner_<Element>,
    ICloner<T>, IGraphCloner<T>
    where T : IEnumerable<Element>
    where ListImplementation : IList<Element>, new()
{
    /// <summary>Method that finalizes list to output type</summary>
    Func<IList<Element>?, T?> builder = (IList<Element>? list) => (T?)list;

    /// <summary></summary>
    public ListCloner() { }
    /// <summary></summary>
    /// <param name="elementCloner">Optional element cloner as <see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>. If assigned, makes deep clone of each element. If not assigned, makes shallow clone.</param>
    public ListCloner(object? elementCloner) => setElementCloner(elementCloner);

    /// <summary></summary>
    public override object Clone(object src) => CloneElements(src, builder)!;
    /// <summary>Graph clone</summary>
    public override object Clone(object src, IGraphClonerContext context) => CloneElements(src, builder, context)!;
    /// <summary></summary>
    public virtual T Clone(in T src) => CloneElements(src, builder, new ListImplementation())!;
    /// <summary></summary>
    public virtual T Clone(in T src, IGraphClonerContext context) => CloneElements(src, builder, context, new ListImplementation())!;
}

/// <summary></summary>
/// <typeparam name="T">The implemented <see cref="ICloner{T}"/> type</typeparam>
/// <typeparam name="Element"></typeparam>
public class ListCloner<T, Element> : ListCloner<T, Element, T> where T : IList<Element>, new()
{
    /// <summary></summary>
    public ListCloner() { }
    /// <summary></summary>
    /// <param name="elementCloner">Optional element cloner as <see cref="ICloner"/>, <see cref="ICloner{Element}"/>, <see cref="IGraphCloner"/> or <see cref="IGraphCloner{Element}"/>, or <see cref="IProvider"/>, <see cref="IProvider{Type, ICloner}"/>, <see cref="IProvider{Type, IGraphCloner}"/>. If assigned, makes deep clone of each element. If not assigned, makes shallow clone.</param>
    public ListCloner(object? elementCloner) => setElementCloner(elementCloner);
}
