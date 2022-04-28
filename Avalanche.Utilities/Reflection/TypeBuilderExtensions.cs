// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

/// <summary>Extension methods for <see cref="TypeBuilder"/>.</summary>
public static class TypeBuilderExtensions
{
    /// <summary>
    /// When requesting a type from <see cref="ModuleBuilder"/>, it may return a built type, or a type being built in aonther thread.
    /// 
    /// This method returns an existing type, or waits until another thread has built a type, or returns <see cref="TypeBuilder"/> for one thread.
    /// 
    /// For this method to work, every type builder construction for this <paramref name="typeName"/> must be processed through this method.
    /// </summary>
    /// <param name="moduleBuilder"></param>
    /// <param name="typeName"></param>
    /// <param name="createTypeBuilder"></param>
    /// <param name="type"></param>
    /// <param name="typeBuilder"></param>
    /// <returns>
    /// false = returned type,
    /// true = returned type builder (for one thread only)
    /// </returns>
    public static bool CreateSingleTypeBuilderOrWaitType(this ModuleBuilder moduleBuilder, string typeName, Func<TypeBuilder> createTypeBuilder, [NotNullWhen(false)] out Type type, [MaybeNullWhen(false)] out TypeBuilder typeBuilder)
    {
        // Get existing type without lock
        if (moduleBuilder.TryGetOrWaitBuiltType(typeName, out type)) { typeBuilder = null; return false; }
        // Place type builder here
        TypeBuilder _typeBuilder = null!;
        // Lock module builder
        Monitor.Enter(moduleBuilder);
        // 
        try
        {
            // Get existing type with-in lock
            if (moduleBuilder.TryGetOrWaitBuiltType(typeName, out type)) { typeBuilder = null; return false; }
            // Create type builder
            _typeBuilder = createTypeBuilder();
        }
        finally
        {
            Monitor.Exit(moduleBuilder);
        }
        // 
        type = null!;
        typeBuilder = _typeBuilder;
        return true;
    }

    /// <summary>
    /// Create type builder one time.
    /// 
    /// For this method to work, every type builder construction for this <paramref name="typeName"/> must be processed through this method.
    /// </summary>
    /// <param name="moduleBuilder"></param>
    /// <param name="typeName"></param>
    /// <param name="createTypeBuilder"></param>
    /// <param name="typeBuilder"></param>
    /// <returns>
    /// false = no type builder (assigns <paramref name="type"/> if it is built)
    /// true = returned type builder (for one thread only)
    /// </returns>
    public static bool CreateSingleTypeBuilderNoWait(this ModuleBuilder moduleBuilder, string typeName, Func<TypeBuilder> createTypeBuilder, out Type? type, [NotNullWhen(true)] out TypeBuilder typeBuilder)
    {
        // Get existing type without lock
        if (moduleBuilder.TryGetTypeNoWait(typeName, out type)) { typeBuilder = null!; return false; }
        // Place type builder here
        TypeBuilder _typeBuilder = null!;
        // Lock module builder
        Monitor.Enter(moduleBuilder);
        // 
        try
        {
            Type? _type = moduleBuilder.GetType(typeName);
            // Got type builder being built in another thread
            if (_type is TypeBuilder typeBuilder1) { type = null; typeBuilder = null!; return false; }
            // Got already built type
            else if (_type != null) { type = _type; typeBuilder = null!; return false; }
            // Get existing type with-in lock
            if (moduleBuilder.TryGetTypeNoWait(typeName, out type)) { typeBuilder = null!; return false; }
            // Create type builder
            _typeBuilder = createTypeBuilder();
        }
        finally
        {
            Monitor.Exit(moduleBuilder);
        }
        // 
        typeBuilder = _typeBuilder;
        return true;
    }

    /// <summary>Get existing type. If type is being built in concurrent thread, wait for completion.</summary>
    /// <param name="module"></param>
    /// <param name="typeName"></param>
    /// <param name="type"></param>
    /// <returns>true if got built type.</returns>
    public static bool TryGetOrWaitBuiltType(this Module module, string typeName, [NotNullWhen(true)] out Type type)
    {
        while (true)
        {
            // Get type without lock
            Type? _type = module.GetType(typeName);
            // Module builder did not have a type or type builder
            if (_type == null) { type = null!; return false; }
            // Got type builder being built in another thread
            if (_type is TypeBuilder typeBuilder1)
            {
                // Wait for other thread to finish building type. 
                Monitor.Enter(typeBuilder1);
                Monitor.Exit(typeBuilder1);
            }
            // Got a built type
            else
            {
                type = _type;
                return true;
            }
        }
    }

    /// <summary>Try get existing built type. If type is being built by concurrent thread, does not wait for completion</summary>
    /// <param name="module"></param>
    /// <param name="typeName"></param>
    /// <param name="type"></param>
    /// <returns>true if got built type.</returns>
    public static bool TryGetTypeNoWait(this Module module, string typeName, [NotNullWhen(true)] out Type type)
    {
        while (true)
        {
            // Get type without lock
            Type? _type = module.GetType(typeName);
            // Module builder did not have a type or type builder
            if (_type == null) { type = null!; return false; }
            // Got type builder being built in another thread
            if (_type is TypeBuilder typeBuilder1)
            {
                type = null!;
                return false;
            }
            // Got a built type
            else
            {
                type = _type;
                return true;
            }
        }
    }


}
