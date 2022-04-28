// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;

/// <summary>Creates integer identities between 0 .. 2147483647</summary>
public class IntegerGenerator : IdGeneratorBase<int>
{
    /// <summary>Singleton</summary>
    static IIdGenerator<int> instance = new IntegerGenerator(IIdGenerator.Scope.Process);
    /// <summary>Singleton</summary>
    public static IIdGenerator<int> Instance => instance;

    /// <summary>Last value</summary>
    protected int value = -1;
    /// <summary>Lock</summary>
    protected object mLock = new object();

    /// <summary>Create provider</summary>
    public IntegerGenerator(IIdGenerator.Scope uniqueness = IIdGenerator.Scope.Instance) : base(uniqueness) { }

    /// <summary>Test if there are more Ids</summary>
    public override bool HasMore { get { lock (mLock) return value < int.MaxValue; } }
    /// <summary>Try create next identity</summary>
    public override bool TryGetNext(out int value)
    {
        lock (mLock)
        {
            // Test whether there are more ids
            if (this.value == int.MaxValue) { value = int.MinValue; return false; }
            // Increment and assign
            value = ++this.value;
            // 
            return true;
        }
    }
}
