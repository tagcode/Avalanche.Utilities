// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Linq.Expressions;
using System.Reflection;

/// <summary>A single case of a property.</summary>
public class Case
{
    /// <summary>Name of the propety this is case of</summary>
    public readonly string PropertyName;
    /// <summary>Name of the case</summary>
    public readonly string Name;
    /// <summary>Dependencies to other properties. Before initializer is called, dependent properties are resolved first.</summary>
    public readonly string[] PropertyDependencies;
    /// <summary>Function that initializes this case. Returns case value.</summary>
    public readonly Func<Run, object>? InitializerFunc;
    /// <summary>(Optional) Function that runs a case.</summary> 
    public readonly Action<Run>? RunFunc;
    /// <summary>(Optional) Function that does cleanup.</summary> 
    public readonly Action<Run>? CleanupFunc;

    /// <summary>
    /// Searches for annotation [Case()], if not found returns null.
    /// 
    /// Type is searched for methods:
    ///     X Initialize(Run run);
    ///     void Cleanup(Run run);
    ///     void Run(Run run);
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns>a case or null</returns>
    public static Case? ReadAnnotated(Type type)
    {
        CaseAttribute? caseAttribute = Enumerable.SingleOrDefault(type.GetCustomAttributes(typeof(CaseAttribute), false).Where(o => o is CaseAttribute).Cast<CaseAttribute>());
        if (caseAttribute == null) return null;

        Object? obj = Activator.CreateInstance(type);

        // Initializer
        MethodInfo? miInitialize = Enumerable.SingleOrDefault(type
            .GetMethods()
            .Where(
                mi => mi.Name.Equals(nameof(Initialize)) && mi.GetParameters().Length == 1 && mi.GetParameters()[0].ParameterType.Equals(typeof(Run)) && typeof(object).IsAssignableFrom(mi.ReturnType)
             )
);
        Func<Run, object>? initializerFunc = null;
        if (miInitialize != null)
        {
            ParameterExpression runParam = Expression.Parameter(typeof(Run), nameof(Run));
            Expression body = Expression.Call(Expression.Constant(obj), miInitialize, runParam);
            if (typeof(object).Equals(body.Type)) body = Expression.Convert(body, typeof(object));
            Expression<Func<Run, object>> lambda = Expression.Lambda<Func<Run, object>>(body, runParam);
            initializerFunc = lambda.Compile();
        }

        // Cleanup
        MethodInfo? miCleanup = Enumerable.SingleOrDefault(type
            .GetMethods()
            .Where(
                mi => mi.Name.Equals(nameof(Cleanup)) && mi.GetParameters().Length == 1 && mi.GetParameters()[0].ParameterType.Equals(typeof(Run))
             )
);
        Action<Run>? cleanupFunc = null;
        if (miCleanup != null)
        {
            ParameterExpression runParam = Expression.Parameter(typeof(Run), nameof(Run));
            Expression body = Expression.Call(Expression.Constant(obj), miCleanup, runParam);
            Expression<Action<Run>> lambda = Expression.Lambda<Action<Run>>(body, runParam);
            cleanupFunc = lambda.Compile();
        }

        // Run
        MethodInfo? miRun = Enumerable.SingleOrDefault(type
            .GetMethods()
            .Where(
                mi => mi.Name.Equals(nameof(Run)) && mi.GetParameters().Length == 1 && mi.GetParameters()[0].ParameterType.Equals(typeof(Run))
             )
);
        Action<Run>? runFunc = null;
        if (miRun != null)
        {
            ParameterExpression runParam = Expression.Parameter(typeof(Run), nameof(Case.Run));
            Expression body = Expression.Call(Expression.Constant(obj), miRun, runParam);
            Expression<Action<Run>> lambda = Expression.Lambda<Action<Run>>(body, runParam);
            runFunc = lambda.Compile();
        }

        if (runFunc == null && cleanupFunc == null && initializerFunc == null)
            throw new ArgumentException($"{type.FullName} with {nameof(CaseAttribute)}, expected atleast one function: Run, Initialize, Cleanup");

        return new Case(caseAttribute.PropertyName, caseAttribute.Name, caseAttribute.PropertyDependencies, initializerFunc, cleanupFunc, runFunc);
    }

    /// <summary>Create case</summary>
    public Case(string propertyName, string caseName, string[]? propertyDependencies = null, Func<Run, object>? initializerFunc = null, Action<Run>? cleanupFunc = null, Action<Run>? runFunc = null)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        Name = caseName ?? throw new ArgumentNullException(nameof(caseName));
        InitializerFunc = initializerFunc;
        CleanupFunc = cleanupFunc;
        RunFunc = runFunc;
        this.PropertyDependencies = propertyDependencies ?? Array.Empty<string>();
    }

    /// <summary></summary>
    public virtual object? Initialize(Run run) => InitializerFunc == null ? null : InitializerFunc(run);

    /// <summary></summary>
    public virtual void Cleanup(Run run)
    {
        if (CleanupFunc != null) CleanupFunc(run);
    }

    /// <summary></summary>
    public virtual void Run(Run run)
    {
        if (RunFunc != null) RunFunc(run);
    }

    /// <summary></summary>
    public override string ToString() => $"{PropertyName}={Name}";
}
