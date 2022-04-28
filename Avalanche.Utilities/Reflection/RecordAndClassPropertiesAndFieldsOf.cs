// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections;
using Avalanche.Utilities.Reflection;

/// <summary>Base for class that enumerates its fields of T.</summary>
public abstract class ClassFieldsOf
{
    /// <summary></summary>
    static readonly ConstructorT<ClassFieldsOf> constructor = new(typeof(ClassFieldsOf<>));
    /// <summary></summary>
    public static ClassFieldsOf Create(Type recordType) => constructor.Create(recordType);
}

/// <summary>Base for class that enumerates its fields of <typeparamref name="T"/>.</summary>
/// <typeparam name="T">enumerated field type</typeparam>
public abstract class ClassFieldsOf<T> : ClassFieldsOf, IEnumerable<T>
{
    /// <summary></summary>
    FieldValues<T> fields;
    /// <summary></summary>
    public ClassFieldsOf() => this.fields = new FieldValues<T>(FieldsOf.Create(GetType(), typeof(T)).Fields, this);
    /// <summary>Get enumerator of fields.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)fields).GetEnumerator();
    /// <summary>Get enumerator of fields.</summary>
    IEnumerator IEnumerable.GetEnumerator() => fields.GetEnumerator();
}

/// <summary>Base for records that enumerate its fields of T.</summary>
public abstract record RecordFieldsOf
{
    /// <summary></summary>
    static readonly ConstructorT<RecordFieldsOf> constructor = new(typeof(RecordFieldsOf<>));
    /// <summary></summary>
    public static RecordFieldsOf Create(Type recordType) => constructor.Create(recordType);
}

/// <summary>Base for records that enumerate its fields of <typeparamref name="T"/>.</summary>
/// <typeparam name="T">enumerated field type</typeparam>
public abstract record RecordFieldsOf<T> : RecordFieldsOf, IEnumerable<T>
{
    /// <summary></summary>
    FieldValues<T> fields;
    /// <summary></summary>
    public RecordFieldsOf() => this.fields = new FieldValues<T>(FieldsOf.Create(GetType(), typeof(T)).Fields, this);
    /// <summary>Get enumerator of fields.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)fields).GetEnumerator();
    /// <summary>Get enumerator of fields.</summary>
    IEnumerator IEnumerable.GetEnumerator() => fields.GetEnumerator();
}



/// <summary>Base for class that enumerates its properties of T.</summary>
public abstract class ClassPropertiesOf
{
    /// <summary></summary>
    static readonly ConstructorT<ClassPropertiesOf> constructor = new(typeof(ClassPropertiesOf<>));
    /// <summary></summary>
    public static ClassPropertiesOf Create(Type recordType) => constructor.Create(recordType);
}

/// <summary>Base for class that enumerates its properties of <typeparamref name="T"/>.</summary>
/// <typeparam name="T">enumerated property type</typeparam>
public abstract class ClassPropertiesOf<T> : ClassPropertiesOf, IEnumerable<T>
{
    /// <summary></summary>
    PropertyValues<T> properties;
    /// <summary></summary>
    public ClassPropertiesOf() => this.properties = new PropertyValues<T>(PropertiesOf.Create(GetType(), typeof(T)).Properties, this);
    /// <summary>Get enumerator of fields.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)properties).GetEnumerator();
    /// <summary>Get enumerator of fields.</summary>
    IEnumerator IEnumerable.GetEnumerator() => properties.GetEnumerator();
}

/// <summary>Base for records that enumerate its properties of T.</summary>
public abstract record RecordPropertiesOf
{
    /// <summary></summary>
    static readonly ConstructorT<RecordPropertiesOf> constructor = new(typeof(RecordPropertiesOf<>));
    /// <summary></summary>
    public static RecordPropertiesOf Create(Type recordType) => constructor.Create(recordType);
}

/// <summary>Base for records that enumerate its properties of <typeparamref name="T"/>.</summary>
/// <typeparam name="T">enumerated property type</typeparam>
public abstract record RecordPropertiesOf<T> : RecordPropertiesOf, IEnumerable<T>
{
    /// <summary></summary>
    PropertyValues<T> properties;
    /// <summary></summary>
    public RecordPropertiesOf() => this.properties = new PropertyValues<T>(PropertiesOf.Create(GetType(), typeof(T)).Properties, this);
    /// <summary>Get enumerator of fields.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)properties).GetEnumerator();
    /// <summary>Get enumerator of fields.</summary>
    IEnumerator IEnumerable.GetEnumerator() => properties.GetEnumerator();
}

/// <summary></summary>
public abstract record RecordPropertyDictionaryOf<Key, Value> : LockableDictionaryRecord<Key, Value>, IEnumerable<Value> where Key : notnull
{
    /// <summary></summary>
    public RecordPropertyDictionaryOf(Func<Value, Key> selector) : base()
    {
        //
        Init();
        // Property enumerable
        IEnumerable<Value> values = new PropertyValues<Value>(PropertiesOf.Create(GetType(), typeof(Value)).Properties, this);
        // Add properties
        foreach (var value in values)
        {
            // Get key
            Key key = selector(value);
            // Add to dictionary
            if (!this.TryAdd(key, value)) throw new InvalidOperationException($"Key {key} already exists in dictionary");
        }
        // Make immutable
        this.@readonly = true;
    }
    /// <summary></summary>
    protected virtual void Init() { }
    /// <summary>Get enumerator of fields.</summary>
    IEnumerator<Value> IEnumerable<Value>.GetEnumerator() => this.Values.GetEnumerator();
    /// <summary>Get enumerator of fields.</summary>
    IEnumerator IEnumerable.GetEnumerator() => this.Values.GetEnumerator();
}

